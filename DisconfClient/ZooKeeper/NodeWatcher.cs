using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;

namespace DisconfClient
{

    /// <summary>
    /// 节点监视器
    /// </summary>
    internal class NodeWatcher : IWatcher
    {
        private readonly string _monitorPath;
        private readonly string _nodeName;
        private string _nodeData;
        private readonly DisconfNodeType _disconfNodeType;
        private readonly ZooKeeperClient _zooKeeperClient;
        public static readonly BlockingCollection<NodeWatcher> NodeWatchers = new BlockingCollection<NodeWatcher>();

        /// <summary>
        /// 构建节点监视器
        /// </summary>
        /// <param name="zooKeeperClient"></param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="nodeData">节点值</param>
        /// <param name="disconfNodeType">节点类型</param>
        /// <param name="monitorPath">监视路径</param>
        public NodeWatcher(ZooKeeperClient zooKeeperClient, string nodeName, string nodeData, DisconfNodeType disconfNodeType, string monitorPath)
        {
            _zooKeeperClient = zooKeeperClient;
            _monitorPath = monitorPath;
            _nodeName = nodeName;
            _nodeData = nodeData;
            _disconfNodeType = disconfNodeType;
        }

        /// <summary>
        /// ZooKeeper事件处理
        /// </summary>
        /// <param name="watchedEvent">ZooKeeper事件</param>
        public void Process(WatchedEvent watchedEvent)
        {
            try
            {
                //结点更新时
                if (watchedEvent.Type == EventType.NodeDataChanged)
                {
                    CallBack();
                }

                //结点断开连接，这时不要进行处理
                if (watchedEvent.State == KeeperState.Disconnected)
                {
                    //todo log
                }

                //session expired，需要重新关注哦
                if (watchedEvent.State == KeeperState.Expired)
                {
                    _zooKeeperClient.ReConnect();
                    CallBack();
                }
                LogManager.GetLogger().Info(string.Format("DisconfClient.NodeWatcher,WatchedEvent:{0}", watchedEvent));
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Info(string.Format("DisconfClient.NodeWatcher,WatchedEvent:{0},Exception:{1}", watchedEvent, ex));
            }
        }

        /// <summary>
        /// 启动监视
        /// </summary>
        public void Monitor()
        {
            try
            {
                bool flag = _zooKeeperClient.IsExistWatche(_monitorPath);
                if (flag)
                    return;
                Stat stat = new Stat();
                string data = _zooKeeperClient.GetData(_monitorPath, this, stat);
                NodeWatchers.Add(this);
                LogManager.GetLogger().Info(string.Format("DisconfClient.NodeWatcher.Monitor, MonitorPath:{0}", _monitorPath));
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.NodeWatcher.Monitor, MonitorPath:{0}, Exception:{1}", _monitorPath, ex));
            }
        }

        /// <summary>
        /// 节点更新回调
        /// </summary>
        private void CallBack()
        {
            ConfigMetadataApiResult metadata = new ConfigMetadataApiResult
            {
                Name = _nodeName,
                Type = _disconfNodeType
            };
            ConfigStorageManager.ReloadConfigItem(metadata);
        }

        public static void ReRegisterAllWatcher()
        {
            foreach (NodeWatcher nodeWatcher in NodeWatcher.NodeWatchers)
            {
                if (nodeWatcher == null)
                    continue;
                nodeWatcher.Monitor();
            }
        }
    }
}
