using System;
using System.Configuration;
namespace DisconfClient
{
    public static class DisconfClientSettings
    {
        static DisconfClientSettings()
        {
            ZooKeeperSessionTimeout = ConfigManager.AppSettings<int>("DisconfClient.ZooKeeperSessionTimeout", 90);
            AppId = ConfigManager.AppSettings<string>("AppId");
            Environment = ConfigManager.AppSettings<string>("DisconfClient.Environment");
            DisconfServerHost = ConfigManager.AppSettings<string>("DisconfClient.DisconfServerHost");
            Version = ConfigManager.AppSettings<string>("DisconfClient.Version", "1.0");
            Logger = ConfigManager.AppSettings<string>("DisconfClient.Logger");
            OnlyLoadLocalConfig = ConfigManager.AppSettings<bool>("DisconfClient.OnlyLoadLocalConfig");
            DisconfClientLocalConfigPath = ConfigManager.AppSettings<string>("DisconfClient.DisconfClientLocalConfigPath", "D:\\DisconfClientConfig");
            DisableZooKeeper = ConfigManager.AppSettings<bool>("DisconfClient.DisableZooKeeper");
            ConfigAssemblies = ConfigManager.AppSettings<string>("DisconfClient.ConfigAssemblies");
            int refreshTime = ConfigManager.AppSettings<int>("DisconfClient.RefreshTime", 15);
            if (refreshTime < 15)
                refreshTime = 15;
            if (refreshTime > 120)
                refreshTime = 120;
            RefreshTime = refreshTime;
        }

        public static int ZooKeeperSessionTimeout { get; private set; }

        public static string DisconfServerHost { get; private set; }

        public static string AppId { get; private set; }

        public static string Environment { get; private set; }

        public static string Version { get; private set; }

        public static string Logger { get; private set; }

        public static bool OnlyLoadLocalConfig { get; private set; }

        public static string DisconfClientLocalConfigPath { get; private set; }

        /// <summary>
        /// 是否禁用ZooKeeper通信
        /// </summary>
        public static bool DisableZooKeeper { get; private set; }

        /// <summary>
        /// 配置项定时比对刷新时间间隔，单位：秒
        /// </summary>
        public static int RefreshTime { get; private set; }

        /// <summary>
        /// 配置类所在的程序集，多个用','隔开
        /// </summary>
        public static string ConfigAssemblies { get; private set; }

        internal static void Verify()
        {
            if (string.IsNullOrWhiteSpace(AppId))
                throw new ArgumentNullException("AppId");
            if (string.IsNullOrWhiteSpace(Environment))
                throw new ArgumentNullException("DisconfClient.Environment");
            if (string.IsNullOrWhiteSpace(DisconfServerHost))
                throw new ArgumentNullException("DisconfClient.DisconfServerHost");
        }
    }
}
