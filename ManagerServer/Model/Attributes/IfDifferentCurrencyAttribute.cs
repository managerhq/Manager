using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class IfDifferentCurrencyAttribute : IfAttribute
    {
        public IfDifferentCurrencyAttribute()
        {
        }

        public override string GetIfExpression()
        {
            return "this.getAccount(lineItem) != null && (this.getAccount(lineItem).IsAccountsReceivable || this.getAccount(lineItem).IsAccountsPayable || this.getAccount(lineItem).IsEmployeeClearingAccount || this.getAccount(lineItem).IsControlAccountForSpecialAccounts || this.getAccount(lineItem).IsCashAtBank) && this.getForeignCurrencyKey() != this.getAccountCurrency(lineItem)";
        }
    }
}
