using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public class WebServiceAttribute : Attribute
    {
        public Type Type { get; set; }

        public WebServiceAttribute(Type type)
        {
            Type = type;
        }
    }
}
