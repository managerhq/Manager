using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("635ddd64-1176-4d35-b1c2-2d7d3bb12bb6")]
    [Singleton]
    public sealed class ProfitAndLossStatementAccountCurrencyGainsLosses : NamedObject, IProfitAndLossAccount, ICode
    {
        [Guide("Name of account. The default name is `CurrencyGainsLosses` but it can be renamed.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.CurrencyGainsLosses))] public string Name { get; set; }
        [Guide("Enter code of the account if desired")]
        [ProtoMember(11), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select group on `ProfitAndLossStatement` under which this account should be presented.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(ProfitAndLossStatementGroup)), Prepend(nameof(Strings.ProfitAndLossStatement))] public Guid? Group { get; set; }
        [Guide("Select group on `CashFlowStatement` under which this account should be presented.")]
        [ProtoMember(12), EmptyLabel, Autocomplete(typeof(CashFlowStatementOperatingActivityGroup)), Prepend(nameof(Strings.CashFlowStatement), "-", nameof(Strings.OperatingActivities))] public Guid? CashFlowStatementGroup { get; set; }
        [ProtoMember(10)] public int Position { get; set; }

        public override string GetName()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            return Strings.CurrencyGainsLosses;
        }

        Guid IGeneralLedgerAccount.Key => Key;
        string IGeneralLedgerAccount.Name => Name;
        string IGeneralLedgerAccount.Code => Code;
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.OperatingActivities;
        public Guid? GetCashFlowStatementGroup() { return CashFlowStatementGroup; }
        string ICode.Code => Code;

        public string GetCode()
        {
            return Code;
        }

        public override string GetCodeAndName()
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
    }
}