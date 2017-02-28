using System.Collections.Generic;

namespace DisconfClient
{
    public interface IDisconfWebApi
    {
        string GetZooKeeperHost();
        string GetZooKeeperRootNode();

        string GetConfigFileContent(string name);
        string GetConfigItemContent(string name);

        IList<ConfigMetadataApiResult> GetConfigMetadatas();
        ConfigMetadataApiResult GetConfigMetadata(string name);
        IList<ConfigItemContentApiResult> GetAllConfigItemContent();
    }
}
