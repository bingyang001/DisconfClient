using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DisconfClient
{
    internal static class Utils
    {
        /// <summary>
        /// 检测IP地址规则的正则表达式
        /// </summary>
        private const string Pattern =
            @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$";

        private static string _localIp = null;
        private static string GetLocalIp()   //获取本地IP 
        {
            if (_localIp != null) return _localIp;
            IPAddress[] ipAddrs = Dns.GetHostAddresses(Dns.GetHostName());
            string ip = string.Empty;
            foreach (IPAddress ipAddr in ipAddrs)
            {
                ip = ipAddr.ToString();
                if (IsIp(ip))
                    break;
            }
            _localIp = ip;
            return _localIp;

        }
        /// <summary>
        /// IP地址验证的正则表达式
        /// </summary>
        private static readonly Regex PatternRegex = new Regex(Pattern);

        public static bool IsIp(string ip)
        {
            return !string.IsNullOrWhiteSpace(ip) && PatternRegex.IsMatch(ip.Trim());
        }

        /// <summary>
        /// 本机IP
        /// </summary>
        public static string LocalIp
        {
            get { return _localIp ?? (_localIp = GetLocalIp()); }
        }

        public static string GetAppInstanceId()
        {
            return string.Format("{0}-{1}", LocalIp, Process.GetCurrentProcess().Id);
        }

        public static TAttribute GetFirstAttribute<TAttribute>(Type type, bool inherit)
        {
            var attrs = type.GetCustomAttributes(typeof(TAttribute), inherit);
            return (TAttribute)(attrs.Length > 0 ? attrs[0] : null);
        }

        public static TAttribute GetFirstAttribute<TAttribute>(PropertyInfo propertyInfo, bool inherit)
        {
            var attrs = propertyInfo.GetCustomAttributes(typeof(TAttribute), inherit);
            return (TAttribute)(attrs.Length > 0 ? attrs[0] : null);
        }
    }
}
