using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace DisconfClient
{
    public class DisconfWebApi : IDisconfWebApi
    {
        private static string _zooKeeperHost;
        private static string _zooKeeperRootNode;
        public string GetZooKeeperHost()
        {
            if (!string.IsNullOrWhiteSpace(_zooKeeperHost))
                return _zooKeeperHost;
            string url = null;
            try
            {
                url = string.Format("{0}/api/zoo/hosts", DisconfClientSettings.DisconfServerHost);
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "DisconfClient");
                byte[] bytes = webClient.DownloadData(new Uri(url));
                string data = Encoding.UTF8.GetString(bytes);
                ApiResult apiResult = JsonConvert.DeserializeObject<ApiResult>(data);
                if (apiResult.Status != 1)
                    throw new Exception(string.Format("Disconf Api Url:{0}, Status:{1},Message:{2}", url, apiResult.Status, apiResult.Message));
                _zooKeeperHost = apiResult.Value;
                return apiResult.Value;
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.DisconfWebApi,Url:{0}", url, ex));
                throw;
            }

        }

        public string GetZooKeeperRootNode()
        {
            if (!string.IsNullOrWhiteSpace(_zooKeeperRootNode))
                return _zooKeeperRootNode;
            string url = null;
            try
            {
                url = string.Format("{0}/api/zoo/prefix", DisconfClientSettings.DisconfServerHost);
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "DisconfClient");
                byte[] bytes = webClient.DownloadData(new Uri(url));
                string data = Encoding.UTF8.GetString(bytes);
                ApiResult apiResult = JsonConvert.DeserializeObject<ApiResult>(data);
                if (apiResult.Status != 1)
                    throw new Exception(string.Format("Disconf Api Url:{0}, Status:{1},Message:{2}", url, apiResult.Status, apiResult.Message));
                _zooKeeperRootNode = apiResult.Value;
                return apiResult.Value;
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.DisconfWebApi,Url:{0}", url, ex));
                throw;
            }

        }

        public string GetConfigFileContent(string name)
        {
            string url = null;
            try
            {
                url = string.Format("{0}/api/config/file?app={1}&env={2}&type=0&version={3}&key={4}"
                 , DisconfClientSettings.DisconfServerHost, DisconfClientSettings.AppId,
                DisconfClientSettings.Environment, DisconfClientSettings.Version, name);
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "DisconfClient");
                byte[] bytes = webClient.DownloadData(new Uri(url));
                string data = Encoding.UTF8.GetString(bytes);
                data = data.Trim("\\ufeff".ToCharArray());
                return data;
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.DisconfWebApi,Url:{0}", url, ex));
                throw;
            }

        }

        public string GetConfigItemContent(string name)
        {
            string url = null;
            try
            {
                url = string.Format("{0}/api/config/item?app={1}&env={2}&type=1&version={3}&key={4}"
                , DisconfClientSettings.DisconfServerHost, DisconfClientSettings.AppId,
                DisconfClientSettings.Environment, DisconfClientSettings.Version, name);
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "DisconfClient");
                byte[] bytes = webClient.DownloadData(new Uri(url));
                string data = Encoding.UTF8.GetString(bytes);
                ApiResult apiResult = JsonConvert.DeserializeObject<ApiResult>(data);
                if (apiResult.Status != 1)
                    throw new Exception(string.Format("Disconf Api Url:{0}, Status:{1},Message:{2}", url, apiResult.Status, apiResult.Message));
                return apiResult.Value;
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.DisconfWebApi,Url:{0}", url, ex));
                throw;
            }

        }

        public IList<ConfigMetadataApiResult> GetConfigMetadatas()
        {
            string url = null;
            try
            {
                url = string.Format("{0}/api/config/metas?app={1}&env={2}&version={3}",
                DisconfClientSettings.DisconfServerHost, DisconfClientSettings.AppId, DisconfClientSettings.Environment,
                DisconfClientSettings.Version);
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "DisconfClient");
                byte[] bytes = webClient.DownloadData(new Uri(url));
                string data = Encoding.UTF8.GetString(bytes);
                ApiResult apiResult = JsonConvert.DeserializeObject<ApiResult>(data);
                if (apiResult.Status != 1)
                    throw new Exception(string.Format("Disconf Api Url:{0}, Status:{1},Message:{2}", url, apiResult.Status, apiResult.Message));
                IList<ConfigMetadataApiResult> list = JsonConvert.DeserializeObject<IList<ConfigMetadataApiResult>>(apiResult.Value);
                return list;
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.DisconfWebApi,Url:{0}", url, ex));
                throw;
            }

        }

        public ConfigMetadataApiResult GetConfigMetadata(string name)
        {
            string url = null;
            try
            {
                url = string.Format("{0}/api/config/metas?app={1}&env={2}&version={3}&key={4}",
                DisconfClientSettings.DisconfServerHost, DisconfClientSettings.AppId, DisconfClientSettings.Environment,
                DisconfClientSettings.Version, name);
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "DisconfClient");
                byte[] bytes = webClient.DownloadData(new Uri(url));
                string data = Encoding.UTF8.GetString(bytes);
                ApiResult apiResult = JsonConvert.DeserializeObject<ApiResult>(data);
                if (apiResult.Status != 1)
                    throw new Exception(string.Format("Disconf Api Url:{0}, Status:{1},Message:{2}", url, apiResult.Status, apiResult.Message));
                IList<ConfigMetadataApiResult> list = JsonConvert.DeserializeObject<IList<ConfigMetadataApiResult>>(apiResult.Value);
                if (list == null || list.Count <= 0)
                    return null;
                return list.FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.DisconfWebApi,Url:{0}", url, ex));
                throw;
            }
        }

        public IList<ConfigItemContentApiResult> GetAllConfigItemContent()
        {
            string url = null;
            try
            {
                url = string.Format("{0}/api/config/item/values?app={1}&env={2}&version={3}",
                DisconfClientSettings.DisconfServerHost, DisconfClientSettings.AppId, DisconfClientSettings.Environment,
                DisconfClientSettings.Version);
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "DisconfClient");
                byte[] bytes = webClient.DownloadData(new Uri(url));
                string data = Encoding.UTF8.GetString(bytes);
                ApiResult apiResult = JsonConvert.DeserializeObject<ApiResult>(data);
                if (apiResult.Status != 1)
                    throw new Exception(string.Format("Disconf Api Status:{0},Message:{1}", apiResult.Status, apiResult.Message));
                IList<ConfigItemContentApiResult> list = JsonConvert.DeserializeObject<IList<ConfigItemContentApiResult>>(apiResult.Value);
                return list;
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(string.Format("DisconfClient.DisconfWebApi,Url:{0}", url, ex));
                throw;
            }

        }

    }

    internal class ApiResult
    {
        public int Status { get; set; }

        public string Message { get; set; }

        public string Value { get; set; }
    }
}
