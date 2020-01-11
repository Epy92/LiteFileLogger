using System;
using System.IO;

namespace LiteFileLogger
{
    /// <summary>
    /// Will create 1 file every day and will log all messages in that file
    /// </summary>
    public class SingleFileLogger : FileLoggerBase, IFileLogger
    {
        private static volatile SingleFileLogger instance;
        private const string DIRECTORY_NAME = "DailyLogs";

        private static object SingleInstanceLock = new object();

        public static SingleFileLogger Instance
        {
            get
            {
                if (instance == null || instance._path == null || instance._path.Equals(""))
                {
                    lock (SingleInstanceLock)
                    {
                        if (instance == null || instance._path == null || instance._path.Equals(""))
                        {
                            instance = new SingleFileLogger();
                            instance._path = GetLogPath();
                            if (instance._path == null || instance._path.Equals(""))
                                instance = null;
                        }
                    }
                }
                return instance;
            }
        }

        public void Log(LogMessageType type, string message)
        {
            string log = DateTime.Now.ToString("HH:mm:ss") + " [" + type.ToString().ToUpper() + "] : " + message;
            _queueMessages.Enqueue(log);
            StartLogging();
        }        

        private static string GetLogPath()
        {
            var logDirectory = Path.Combine(AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf("bin")), DIRECTORY_NAME);

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            var filePath = Path.Combine(logDirectory, "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
            return filePath;
        }
    }
}