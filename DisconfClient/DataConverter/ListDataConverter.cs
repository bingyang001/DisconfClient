using System;
using System.Collections.Generic;

namespace DisconfClient
{
    public class ListDataConverter : IDataConverter
    {
        public object Parse(Type type, string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            value = value.Replace("\r\n", "\n");
            string[] array = value.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            IList<string> list = new List<string>();
            foreach (string str in array)
            {
                list.Add(str);
            }
            return list;
        }
    }
}
