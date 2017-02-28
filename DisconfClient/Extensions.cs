using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DisconfClient
{
    public static class Extensions
    {
        public static T Get<T>(this IDictionary<string, string> dic, string key, T defaultValue = default(T))
        {
            if (dic == null)
                return defaultValue;
            if (!dic.ContainsKey(key))
                return defaultValue;
            DefalutDataConverter converter = new DefalutDataConverter();
            return (T)converter.Parse(typeof(T), dic[key]);
        }

        public static string GetAlias(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                return null;
            AliasAttribute aliasAttribute = Utils.GetFirstAttribute<AliasAttribute>(propertyInfo, true);
            if (aliasAttribute != null && !string.IsNullOrWhiteSpace(aliasAttribute.Name))
                return aliasAttribute.Name.Trim();
            else
                return propertyInfo.Name;
        }

        /// <summary>
        /// 获取完全限定类型名
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>完全限定类型名</returns>
        public static string GetFullTypeName(this Type type)
        {
            if (type == null)
                return string.Empty;
            return string.Concat(type.ToString(), ",", type.Assembly.FullName.Split(',')[0]);
        }
    }
}
