using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("4a0e8917-fee2-4033-9161-48dd513fdb73")]
    [Singleton]
    public sealed class BalanceSheetFixedAssetsAtCostAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, IInventoryWriteOffAccount, IReceiptOrPaymentAccount, IPurchaseInvoiceAccount, ISalesInvoiceAccount, ICode
    {
        [Guide("Enter the name for this account. The default name is `Fixed_assets_at_cost`, but you can rename it to better suit your business needs.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.Fixed_assets_at_cost))] public string Name { get; set; }
        [Guide("Optionally, enter an account code. Codes help organize accounts and can be used for searching and sorting in reports.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the `BalanceSheet` group where this account should appear. This determines its placement on the balance sheet report.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.Fixed_assets_at_cost;
            return Name;
        }

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + GetName();
                else return GetName();
            }
        }

        public override string GetCodeAndName()
        {
            return NameWithCode;
        }        

        public string GetCode()
        {
            return Code;
        }

        Guid IGeneralLedgerAccount.Key => Key;
        string IGeneralLedgerAccount.Name => Name;
        string IGeneralLedgerAccount.Code => Code;
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.InvestingActivities;
        string ICode.Code => Code;
    }
}
