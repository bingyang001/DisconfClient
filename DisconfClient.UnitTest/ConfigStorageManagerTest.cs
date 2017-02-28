using System;
using System.Collections.Generic;
using System.Globalization;
using DisconfClient.UnitTest.Configs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DisconfClient.UnitTest
{
    [TestClass]
    public class ConfigStorageManagerTest
    {
        private readonly MockDisconfWebApi _webApi = null;
        public ConfigStorageManagerTest()
        {
            _webApi = new MockDisconfWebApi();
            ConfigManager.Init(_webApi, typeof(ConfigTest1).Assembly);
        }

        [TestMethod]
        public void ConfigTest1Test()
        {
            ConfigTest1 configTest1 = ConfigManager.GetConfigClass<ConfigTest1>();
            Assert.AreEqual("127.0.0.1", configTest1.Host);
            Assert.AreEqual(81, configTest1.Port);
            IDictionary<string, string> configTest1Dictionary = ConfigManager.GetConfigValue<IDictionary<string, string>>("PropertiesDataConverterTestValue1");
            Assert.AreEqual("127.0.0.1", configTest1Dictionary.Get<string>("Host"));
            Assert.AreEqual(81, configTest1Dictionary.Get<int>("Port"));
        }

        [TestMethod]
        public void ConfigTest2Test()
        {
            ConfigTest2 configTest2 = ConfigManager.GetConfigClass<ConfigTest2>();
            Assert.AreEqual("127.0.0.1", configTest2.Host);
            Assert.AreEqual(81, configTest2.Port);
            IDictionary<string, string> configTest1Dictionary = ConfigManager.GetConfigValue<IDictionary<string, string>>("propertiesTest1.properties");
            Assert.AreEqual("127.0.0.1", configTest1Dictionary.Get<string>("Host"));
            Assert.AreEqual(81, configTest1Dictionary.Get<int>("Port"));

            IDictionary<string, string> dictionary =
               ConfigManager.GetConfigValue<IDictionary<string, string>>("propertiesTest1.properties");
            Assert.IsNotNull(dictionary);
            Assert.AreEqual("127.0.0.1", dictionary.Get<string>("Host"));
            Assert.AreEqual(81, dictionary.Get<int>("Port"));
        }

        [TestMethod]
        public void ConfigTest3Test()
        {
            ConfigTest3 configTest3 = ConfigManager.GetConfigClass<ConfigTest3>();
            Assert.AreEqual("127.0.0.1", configTest3.Host);
            Assert.AreEqual(81, configTest3.Port);

            ConfigTest3 configTest = ConfigManager.GetConfigValue<ConfigTest3>("jsonTest1.json");
            Assert.IsNotNull(configTest);
            Assert.AreEqual("127.0.0.1", configTest.Host);
            Assert.AreEqual(81, configTest.Port);

            IDictionary<string, string> dictionary =
                ConfigManager.GetConfigValue<IDictionary<string, string>>("jsonTest1.json");
            Assert.IsNotNull(dictionary);
            Assert.AreEqual("127.0.0.1", dictionary.Get<string>("Host"));
            Assert.AreEqual(81, dictionary.Get<int>("Port"));
        }

        [TestMethod]
        public void ConfigTest4Test()
        {
            ConfigTest4 configTest4 = ConfigManager.GetConfigClass<ConfigTest4>();
            Assert.AreEqual("127.0.0.1", configTest4.Host);
            Assert.AreEqual(81, configTest4.Port);

            ConfigTest4 configTest = ConfigManager.GetConfigValue<ConfigTest4>("appSettingTest1.config");
            Assert.IsNotNull(configTest);
            Assert.AreEqual("127.0.0.1", configTest.Host);
            Assert.AreEqual(81, configTest.Port);

            IDictionary<string, string> dictionary =
                ConfigManager.GetConfigValue<IDictionary<string, string>>("appSettingTest1.config");
            Assert.IsNotNull(dictionary);
            Assert.AreEqual("127.0.0.1", dictionary.Get<string>("Host"));
            Assert.AreEqual(81, dictionary.Get<int>("Port"));
        }

        [TestMethod]
        public void ConfigTest5Test()
        {
            ConfigTest5 configTest5 = ConfigManager.GetConfigClass<ConfigTest5>();
            Assert.AreEqual(Constant.DefalutDataConverterTestValue1, configTest5.TestString);
            Assert.AreEqual(int.Parse(Constant.DefalutDataConverterTestValue2), configTest5.TestInt);
            Assert.AreEqual(long.Parse(Constant.DefalutDataConverterTestValue2), configTest5.TestLong);
            Assert.AreEqual(uint.Parse(Constant.DefalutDataConverterTestValue2), configTest5.TestUint);
            Assert.AreEqual(ulong.Parse(Constant.DefalutDataConverterTestValue2), configTest5.TestUlong);
            Assert.AreEqual(float.Parse(Constant.DefalutDataConverterTestValue3), configTest5.TestFloat);
            Assert.AreEqual(double.Parse(Constant.DefalutDataConverterTestValue3), configTest5.TestDouble);
            Assert.AreEqual(decimal.Parse(Constant.DefalutDataConverterTestValue3), configTest5.TestDecimal);
            Assert.AreEqual((DisconfNodeType)(int.Parse(Constant.DefalutDataConverterTestValue4)), configTest5.TestEnumInt);
            Assert.AreEqual(Enum.Parse(typeof(DisconfNodeType), Constant.DefalutDataConverterTestValue9, true), configTest5.TestEnumString);
            Assert.AreEqual((Guid)Guid.Parse(Constant.DefalutDataConverterTestValue5), configTest5.TestGuid);
            Assert.AreEqual((Type)Type.GetType(Constant.DefalutDataConverterTestValue6), configTest5.TestType);
            Assert.AreEqual((bool)bool.Parse(Constant.DefalutDataConverterTestValue7), configTest5.TestBool);
            Assert.AreEqual((char)char.Parse(Constant.DefalutDataConverterTestValue8), configTest5.TestChar);
            Assert.AreEqual((DateTime)DateTime.Parse(Constant.DefalutDataConverterTestValue10), configTest5.TestDateTime);

            Assert.IsNotNull(configTest5.TestStringList);
            Assert.AreEqual(2, configTest5.TestStringList.Count);
            Assert.AreEqual("host=127.0.0.1", configTest5.TestStringList[0]);
            Assert.AreEqual("port=81", configTest5.TestStringList[1]);

            Assert.IsNotNull(configTest5.TestStringDictionary);
            Assert.AreEqual(2, configTest5.TestStringDictionary.Count);
            Assert.AreEqual("127.0.0.1", configTest5.TestStringDictionary["Host"]);
            Assert.AreEqual("81", configTest5.TestStringDictionary["Port"]);

            _webApi.GetConfigItem("DefalutDataConverterTestValue1").Data = "Hello!";
            ConfigStorageManager.ReloadConfigItem(new ConfigMetadataApiResult() { Name = "DefalutDataConverterTestValue1", UpdateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture) });

            Assert.AreEqual(Constant.DefalutDataConverterTestValue1, configTest5.TestString);

            ConfigTest5 newConfigTest5 = ConfigManager.GetConfigClass<ConfigTest5>();
            Assert.AreEqual("Hello!", newConfigTest5.TestString);

        }

        [TestMethod]
        public void RedisConfigTest()
        {
            RedisConfig redisConfig = ConfigManager.GetConfigClass<RedisConfig>();
            Assert.AreEqual("127.0.0.1", redisConfig.Host);
            Assert.AreEqual(81, redisConfig.Port);

            IDictionary<string, string> dictionary = ConfigManager.GetConfigValue<IDictionary<string, string>>("redis.properties");
            Assert.IsNotNull(dictionary);
            Assert.AreEqual("127.0.0.1", dictionary.Get<string>("redis_host"));
            Assert.AreEqual(81, dictionary.Get<int>("redis_port"));

            _webApi.GetConfigItem("redis.properties").Data = "redis_host=192.168.1.10\r\nredis_port=82";

            ConfigStorageManager.ReloadConfigItem(new ConfigMetadataApiResult() { Name = "redis.properties" });

            redisConfig = ConfigManager.GetConfigClass<RedisConfig>();
            Assert.AreEqual("192.168.1.10", redisConfig.Host);
            Assert.AreEqual(82, redisConfig.Port);
        }
    }
}
