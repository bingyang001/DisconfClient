using System;
using System.Threading;
using ZooKeeperNet;

namespace DisconfClient
{
    /// <summary>
    /// Zookeeper连接监视器
    /// </summary>
    internal class ConnectionWatcher : IWatcher
    {
        private readonly IDisconfWebApi _webApi;
        public ConnectionWatcher(IDisconfWebApi webApi)
        {
            _webApi = webApi;
            Guid id = ZooKeeper.Id;
        }

        public IDisconfWebApi DisconfWebApi
        {
            get { return _webApi; }
        }

        private static ZooKeeper _zooKeeper;
        private static readonly object SyncRoot = new object();


        /// <summary>
        /// ZooKeeper实例
        /// </summary>
        protected ZooKeeper ZooKeeper
        {
            get
            {
                if (_zooKeeper == null)
                {
                    lock (SyncRoot)
                    {
                        if (_zooKeeper == null)
                        {
                            Connect();
                        }
                    }

                }
                return _zooKeeper;
            }
        }

        /// <summary>
        /// 连接ZooKeeper
        /// </summary>
        private void Connect()
        {
            string host = _webApi.GetZooKeeperHost();
            _zooKeeper = new ZooKeeper(host, new TimeSpan(0, 0, 0, DisconfClientSettings.ZooKeeperSessionTimeout), this);
            LogManager.GetLogger().Info(string.Format("DisconfClient.ConnectionWatcher.Connect,host:{0},connected.", host));
        }

        /// <summary>
        /// 重新连接ZooKeeper
        /// </summary>
        public void ReConnect()
        {
            string host = _webApi.GetZooKeeperHost();
            ActionRetryHelper.Retry(() =>
            {
                LogManager.GetLogger().Info(string.Format("DisconfClient.ConnectionWatcher.Connect,host:{0},Start ReConnect.", host));

                Close();
                Connect();
                NodeWatcher.ReRegisterAllWatcher();
            }, 3, new TimeSpan(10), () =>
            {
                //exceptionAction
            }, (ex) =>
            {
                //errorHandle
                LogManager.GetLogger().Error(string.Format("DisconfClient.ConnectionWatcher.Connect,host:{0},ReConnect,{1}.", host, ex));
            }, () =>
            {
                //retryExceptionAction
            });

        }

        public void MonitorZooKeeperState()
        {
            if (_zooKeeper == null || !_zooKeeper.State.IsAlive())
            {
                ReConnect();
            }
        }

        /// <summary>
        /// 关闭ZooKeeper
        /// </summary>
        public void Close()
        {
            try
            {
                if (_zooKeeper != null)
                {
                    LogManager.GetLogger().Info(string.Format("DisconfClient.ConnectionWatcher.Close,Begin,State:{0},IsActive:{1}", _zooKeeper.State.State, _zooKeeper.State.IsAlive()));
                    _zooKeeper.Dispose();
                    LogManager.GetLogger().Info(string.Format("DisconfClient.ConnectionWatcher.Close,End,State:{0},IsActive:{1}", _zooKeeper.State.State, _zooKeeper.State.IsAlive()));
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error("DisconfClient.ConnectionWatcher.Close", ex);
            }

        }

        /// <summary>
        /// ZooKeeper事件处理
        /// </summary>
        /// <param name="watchedEvent">ZooKeeper事件</param>
        public void Process(WatchedEvent watchedEvent)
        {
            string host = _webApi.GetZooKeeperHost();
            if (watchedEvent.State == KeeperState.SyncConnected)
            {
                LogManager.GetLogger().Info(string.Format("DisconfClient.ConnectionWatcher.Connect,host:{0},KeeperState.SyncConnected.", host));
                NodeWatcher.ReRegisterAllWatcher();
            }
            else if (watchedEvent.State == KeeperState.NoSyncConnected)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.ConnectionWatcher.Connect,host:{0},KeeperState.NoSyncConnected.", host));
            }
            else if (watchedEvent.State == KeeperState.Disconnected)
            {
                // 这时收到断开连接的消息，这里其实无能为力，因为这时已经和ZK断开连接了，只能等ZK再次开启了
                LogManager.GetLogger().Error(string.Format("DisconfClient.ConnectionWatcher.Connect,host:{0},KeeperState.Disconnected.", host));
            }
            else if (watchedEvent.State == KeeperState.Expired)
            {
                // 这时收到这个信息，表示，ZK已经重新连接上了，但是会话丢失了，这时需要重新建立会话。
                LogManager.GetLogger().Error(string.Format("DisconfClient.ConnectionWatcher.Connect,host:{0},KeeperState.Expired.", host));
                ReConnect();
            }
            else if (watchedEvent.State == KeeperState.Unknown)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.ConnectionWatcher.Connect,host:{0},KeeperState.Unknown.", host));
            }

        }
    }
}
