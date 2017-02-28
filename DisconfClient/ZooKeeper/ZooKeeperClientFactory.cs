using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisconfClient
{
    internal static class ZooKeeperClientFactory
    {
        private  static  readonly  SynchronizedDictionary<string,ZooKeeperClient> Clients = new SynchronizedDictionary<string, ZooKeeperClient>();

        public static ZooKeeperClient GetZooKeeperClient(string host)
        {
            if(string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException("host");

            return Clients.GetOrSet(host, () =>
            {
                ZooKeeperClient client = new ZooKeeperClient();
                client.Connect(host);
                return client;
            });
        }
    }
}
