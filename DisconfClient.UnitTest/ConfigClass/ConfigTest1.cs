using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisconfClient.UnitTest.Configs
{
    [Disconf(Name = "JsonDataConverterTestValue1", DataConverterType = typeof(JsonDataConverter))]
    public class ConfigTest1
    {
        public string Host { get; set; }

        public int Port { get; set; }
    }
}
