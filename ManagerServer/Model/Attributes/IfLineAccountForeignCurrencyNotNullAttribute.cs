using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class IfLineAccountForeignCurrencyNotNullAttribute : IfAttribute
    {
        public IfLineAccountForeignCurrencyNotNullAttribute()
        {
        }

        public override string GetIfExpression()
        {
            return "this.getAccountCurrency(lineItem) != null";
        }
    }
}
