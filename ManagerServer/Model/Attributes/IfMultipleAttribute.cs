using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class IfMultipleAttribute : IfAttribute
    {
        private string field;

        public IfMultipleAttribute(string field)
        {
            this.field = field;
        }

        public override string GetIfExpression()
        {
            return $"this.{field}.length > 1";
        }
    }
}
