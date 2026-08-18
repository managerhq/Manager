using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("03d41bd1-8dd4-4ce3-9b82-5ce17a40171a")]
    [Singleton]
    public sealed class ProfitAndLossStatementAccountBillableTimeMovement : NamedObject, IProfitAndLossAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, ICode
    {
        [Guide("Name of account. The default name is `BillableTime_Movement` but it can be renamed.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.BillableTime_Movement))] public string Name { get; set; }
        [Guide("Enter code of the account if desired")]
        [ProtoMember(11), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select group on `ProfitAndLossStatement` under which this account should be presented.")]
        [ProtoMember(3), Autocomplete(typeof(ProfitAndLossStatementGroup)), Prepend(nameof(Strings.ProfitAndLossStatement))] public Guid? Group { get; set; }
        [ProtoMember(10)] public int Position { get; set; }

        public override string GetName()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            return Strings.BillableTime_Movement;
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
