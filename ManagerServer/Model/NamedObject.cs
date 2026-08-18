using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model
{
    public abstract class NamedObject : ManagerServer.Model.Object, IComparable<NamedObject>
    {
        public abstract string GetName();

        public string text => GetName(); // For Select2 component

        public virtual string GetCodeAndName()
        {
            return GetName();
        }

        public string UniqueName { get { return GetCodeAndName(); } }
        public bool IsBillableExpense { get { return this is BalanceSheetBillableExpensesAccount; } }
        public bool IsControlAccountForInvestments { get { return this is BalanceSheetInvestmentsAccount || this is ControlAccountForInvestments; } }
        public bool IsWithholdingTaxPayablePayable { get { return this is BalanceSheetWithholdingTaxPayableAccount; } }
        public bool IsAccountsReceivable { get { return this is ControlAccountForCustomers || this is BalanceSheetAccountsReceivableAccount; } }
        public bool IsAccountsPayable { get { return this is ControlAccountForSuppliers || this is BalanceSheetAccountsPayableAccount; } }
        public bool IsCashAtBank { get { return this is ControlAccountForBankAccounts || this is BalanceSheetCashAtBankAccount; } }
        public bool IsInterAccountTransfers { get { return this is BalanceSheetInterAccountTransfers; } }
        public bool IsInventoryOnHand { get { return this is ControlAccountForInventoryItems || this is BalanceSheetInventoryOnHandAccount; } }
        public bool IsControlAccountForCapitalAccounts { get { return this is ControlAccountForCapitalAccounts || this is BalanceSheetCapitalAccountsAccount; } }
        public bool IsEmployeeClearingAccount { get { return this is ControlAccountForEmployees || this is BalanceSheetEmployeeClearingAccount; } }
        public bool IsControlAccountForSpecialAccounts { get { return this is ControlAccountForSpecialAccounts || this is BalanceSheetSpecialAccountsAccount; } }
        public bool IsControlAccountForFixedAssets { get { return this is ControlAccountForFixedAssets || this is BalanceSheetFixedAssetsAtCostAccount; } }
        public bool IsControlAccountForFixedAssetsAccumulatedDepreciation { get { return this is ControlAccountForFixedAssetsAccumulatedDepreciation || this is BalanceSheetFixedAssetsAccumulatedDepreciationAccount; } }
        public bool IsControlAccountForIntangibleAssets { get { return this is ControlAccountForIntangibleAssets || this is BalanceSheetIntangibleAssetsAtCostAccount; } }
        public bool IsControlAccountForIntangibleAssetsAccumulatedAmortization { get { return this is ControlAccountForIntangibleAssetsAccumulatedAmortization || this is BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount; } }
        public bool HasCustomers { get { return IsAccountsReceivable || this is BalanceSheetBillableExpensesAccount || this is BalanceSheetBillableTimeAccount || this is BalanceSheetWithholdingTaxReceivableAccount; } }
        public bool HasInvestments { get { return this is BalanceSheetInvestmentsAccount || this is ControlAccountForInvestments; } }
        public bool HasSuppliers { get { return IsAccountsPayable || this is BalanceSheetWithholdingTaxPayableAccount; } }
        public bool HasFixedAssets { get { return IsControlAccountForFixedAssets || IsControlAccountForFixedAssetsAccumulatedDepreciation; } }
        public bool HasIntangibleAssets { get { return IsControlAccountForIntangibleAssets || IsControlAccountForIntangibleAssetsAccumulatedAmortization; } }
        public bool IsProfitAndLossAccount { get { return this is IProfitAndLossAccount; } }
        public bool HasExpenseClaimPayers { get { return this is BalanceSheetExpenseClaimsAccount; } }
        public bool TaxCodeEnabled { get { return !IsAccountsPayable && !IsAccountsReceivable && !IsEmployeeClearingAccount && !IsCashAtBank && !IsControlAccountForFixedAssetsAccumulatedDepreciation && !IsControlAccountForIntangibleAssetsAccumulatedAmortization && !IsInterAccountTransfers; } }
        public bool DivisionEnabled { get { return IsProfitAndLossAccount || this is BalanceSheetRetainedEarningsAccount || this is BalanceSheetBillableExpensesAccount || this is BalanceSheetAccount; } }
        public bool ProjectEnabled { get { return !IsAccountsPayable && !IsAccountsReceivable && !IsEmployeeClearingAccount && !IsInventoryOnHand && !IsCashAtBank && !IsInterAccountTransfers && !IsControlAccountForFixedAssetsAccumulatedDepreciation && !IsControlAccountForIntangibleAssetsAccumulatedAmortization; } }
        public bool CanHaveCurrencyAmount { get { return IsAccountsReceivable || IsAccountsPayable || IsEmployeeClearingAccount || IsControlAccountForSpecialAccounts || IsCashAtBank ||  IsInterAccountTransfers; } }

        public virtual bool OnAutocomplete(ManagerServer.Model.Object filter)
        {
            return true;
        }

        public bool IsActive(Database database)
        {
            if (this is BalanceSheetAccountsPayableAccount && !database.OfType<ManagerServer.Model.Supplier>().Any()) return false;
            if (this is BalanceSheetAccountsReceivableAccount && !database.OfType<ManagerServer.Model.Customer>().Any()) return false;
            if (this is BalanceSheetBillableExpensesAccount && !database.Single<BillableExpenses>().Enabled) return false;
            if (this is BalanceSheetBillableTimeAccount && !database.OfType<ManagerServer.Model.BillableTime>().Any()) return false;
            if (this is BalanceSheetCapitalAccountsAccount && !database.OfType<ManagerServer.Model.CapitalAccount>().Any()) return false;
            if (this is BalanceSheetCashAtBankAccount && !database.OfType<ManagerServer.Model.BankOrCashAccount>().Any()) return false;
            if (this is BalanceSheetEmployeeClearingAccount && !database.OfType<ManagerServer.Model.Employee>().Any()) return false;
            if (this is BalanceSheetExpenseClaimsAccount && !database.OfType<ManagerServer.Model.ExpenseClaimsPayer>().Any()) return false;
            if (this is BalanceSheetFixedAssetsAccumulatedDepreciationAccount && !database.OfType<ManagerServer.Model.FixedAsset>().Any()) return false;
            if (this is BalanceSheetFixedAssetsAtCostAccount && !database.OfType<ManagerServer.Model.FixedAsset>().Any()) return false;
            if (this is BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount && !database.OfType<ManagerServer.Model.IntangibleAsset>().Any()) return false;
            if (this is BalanceSheetIntangibleAssetsAtCostAccount && !database.OfType<ManagerServer.Model.IntangibleAsset>().Any()) return false;
            if (this is BalanceSheetInventoryOnHandAccount && !database.OfType<ManagerServer.Model.InventoryItem>().Any()) return false;
            if (this is BalanceSheetSpecialAccountsAccount && !database.OfType<ManagerServer.Model.SpecialAccount>().Any()) return false;
            if (this is BalanceSheetInvestmentsAccount && !database.OfType<ManagerServer.Model.Investment>().Any()) return false;
            if (this is BalanceSheetWithholdingTaxAccount && !database.OfType<ManagerServer.Model.WithholdingTaxReceipt>().Any()) return false;
            if (this is BalanceSheetWithholdingTaxReceivableAccount && !database.Single<WithholdingTax>().WithholdingTaxReceivable) return false;
            if (this is BalanceSheetWithholdingTaxPayableAccount && !database.Single<WithholdingTax>().WithholdingTaxPayable) return false;

            if (this is BalanceSheetTaxPayableAccount && !database.OfType<ManagerServer.Model.TaxCode>().Any(x => x.HasDefaultControlAccount())) return false;

            if (this is ProfitAndLossStatementAccountBillableExpensesCost && !database.Single<BillableExpenses>().Enabled) return false;
            if (this is ProfitAndLossStatementAccountBillableExpensesInvoiced && !database.Single<BillableExpenses>().Enabled) return false;
            if (this is ProfitAndLossStatementAccountBillableTimeInvoiced && !database.OfType<ManagerServer.Model.BillableTime>().Any()) return false;
            if (this is ProfitAndLossStatementAccountBillableTimeMovement && !database.OfType<ManagerServer.Model.BillableTime>().Any()) return false;
            //if (this is ProfitAndLossStatementAccountCurrencyGainsLosses) return false;
            if (this is ProfitAndLossStatementAccountFixedAssetDepreciation && !database.OfType<ManagerServer.Model.FixedAsset>().Any()) return false;
            if (this is ProfitAndLossStatementAccountFixedAssetLossOnDisposal && !database.OfType<ManagerServer.Model.FixedAsset>().Any()) return false;
            if (this is ProfitAndLossStatementAccountIntangibleAssetsAmortization && !database.OfType<ManagerServer.Model.IntangibleAsset>().Any()) return false;
            if (this is ProfitAndLossStatementAccountIntangibleAssetsGainsLossOnDisposal && !database.OfType<ManagerServer.Model.IntangibleAsset>().Any()) return false;
            if (this is ProfitAndLossStatementAccountInventoryPurchases && !database.OfType<ManagerServer.Model.InventoryItem>().Any()) return false;
            if (this is ProfitAndLossStatementAccountInventorySales && !database.OfType<ManagerServer.Model.InventoryItem>().Any()) return false;
            if (this is ProfitAndLossStatementAccountInventoryWriteOffs && !database.OfType<ManagerServer.Model.InventoryItem>().Any()) return false;
            if (this is ProfitAndLossStatementAccountLatePaymentFees && !database.OfType<ManagerServer.Model.LatePaymentFee>().Any()) return false;
            if (this is ProfitAndLossStatementCapitalGainsOnInvestments && !database.OfType<ManagerServer.Model.Investment>().Any()) return false;
            //if (this is ProfitAndLossStatementAccountRoundingExpense) return false;

            return true;
        }

        public int CompareTo(NamedObject other)
        {
            if (other == null) return 1; // This instance is greater than null.
            return (this.IsInactive(), this.GetName()).CompareTo((other.IsInactive(), other.GetName()));
        }
    }
}
