using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Attributes
{
    public sealed class KeyAttribute : Attribute
    {
        public string Key { get; init; }

        public KeyAttribute(string key)
        {
            Key = key;
        }
    }
}
