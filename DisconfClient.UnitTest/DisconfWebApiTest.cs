using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DisconfClient.UnitTest
{
    [TestClass]
    public class DisconfWebApiTest
    {
        [TestMethod]
        public void GetZooKeeperHostTest()
        {
            IDisconfWebApi webApi = new DisconfWebApi();
            string value = webApi.GetZooKeeperHost();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(value));
        }

        [TestMethod]
        public void GetZooKeeperPrefixTest()
        {
            IDisconfWebApi webApi = new DisconfWebApi();
            string value = webApi.GetZooKeeperRootNode();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(value));
        }

        [TestMethod]
        public void GetConfigFileContentTest()
        {
            string name = "redis1.properties";
            IDisconfWebApi webApi = new DisconfWebApi();
            string value = webApi.GetConfigFileContent(name);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(value));
        }

        [TestMethod]
        public void GetConfigItemContentTest()
        {
            string name = "node1";
            IDisconfWebApi webApi = new DisconfWebApi();
            string value = webApi.GetConfigItemContent(name);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(value));
        }

        [TestMethod]
        public void GetConfigMetas()
        {
            IDisconfWebApi webApi = new DisconfWebApi();
            IList<ConfigMetadataApiResult> list = webApi.GetConfigMetadatas();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void GetAllConfigItemContent()
        {
            IDisconfWebApi webApi = new DisconfWebApi();
            IList<ConfigItemContentApiResult> list = webApi.GetAllConfigItemContent();
            Assert.IsNotNull(list);
        }
    }
}
