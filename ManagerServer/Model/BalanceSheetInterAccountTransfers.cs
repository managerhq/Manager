using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("b300e679-cf46-4c99-bef0-cfa5cb3e0271")]
    [Singleton]
    public sealed class BalanceSheetInterAccountTransfers : NamedObject, IBalanceSheetAccount, IReceiptOrPaymentAccount, ICode
    {
        [Guide("Name of account. The default name is `InterAccountTransfers` but it can be renamed.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.InterAccountTransfers))] public string Name { get; set; }
        [Guide("Enter code of the account if desired")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select group on `BalanceSheet` under which this account should be presented.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.InterAccountTransfers;
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
