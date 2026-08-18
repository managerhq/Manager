using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("ea44f579-9548-4954-baf0-48538aceff1e")]
    [Singleton]
    public sealed class ProfitAndLossStatementAccountInventorySales : NamedObject, IProfitAndLossAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, ICode
    {
        [Guide("Name of account. The default name is `InventorySales` but it can be renamed.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.InventorySales))] public string Name { get; set; }
        [Guide("Enter code of the account if desired")]
        [ProtoMember(11), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select group on `ProfitAndLossStatement` under which this account should be presented.")]
        [ProtoMember(3), Autocomplete(typeof(ProfitAndLossStatementGroup)), Prepend(nameof(Strings.ProfitAndLossStatement))] public Guid? Group { get; set; }
        [Guide("Select default tax codes for this account if you are using `TaxCodes`.")]
        [ProtoMember(12), IfContains<TaxCode>, Label(nameof(Strings.Autofill), nameof(Strings.TaxCode))] public bool HasDefaultTaxCode { get; set; }
        [ProtoMember(8), IfTrue(nameof(HasDefaultTaxCode)), Autocomplete(typeof(TaxCode)), NoLabel, Short] public Guid? DefaultTaxCode { get; set; }
        [ProtoMember(10)] public int Position { get; set; }

        public override string GetName()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            return Strings.InventorySales;
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
