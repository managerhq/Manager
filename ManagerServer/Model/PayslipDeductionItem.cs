using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("0444eb18-6fc5-4d1f-be8b-c114da01832c")]
    public sealed class PayslipDeductionItem : NamedObject, IComparable<PayslipDeductionItem>
    {
        [Guide("Enter a descriptive name for this deduction item, such as 'Income Tax', 'Employee Pension Contribution', or 'Union Dues'.")]
        [ProtoMember(1)] public string Name { get; set; }
        [Guide("Select the account where deducted amounts will be recorded. This is typically a liability account for amounts owed to tax authorities or other parties.")]
        [ProtoMember(3), Autocomplete(typeof(ICustomGeneralLedgerAccount), Subtext = nameof(BalanceSheetAccount.Group)), Placeholder(nameof(Strings.Suspense))] public Guid? Account { get; set; }
        [Guide("Select a reporting category to group this deduction item for reporting purposes. This helps in analyzing deductions by category.")]
        [ProtoMember(5), Autocomplete(typeof(PayslipDeductionItemReportingCategory)), Short] public Guid? ReportingCategory { get; set; }
        [Guide("Check this box to make the deduction item inactive. Inactive items won't appear in selection lists but remain in historical records.")]
        [ProtoMember(7)] public bool Inactive { get; set; }
        [ProtoMember(4)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(6)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(2)] public string Obsolete_Description { get; set; }

        public override bool IsInactive()
        {
            return Inactive;
        }

        public override string GetName()
        {
            return Name;
        }

        public int CompareTo(PayslipDeductionItem other)
        {
            return (this.Inactive, this.Name).CompareTo((other.Inactive, other.Name));
        }
    }
}
