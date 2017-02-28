using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace DisconfClient
{
    public class PropertiesDataConverter : IDataConverter
    {
        public object Parse(Type type, string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            if (!value.Contains("="))
                throw new Exception("格式错误，正确格式形如：" +
                                    "Key1=Value1" + Environment.NewLine +
                                    "Key2=Value2");
            value = value.Replace("\r\n", "\n");
            string[] items = value.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (type == typeof(IDictionary<string, string>))
            {
                IDictionary<string, string> dic = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
                foreach (string item in items)
                {
                    if (!value.Contains("="))
                        continue;
                    string[] keyValuePairs = item.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (keyValuePairs.Length != 2) continue;
                    string strKey = keyValuePairs[0].Trim();
                    string strValue = keyValuePairs[1].Trim();
                    dic.Add(strKey, strValue);
                }
                return dic;
            }
            else
            {
                object obj = Activator.CreateInstance(type, true);
                foreach (string item in items)
                {
                    if (!value.Contains("="))
                        continue;
                    string[] keyValuePairs = item.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (keyValuePairs.Length != 2) continue;
                    string strKey = keyValuePairs[0].Trim();
                    string strValue = keyValuePairs[1].Trim();

                    PropertyInfo propertyInfo = type.GetProperties().FirstOrDefault(m => m != null && string.Compare(m.GetAlias(), strKey, StringComparison.OrdinalIgnoreCase) == 0);
                    if (propertyInfo == null) continue;
                    DefalutDataConverter converter = new DefalutDataConverter();
                    object itemValue = converter.Parse(propertyInfo.PropertyType, strValue);
                    propertyInfo.SetValue(obj, itemValue, null);
                }
                return obj;
            }
        }
    }
}
