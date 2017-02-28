using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;

namespace DisconfClient
{
    /// <summary>
    /// 包装后的ZooKeeper客户端
    /// </summary>
    internal class ZooKeeperClient : ConnectionWatcher
    {
        public ZooKeeperClient(IDisconfWebApi webApi)
            : base(webApi)
        {

        }

        /// <summary>
        /// 设置某个路径的值
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="value">值</param>
        public void SetData(string path, string value)
        {
            ActionRetryHelper.Retry(() =>
            {
                Stat stat = ZooKeeper.Exists(path, false);
                byte[] data = value == null ? null : value.GetBytes();
                if (stat == null)
                {
                    ZooKeeper.Create(path, data, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                }
                else
                {
                    ZooKeeper.SetData(path, data, stat.Version);
                }

            }, 3, new TimeSpan(10), () =>
            {
                //exceptionAction
            }, (ex) =>
            {
                //errorHandle
                LogManager.GetLogger().Error(string.Format("DisconfClient.ZooKeeperClient.SetData(path={0},value={1}),ex:{2}", path, value, ex));
            }, () =>
            {
                //retryExceptionAction
                LogManager.GetLogger().Error(string.Format("DisconfClient.ZooKeeperClient.SetData(path={0},value={1}),error", path, value));
            });
        }

        /// <summary>
        /// 创建一个路径节点
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="value">值</param>
        /// <param name="createMode">创建模式</param>
        /// <returns></returns>
        public string CreateNode(string path, string value, CreateMode createMode)
        {
            return ActionRetryHelper.Retry<string>(() =>
            {
                Stat stat = ZooKeeper.Exists(path, false);
                byte[] data = value == null ? null : value.GetBytes();
                if (stat == null)
                {
                    return ZooKeeper.Create(path, data, Ids.OPEN_ACL_UNSAFE, createMode);
                }
                else
                {
                    ZooKeeper.SetData(path, data, -1);
                }
                return path;

            }, 3, new TimeSpan(10), () =>
            {

            }, (ex) =>
            {
                //errorHandle
                LogManager.GetLogger()
                    .Error(string.Format("DisconfClient.ZooKeeperClient.CreateNode(path={0},value={1},createMode={2}),ex:{3}", path, value, createMode, ex));
            });
        }

        /// <summary>
        /// 检查某个路径在Zookeeper中是否已存在
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public bool Exists(string path)
        {
            return ActionRetryHelper.Retry<bool>(() =>
            {
                Stat stat = ZooKeeper.Exists(path, false);
                return stat != null;

            }, 3, new TimeSpan(10), () =>
            {

            }, (ex) =>
            {
                //errorHandle
                LogManager.GetLogger()
                    .Error(string.Format("DisconfClient.ZooKeeperClient.Exists(path={0}),ex:{1}", path, ex));
            });
        }

        /// <summary>
        /// 获取路径的值
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="watcher">监视器</param>
        /// <param name="stat">状态信息</param>
        /// <returns></returns>
        public string GetData(string path, IWatcher watcher, Stat stat = null)
        {
            try
            {
                byte[] data = ZooKeeper.GetData(path, watcher, stat);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.ZooKeeperClient.GetData(path={0}),ex:{1}", path, ex));
                throw;
            }

        }

        /// <summary>
        /// 获根节点下的子节点
        /// </summary>
        /// <returns></returns>
        public List<string> GetRootChildren()
        {
            List<string> list = new List<string>();
            try
            {
                list = ZooKeeper.GetChildren("/", false).ToList();
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.ZooKeeperClient.GetRootChildren,error:{0}", ex));
            }
            return list;
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="path"></param>
        public void DeleteNode(string path)
        {
            try
            {
                ZooKeeper.Delete(path, -1);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.ZooKeeperClient.DeleteNode(path={0}),error:{1}", path, ex));
            }
        }

        public void CreatePersistentPath(string path, string data)
        {
            try
            {
                bool flag = Exists(path);
                if (!flag)
                {
                    SetData(path, data);
                    LogManager.GetLogger().Info(string.Format("DisconfClient.ZooKeeperManager.CreatePersistentPath(path={0},data={1}", path, data));
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.ZooKeeperManager.CreatePersistentPath(path={0},data={1},error:{2}", path, data, ex));
            }
        }

        /// <summary>
        /// 检查某个路径的监视器是否已被注册过了
        /// </summary>
        /// <param name="path">ZooKeeper监视路径</param>
        /// <returns></returns>
        public bool IsExistWatche(string path)
        {
            return ZooKeeper.DataWatches.Contains(path);
        }
    }
}
