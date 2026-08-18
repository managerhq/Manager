using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class LabelAttribute : Attribute
    {
        public string[] Value;

        public LabelAttribute(params string[] value)
        {
            this.Value = value;
        }

        public string GetValueForGuides()
        {
            return string.Join("-", Value);
        }

        public override string ToString()
        {
            return string.Join(" — ", Value.Select(x => ManagerServer.Globalization.Strings.GetPropertyValue(x)));
        }
    }
}
