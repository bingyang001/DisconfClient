using System;
using System.Collections.Generic;

namespace DisconfClient.DataConverter
{
    public static class DataConverterManager
    {
        private static readonly IDictionary<string, IDataConverter> DataConverters = new Dictionary<string, IDataConverter>(StringComparer.CurrentCultureIgnoreCase);
        private static readonly object SyncRoot = new object();

        public static void RegisterDataConverter(string name, IDataConverter dataConverter)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (dataConverter == null)
                throw new ArgumentNullException("dataConverter");
            lock (SyncRoot)
            {
                if (!DataConverters.ContainsKey(name))
                    DataConverters.Add(name, dataConverter);
            }
        }

        public static void RegisterDataConverter(Type configClassType, IDataConverter dataConverter)
        {
            if (configClassType == null)
                throw new ArgumentNullException("configClassType");
            RegisterDataConverter(configClassType.GetFullTypeName(), dataConverter);
        }

        public static IDataConverter GetDataConverter(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            return DataConverters.ContainsKey(name) ? DataConverters[name] : null;
        }

        public static IDataConverter GetDataConverter(Type configClassType)
        {
            return configClassType == null ? null : GetDataConverter(configClassType.GetFullTypeName());
        }
    }
}
