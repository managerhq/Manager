using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class PlaceholderAttribute : Attribute
    {
        private string value;

        public PlaceholderAttribute(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return ManagerServer.Globalization.Strings.GetPropertyValue(value);
        }
    }
}
