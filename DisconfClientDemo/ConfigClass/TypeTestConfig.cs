using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisconfClient;

namespace DisconfClientDemo.ConfigClass
{
    public class TypeTestConfig
    {
        [Disconf(Name = "TestString", CallbackType = typeof(TestStringCallback))]
        public string TestString { get; set; }

        [Disconf(Name = "TestInt")]
        public int TestInt { get; set; }

        [Disconf(Name = "TestLong")]
        public long TestLong { get; set; }

        [Disconf(Name = "TestUint")]
        public uint TestUint { get; set; }

        [Disconf(Name = "TestUlong")]
        public ulong TestUlong { get; set; }

        [Disconf(Name = "TestFloat")]
        public float TestFloat { get; set; }

        [Disconf(Name = "TestDouble")]
        public double TestDouble { get; set; }

        [Disconf(Name = "TestDecimal")]
        public decimal TestDecimal { get; set; }

        [Disconf(Name = "TestEnumInt")]
        public DisconfNodeType TestEnumInt { get; set; }

        [Disconf(Name = "TestEnumString")]
        public DisconfNodeType TestEnumString { get; set; }

        [Disconf(Name = "TestGuid")]
        public Guid TestGuid { get; set; }

        [Disconf(Name = "TestType")]
        public Type TestType { get; set; }

        [Disconf(Name = "TestBool")]
        public bool TestBool { get; set; }

        [Disconf(Name = "TestChar")]
        public char TestChar { get; set; }

        [Disconf(Name = "TestDateTime")]
        public DateTime TestDateTime { get; set; }

        [Disconf(Name = "TestStringList")]
        public IList<string> TestStringList { get; set; }

        [Disconf(Name = "TestStringDictionary")]
        public IDictionary<string, string> TestStringDictionary { get; set; }
    }

    public class TestStringCallback : ICallback
    {
        public void Invoke()
        {
            TypeTestConfig configTest= ConfigManager.GetConfigClass<TypeTestConfig>();
            Console.WriteLine("TypeTestConfig.TestString Changed: {0}", configTest.TestString);
        }
    }
}
