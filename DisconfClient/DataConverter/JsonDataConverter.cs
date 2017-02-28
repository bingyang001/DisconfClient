using System;
using Newtonsoft.Json;
namespace DisconfClient
{
    public class JsonDataConverter : IDataConverter
    {
        public object Parse(Type type, string value)
        {
            if (type == null)
                return null;
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return JsonConvert.DeserializeObject(value, type);
        }
    }
}
