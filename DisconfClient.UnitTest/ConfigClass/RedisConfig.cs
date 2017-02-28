using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DisconfClient.UnitTest
{
    [Disconf(Name = "redis.properties", CallbackType = typeof(RedisConfigCallback))]
    public class RedisConfig
    {
        [Alias("redis_host")]
        public string Host { get; set; }

        [Alias("redis_port")]
        public int Port { get; set; }
    }

    public class RedisConfigCallback : ICallback
    {
        public void Invoke()
        {
            RedisConfig redisConfig = ConfigManager.GetConfigClass<RedisConfig>();
            Assert.AreEqual("192.168.1.10", redisConfig.Host);
            Assert.AreEqual(82, redisConfig.Port);
            Console.WriteLine("RedisConfigChanged:Host:{0},Port:{1}", redisConfig.Host, redisConfig.Port);
        }
    }

}
