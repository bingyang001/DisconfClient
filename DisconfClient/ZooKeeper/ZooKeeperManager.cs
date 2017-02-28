using System;
using System.Collections.Generic;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;

namespace DisconfClient
{
    internal static class ZooKeeperManager
    {
        private static string _host;
        private static string _rootNode;
        private static ZooKeeperClient _client;


        private static readonly object SyncRoot = new object();
        public static void Init(string host, string rootNode)
        {
            lock (SyncRoot)
            {
                _host = host;
                _rootNode = rootNode;
                _client = new ZooKeeperClient();
                _client.Connect(host);
            }
        }

        public static void ReConnect()
        {
            if(_client==null)
                return;
            
            lock (SyncRoot)
            {
                 _client.ReConnect();
            }

        }

        public static void CreatePersistentPath(string path, string data)
        {
            if (_client == null)
                return;
            try
            {
                lock (SyncRoot)
                {
                    bool flag = _client.Exists(path);
                    if (!flag)
                    {
                        SetData(path, data);
                        LogManager.GetLogger().Info(string.Format("DisconfClient.ZooKeeperManager.CreatePersistentPath(path={0},data={1}", path, data));
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.ZooKeeperManager.CreatePersistentPath(path={0},data={1},error:{2}", path, data, ex));
            }
        }

        public static void SetData(string path, string data)
        {
            if (_client == null)
                return;
            lock (SyncRoot)
            {
                _client.SetData(path, data);
            }
        }

        public static void Close()
        {
            if (_client == null)
                return;
            lock (SyncRoot)
            {
                _client.Close();
            }
        }

        public static List<string> GetRootChildren()
        {
            if (_client == null)
                return null;
            lock (SyncRoot)
            {
                return _client.GetRootChildren();
            }
        }

        public static string GetData(string path, IWatcher watcher)
        {
            if (_client == null)
                return null;
            lock (SyncRoot)
            {
                return _client.GetData(path, watcher, null);
            }
        }

        public static bool Exists(string path)
        {
            if (_client == null)
                return false;
            lock (SyncRoot)
            {
                return _client.Exists(path);
            }
        }

        public static string CreateNode(string path, string data, CreateMode createMode)
        {
            if (_client == null)
                return "";
            lock (SyncRoot)
            {
                return _client.CreateNode(path, data, createMode);
            }
        }

        public static string GetData(string path, IWatcher watcher, Stat stat)
        {
            if (_client == null)
                return "";
            lock (SyncRoot)
            {
                return _client.GetData(path, watcher, stat);
            }
        }

        public static void DeleteNode(string path)
        {
            if (_client == null)
                return;
            lock (SyncRoot)
            {
                _client.DeleteNode(path);
            }
        }

        public static bool IsExistWatche(string path)
        {
            if (_client == null)
                return false;
            lock (SyncRoot)
            {
                return _client.IsExistWatche(path);
            }
        }

        public static void MonitorZooKeeperState()
        {
            if (_client == null)
                return;
            _client.MonitorZooKeeperState();
        }
    }
}
