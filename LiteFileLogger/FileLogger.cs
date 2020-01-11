using System;
using System.Collections;
using System.IO;

namespace LiteFileLogger
{
    /// <summary>
    /// Will log messages in the file created.
    /// </summary>
    public class FileLogger : FileLoggerBase, IMultiLogger, IFileLogger
    {    
        public FileLogger(string filePath)
        {
            CreateLogFile(filePath);
        }

        public void Log(LogMessageType type, string message)
        {
            string log = DateTime.Now.ToString("HH.mm.ss.FFF") + " [" + type + "] : " + message;
            _queueMessages.Enqueue(log);
            StartLogging();
        }

        /// <summary>
        /// Can log multiple messages of a single type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="messages"></param>
        public void LogMultiple(LogMessageType type, IEnumerable messages)
        {
            foreach (var message in messages)
            {
                string log = DateTime.Now.ToString("HH.mm.ss.FFF") + " [" + type + "] : " + message;
                _queueMessages.Enqueue(log);
            }
            StartLogging();
        }

        private void CreateLogFile(string filePath)
        {
            if (!filePath.EndsWith(".txt"))
            {
                filePath = Path.Combine(filePath, ".txt");
            }
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
            _path = filePath;
        }
    }
}
