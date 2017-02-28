using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisconfClient
{

    /// <summary>
    /// 配置项别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AliasAttribute : Attribute
    {
        public AliasAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 别名
        /// </summary>
        public string Name { get; set; }
    }
}
