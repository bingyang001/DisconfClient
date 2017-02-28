using System;
using log4net;

namespace DisconfClient
{
    public class Log4NetAdapter : ILogger
    {
        private readonly ILog _logger = log4net.LogManager.GetLogger(typeof(Log4NetAdapter));


        public void Debug(string message, Exception exception = null)
        {
            if (_logger.IsDebugEnabled)
                _logger.Debug(message, exception);
        }

        public void Info(string message, Exception exception = null)
        {
            if (_logger.IsInfoEnabled)
                _logger.Info(message, exception);
        }

        public void Warn(string message, Exception exception = null)
        {
            if (_logger.IsWarnEnabled)
                _logger.Warn(message, exception);
        }

        public void Error(string message, Exception exception = null)
        {
            if (_logger.IsErrorEnabled)
                _logger.Error(message, exception);
        }

        public void Fatal(string message, Exception exception = null)
        {
            if (_logger.IsFatalEnabled)
                _logger.Fatal(message, exception);
        }
    }
}
