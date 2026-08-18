using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class AppendValueAttribute : Attribute
    {
        private string[] path;

        public AppendValueAttribute(params string[] path)
        {
            this.path = path;
        }

        public string GetExpression()
        {
            var s = string.Empty;
            foreach (var e in path)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    s = $"(typeof lineItem === typeof undefined ? {e} : lineItem.{e})";
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
