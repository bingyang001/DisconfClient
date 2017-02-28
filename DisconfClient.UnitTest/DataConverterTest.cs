using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DisconfClient;
using DisconfClient.DataConverter;
using DisconfClient.UnitTest.Configs;

namespace DisconfClient.UnitTest
{
    [TestClass]
    public class DataConverterTest
    {
        public DataConverterTest()
        {

        }

        [TestMethod]
        public void DefalutDataConverterTest()
        {
            IDataConverter dataConverter = new DefalutDataConverter();
            string result1String = (string)dataConverter.Parse(typeof(string), Constant.DefalutDataConverterTestValue1);
            Assert.AreEqual(result1String, Constant.DefalutDataConverterTestValue1);

            int result2Int = (int)dataConverter.Parse(typeof(int), Constant.DefalutDataConverterTestValue2);
            Assert.AreEqual(int.Parse(Constant.DefalutDataConverterTestValue2), result2Int);

            long result2Long = (long)dataConverter.Parse(typeof(long), Constant.DefalutDataConverterTestValue2);
            Assert.AreEqual(long.Parse(Constant.DefalutDataConverterTestValue2), result2Long);

            uint result2Uint = (uint)dataConverter.Parse(typeof(uint), Constant.DefalutDataConverterTestValue2);
            Assert.AreEqual(uint.Parse(Constant.DefalutDataConverterTestValue2), result2Uint);

            ulong result2Ulong = (ulong)dataConverter.Parse(typeof(ulong), Constant.DefalutDataConverterTestValue2);
            Assert.AreEqual(ulong.Parse(Constant.DefalutDataConverterTestValue2), result2Ulong);

            float result3Float = (float)dataConverter.Parse(typeof(float), Constant.DefalutDataConverterTestValue3);
            Assert.AreEqual(float.Parse(Constant.DefalutDataConverterTestValue3), result3Float);

            double result3Double = (double)dataConverter.Parse(typeof(double), Constant.DefalutDataConverterTestValue3);
            Assert.AreEqual(double.Parse(Constant.DefalutDataConverterTestValue3), result3Double);

            decimal result3Decimal = (decimal)dataConverter.Parse(typeof(decimal), Constant.DefalutDataConverterTestValue3);
            Assert.AreEqual(decimal.Parse(Constant.DefalutDataConverterTestValue3), result3Decimal);

            DisconfNodeType result4Enum = (DisconfNodeType)dataConverter.Parse(typeof(DisconfNodeType), Constant.DefalutDataConverterTestValue4);
            Assert.AreEqual((DisconfNodeType)int.Parse(Constant.DefalutDataConverterTestValue4), result4Enum);
            DisconfNodeType result9Enum = (DisconfNodeType)dataConverter.Parse(typeof(DisconfNodeType), Constant.DefalutDataConverterTestValue9);
            Assert.AreEqual(Enum.Parse(typeof(DisconfNodeType), Constant.DefalutDataConverterTestValue9, true), result9Enum);

            Guid result5Guid = (Guid)dataConverter.Parse(typeof(Guid), Constant.DefalutDataConverterTestValue5);
            Assert.AreEqual((Guid)Guid.Parse(Constant.DefalutDataConverterTestValue5), result5Guid);

            Type resutl6Type = (Type)dataConverter.Parse(typeof(Type), Constant.DefalutDataConverterTestValue6);
            Assert.AreEqual((Type)Type.GetType(Constant.DefalutDataConverterTestValue6), resutl6Type);

            bool result7Bool = (bool)dataConverter.Parse(typeof(bool), Constant.DefalutDataConverterTestValue7);
            Assert.AreEqual((bool)bool.Parse(Constant.DefalutDataConverterTestValue7), result7Bool);

            char result8Char = (char)dataConverter.Parse(typeof(char), Constant.DefalutDataConverterTestValue8);
            Assert.AreEqual((char)char.Parse(Constant.DefalutDataConverterTestValue8), result8Char);

            DateTime result10DateTime = (DateTime)dataConverter.Parse(typeof(DateTime), Constant.DefalutDataConverterTestValue10);
            Assert.AreEqual((DateTime)DateTime.Parse(Constant.DefalutDataConverterTestValue10), result10DateTime);


            IList<string> list = (IList<string>)dataConverter.Parse(typeof(IList<string>), Constant.PropertiesDataConverterTestValue1);
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);

            IDictionary<string, string> dic = (IDictionary<string, string>)dataConverter.Parse(typeof(IDictionary<string, string>), Constant.PropertiesDataConverterTestValue1);
            Assert.IsNotNull(dic);
            Assert.AreEqual("127.0.0.1", dic["Host"]);
            Assert.AreEqual("81", dic["Port"]);
        }

        [TestMethod]
        public void PropertiesDataConverterTest()
        {
            IDataConverter dataConverter = new PropertiesDataConverter();
            ConfigTest1 configTest1 = (ConfigTest1)dataConverter.Parse(typeof(ConfigTest1), Constant.PropertiesDataConverterTestValue1);
            Assert.IsNotNull(configTest1);
            Assert.AreEqual("127.0.0.1", configTest1.Host);
            Assert.AreEqual(81, configTest1.Port);
        }

        [TestMethod]
        public void DictionaryDataConverterTest()
        {
            IDataConverter dataConverter = new DictionaryDataConverter();
            IDictionary<string, string> dic = (IDictionary<string, string>)dataConverter.Parse(typeof(IDictionary<string, string>), Constant.PropertiesDataConverterTestValue1);
            Assert.IsNotNull(dic);
            Assert.AreEqual("127.0.0.1", dic["Host"]);
            Assert.AreEqual("81", dic["Port"]);
        }

        [TestMethod]
        public void ListDataConverterTest()
        {
            IDataConverter dataConverter = new ListDataConverter();
            IList<string> list = (IList<string>)dataConverter.Parse(typeof(IList<string>), Constant.PropertiesDataConverterTestValue1);
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("host=127.0.0.1", list[0]);
            Assert.AreEqual("port=81", list[1]);
        }

        [TestMethod]
        public void JsonDataConverterTest()
        {
            IDataConverter dataConverter = new JsonDataConverter();
            ConfigTest1 configTest1 = (ConfigTest1)dataConverter.Parse(typeof(ConfigTest1), Constant.JsonDataConverterTestValue1);
            Assert.IsNotNull(configTest1);
            Assert.AreEqual("127.0.0.1", configTest1.Host);
            Assert.AreEqual(81, configTest1.Port);
        }

        [TestMethod]
        public void AppSettingsDataConverterTest()
        {
            IDataConverter dataConverter = new AppSettingsDataConverter();
            ConfigTest1 configTest1 = (ConfigTest1)dataConverter.Parse(typeof(ConfigTest1), Constant.AppSettingsDataConverterTestValue1);
            Assert.IsNotNull(configTest1);
            Assert.AreEqual("127.0.0.1", configTest1.Host);
            Assert.AreEqual(81, configTest1.Port);
        }


    }
}
