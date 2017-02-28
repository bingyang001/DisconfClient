using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisconfClient.UnitTest
{
    public class Constant
    {
        public const string DefalutDataConverterTestValue1 = "Hello123456!";//string
        public const string DefalutDataConverterTestValue2 = "123456";//int,long,uint,ulong
        public const string DefalutDataConverterTestValue3 = "135.25";//float/double/decimal
        public const string DefalutDataConverterTestValue4 = "1";//enum (int)
        public const string DefalutDataConverterTestValue5 = "eb3dfdde-ce88-49fd-b89e-25945858df4e";//Guid
        public const string DefalutDataConverterTestValue6 = "DisconfClient.UnitTest.Configs.ConfigTest1,DisconfClient.UnitTest";//Type
        public const string DefalutDataConverterTestValue7 = "true";//bool
        public const string DefalutDataConverterTestValue8 = "a";//char
        public const string DefalutDataConverterTestValue9 = "file";//enum (string)
        public const string DefalutDataConverterTestValue10 = "2015-10-12 10:10";//DateTime


        public const string PropertiesDataConverterTestValue1 = "host=127.0.0.1\r\nport=81";
        public const string PropertiesDataConverterTestValue2 = "redis_host=127.0.0.1\r\nredis_port=81";
        public const string JsonDataConverterTestValue1 = "{\"Host\":\"127.0.0.1\",\"Port\":81}";
        public const string AppSettingsDataConverterTestValue1 = "<?xml version=\"1.0\" encoding=\"utf-8\"?><appSettings><add key=\"host\" value=\"127.0.0.1\"/><add key=\"port\" value=\"81\"/></appSettings>";
    }
}
