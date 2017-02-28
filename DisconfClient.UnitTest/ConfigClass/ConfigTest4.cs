using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisconfClient.UnitTest
{
    [Disconf(Name = "appSettingTest1.config")]
    public class ConfigTest4
    {
        public string Host { get; set; }

        public int Port { get; set; }
    }
}
