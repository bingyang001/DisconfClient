using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace DisconfClient
{
    public class AppSettingsDataConverter : IDataConverter
    {
        public object Parse(Type type, string value)
        {
            if (type == null)
                return null;
            if (string.IsNullOrWhiteSpace(value))
                return null;

            XmlDocument document = new XmlDocument();
            document.LoadXml(value);
            XmlNodeList xmnoNodeList = document.SelectNodes("/appSettings/add");
            if (xmnoNodeList == null)
                return null;
            if (type == typeof(IDictionary<string, string>))
            {
                IDictionary<string, string> dic = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
                foreach (XmlNode xmlNode in xmnoNodeList)
                {
                    if (xmlNode.Attributes == null)
                        continue;
                    string nodeKey = xmlNode.Attributes["key"].Value;
                    string nodeValue = xmlNode.Attributes["value"].Value;
                    dic.Add(nodeKey, nodeValue);
                }
                return dic;
            }
            else
            {
                object obj = Activator.CreateInstance(type, true);
                foreach (XmlNode xmlNode in xmnoNodeList)
                {
                    if (xmlNode.Attributes == null)
                        continue;
                    string nodeKey = xmlNode.Attributes["key"].Value;
                    string nodeValue = xmlNode.Attributes["value"].Value;
                    PropertyInfo propertyInfo = type.GetProperties().FirstOrDefault(m => m != null && string.Compare(m.GetAlias(), nodeKey, StringComparison.OrdinalIgnoreCase) == 0);
                    if (propertyInfo == null) continue;
                    DefalutDataConverter converter = new DefalutDataConverter();
                    object itemValue = converter.Parse(propertyInfo.PropertyType, nodeValue);
                    propertyInfo.SetValue(obj, itemValue, null);
                }
                return obj;
            }
        }
    }
}
