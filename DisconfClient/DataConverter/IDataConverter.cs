using System;

namespace DisconfClient
{
    /// <summary>
    /// 数据转换器
    /// </summary>
    public interface IDataConverter
    {
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        object Parse(Type type, string value);
    }
}
