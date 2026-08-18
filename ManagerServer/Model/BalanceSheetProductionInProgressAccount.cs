/*
using System;
using Manager.Model.Attributes;
using Manager.Globalization;
using Manager.Model.Enums;
using ProtoBuf;

namespace Manager.Model
{
    [ProtoContract]
    [Guid("30a1b83c-68a8-4f2c-ae70-25b0acc2d12a")]
    [Singleton]
    public sealed class BalanceSheetProductionInProgressAccount : NamedObject, IBalanceSheetAccount, ICode
    {
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.ProductionInProgress))] public string Name { get; set; }
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.ProductionInProgress;
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
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.OperatingActivities;
        string ICode.Code => Code;
    }
}
*/