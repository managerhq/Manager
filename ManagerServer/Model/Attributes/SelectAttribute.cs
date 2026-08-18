using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class SelectAttribute : Attribute
    {
        public Type Type { get; set; }

        public SelectAttribute(Type type)
        {
            Type = type;
        }
    }
}
