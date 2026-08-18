using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class SubstituteAttribute : Attribute
    {
        public string[] Path;

        public SubstituteAttribute(params string[] path)
        {
            Path = path;
        }

        public string GetExpression()
        {
            var s = string.Empty;
            foreach (var e in Path)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    s = $"this.get{e}(typeof lineItem == typeof undefined ? null : lineItem)";
                }
                else
                {
                    s = "(" + s + " || {})." + e;
                }
            }
            return s;
        }
    }
}
