using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DisconfClient;
using Newtonsoft.Json;

namespace DisconfClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ConfigManager.Init();
                //ConfigManager.Init(null, typeof(RedisProperties).Assembly);
                ConfigCallbackManager.RegisterCallback("node1", new NodeCallback("node1"));
                ConfigCallbackManager.RegisterCallback("node2", new NodeCallback("node2"));
                while (true)
                {
                    string cmd = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(cmd))
                        continue;
                    if (cmd == "print")
                    {
                        string json = ConfigStorageManager.PrintConfigItems();
                        Console.WriteLine(json);
                        continue;
                    }
                    if (cmd == "get redis.properties")
                    {
                        RedisProperties redisProperties = ConfigManager.GetConfigClass<RedisProperties>();
                        string json = redisProperties == null ? "" : JsonConvert.SerializeObject(redisProperties);
                        Console.WriteLine(json);
                        continue;
                    }

                    if (cmd == "get redis.config")
                    {
                        RedisConfig redisConfig = ConfigManager.GetConfigClass<RedisConfig>();
                        string json = redisConfig == null ? "" : JsonConvert.SerializeObject(redisConfig);
                        Console.WriteLine(json);
                        continue;
                    }

                    if (cmd == "get ItemNodeCofig")
                    {
                        ItemNodeCofig config = ConfigManager.GetConfigClass<ItemNodeCofig>();
                        string json = config == null ? "" : JsonConvert.SerializeObject(config);
                        Console.WriteLine(json);
                        continue;
                    }

                    if (cmd.StartsWith("get "))
                    {
                        string[] array = cmd.Split(' ');
                        if (array.Length == 2)
                        {
                            string key = array[1];
                            string value = ConfigManager.GetConfigValue<string>(key);
                            Console.WriteLine(value);
                        }
                        continue;
                    }
                    if (cmd == "close")
                    {
                        ConfigStorageManager.CloseZooKeeperClient();
                        continue;
                    }
                    if (cmd == "clear")
                    {
                        Console.Clear();
                        continue;
                    }

                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(ex.Message, ex);
                Console.WriteLine(ex);
            }

            Console.ReadLine();
        }
    }

    class NodeCallback : ICallback
    {
        private readonly string _name;

        public NodeCallback(string name)
        {
            this._name = name;
        }
        public void Invoke()
        {
            Console.WriteLine("Callback,Name:{0},Data:{1}", _name, ConfigManager.GetConfigValue<string>(_name));
        }
    }

    [Disconf(Name = "redis.properties",  CallbackType = typeof(RedisPropertiesCallback))]
    public class RedisProperties
    {
        [Alias("redis.host")]
        public string Host { get; set; }

        [Alias("redis.port")]
        public int Port { get; set; }
    }

    public class RedisPropertiesCallback : ICallback
    {
        public void Invoke()
        {
            RedisProperties redisProperties = ConfigManager.GetConfigClass<RedisProperties>();
            Console.WriteLine("Callback,RedisPropertiesChanged:Host:{0},Port:{1}", redisProperties.Host, redisProperties.Port);
        }
    }

    [Disconf(Name = "redis1.config")]
    public class RedisConfig
    {
        public string Host { get; set; }

        public int Port { get; set; }
    }


    public class ItemNodeCofig
    {
        [Disconf(Name = "item_node1", CallbackType = typeof(Node1Callback))]
        public string Node1 { get; set; }

        [Disconf(Name = "item_node2", CallbackType = typeof(Node2Callback))]
        public string Node2 { get; set; }

        [Disconf(Name = "item_node3")]
        public string Node3 { get; set; }
    }

    public class Node1Callback : ICallback
    {
        public void Invoke()
        {
            ItemNodeCofig itemNodeCofig = ConfigManager.GetConfigClass<ItemNodeCofig>();
            Console.WriteLine("Callback,:Node1:{0}", itemNodeCofig.Node1);
        }
    }

    public class Node2Callback : ICallback
    {
        public void Invoke()
        {
            ItemNodeCofig itemNodeCofig = ConfigManager.GetConfigClass<ItemNodeCofig>();
            Console.WriteLine("Callback,:Node2:{0}", itemNodeCofig.Node2);
        }
    }
}
