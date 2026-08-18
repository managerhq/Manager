using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class IfDoesNotContainAttribute : IfAttribute
    {
        private string field;
        private string value;

        public IfDoesNotContainAttribute(string value, string field)
        {
            this.field = field;
            this.value = value;
        }

        public override string GetIfExpression()
        {
            return $"!(this.{field} || '').includes('{value}')";
        }
    }
}
