using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class IfWithholdingTaxReceivable : IfAttribute
    {
        public override string GetIfExpression()
        {
            return "withholdingTaxReceivable == true";
        }
    }
}
