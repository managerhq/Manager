using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class MemberInfoAutocompleteAttribute : Attribute
    {
        public object Filter;

        public MemberInfoAutocompleteAttribute(object filter)
        {
            this.Filter = filter;
        }
    }
}
