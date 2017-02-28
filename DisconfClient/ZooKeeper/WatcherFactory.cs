using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisconfClient
{
    /// <summary>
    /// 监视器工厂
    /// </summary>
    internal static class WatcherFactory
    {
        /// <summary>
        /// 获取一个监视管理器
        /// </summary>
        /// <param name="webApi">配置中心服务接口</param>
        /// <returns></returns>
        public static WatcherManager GetWatcherManager(IDisconfWebApi webApi)
        {
            if (webApi == null)
                throw new ArgumentNullException("webApi");
            string host = webApi.GetZooKeeperHost();
            string rootNode = webApi.GetZooKeeperRootNode();
            WatcherManager watcherManager = new WatcherManager();
            watcherManager.Init(host, rootNode);
            return watcherManager;
        }
    }
}
