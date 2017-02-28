using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisconfClient.UnitTest
{
    [Disconf(Name = "propertiesTest1.properties")]
    public class ConfigTest2
    {
        public string Host { get; set; }

        public int Port { get; set; }
    }
}
