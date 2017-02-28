using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ymatou.CommonService;

namespace DisconfClient.Logger
{
    public class YmatouLogAdapter : ILogger
    {
        private const string LogPrefix = "DisconfClient,";
        public void Debug(string message, Exception exception = null)
        {
            if(string.IsNullOrWhiteSpace(message))
                return;
            message = string.Concat(LogPrefix, message);
            ApplicationLog.Debug(message + (exception == null ? "" : exception.Message));
        }

        public void Info(string message, Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            message = string.Concat(LogPrefix, message);
            ApplicationLog.Info(message + (exception == null ? "" : exception.Message));
        }

        public void Warn(string message, Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            message = string.Concat(LogPrefix, message);
            ApplicationLog.Warn(message, exception);
        }

        public void Error(string message, Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            message = string.Concat(LogPrefix, message);
            ApplicationLog.Error(message, exception);
        }

        public void Fatal(string message, Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            message = string.Concat(LogPrefix, message);
            ApplicationLog.Fatal(message, exception);
        }
    }
}
