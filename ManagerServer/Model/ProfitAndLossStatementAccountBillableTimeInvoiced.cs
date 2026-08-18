using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("8d86390b-b90f-4cf6-862b-9b4050449b91")]
    [Singleton]
    public sealed class ProfitAndLossStatementAccountBillableTimeInvoiced : NamedObject, IProfitAndLossAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, ISalesInvoiceAccount, ICode
    {
        [Guide("Name of account. The default name is `Billable_time_invoiced` but it can be renamed.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.Billable_time_invoiced))] public string Name { get; set; }
        [Guide("Enter code of the account if desired")]
        [ProtoMember(11), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select group on `ProfitAndLossStatement` under which this account should be presented.")]
        [ProtoMember(3), Autocomplete(typeof(ProfitAndLossStatementGroup)), Prepend(nameof(Strings.ProfitAndLossStatement))] public Guid? Group { get; set; }
        [Guide("Select default tax code for this account if you are using `TaxCodes`.")]
        [ProtoMember(12), IfContains<TaxCode>, Label(nameof(Strings.Autofill), nameof(Strings.TaxCode))] public bool HasDefaultTaxCode { get; set; }
        [ProtoMember(8), IfTrue(nameof(HasDefaultTaxCode)), Autocomplete(typeof(TaxCode)), NoLabel, Short] public Guid? DefaultTaxCode { get; set; }
        [ProtoMember(10)] public int Position { get; set; }

        public override string GetName()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            return Strings.Billable_time_invoiced;
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
