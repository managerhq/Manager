using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("dd43296a-c233-4be8-93e4-fb3c2b8138f5")]
    [Singleton]
    public sealed class BalanceSheetNegativeInventoryClearing : NamedObject, IBalanceSheetAccount, ICode
    {
        [Guide("Name of account. The default name is `NegativeInventoryClearing` but it can be renamed.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.NegativeInventoryClearing))] public string Name { get; set; }
        [Guide("Enter code of the account if desired")]
        [ProtoMember(11), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select group on `BalanceSheet` under which this account should be presented.")]
        [ProtoMember(4), Autocomplete(typeof(BalanceSheetGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(10)] public int Position { get; set; }

        public override string GetName()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            return Strings.NegativeInventoryClearing;
        }

        Guid IGeneralLedgerAccount.Key => Key;
        string IGeneralLedgerAccount.Name => Name;
        string IGeneralLedgerAccount.Code => Code;
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.OperatingActivities;
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
