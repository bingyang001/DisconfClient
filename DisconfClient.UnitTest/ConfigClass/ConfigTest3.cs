using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisconfClient.UnitTest
{
    [Disconf(Name = "jsonTest1.json")]
    public class ConfigTest3
    {
        public string Host { get; set; }

        public int Port { get; set; }
    }
}
