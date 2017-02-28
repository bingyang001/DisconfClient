using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DisconfClient
{
    /// <summary>
    /// Disconf Zookeeper 监视路径管理器
    /// </summary>
    internal static class PathManager
    {
        /// <summary>
        ///  构建一个监视路径的基础部分
        /// </summary>
        /// <param name="rootNode">根节点</param>
        /// <param name="appId">应用编号</param>
        /// <param name="env">环境名称</param>
        /// <param name="version">版本</param>
        /// <returns></returns>
        public static string GetBasePath(string rootNode, string appId, string env, string version)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(rootNode);
            sb.Append("/");
            sb.Append(appId);
            sb.Append("_");
            sb.Append(version);
            sb.Append("_");
            sb.Append(env);
            return sb.ToString();
        }

        /// <summary>
        /// 合并两个路径
        /// </summary>
        /// <param name="path1">路径1</param>
        /// <param name="path2">路径2</param>
        /// <returns></returns>
        public static string JoinPath(string path1, string path2)
        {
            if (path1 != null)
                path1 = path1.TrimEnd('/');
            if (path2 != null)
                path2 = path2.TrimStart('/');
            return string.Concat(path1, "/", path2);
        }

        /// <summary>
        /// 获取配置项的路径
        /// </summary>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static string GetItemPath(string basePath)
        {
            return basePath + "/item";
        }

        /// <summary>
        /// 获取配置文件的路径
        /// </summary>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static string GetFilePath(string basePath)
        {
            return basePath + "/file";
        }

    }
}
