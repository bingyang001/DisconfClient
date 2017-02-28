using System;
using System.Collections;
using System.Reflection;

namespace DisconfClient
{
    /// <summary>
    /// 配置类映射器
    /// </summary>
    internal class ConfigClassMapper
    {
        private static readonly Hashtable Mappers = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 设置配置类的实例
        /// </summary>
        /// <param name="type">配置类的类型</param>
        /// <param name="instance">配置类的实例</param>
        public static void SetConfigClassInstance(Type type, object instance)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            Mappers[type] = instance;
        }

        /// <summary>
        /// 获取配置类的实例
        /// </summary>
        /// <param name="type">配置类的类型</param>
        /// <returns></returns>
        public static object GetConfigClassInstance(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return Mappers[type];
        }

        /// <summary>
        /// 配置项的类型
        /// </summary>
        //public DisconfNodeType DisconfNodeType { get; set; }

        /// <summary>
        /// 配置类的类型
        /// </summary>
        public Type ConfigClassType { get; set; }

        /// <summary>
        /// 配置属性的类型
        /// </summary>
        public PropertyInfo ConfigPropertyInfo { get; set; }

        /// <summary>
        /// 配置项的名称
        /// </summary>
        public string ConfigNodeName { get; set; }

    }


}
