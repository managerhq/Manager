using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("73af4c68-c347-4088-8846-758f1e7bc5bb")]
    public sealed class PayslipContributionItem : NamedObject, IComparable<PayslipContributionItem>
    {
        [Guide("Enter a descriptive name for this contribution item, such as 'Employer Superannuation' or 'Company Pension Contribution'.")]
        [ProtoMember(1)] public string Name { get; set; }
        [Guide("Select the expense account where employer contributions will be recorded. This is typically a payroll expense account.")]
        [ProtoMember(3), Autocomplete(typeof(ProfitAndLossStatementAccount), Subtext = nameof(ProfitAndLossStatementAccount.Group)), Placeholder(nameof(Strings.Suspense))] public Guid? ExpenseAccount { get; set; }
        [Guide("Select the liability account where unpaid contributions will be recorded until they are paid to the relevant authority or fund.")]
        [ProtoMember(4), Autocomplete(typeof(BalanceSheetAccount), Subtext = nameof(BalanceSheetAccount.Group)), Placeholder(nameof(Strings.Suspense))] public Guid? LiabilityAccount { get; set; }
        [Guide("Select a reporting category to group this contribution item for reporting purposes. This helps in analyzing contribution costs by category.")]
        [ProtoMember(6), Autocomplete(typeof(PayslipContributionItemReportingCategory)), Short] public Guid? ReportingCategory { get; set; }
        [Guide("Check this box to make the contribution item inactive. Inactive items won't appear in selection lists but remain in historical records.")]
        [ProtoMember(8)] public bool Inactive { get; set; }
        [ProtoMember(5)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(7)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(2)] public string Obsolete_Description { get; set; }

        public override bool IsInactive()
        {
            return Inactive;
        }

        public override string GetName()
        {
            return Name;
        }

        public int CompareTo(PayslipContributionItem other)
        {
            return (this.Inactive, this.Name).CompareTo((other.Inactive, other.Name));
        }
    }
}