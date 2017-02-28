using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DisconfClient.DataConverter;
using Newtonsoft.Json;

namespace DisconfClient
{

    /// <summary>
    /// 本地配置仓储管理器
    /// </summary>
    internal class ConfigStorageManager
    {
        private static IDisconfWebApi _disconfWebApi;
        private static Thread _thread;
        private static readonly SynchronizedDictionary<string, ConfigStorageItem> ConfigStorage = new SynchronizedDictionary<string, ConfigStorageItem>(StringComparer.CurrentCultureIgnoreCase);
        private static readonly object SyncRoot = new object();
        private static ZooKeeperClient _zooKeeperClient;
        private static readonly FileSystemWatcher Fsw = new FileSystemWatcher();
        private static bool _disconfServerIsActive;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="disconfWebApi">配置中心接口服务</param>
        /// <param name="assemblies">配置类所在的程序集</param>
        public static void Init(IDisconfWebApi disconfWebApi = null, params Assembly[] assemblies)
        {
            if (disconfWebApi == null)
                disconfWebApi = new DisconfWebApi();
            _disconfWebApi = disconfWebApi;
            if (!DisconfClientSettings.DisableZooKeeper)
            {
                _zooKeeperClient = new ZooKeeperClient(_disconfWebApi);
                //向Zookeeper发送一个Watch,以便近快的拿到SyncConnected状态
                WatcherManager watcherManager = new WatcherManager(_zooKeeperClient);
                watcherManager.WatchPath("test", "test", DisconfNodeType.Item);
            }
            DisconfClientSettings.Verify();
            if (DisconfClientSettings.OnlyLoadLocalConfig)
            {
                LoadLocalConfig();
            }
            else
            {
                try
                {
                    LoadConfigItemsFromServer();
                }
                catch (Exception ex)
                {
                    string localConfigPath = GetLocalConfigPath();
                    bool isExistsLocalConfig = File.Exists(localConfigPath);
                    if (!isExistsLocalConfig)
                        throw;
                    LoadLocalConfig();
                    LogManager.GetLogger().Error(string.Format("从配置中心获取数据加载全量数据失败，已从本地副本中加载成功({0})！", localConfigPath), ex);
                }
            }

            AssemblyScan(assemblies);
            AutoRefresh();
            StartFileSystemWatcher();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="assemblies">配置类所在的程序集</param>
        public static void Init(params Assembly[] assemblies)
        {
            Init(null, assemblies);
        }

        private static void StartFileSystemWatcher()
        {
            string path = Path.Combine(DisconfClientSettings.DisconfClientLocalConfigPath, DisconfClientSettings.AppId, DisconfClientSettings.Environment);
            Fsw.Path = path;
            Fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;   //设置文件的文件名、目录名及文件的大小改动会触发Changed事件  
            Fsw.Changed += new FileSystemEventHandler(FileSystemWatcherChanged);
            Fsw.EnableRaisingEvents = true;  //启动监控 
            Fsw.IncludeSubdirectories = false;
            Fsw.Filter = DisconfClientSettings.Version + ".config";
        }

        private static void FileSystemWatcherChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (_disconfServerIsActive)
                    return;
                LogManager.GetLogger().Debug(string.Format("DisconfClient.ConfigStorageManager.FileSystemWatcherChanged,FullPath:{0},_disconfServerIsActive:{1}", e.FullPath, _disconfServerIsActive));
                MonitorLocalConfig();
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error("DisconfClient.ConfigStorageManager.FileSystemWatcherChanged", ex);
            }
        }


        /// <summary>
        /// 获取所有的程序集
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Assembly> GetAllAssemblies()
        {
            IList<Assembly> assemblys = AppDomain.CurrentDomain.GetAssemblies();
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                if (fileInfo == null || (fileInfo.Extension != ".dll" && fileInfo.Extension != ".exe"))
                    continue;
                try
                {
                    Assembly assembly = Assembly.LoadFile(fileInfo.FullName);
                    if (assembly == null || assemblys.Contains(assembly))
                        continue;
                    assemblys.Add(assembly);
                }
                catch
                {
                    // ignored
                }
            }
            return assemblys;
        }

        /// <summary>
        /// 扫描配置类
        /// </summary>
        /// <param name="assemblies"></param>
        private static void AssemblyScan(params Assembly[] assemblies)
        {
            if (!string.IsNullOrWhiteSpace(DisconfClientSettings.ConfigAssemblies))
            {
                List<Assembly> list = new List<Assembly>(assemblies);
                string[] array = DisconfClientSettings.ConfigAssemblies.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);
                if (array.Length > 0)
                {
                    foreach (string assemby in array)
                    {
                        list.Add(Assembly.Load(assemby));
                    }
                }
                assemblies = list.ToArray();
            }

            if (assemblies == null || assemblies.Length <= 0)
            {
                assemblies = GetAllAssemblies().ToArray();
            }
            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                    continue;
                try
                {
                    Type[] types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        bool flag = false;
                        DisconfAttribute classAttribute = Utils.GetFirstAttribute<DisconfAttribute>(type, true);
                        if (classAttribute != null)
                        {
                            SetConfigClassMapper(type, null, classAttribute);
                            flag = true;
                        }
                        PropertyInfo[] propertyInfos = type.GetProperties();
                        foreach (PropertyInfo propertyInfo in propertyInfos)
                        {
                            DisconfAttribute propertyAttribute = Utils.GetFirstAttribute<DisconfAttribute>(propertyInfo, true);
                            if (propertyAttribute != null)
                            {
                                SetConfigClassMapper(type, propertyInfo, propertyAttribute);
                                flag = true;
                            }
                        }
                        if (flag)
                            LogManager.GetLogger().Info(string.Format("Regsiter ConfigClass:{0}", type.GetFullTypeName()));
                    }
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger().Warn(string.Format("AssemblyScan:{0}", assembly.FullName));
                }

            }
        }

        /// <summary>
        /// 定时自动刷新本地仓储值
        /// </summary>
        private static void AutoRefresh()
        {
            try
            {
                _thread = new Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            if (!DisconfClientSettings.OnlyLoadLocalConfig)
                                MonitorDisconfItemVersion();
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                LogManager.GetLogger().Error("DisconfClient.ConfigStorageManager.AutoRefresh,Running", ex);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                        finally
                        {
                            Thread.Sleep(new TimeSpan(0, 0, 0, DisconfClientSettings.RefreshTime));
                        }
                    }
                }) { IsBackground = true };
                _thread.Start();
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error("DisconfClient.ConfigStorageManager.AutoRefresh", ex);
            }

        }

        /// <summary>
        /// 从本地仓储中获取指定的配置项
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <returns></returns>
        public static ConfigStorageItem GetConfigStorageItem(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            return ConfigStorage.GetWithNoLock(name);
        }

        /// <summary>
        /// 设置配置类的映射关系
        /// </summary>
        /// <param name="configClassType">配置类的类型</param>
        /// <param name="configPropertyInfo">配置属性的信息</param>
        /// <param name="disconfAttribute">配置特性</param>
        private static void SetConfigClassMapper(Type configClassType, PropertyInfo configPropertyInfo, DisconfAttribute disconfAttribute)
        {
            if (disconfAttribute == null)
                throw new ArgumentNullException("disconfAttribute");
            if (string.IsNullOrWhiteSpace(disconfAttribute.Name))
                throw new ArgumentNullException("disconfAttribute.Name");

            if (!ConfigStorage.ContainsKey(disconfAttribute.Name))
            {
                throw new Exception(string.Format("配置中心未配置名为：{0}的配置项！", disconfAttribute.Name));
            }

            //将一些常用的配置文件的扩展名匹配到指定的数据转换器
            Type dataConverterType = disconfAttribute.DataConverterType;
            if (dataConverterType == null)
            {
                if (disconfAttribute.Name.EndsWith(".json"))
                {
                    dataConverterType = typeof(JsonDataConverter);
                }
                else if (disconfAttribute.Name.EndsWith(".config"))
                {
                    dataConverterType = typeof(AppSettingsDataConverter);
                }
                else if (disconfAttribute.Name.EndsWith(".properties"))
                {
                    dataConverterType = typeof(PropertiesDataConverter);
                }
                else
                {
                    dataConverterType = typeof(DefalutDataConverter);
                }
            }

            //根据配置类的类型或配置属性的类型注册对应的数据转换器
            IDataConverter dataConverter = (IDataConverter)Activator.CreateInstance(dataConverterType, true);
            Type registerType = configPropertyInfo == null ? configClassType : configPropertyInfo.PropertyType;
            DataConverterManager.RegisterDataConverter(registerType, dataConverter);


            ConfigStorageItem item = ConfigStorage[disconfAttribute.Name];
            ConfigClassMapper mapper = new ConfigClassMapper
            {
                //DisconfNodeType = disconfAttribute.DisconfNodeType,
                ConfigPropertyInfo = configPropertyInfo,
                ConfigClassType = configClassType
            };
            item.ConfigClassMapper = mapper;
            item.RefreshConfigObject();
            if (disconfAttribute.CallbackType != null)
            {
                ICallback callback = (ICallback)Activator.CreateInstance(disconfAttribute.CallbackType, true);
                ConfigCallbackManager.RegisterCallback(disconfAttribute.Name, callback);
                item.ConfigClassMapper.ConfigNodeName = disconfAttribute.Name;
            }
            ConfigStorage[item.Name] = item;
        }

        /// <summary>
        /// 创建本地存储配置项
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <param name="disconfNodeType">配置项类型</param>
        /// <param name="version">版本号（对应配置中的中的配置项的最后一次更新时间值）</param>
        /// <param name="data">配置项值</param>
        /// <returns></returns>
        private static ConfigStorageItem CreateConfigStorageItem(string name, DisconfNodeType disconfNodeType, string version, string data = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            if (string.IsNullOrWhiteSpace(data))
            {
                if (disconfNodeType == DisconfNodeType.File)
                    data = _disconfWebApi.GetConfigFileContent(name);
                else
                    data = _disconfWebApi.GetConfigItemContent(name);
            }
            ConfigStorageItem configStorageItem = new ConfigStorageItem
            {
                Name = name,
                Data = data,
                Version = version,
                LastUpdateTime = DateTime.Now
            };

            if (!DisconfClientSettings.DisableZooKeeper)
            {
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    try
                    {
                        WatcherManager watcherManager = new WatcherManager(_zooKeeperClient);
                        watcherManager.WatchPath(name, data, disconfNodeType);
                    }
                    catch (Exception ex)
                    {
                        LogManager.GetLogger()
                            .Error(
                                string.Format(
                                    "DisconfClient.ConfigStorageManager.CreateConfigStorageItem.WatchPath,Exception:{0}",
                                    ex));
                    }
                });
            }

            return configStorageItem;
        }

        /// <summary>
        /// 从配置中心加载该应用下的全部配置信息
        /// </summary>
        private static void LoadConfigItemsFromServer()
        {
            IList<ConfigMetadataApiResult> list = _disconfWebApi.GetConfigMetadatas();
            if (list == null || list.Count <= 0)
                throw new Exception(string.Format("配置中心没有为应用：{0}配置过任何配置项！", DisconfClientSettings.AppId));

            //获取所有文件类型的配置信息
            IList<Task> tasks = new List<Task>();
            foreach (ConfigMetadataApiResult item in list)
            {
                if (item == null || item.Type == DisconfNodeType.Item)
                    continue;
                ConfigMetadataApiResult configMetadata = item;
                Task task = Task.Factory.StartNew(() =>
                {
                    ConfigStorageItem configStorageItem = CreateConfigStorageItem(configMetadata.Name, configMetadata.Type, configMetadata.UpdateTime);
                    if (!ConfigStorage.ContainsKey(configStorageItem.Name))
                        ConfigStorage.Add(configStorageItem.Name, configStorageItem);
                });
                tasks.Add(task);
            }
            //获取所有配置项类型的配置信息
            Task task1 = Task.Factory.StartNew(() =>
            {
                IList<ConfigItemContentApiResult> items = _disconfWebApi.GetAllConfigItemContent();
                if (items != null)
                {
                    foreach (ConfigItemContentApiResult item in items)
                    {
                        if (item == null)
                            continue;
                        ConfigStorageItem configStorageItem = null;
                        if (ConfigStorage.ContainsKey(item.Name))
                        {
                            configStorageItem = ConfigStorage[item.Name];
                        }
                        if (configStorageItem == null)
                        {
                            string name = item.Name;
                            ConfigMetadataApiResult apiResult = list.FirstOrDefault(m => m.Name == name);
                            string version = null;
                            if (apiResult != null)
                                version = apiResult.UpdateTime;
                            configStorageItem = CreateConfigStorageItem(item.Name, DisconfNodeType.Item, version, item.Value);
                        }
                        else
                        {
                            configStorageItem.Data = item.Value;
                        }

                        ConfigStorage[configStorageItem.Name] = configStorageItem;
                    }
                }
            });
            tasks.Add(task1);
            Task.WaitAll(tasks.ToArray());
            SaveLocalConfig();
            _disconfServerIsActive = true;
        }

        /// <summary>
        /// 刷新本地配置值
        /// </summary>
        /// <param name="metadata">配置项信息</param>
        internal static void ReloadConfigItem(ConfigMetadataApiResult metadata)
        {
            if (metadata == null)
                return;
            if (string.IsNullOrWhiteSpace(metadata.UpdateTime))
            {
                try
                {
                    ConfigMetadataApiResult result = _disconfWebApi.GetConfigMetadata(metadata.Name);
                    if (result != null)
                    {
                        metadata.UpdateTime = result.UpdateTime;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger().Warn(string.Format("获取配置项元数据信息时出现异常！配置项：{0},AppId:{1},Evn:{2},Version:{3}", metadata.Name, DisconfClientSettings.AppId, DisconfClientSettings.Environment, DisconfClientSettings.Version), ex);
                }
            }
            try
            {
                ConfigStorageItem configStorageItem = null;
                if (ConfigStorage.ContainsKey(metadata.Name))
                    configStorageItem = ConfigStorage[metadata.Name];

                ConfigStorageItem newConfigStorageItem = CreateConfigStorageItem(metadata.Name, metadata.Type, metadata.UpdateTime);

                if (configStorageItem == null)
                {
                    configStorageItem = newConfigStorageItem;
                }
                else
                {
                    configStorageItem.Data = newConfigStorageItem.Data;
                    configStorageItem.Version = newConfigStorageItem.Version;
                    configStorageItem.LastUpdateTime = DateTime.Now;
                }
                ConfigStorage[metadata.Name] = configStorageItem;
                configStorageItem.Callback();
                _disconfServerIsActive = true;
                SaveLocalConfig();
                LogManager.GetLogger().Info(string.Format("刷新本地配置项,{0}", JsonConvert.SerializeObject(configStorageItem)));
            }
            catch (Exception ex)
            {
                _disconfServerIsActive = false;
                LogManager.GetLogger().Error(string.Format("刷新本地配置值时出现异常！配置项：{0},AppId:{1},Evn:{2},Version:{3}", metadata.Name, DisconfClientSettings.AppId, DisconfClientSettings.Environment, DisconfClientSettings.Version), ex);
            }

        }

        /// <summary>
        /// 定时比较本地配置与配置中心配置的版本值
        /// </summary>
        private static void MonitorDisconfItemVersion()
        {
            try
            {
                IList<ConfigMetadataApiResult> changedList = new List<ConfigMetadataApiResult>();

                IList<ConfigMetadataApiResult> list = _disconfWebApi.GetConfigMetadatas();
                if (list == null || list.Count <= 0)
                    return;
                foreach (ConfigMetadataApiResult item in list)
                {
                    if (item == null)
                        continue;
                    if (!ConfigStorage.ContainsKey(item.Name))
                    {
                        ConfigStorageItem configStorageItem = CreateConfigStorageItem(item.Name, item.Type, item.UpdateTime);
                        if (!ConfigStorage.ContainsKey(configStorageItem.Name))
                        {
                            ConfigStorage.Add(configStorageItem.Name, configStorageItem);
                            changedList.Add(item);
                        }
                    }
                    else
                    {
                        ConfigStorageItem configStorageItem = ConfigStorage[item.Name];
                        if (configStorageItem != null && configStorageItem.Version != item.UpdateTime)
                        {
                            changedList.Add(item);
                            LogManager.GetLogger().Info(string.Format("ChangeItem:{0},Version_Old:{1},Version_New:{2}", item.Name, configStorageItem.Version, item.UpdateTime));
                        }
                    }
                }
                StringBuilder sbChanged = new StringBuilder();
                foreach (ConfigMetadataApiResult item in changedList)
                {
                    if (item == null)
                        continue;
                    ReloadConfigItem(item);
                    sbChanged.Append(item.Name).Append(" , ");
                }
                if (!DisconfClientSettings.DisableZooKeeper)
                {
                    _zooKeeperClient.MonitorZooKeeperState();
                }
                
                LogManager.GetLogger().Info(string.Format("DisconfClient.ConfigStorageManager.MonitorDisconfItemVersion,changed:{0}.items:{1}", changedList.Count, sbChanged));
            }
            catch (Exception ex)
            {
                try
                {
                    _disconfServerIsActive = false;
                    LogManager.GetLogger().Error("定时比较本地配置与配置中心配置的版本值时出现异常！", ex);
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// 将本地仓储中的值保存到本地配置文件中
        /// </summary>
        private static void SaveLocalConfig()
        {
            if (DisconfClientSettings.OnlyLoadLocalConfig)
                return;

            string path = GetLocalConfigPath();
            try
            {
                string json = JsonConvert.SerializeObject(ConfigStorage.Values.ToList(), Formatting.Indented);
                lock (SyncRoot)
                {
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
                    {
                        fileInfo.Directory.Create();
                    }
                    File.WriteAllText(path, json);
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("保存配置到本地文件失败！{0}", path), ex);
            }
        }

        /// <summary>
        /// 定时监控本地配置文件的变化
        /// </summary>
        private static void MonitorLocalConfig()
        {
            string path = GetLocalConfigPath();
            try
            {
                IList<ConfigStorageItem> changedList = new List<ConfigStorageItem>();

                List<ConfigStorageItem> list = GetLocalConfig();
                if (list == null || list.Count <= 0)
                    return;
                foreach (ConfigStorageItem item in list)
                {
                    if (item == null)
                        continue;
                    if (!ConfigStorage.ContainsKey(item.Name))
                    {
                        changedList.Add(item);
                        ConfigStorage.Add(item.Name, item);
                    }
                    else
                    {
                        if (ConfigStorage[item.Name].Data != item.Data)
                        {
                            ConfigStorage[item.Name] = item;
                            item.Callback();
                        }
                    }
                }
                LogManager.GetLogger().Info(string.Format("MonitorLocalConfig,已成功从{0}加载配置文件！", path));
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("MonitorLocalConfig,从本地配置文件中加数据失败！{0}", path), ex);
            }
        }

        /// <summary>
        /// 加载本地配置文件
        /// </summary>
        private static void LoadLocalConfig()
        {
            string path = GetLocalConfigPath();
            try
            {
                List<ConfigStorageItem> list = GetLocalConfig();
                if (list == null || list.Count <= 0)
                    throw new Exception(string.Format("未能从本地({0})配置中加载到任何配置项！", path));
                foreach (ConfigStorageItem item in list)
                {
                    if (item == null)
                        continue;
                    ConfigStorage[item.Name] = item;
                }
                LogManager.GetLogger().Info(string.Format("已成功从{0}加载配置文件！", path));
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("从本地配置文件中加载数据失败！{0}", path), ex);
                throw;
            }

        }

        private static List<ConfigStorageItem> GetLocalConfig()
        {
            string path = GetLocalConfigPath();
            try
            {
                string json = null;
                lock (SyncRoot)
                {
                    json = File.ReadAllText(path);
                }
                return JsonConvert.DeserializeObject<List<ConfigStorageItem>>(json);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("从本地配置文件中获取数据失败！{0}", path), ex);
                return null;
            }

        }

        /// <summary>
        /// 获取本地配置文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetLocalConfigPath()
        {
            return Path.Combine(DisconfClientSettings.DisconfClientLocalConfigPath, DisconfClientSettings.AppId, DisconfClientSettings.Environment,
                DisconfClientSettings.Version + ".config");
        }

        /// <summary>
        /// 以JSON格式输出本地仓储中的当前数据
        /// </summary>
        /// <returns></returns>
        public static string PrintConfigItems()
        {
            return JsonConvert.SerializeObject(ConfigStorage.Values.ToList());
        }

        public static void CloseZooKeeperClient()
        {
            _zooKeeperClient.Close();
        }
    }
}
