using System;
using System.Collections.Generic;

namespace DisconfClient
{
    /// <summary>
    /// 配置变更回调管理器
    /// </summary>
    public static class ConfigCallbackManager
    {
        private static readonly SynchronizedDictionary<string, ICallback> CallbackContainer = new SynchronizedDictionary<string, ICallback>();

        /// <summary>
        ///  注册回调类
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <param name="callback">回调类的实例</param>
        public static void RegisterCallback(string name, ICallback callback)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (!CallbackContainer.ContainsKey(name))
            {
                CallbackContainer.Add(name, callback);
                LogManager.GetLogger().Info(string.Format("DisconfClient.ConfigCallbackManager.RegisterCallback(name={0},callback={1}", name, callback.GetType().GetFullTypeName()));
            }
        }

        /// <summary>
        /// 获取回调实例
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <returns></returns>
        public static ICallback GetCallback(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            return !CallbackContainer.ContainsKey(name) ? null : CallbackContainer[name];
        }

    }
}
