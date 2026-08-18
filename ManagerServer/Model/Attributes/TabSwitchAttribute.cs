using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class TabSwitchAttribute : Attribute
    {
        public bool Popular;

        public TabSwitchAttribute(bool popular)
        {
            this.Popular = popular;
        }
    }
}
