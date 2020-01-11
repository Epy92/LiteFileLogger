using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace LiteFileLogger
{
    public abstract class FileLoggerBase
    {
        protected string _path = null;
        private bool _isRunning;
        protected ConcurrentQueue<string> _queueMessages = new ConcurrentQueue<string>();
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
        protected bool IsFileLocked()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (Exception)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
            return false;
        }

        protected void WriteToFile(string entry)
        {
            using (var streamWriter = new StreamWriter(_path, true))
            {
                streamWriter.WriteLine(entry);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        protected void StartLogging()
        {
            if (!_isRunning)
            {
                BackgroundWorker bgWorker = new BackgroundWorker();
                bgWorker.DoWork += WriteLogMessages_DoWork;
                bgWorker.RunWorkerCompleted += WriteLogMessages_Completed;
                bgWorker.RunWorkerAsync();
            }
        }


        private void WriteLogMessages_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            _isRunning = false;
        }

        private void WriteLogMessages_DoWork(object sender, DoWorkEventArgs e)
        {
            _isRunning = true;
            while (!_queueMessages.IsEmpty)
            {
                _queueMessages.TryDequeue(out string message);
                if (!string.IsNullOrEmpty(message))
                {
                    try
                    {
                        _readWriteLock.EnterWriteLock();
                        WriteToFile(message);
                    }
                    catch (Exception)
                    {
                        _queueMessages.Enqueue(message);
                    }
                    finally
                    {
                        _readWriteLock.ExitWriteLock();
                    }
                }
            }
        }
    }
}
