using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("31d369e3-32c7-4bd2-bb83-9c1c58010c1a")]
    [Singleton]
    public sealed class BalanceSheetIntangibleAssetsAtCostAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, IPurchaseInvoiceAccount, ISalesInvoiceAccount, ICode
    {
        [Guide("Name of account. The default name is `Intangible_assets_at_cost` but it can be renamed.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.Intangible_assets_at_cost))] public string Name { get; set; }
        [Guide("Enter code of the account if desired")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select group on `BalanceSheet` under which this account should be presented.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.Intangible_assets_at_cost;
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
