using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ZooKeeperNet;

namespace DisconfClient
{
    /// <summary>
    /// 监视器管理器
    /// </summary>
    internal class WatcherManager
    {
        private readonly ZooKeeperClient _zooKeeperClient;

        public WatcherManager(ZooKeeperClient zooKeeperClient)
        {
            _zooKeeperClient = zooKeeperClient;
        }


        /// <summary>
        ///  创建一个监视路径
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="nodeData">节点数据</param>
        /// <param name="disconfNodeType">节点类型</param>
        /// <returns></returns>
        private string CreateMonitorPath(string nodeName, string nodeData, DisconfNodeType disconfNodeType)
        {
            string rootNode = _zooKeeperClient.DisconfWebApi.GetZooKeeperRootNode();
            string clientRootZooPath = PathManager.GetBasePath(rootNode, DisconfClientSettings.AppId,
                DisconfClientSettings.Environment, DisconfClientSettings.Version);
            _zooKeeperClient.CreatePersistentPath(clientRootZooPath, Utils.LocalIp);
            string monitorPath;
            if (disconfNodeType == DisconfNodeType.File)
            {
                string clientDisconfFileZooPath = PathManager.GetFilePath(clientRootZooPath);
                _zooKeeperClient.CreatePersistentPath(clientDisconfFileZooPath, Utils.LocalIp);
                monitorPath = PathManager.JoinPath(clientDisconfFileZooPath, nodeName);
            }
            else
            {
                string clientDisconfItemZooPath = PathManager.GetItemPath(clientRootZooPath);
                _zooKeeperClient.CreatePersistentPath(clientDisconfItemZooPath, Utils.LocalIp);
                monitorPath = PathManager.JoinPath(clientDisconfItemZooPath, nodeName);
            }
            _zooKeeperClient.CreatePersistentPath(monitorPath, "");
            CreateTempPath(monitorPath, nodeData);
            return monitorPath;
        }

        /// <summary>
        /// 创建一个临时路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="data">数据</param>
        private void CreateTempPath(string path, string data)
        {
            string tempPath = path + "/" + Utils.LocalIp + "-" + Process.GetCurrentProcess().Id;
            _zooKeeperClient.CreateNode(tempPath, data, CreateMode.Ephemeral);
        }

        /// <summary>
        /// 监视路径
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="nodeData">节点值</param>
        /// <param name="disconfNodeType">节点类型</param>
        public void WatchPath(string nodeName, string nodeData, DisconfNodeType disconfNodeType)
        {
            string monitorPath = CreateMonitorPath(nodeName, nodeData, disconfNodeType);
            NodeWatcher nodeWatcher = new NodeWatcher(_zooKeeperClient, nodeName, nodeData, disconfNodeType, monitorPath);
            nodeWatcher.Monitor();

        }

        /// <summary>
        /// 关闭Zookeeper
        /// </summary>
        public void Close()
        {
            _zooKeeperClient.Close();
        }
    }
}
