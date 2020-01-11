using System.Collections;

namespace LiteFileLogger
{
    public interface IFileLogger
    {
        void Log(LogMessageType type, string message);
    }

    public interface IMultiLogger : IFileLogger
    {
        void LogMultiple(LogMessageType type, IEnumerable messages);
    }
}