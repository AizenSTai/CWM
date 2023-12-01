using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using System.Runtime.InteropServices;

namespace Microsis.CWM.Util
{
    public static class LoggerProxy
    {
        public enum LogLevels : int
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
            Warn = 3,
            Error = 4,
            Fatal = 5,
        }

        static readonly Logger _Log;

        static LoggerProxy()
        {
            string path = System.IO.Path.GetFullPath(System.IO.Path.Combine("NLog.config"));
            LogManager.Configuration = new XmlLoggingConfiguration(path);

            _Log = LogManager.GetCurrentClassLogger();

        }

        public static void Log(LogLevels level, string message, System.Exception exception = null, params object[] args)
        {
            Logger oLogger = _Log;

            switch (level)
            {
                case LogLevels.Debug:
                    {
                        oLogger.Debug(exception: exception, message: message, args: args);
                        break;
                    }
                case LogLevels.Error:
                    {
                        oLogger.Error(exception: exception, message: message, args: args);
                        break;
                    }
                case LogLevels.Fatal:
                    {
                        oLogger.Fatal(exception: exception, message: message, args: args);
                        break;
                    }
                case LogLevels.Info:
                    {
                        oLogger.Info(exception: exception, message: message, args: args);
                        break;
                    }
                case LogLevels.Trace:
                    {
                        oLogger.Trace(exception: exception, message: message, args: args);
                        break;
                    }
                case LogLevels.Warn:
                    {
                        oLogger.Warn(exception: exception, message: message, args: args);
                        break;
                    }
            }
        }

    }
}
