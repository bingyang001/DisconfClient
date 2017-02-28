using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisconfClient.UnitTest
{
    public class MockDisconfWebApi : IDisconfWebApi
    {
        public List<ConfigItem> Configs = new List<ConfigItem>();

        public MockDisconfWebApi()
        {
            AddConfigItemContentApiResult("DefalutDataConverterTestValue1", Constant.DefalutDataConverterTestValue1);
            AddConfigItemContentApiResult("DefalutDataConverterTestValue2", Constant.DefalutDataConverterTestValue2);

            AddConfigItemContentApiResult("DefalutDataConverterTestValue3", Constant.DefalutDataConverterTestValue3);
            AddConfigItemContentApiResult("DefalutDataConverterTestValue4", Constant.DefalutDataConverterTestValue4);
            AddConfigItemContentApiResult("DefalutDataConverterTestValue5", Constant.DefalutDataConverterTestValue5);
            AddConfigItemContentApiResult("DefalutDataConverterTestValue6", Constant.DefalutDataConverterTestValue6);
            AddConfigItemContentApiResult("DefalutDataConverterTestValue7", Constant.DefalutDataConverterTestValue7);
            AddConfigItemContentApiResult("DefalutDataConverterTestValue8", Constant.DefalutDataConverterTestValue8);
            AddConfigItemContentApiResult("DefalutDataConverterTestValue9", Constant.DefalutDataConverterTestValue9);
            AddConfigItemContentApiResult("DefalutDataConverterTestValue10", Constant.DefalutDataConverterTestValue10);


            AddConfigItemContentApiResult("PropertiesDataConverterTestValue1", Constant.PropertiesDataConverterTestValue1);
            AddConfigItemContentApiResult("propertiesTest1.properties", Constant.PropertiesDataConverterTestValue1, DisconfNodeType.File);
            AddConfigItemContentApiResult("redis.properties", Constant.PropertiesDataConverterTestValue2);

            AddConfigItemContentApiResult("JsonDataConverterTestValue1", Constant.JsonDataConverterTestValue1);
            AddConfigItemContentApiResult("jsonTest1.json", Constant.JsonDataConverterTestValue1, DisconfNodeType.File);

            AddConfigItemContentApiResult("AppSettingsDataConverterTestValue1", Constant.AppSettingsDataConverterTestValue1);
            AddConfigItemContentApiResult("appSettingTest1.config", Constant.AppSettingsDataConverterTestValue1, DisconfNodeType.File);
        }

        public string GetZooKeeperHost()
        {
            return "127.0.0.1:8080";
        }

        public string GetZooKeeperRootNode()
        {
            return "disconf";
        }

        public string GetConfigFileContent(string name)
        {
            ConfigItem configItem = Configs.FirstOrDefault(m => m != null && m.Name == name);
            return configItem == null ? null : configItem.Data;
        }

        public string GetConfigItemContent(string name)
        {
            ConfigItem configItem = Configs.FirstOrDefault(m => m != null && m.Name == name);
            return configItem == null ? null : configItem.Data;
        }

        public IList<ConfigMetadataApiResult> GetConfigMetadatas()
        {
            List<ConfigMetadataApiResult> list = new List<ConfigMetadataApiResult>();
            foreach (ConfigItem configItem in Configs)
            {
                if (configItem == null)
                    continue;
                ConfigMetadataApiResult apiResult = new ConfigMetadataApiResult
                {
                    Name = configItem.Name,
                    Type = configItem.DisconfNodeType,
                    UpdateTime = configItem.UpdateTime
                };
                list.Add(apiResult);
            }
            return list;
        }

        public ConfigMetadataApiResult GetConfigMetadata(string name)
        {
            ConfigMetadataApiResult apiResult = null;
            foreach (ConfigItem configItem in Configs)
            {
                if (configItem == null)
                    continue;
                if (configItem.Name == name)
                {
                    apiResult = new ConfigMetadataApiResult
                    {
                        Name = configItem.Name,
                        Type = configItem.DisconfNodeType,
                        UpdateTime = configItem.UpdateTime
                    };
                }
            }
            return apiResult;
        }

        public IList<ConfigItemContentApiResult> GetAllConfigItemContent()
        {
            IList<ConfigItemContentApiResult> list = new List<ConfigItemContentApiResult>();
            foreach (ConfigItem configItem in Configs)
            {
                if (configItem == null || configItem.DisconfNodeType == DisconfNodeType.File)
                    continue;
                ConfigItemContentApiResult apiResult = new ConfigItemContentApiResult
                {
                    Name = configItem.Name,
                    Value = configItem.Data
                };
                list.Add(apiResult);
            }


            return list;
        }

        public void AddConfigItemContentApiResult(string name, string data, DisconfNodeType disconfNodeType = DisconfNodeType.Item, string version = "1.0")
        {
            ConfigItem configItem = new ConfigItem
            {
                Name = name,
                DisconfNodeType = disconfNodeType,
                Data = data,
                UpdateTime = version
            };
            Configs.Add(configItem);
        }

        public ConfigItem GetConfigItem(string name)
        {
            return Configs.FirstOrDefault(m => m != null && m.Name == name);
        }

    }

    public class ConfigItem
    {
        public string Name { get; set; }

        public string UpdateTime { get; set; }

        public DisconfNodeType DisconfNodeType { get; set; }

        public string Data { get; set; }
    }
}
