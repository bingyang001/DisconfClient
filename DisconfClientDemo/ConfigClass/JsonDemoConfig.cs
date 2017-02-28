using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisconfClient;

namespace DisconfClientDemo.ConfigClass
{
    [Disconf(Name = "redis.json")]
    public class JsonDemoConfig
    {
        public string Host { get; set; }

        public int Port { get; set; }
    }
}
