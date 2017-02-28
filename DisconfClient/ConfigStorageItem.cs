using System;
using DisconfClient.DataConverter;
using Newtonsoft.Json;
namespace DisconfClient
{
    /// <summary>
    /// 本地仓储配置项
    /// </summary>
    internal class ConfigStorageItem
    {
        /// <summary>
        /// 配置项名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 配置项名称值
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 配置项版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 本地最后一次更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 配置类的映射关系
        /// </summary>
        [JsonIgnore]
        public ConfigClassMapper ConfigClassMapper { get; set; }

        /// <summary>
        /// 刷新配置类的值
        /// </summary>
        public void RefreshConfigObject()
        {
            if (ConfigClassMapper == null)
                return;

            try
            {
                if (ConfigClassMapper.ConfigPropertyInfo == null)
                {
                    IDataConverter dataConverter = DataConverterManager.GetDataConverter(ConfigClassMapper.ConfigClassType);
                    object value = dataConverter.Parse(ConfigClassMapper.ConfigClassType, Data);
                    ConfigClassMapper.SetConfigClassInstance(ConfigClassMapper.ConfigClassType, value);
                }
                else
                {
                    IDataConverter dataConverter = DataConverterManager.GetDataConverter(ConfigClassMapper.ConfigPropertyInfo.PropertyType);
                    object propertyValue = dataConverter.Parse(ConfigClassMapper.ConfigPropertyInfo.PropertyType, Data);
                    object obj = null;
                    object oldObject = ConfigClassMapper.GetConfigClassInstance(ConfigClassMapper.ConfigClassType);
                    if (oldObject == null)
                    {
                        obj = Activator.CreateInstance(ConfigClassMapper.ConfigClassType, true);
                    }
                    else
                    {
                        obj = oldObject;
                        try
                        {
                            string json = JsonConvert.SerializeObject(oldObject);
                            object newObject = JsonConvert.DeserializeObject(json, ConfigClassMapper.ConfigClassType);
                            obj = newObject;
                        }
                        catch (Exception ex)
                        {
                            LogManager.GetLogger().Error(string.Format("RefreshConfigObject.Clone,Exception:{0}", ex));
                        }
                    }
                    ConfigClassMapper.ConfigPropertyInfo.SetValue(obj, propertyValue, null);
                    ConfigClassMapper.SetConfigClassInstance(ConfigClassMapper.ConfigClassType, obj);
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("RefreshConfigObject,Exception:{0}", ex));
            }
        }

        /// <summary>
        /// 配置项值变更时的回调处理
        /// </summary>
        public void Callback()
        {
            try
            {
                ICallback callback = null;
                if (this.ConfigClassMapper == null)
                {
                    callback = ConfigCallbackManager.GetCallback(this.Name);
                }
                else
                {
                    this.RefreshConfigObject();
                    callback = ConfigCallbackManager.GetCallback(this.ConfigClassMapper.ConfigNodeName);
                }
                if (callback != null)
                    callback.Invoke();
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.ConfigStorageItem.Callback,Name:{0}", this.Name), ex);
            }
        }
    }

}
