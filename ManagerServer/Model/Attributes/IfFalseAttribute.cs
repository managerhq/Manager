using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class IfFalseAttribute : IfAttribute
    {
        public string[] Path;

        public IfFalseAttribute(params string[] path)
        {
            Path = path;
        }

        public override string GetIfExpression()
        {
            var s = string.Empty;
            foreach (var e in Path)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    s = $"(this.get{e}(typeof lineItem == typeof undefined ? null : lineItem))";
                }
                else
                {
                    s = "(" + s + " || {})."+e;
                }
            }
            return s+" != true";
        }
    }
}
