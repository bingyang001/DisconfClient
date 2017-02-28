using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisconfClient;

namespace DisconfClientDemo.ConfigClass
{
    //当需要配置值发生变更时回调通知，请设定CallbackType
    //当解析配置值需要使用自定义的数据转换器时，请设定DataConverterType
    [Disconf(Name = "redis.properties", CallbackType = typeof(PropertiesDemoConfigCallback),DataConverterType = typeof(PropertiesDataConverter))]
    public class PropertiesDemoConfig
    {
        [Alias("redis.host")]//当配置文件中配置项的key与属性名不一致时需要通过Alias特性来指定别名
        public string Host { get; set; }

        [Alias("redis.port")]
        public int Port { get; set; }
    }

    public class PropertiesDemoConfigCallback : ICallback
    {
        public void Invoke()
        {
            PropertiesDemoConfig redisConfig = ConfigManager.GetConfigClass<PropertiesDemoConfig>();
            Console.WriteLine("PropertiesDemoConfig.Changed:Host:{0},Port:{1}", redisConfig.Host, redisConfig.Port);
        }
    }
}
