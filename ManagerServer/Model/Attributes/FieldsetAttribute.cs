using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class FieldsetAttribute : Attribute
    {
        public string Legend { get; set; }

        public FieldsetAttribute(string legend)
        {
            Legend = legend;
        }
    }
}