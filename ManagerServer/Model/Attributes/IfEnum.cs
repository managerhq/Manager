using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class IfEnumAttribute : IfAttribute
    {
        private string field;
        private int[] enumValues;

        public IfEnumAttribute(string field, params int[] enumValues)
        {
            this.field = field;
            this.enumValues = enumValues;
        }

        public override string GetIfExpression()
        {
            var s = $"this.get{field}(typeof lineItem == typeof undefined ? null : lineItem)";
            return $"[{string.Join(',', enumValues)}].includes({s})";
        }
    }
}
