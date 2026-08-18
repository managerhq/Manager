using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("6d4af96a-0959-4bb2-9160-fa825ec67c43")]
    [Singleton]
    public sealed class BalanceSheetCashAtBankAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, ICode
    {
        [Guide("Enter the name for this control account that represents all bank and cash accounts combined.")]
        [Guide("The default name is `CashAndCashEquivalents` following standard accounting terminology.")]
        [Guide("This account aggregates all individual bank accounts and cash accounts for balance sheet presentation.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.CashAndCashEquivalents))] public string Name { get; set; }
        [Guide("Enter an optional account code to organize your chart of accounts systematically.")]
        [Guide("Account codes help with sorting accounts and can follow your existing numbering system.")]
        [Guide("Common codes for cash accounts range from 1000-1099 in many accounting systems.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the balance sheet group where this asset account should appear in financial reports.")]
        [Guide("Cash and cash equivalents are always current assets and typically appear first on the balance sheet.")]
        [Guide("This represents your most liquid assets available for immediate business use.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.CashAndCashEquivalents;
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
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.CashAndCashEquivalents;
        string ICode.Code => Code;
    }
}
