using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class IfAnyNotNullAttribute : IfAttribute
    {
        private string name;

        public IfAnyNotNullAttribute(string name)
        {
            this.name = name;
        }

        public override string GetIfExpression()
        {
            var s = string.Empty;
            return $"this.get{name}Array().length > 0";
        }
    }
}
