using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("054dfae1-c34a-475e-abde-49e0385ffc9a")]
    [Singleton]
    public sealed class BalanceSheetCapitalAccountsAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, IPurchaseInvoiceAccount, ISalesInvoiceAccount, ICode
    {
        [Guide("Enter the name for this control account that tracks owner or shareholder equity in the business.")]
        [Guide("The default name is `CapitalAccounts` but you can rename it to match your entity type.")]
        [Guide("Alternative names include 'Shareholder Equity', 'Partner Capital', or 'Member Equity'.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.CapitalAccounts))] public string Name { get; set; }
        [Guide("Enter an optional account code to organize your chart of accounts systematically.")]
        [Guide("Account codes help with sorting accounts and can follow your existing numbering system.")]
        [Guide("Common codes for capital accounts range from 3000-3999 in many accounting systems.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the balance sheet group where this equity account should appear in financial reports.")]
        [Guide("Capital accounts belong in the equity section, representing ownership interests in the business.")]
        [Guide("This control account aggregates all individual capital accounts for each owner or partner.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.CapitalAccounts;
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
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.FinancingActivities;
        string ICode.Code => Code;
    }
}
