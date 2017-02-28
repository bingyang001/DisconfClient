using System;
using System.Collections.Generic;

namespace DisconfClient
{
    public class DictionaryDataConverter : IDataConverter
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

            IDictionary<string, string> dic = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            string[] items = value.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in items)
            {
                if (!value.Contains("="))
                    continue;
                string[] keyValuePairs = item.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (keyValuePairs.Length != 2) continue;
                string strKey = keyValuePairs[0].Trim();
                string strValue = keyValuePairs[1].Trim();
                dic.Add(new KeyValuePair<string, string>(strKey, strValue));
            }
            return dic;
        }
    }
}
