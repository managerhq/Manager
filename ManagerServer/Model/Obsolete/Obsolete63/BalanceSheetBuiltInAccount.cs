using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete63
{
    [ProtoContract]
    [Guid("cbc9e14d-660e-4ed9-935a-fe1005b2a430")]
    public sealed class BalanceSheetBuiltInAccount : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(3)]
        public Guid? Group;        
        [ProtoMember(8)]
        public Guid? TaxCode;
        [ProtoMember(11)]
        public int Position;
        [ProtoMember(12)]
        public string Code;

        [ProtoMember(9)]
        internal ManagerServer.Model.Obsolete.Obsolete18.ControlAccount18 Obsolete_ControlAccount;
        [ProtoMember(4)]
        public bool Obsolete_HasStartingBalance;
        [ProtoMember(10)]
        public decimal Obsolete_StartingBalance;
        [ProtoMember(2)]
        public int? Obsolete_Code;
        [ProtoMember(5)]
        public decimal Obsolete_StartingBalance2;
        [ProtoMember(6)]
        public DebitCredit Obsolete_StartingBalanceType;

        /*
        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + GetName();
                else return GetName();
            }
        }

        public string GetCodeAndName()
        {
            return NameWithCode;
        }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return GetDefaultName(Key);
            return Name;
        }

        public string GetCode()
        {
            return Code;
        }

        Guid IGeneralLedgerAccount.Key => Key;
        string IGeneralLedgerAccount.Name => Name;
        string IGeneralLedgerAccount.Code => Code;       

        public static BalanceSheetBuiltInAccount CashAtBank { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.CashAtBank, Name = Strings.CashAtBank, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount CashOnHand { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.CashOnHand, Name = Strings.CashOnHand, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount InventoryOnHand { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.InventoryOnHand, Name = Strings.Inventory_on_hand, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount AccountsReceivable { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.AccountsReceivable, Name = Strings.Accounts_receivable, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount AccountsPayable { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.AccountsPayable, Name = Strings.Accounts_payable, Group = ChartOfAccountGroups.Liabilities };
        public static BalanceSheetBuiltInAccount EmployeeClearingAccount { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.EmployeeClearingAccount, Name = Strings.EmployeeClearingAccount, Group = ChartOfAccountGroups.Liabilities };
        public static BalanceSheetBuiltInAccount Suspense { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.Suspense, Name = Strings.Suspense, Group = ChartOfAccountGroups.Equity };
        public static BalanceSheetBuiltInAccount RetainedEarnings { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.RetainedEarnings, Name = Strings.Retained_earnings, Group = ChartOfAccountGroups.Equity };
        public static BalanceSheetBuiltInAccount SpecialAccounts { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.SpecialAccounts, Name = Strings.SpecialAccounts, Group = ChartOfAccountGroups.Equity };
        public static BalanceSheetBuiltInAccount CapitalAccounts { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.CapitalAccounts, Name = Strings.CapitalAccounts, Group = ChartOfAccountGroups.Equity };
        public static BalanceSheetBuiltInAccount IntangibleAssetsAccumulatedAmortization { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.IntangibleAssetsAccumulatedAmortization, Name = Strings.IntangibleAssetsAccumulatedAmortization, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount BillableExpensesAssetAccount { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.BillableExpensesAssetAccount, Name = Strings.Billable_expenses, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount BillableTimeUnbilled { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.BillableTimeUnbilled, Name = Strings.Billable_time, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount ExpenseClaims { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.ExpenseClaims, Name = Strings.Expense_claims, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount FixedAssets { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.FixedAssets, Name = Strings.Fixed_assets_at_cost, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount FixedAssetsAccumulatedDepreciation { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.FixedAssetsAccumulatedDepreciation, Name = Strings.FixedAssetsAccumulatedDepreciation, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount IntangibleAssets { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.IntangibleAssets, Name = Strings.Intangible_assets_at_cost, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount WithholdingTax { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.WithholdingTax, Name = Strings.WithholdingTax, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount WithholdingTaxReceivable { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.WithholdingTaxReceivable, Name = Strings.WithholdingTaxReceivable, Group = ChartOfAccountGroups.Assets };
        public static BalanceSheetBuiltInAccount ProductionInProgress { get; } = new BalanceSheetBuiltInAccount() { Key = Master.AccountKeys.ProductionInProgress, Name = Strings.ProductionInProgress, Group = ChartOfAccountGroups.Assets };

        public Guid GetGroup()
        {
            if (Group.HasValue) return Group.Value;
            if (Key == Manager.Model.Master.AccountKeys.AccountsPayable) return Manager.Model.Enums.ChartOfAccountGroups.Liabilities;
            if (Key == Manager.Model.Master.AccountKeys.AccountsReceivable) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.BillableExpensesAssetAccount) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.BillableTimeUnbilled) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.CapitalAccounts) return Manager.Model.Enums.ChartOfAccountGroups.Equity;
            if (Key == Manager.Model.Master.AccountKeys.CashAtBank) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.CashOnHand) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.EmployeeClearingAccount) return Manager.Model.Enums.ChartOfAccountGroups.Liabilities;
            if (Key == Manager.Model.Master.AccountKeys.ExpenseClaims) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.FixedAssets) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.FixedAssetsAccumulatedDepreciation) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.IntangibleAssets) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.IntangibleAssetsAccumulatedAmortization) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.InventoryOnHand) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.RetainedEarnings) return Manager.Model.Enums.ChartOfAccountGroups.Equity;
            if (Key == Manager.Model.Master.AccountKeys.SpecialAccounts) return Manager.Model.Enums.ChartOfAccountGroups.Equity;
            if (Key == Manager.Model.Master.AccountKeys.WithholdingTax) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.WithholdingTaxReceivable) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            if (Key == Manager.Model.Master.AccountKeys.ProductionInProgress) return Manager.Model.Enums.ChartOfAccountGroups.Assets;
            return Manager.Model.Enums.ChartOfAccountGroups.Equity;
        }

        public static string GetDefaultName(Guid key)
        {
            if (key == Manager.Model.Master.AccountKeys.AccountsPayable) return Strings.Accounts_payable;
            if (key == Manager.Model.Master.AccountKeys.AccountsReceivable) return Strings.Accounts_receivable;
            if (key == Manager.Model.Master.AccountKeys.BillableExpensesAssetAccount) return Strings.Billable_expenses;
            if (key == Manager.Model.Master.AccountKeys.BillableTimeUnbilled) return Strings.Billable_time;
            if (key == Manager.Model.Master.AccountKeys.CapitalAccounts) return Strings.CapitalAccounts;
            if (key == Manager.Model.Master.AccountKeys.CashAtBank) return Strings.CashAtBank;
            if (key == Manager.Model.Master.AccountKeys.CashOnHand) return Strings.CashOnHand;
            if (key == Manager.Model.Master.AccountKeys.EmployeeClearingAccount) return Strings.EmployeeClearingAccount;
            if (key == Manager.Model.Master.AccountKeys.ExpenseClaims) return Strings.Expense_claims;
            if (key == Manager.Model.Master.AccountKeys.FixedAssets) return Strings.Fixed_assets_at_cost;
            if (key == Manager.Model.Master.AccountKeys.FixedAssetsAccumulatedDepreciation) return Strings.FixedAssetsAccumulatedDepreciation;
            if (key == Manager.Model.Master.AccountKeys.IntangibleAssets) return Strings.Intangible_assets_at_cost;
            if (key == Manager.Model.Master.AccountKeys.IntangibleAssetsAccumulatedAmortization) return Strings.IntangibleAssetsAccumulatedAmortization;
            if (key == Manager.Model.Master.AccountKeys.InventoryOnHand) return Strings.Inventory_on_hand;
            if (key == Manager.Model.Master.AccountKeys.RetainedEarnings) return Strings.Retained_earnings;
            if (key == Manager.Model.Master.AccountKeys.SpecialAccounts) return Strings.SpecialAccounts;
            if (key == Manager.Model.Master.AccountKeys.WithholdingTax) return Strings.WithholdingTax;
            if (key == Manager.Model.Master.AccountKeys.WithholdingTaxReceivable) return Strings.WithholdingTaxReceivable;
            if (key == Manager.Model.Master.AccountKeys.ProductionInProgress) return Strings.ProductionInProgress;
            return null;
        }

        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory
        {
            get
            {
                if (Key == Manager.Model.Master.AccountKeys.CapitalAccounts) return CashFlowStatementCategory.FinancingActivities;
                if (Key == Manager.Model.Master.AccountKeys.RetainedEarnings) return CashFlowStatementCategory.FinancingActivities;
                if (Key == Manager.Model.Master.AccountKeys.FixedAssets) return CashFlowStatementCategory.InvestingActivities;
                if (Key == Manager.Model.Master.AccountKeys.FixedAssetsAccumulatedDepreciation) return CashFlowStatementCategory.FinancingActivities;
                if (Key == Manager.Model.Master.AccountKeys.IntangibleAssets) return CashFlowStatementCategory.InvestingActivities;
                if (Key == Manager.Model.Master.AccountKeys.IntangibleAssetsAccumulatedAmortization) return CashFlowStatementCategory.FinancingActivities;
                return CashFlowStatementCategory.OperatingActivities;
            }
        }
        */
    }
}
