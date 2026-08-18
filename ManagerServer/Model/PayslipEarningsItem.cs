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
    [Guid("ab02f6ab-c91c-4fc2-b979-66a6682c200e")]
    public sealed class PayslipEarningsItem : NamedObject, IComparable<PayslipEarningsItem>
    {
        [Guide("Enter a descriptive name for this earnings item, such as 'Regular Wages', 'Overtime', 'Bonus', or 'Housing Allowance'.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Select the expense account where earnings will be recorded. This is typically a wages or salaries expense account.")]
        [ProtoMember(3), TableColumn, Autocomplete(typeof(ProfitAndLossStatementAccount), Subtext = nameof(ProfitAndLossStatementAccount.Group)), Placeholder(nameof(Strings.Suspense))] public Guid? ExpenseAccount { get; set; }
        [Guide("Select a reporting category to group this earnings item for reporting purposes. This helps in analyzing wage costs by category.")]
        [ProtoMember(5), TableColumn, Autocomplete(typeof(PayslipEarningsItemReportingCategory)), Short] public Guid? ReportingCategory { get; set; }
        [Guide("Check this box to make the earnings item inactive. Inactive items won't appear in selection lists but remain in historical records.")]
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

        public int CompareTo(PayslipEarningsItem other)
        {
            return (this.Inactive, this.Name).CompareTo((other.Inactive, other.Name));
        }
    }
}
