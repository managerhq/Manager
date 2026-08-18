using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class PrependAttribute : Attribute
    {
        private string[] values;

        public PrependAttribute(params string[] values)
        {
            this.values = values;
        }

        public override string ToString()
        {
            return string.Join(' ', values.Select(x => ManagerServer.Globalization.Strings.GetPropertyValue(x)));
        }
    }
}
