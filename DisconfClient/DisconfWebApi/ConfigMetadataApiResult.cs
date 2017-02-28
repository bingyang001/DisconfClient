using Newtonsoft.Json.Serialization;

namespace DisconfClient
{
    /// <summary>
    /// 获取某个应用下全部的配置项、配置文件信息（不包含值）的接口返回结果
    /// </summary>
    public class ConfigMetadataApiResult
    {
        public string Name { get; set; }

        public string UpdateTime { get; set; }

        public DisconfNodeType Type { get; set; }
    }

    /// <summary>
    /// 获取某个应用下全部的配置项信息（包含值）的接口返回结果
    /// </summary>
    public class ConfigItemContentApiResult
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public enum DisconfNodeType
    {
        File = 0,
        Item = 1
    }
}
