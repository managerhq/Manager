using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class AutocompleteAttribute : Attribute
    {
        public Type Value;
        public Type Placeholder;
        public object Filter;
        public string Subtext;

        public AutocompleteAttribute(Type value, Type placeholder = null, object filter = null, string subtext = null)
        {
            this.Value = value;
            this.Filter = filter;
            this.Placeholder = placeholder;
            this.Subtext = subtext;
        }
    }
}