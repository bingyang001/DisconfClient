using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisconfClient.UnitTest
{
    public class ConfigTest5
    {
        [Disconf(Name = "DefalutDataConverterTestValue1", CallbackType = typeof(TestStringCallback))]
        public string TestString { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue2")]
        public int TestInt { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue2")]
        public long TestLong { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue2")]
        public uint TestUint { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue2")]
        public ulong TestUlong { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue3")]
        public float TestFloat { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue3")]
        public double TestDouble { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue3")]
        public decimal TestDecimal { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue4")]
        public DisconfNodeType TestEnumInt { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue9")]
        public DisconfNodeType TestEnumString { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue5")]
        public Guid TestGuid { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue6")]
        public Type TestType { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue7")]
        public bool TestBool { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue8")]
        public char TestChar { get; set; }

        [Disconf(Name = "DefalutDataConverterTestValue10")]
        public DateTime TestDateTime { get; set; }

        [Disconf(Name = "PropertiesDataConverterTestValue1")]
        public IList<string> TestStringList { get; set; }

        [Disconf(Name = "PropertiesDataConverterTestValue1")]
        public IDictionary<string, string> TestStringDictionary { get; set; }

    }

    public class TestStringCallback : ICallback
    {
        public void Invoke()
        {
            ConfigTest5 configTest5 = ConfigManager.GetConfigClass<ConfigTest5>();
            Console.WriteLine("ConfigTest5.TestString Changed: {0}", configTest5.TestString);
        }
    }
}
