using System;
using System.Collections.Generic;

namespace DisconfClient
{
    public class DefalutDataConverter : IDataConverter
    {
        public object Parse(Type type, string value)
        {
            if (type == null)
                return value;
            if (value == null)
                return null;
            else
                value = value.Trim();

            if (type.IsEnum)
            {
                int i;
                bool flag = int.TryParse(value, out i);
                return !flag ? Enum.Parse(type, value, true) : Enum.ToObject(type, i);
            }

            if (type == typeof(Guid))
            {
                return new Guid(value);
            }

            if (type == typeof(Type))
            {
                return Type.GetType(value, true);
            }

            if (type.GetInterface("IConvertible") != null)
            {
                return Convert.ChangeType(value, type);
            }

            if (type == typeof(IList<string>))
            {
                IDataConverter dataConverter = new ListDataConverter();
                return dataConverter.Parse(type, value);
            }

            if (type == typeof(IDictionary<string, string>))
            {
                IDataConverter dataConverter = new DictionaryDataConverter();
                return dataConverter.Parse(type, value);
            }

            throw new Exception(string.Format("类型：{0} 目前还未被默认的数据转换器所支持，请自行实现数据转换器！", type));
        }
    }
}
