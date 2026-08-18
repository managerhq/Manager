using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete62
{
    [ProtoContract]
    [Guid("8d92f526-af35-4552-af6e-4b403b0cc52f")]
    public sealed class ProfitAndLossStatementBuiltInAccount : ManagerServer.Model.Object
    {
        [ProtoMember(1)]
        public string Name;        
        [ProtoMember(3)]
        public Guid? Group;
        [ProtoMember(8)]
        public Guid? TaxCode;
        [ProtoMember(10)]
        public int Position;
        [ProtoMember(11)]
        public string Code;

        [ProtoMember(2)]
        public int? Obsolete_Code;
        [ProtoMember(9)]
        internal ManagerServer.Model.Obsolete.Obsolete18.ControlAccount18 Obsolete_ControlAccount;

        /*
        public static ProfitAndLossStatementBuiltInAccount InventorySales { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.InventorySales, Name = Strings.InventorySales, Group = ChartOfAccountGroups.Income };
        public static ProfitAndLossStatementBuiltInAccount InventoryPurchases { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.InventoryPurchases, Name = Strings.InventoryCost, Group = ChartOfAccountGroups.Expenses };
        public static ProfitAndLossStatementBuiltInAccount IntangibleAssetsAmortization { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.IntangibleAssetsAmortization, Name = Strings.IntangibleAssetsAmortization, Group = ChartOfAccountGroups.Expenses };
        public static ProfitAndLossStatementBuiltInAccount BillableExpensesInvoiced { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.BillableExpensesInvoiced, Name = Strings.Billable_expenses_invoiced, Group = ChartOfAccountGroups.Income };
        public static ProfitAndLossStatementBuiltInAccount BillableExpensesCost { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.BillableExpensesCost, Name = Strings.Billable_expenses_cost, Group = ChartOfAccountGroups.Expenses };
        public static ProfitAndLossStatementBuiltInAccount BillableTimeInvoiced { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.BillableTimeInvoiced, Name = Strings.Billable_time_invoiced, Group = ChartOfAccountGroups.Income };
        public static ProfitAndLossStatementBuiltInAccount BillableTimeMovement { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.BillableTimeMovement, Name = Strings.BillableTime_Movement, Group = ChartOfAccountGroups.Expenses };
        public static ProfitAndLossStatementBuiltInAccount CurrencyGainLoss { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.CurrencyGainLoss, Name = Strings.CurrencyGainsLosses, Group = ChartOfAccountGroups.Expenses };
        public static ProfitAndLossStatementBuiltInAccount FixedAssetDepreciation { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.FixedAssetDepreciation, Name = Strings.Fixed_assets_depreciation, Group = ChartOfAccountGroups.Expenses };
        public static ProfitAndLossStatementBuiltInAccount FixedAssetsLossOnDisposal { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.FixedAssetsLossOnDisposal, Name = Strings.FixedAssetsLossOnDisposal, Group = ChartOfAccountGroups.Expenses };
        public static ProfitAndLossStatementBuiltInAccount IntangibleAssetsGainLossOnDisposal { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.IntangibleAssetsGainLossOnDisposal, Name = Strings.IntangibleAssetsLossOnDisposal, Group = ChartOfAccountGroups.Expenses };
        public static ProfitAndLossStatementBuiltInAccount LatePaymentFees { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.LatePaymentFees, Name = Strings.LatePaymentFees, Group = ChartOfAccountGroups.Expenses };
        public static ProfitAndLossStatementBuiltInAccount RoundingExpense { get; } = new ProfitAndLossStatementBuiltInAccount() { Key = Master.AccountKeys.RoundingExpense, Name = Strings.RoundingExpense, Group = ChartOfAccountGroups.Expenses };

        public static string GetDefaultName(Guid key)
        {
            if (key == Manager.Model.Master.AccountKeys.BillableExpensesInvoiced) return Strings.Billable_expenses_invoiced;
            if (key == Manager.Model.Master.AccountKeys.BillableExpensesCost) return Strings.Billable_expenses_cost;
            if (key == Manager.Model.Master.AccountKeys.BillableTimeInvoiced) return Strings.Billable_time_invoiced;
            if (key == Manager.Model.Master.AccountKeys.BillableTimeMovement) return Strings.BillableTime_Movement;
            if (key == Manager.Model.Master.AccountKeys.CurrencyGainLoss) return Strings.CurrencyGainsLosses;
            if (key == Manager.Model.Master.AccountKeys.FixedAssetDepreciation) return Strings.Fixed_assets_depreciation;
            if (key == Manager.Model.Master.AccountKeys.FixedAssetsLossOnDisposal) return Strings.FixedAssetsLossOnDisposal;
            if (key == Manager.Model.Master.AccountKeys.IntangibleAssetsAmortization) return Strings.IntangibleAssetsAmortization;
            if (key == Manager.Model.Master.AccountKeys.IntangibleAssetsGainLossOnDisposal) return Strings.IntangibleAssetsLossOnDisposal;
            if (key == Manager.Model.Master.AccountKeys.InventoryPurchases) return Strings.InventoryCost;
            if (key == Manager.Model.Master.AccountKeys.InventorySales) return Strings.InventorySales;
            if (key == Manager.Model.Master.AccountKeys.LatePaymentFees) return Strings.LatePaymentFees;
            if (key == Manager.Model.Master.AccountKeys.RoundingExpense) return Strings.RoundingExpense;
            if (key == Manager.Model.Master.AccountKeys.Suspense) return Strings.Suspense;
            return null;
        }

        public static Guid? GetDefaultGroup(Guid key)
        {
            if (key == Manager.Model.Master.AccountKeys.BillableExpensesInvoiced) return ChartOfAccountGroups.Income;
            if (key == Manager.Model.Master.AccountKeys.BillableExpensesCost) return ChartOfAccountGroups.Expenses;
            if (key == Manager.Model.Master.AccountKeys.BillableTimeInvoiced) return ChartOfAccountGroups.Income;
            if (key == Manager.Model.Master.AccountKeys.BillableTimeMovement) return ChartOfAccountGroups.Expenses;
            if (key == Manager.Model.Master.AccountKeys.CurrencyGainLoss) return ChartOfAccountGroups.Expenses;
            if (key == Manager.Model.Master.AccountKeys.FixedAssetDepreciation) return ChartOfAccountGroups.Expenses;
            if (key == Manager.Model.Master.AccountKeys.FixedAssetsLossOnDisposal) return ChartOfAccountGroups.Expenses;
            if (key == Manager.Model.Master.AccountKeys.IntangibleAssetsAmortization) return ChartOfAccountGroups.Expenses;
            if (key == Manager.Model.Master.AccountKeys.IntangibleAssetsGainLossOnDisposal) return ChartOfAccountGroups.Expenses;
            if (key == Manager.Model.Master.AccountKeys.InventoryPurchases) return ChartOfAccountGroups.Expenses;
            if (key == Manager.Model.Master.AccountKeys.InventorySales) return ChartOfAccountGroups.Income;
            if (key == Manager.Model.Master.AccountKeys.LatePaymentFees) return ChartOfAccountGroups.Expenses;
            if (key == Manager.Model.Master.AccountKeys.RoundingExpense) return ChartOfAccountGroups.Expenses;
            throw new Exception();
        }

        public override string GetName()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            return GetDefaultName(Key);
        }

        Guid IGeneralLedgerAccount.Key => Key;
        string IGeneralLedgerAccount.Name => Name;
        string IGeneralLedgerAccount.Code => Code;
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.OperatingActivities;

        public string GetCode()
        {
            return Code;
        }

        public string GetCodeAndName()
        {
            return NameWithCode;
        }        

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + GetName();
                else return GetName();
            }
        }
        */
    }
}
