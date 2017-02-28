using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using DisconfClient.Logger;

namespace DisconfClient
{
    public static class LogManager
    {
        private static ILogger _logger;
        public static ILogger GetLogger()
        {
            if (_logger != null)
                return _logger;
            string typeName = DisconfClientSettings.Logger;
            if (string.IsNullOrWhiteSpace(typeName))
            {
                _logger = new YmatouLogAdapter();
                //_logger = new ConsoleLogAdapter();
            }
            else
            {
                Type type = Type.GetType(typeName, true);
                _logger = (ILogger)Activator.CreateInstance(type);
            }
            return _logger;
        }
    }
}
