using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class AppendAttribute : Attribute
    {
        private string value;

        public AppendAttribute(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return ManagerServer.Globalization.Strings.GetPropertyValue(value);
        }
    }
}
