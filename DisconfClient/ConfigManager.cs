using System.Configuration;
using System.Reflection;
using DisconfClient.DataConverter;
using ZooKeeperNet;

namespace DisconfClient
{
    public static class ConfigManager
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="disconfWebApi"></param>
        /// <param name="assemblies"></param>
        public static void Init(IDisconfWebApi disconfWebApi = null, params Assembly[] assemblies)
        {
            ConfigStorageManager.Init(disconfWebApi, assemblies);
        }

        public static T GetConfigClass<T>()
        {
            return (T)ConfigClassMapper.GetConfigClassInstance(typeof(T));
        }

        public static T GetConfigValue<T>(string key, T defalut = default(T))
        {
            ConfigStorageItem item = ConfigStorageManager.GetConfigStorageItem(key);
            if (item == null)
                return defalut;
            if (typeof(T) == typeof(string))
                return (T)((object)item.Data);

            IDataConverter dataConverter = DataConverterManager.GetDataConverter(key);
            if (dataConverter == null)
            {
                if (key.EndsWith(".json"))
                {
                    dataConverter = new JsonDataConverter();
                }
                else if (key.EndsWith(".config"))
                {
                    dataConverter = new AppSettingsDataConverter();
                }
                else if (key.EndsWith(".properties"))
                {
                    dataConverter = new PropertiesDataConverter();
                }
                else
                {
                    dataConverter = new DefalutDataConverter();
                }
            }
            return (T)dataConverter.Parse(typeof(T), item.Data);
        }

        /// <summary>
        /// 读取本地AppSettings中的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T AppSettings<T>(string key, T defaultValue = default(T))
        {
            string value = ConfigurationManager.AppSettings[key];
            if (value == null)
                return defaultValue;
            DefalutDataConverter converter = new DefalutDataConverter();
            return (T)converter.Parse(typeof(T), value);
        }

    }
}
