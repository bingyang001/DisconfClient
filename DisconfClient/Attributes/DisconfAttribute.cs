using System;

namespace DisconfClient
{

    /// <summary>
    /// 配置特性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class DisconfAttribute : Attribute
    {
        /// <summary>
        /// 配置项名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 配置项的类型
        /// </summary>
        //public DisconfNodeType DisconfNodeType { get; set; }

        /// <summary>
        /// 配置值的数据转换器的类型
        /// </summary>
        public Type DataConverterType { get; set; }

        /// <summary>
        /// 当配置值发生变化时的回调类的类型
        /// </summary>
        public Type CallbackType { get; set; }
    }
}
