using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisconfClient;
using DisconfClient.DataConverter;
using DisconfClientDemo.ConfigClass;

namespace DisconfClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //请将这个初始化的方法调用放在你应用的入口处
                ConfigManager.Init();

                //在应用的入口处注册定制化的类型转换器，如果是配置类的方式，则可以直接在Disconf特性中设置，例见：PropertiesDemoConfig
                DataConverterManager.RegisterDataConverter("TestMyList",new MyListDataConverter());
                //关于DataConverter



                //如果你需要当某个节点的值发生变化时获行通知，请在应用的入口处注册配置节点值变更时的回调通知类
                ConfigCallbackManager.RegisterCallback("TestMyList",new TestMyListCallback());

                //可以将一些配置项归类，定义在一个配置类中
                TypeTestConfig typeTestConfig = ConfigManager.GetConfigClass<TypeTestConfig>();

                //也可以直接以Key-Value的形式来调用
                string testString = ConfigManager.GetConfigValue<string>("TestString");
                int testInt = ConfigManager.GetConfigValue<int>("TestInt");
                long testLong = ConfigManager.GetConfigValue<long>("TestLong");
                uint testUint = ConfigManager.GetConfigValue<uint>("TestUint");
                ulong testUlong = ConfigManager.GetConfigValue<ulong>("TestUlong");
                float testFloat = ConfigManager.GetConfigValue<float>("TestFloat");
                double testDouble = ConfigManager.GetConfigValue<double>("TestDouble");
                decimal testDecimal = ConfigManager.GetConfigValue<decimal>("TestDecimal");
                DisconfNodeType testEnumInt = ConfigManager.GetConfigValue<DisconfNodeType>("TestEnumInt");
                DisconfNodeType testEnumString = ConfigManager.GetConfigValue<DisconfNodeType>("TestEnumString");
                Guid testGuid = ConfigManager.GetConfigValue<Guid>("TestGuid");
                Type testType = ConfigManager.GetConfigValue<Type>("TestType");
                bool testBool = ConfigManager.GetConfigValue<bool>("TestBool");
                char testChar = ConfigManager.GetConfigValue<char>("TestChar");
                DateTime testDateTime = ConfigManager.GetConfigValue<DateTime>("TestDateTime");
                IList<string> testStringList = ConfigManager.GetConfigValue<IList<string>>("TestStringList");
                IDictionary<string, string> testStringDictionary = ConfigManager.GetConfigValue<IDictionary<string, string>>("TestStringDictionary");
                
                //可以为某一个配置文件定义一个配置类
                AppSettingsDemoConfig appSettingsDemoConfig = ConfigManager.GetConfigClass<AppSettingsDemoConfig>();
                //所有的配置项都可以通过这样的方法直接取出配置项中的字符串值
                string appSettingsValue = ConfigManager.GetConfigValue<string>("redis.config");

                //如果配置项的名称是以.config,.properties扩展名结尾的，或注册过DictionaryDataConverter的则可以转换为Dictionary<string, string>类型
                IDictionary<string, string> appSettingsValueToDictionary = ConfigManager.GetConfigValue<IDictionary<string, string>>("redis.config");
                //对IDictionary<string, string>类型做了“Get<T>'扩展方法,类型的转换使用的是DefalutDataConverter
                string host = appSettingsValueToDictionary.Get<string>("Host");
                int port = appSettingsValueToDictionary.Get<int>("Port");

                //以.json扩展名结尾的，或注册过JsonDataConverter的则可以转换为指定的类型
                JsonDemoConfig jsonDemoConfig = ConfigManager.GetConfigClass<JsonDemoConfig>();
                string json = ConfigManager.GetConfigValue<string>("redis.json");
                JsonDemoConfig jsonDemoConfig2 = ConfigManager.GetConfigValue<JsonDemoConfig>("redis.json");

                //以.properties扩展名结尾的，或注册过PropertiesDataConverter的则可以进行类型转换
                PropertiesDemoConfig propertiesDemoConfig = ConfigManager.GetConfigClass<PropertiesDemoConfig>();
                string properties = ConfigManager.GetConfigValue<string>("redis.properties");
                PropertiesDemoConfig propertiesDemoConfig2 = ConfigManager.GetConfigValue<PropertiesDemoConfig>("redis.properties");

                //调用本地appSettings中的配置项,类型的转换使用的是DefalutDataConverter
                string appId = ConfigManager.AppSettings<string>("AppId");
                int refreshTime = ConfigManager.AppSettings<int>("DisconfClient.RefreshTime", 15);

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }

            Console.ReadLine();
        }
    }

    public class MyListDataConverter : IDataConverter
    {
        public object Parse(Type type, string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            string[] array = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            IList<string> list = new List<string>(array);
            return list;
        }
    }

    public class TestMyListCallback : ICallback
    {
        public void Invoke()
        {
            IList<string> list = ConfigManager.GetConfigValue<IList<string>>("TestMyList");
            if(list==null)
                Console.WriteLine("TestMyList is : null");
            else if(list.Count<=0)
                Console.WriteLine("TestMyList is : empty");
            else
            {
                string value = string.Join(",", list);
                Console.WriteLine("TestMyList is :" + value);
            }
               
        }
    }
}
