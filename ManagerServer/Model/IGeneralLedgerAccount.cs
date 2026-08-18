using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model
{
    public interface IGeneralLedgerAccount
    {        
        string GetCodeAndName();
        string GetCode();
        string GetName();

        Guid Key { get; }

        string Name { get; }
        string Code { get; }
        bool IsBillableExpense { get; }
        bool IsAccountsReceivable { get; }
        bool IsAccountsPayable { get; }
        bool IsWithholdingTaxPayablePayable { get; }
        bool IsCashAtBank { get; }
        bool IsInterAccountTransfers { get; }
        bool IsInventoryOnHand { get; }
        bool IsControlAccountForCapitalAccounts { get; }
        bool IsEmployeeClearingAccount { get; }
        bool IsControlAccountForSpecialAccounts { get; }
        bool IsControlAccountForFixedAssets { get; }
        bool IsControlAccountForFixedAssetsAccumulatedDepreciation { get; }
        bool IsControlAccountForIntangibleAssets { get; }
        bool IsControlAccountForIntangibleAssetsAccumulatedAmortization { get; }
        bool HasCustomers { get; }
        bool HasSuppliers { get; }
        bool HasInvestments { get; }
        bool HasExpenseClaimPayers { get; }
        bool HasFixedAssets { get; }
        bool HasIntangibleAssets { get; }
        bool IsProfitAndLossAccount { get; }
        bool DivisionEnabled { get; }
        bool IsControlAccountForInvestments { get; }
        CashFlowStatementCategory CashFlowStatementCategory { get; }
        bool CanHaveCurrencyAmount { get; }

        Guid? GetCashFlowStatementGroup() { return null; }
    }
}