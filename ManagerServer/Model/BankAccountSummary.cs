using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("77afd97c-51d7-484b-8192-b7eb006daadb")]
    public sealed class BankAccountSummary : Object, IHasCustomTheme
    {
        [Guide("Select the bank or cash account you want to analyze.")]
        [Guide("This report shows all transactions affecting the selected account during the specified periods.")]
        [Guide("Use this report to review account activity, verify balances, and analyze cash flow patterns.")]
        [ProtoMember(1), Autocomplete(typeof(ManagerServer.Model.BankOrCashAccount))] public Guid? BankAccount { get; set; }
        [Guide("Define one or more date ranges to analyze account activity.")]
        [Guide("Add multiple periods to compare account movements across different time frames.")]
        [Guide("Common uses include month-over-month or year-over-year comparisons.")]
        [ProtoMember(2), AddLineLabel(nameof(Strings.AddComparativeColumn))] public Period[] Periods { get; set; }
        [Guide("Enable this option to show account codes next to account names in the report.")]
        [Guide("Account codes help identify accounts quickly and are useful for mapping to external systems.")]
        [ProtoMember(3)] public bool AccountCodes { get; set; }
        [Guide("Enable this option to hide accounts that have no activity or zero net change during the period.")]
        [Guide("This creates a cleaner report by focusing only on accounts with actual transactions.")]
        [ProtoMember(4)] public bool ExcludeZeroBalances { get; set; }

        [ProtoMember(5), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(6), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [Guide("Enter the first date of the period you want to analyze.")]
            [Guide("Only cleared transactions on or after this date will be included in the summary.")]
            [ProtoMember(2)] public DateTime FromDate { get; set; }
            [Guide("Enter the last date of the period you want to analyze.")]
            [Guide("Only cleared transactions on or before this date will be included in the summary.")]
            [ProtoMember(3)] public DateTime ToDate { get; set; }
        }
    }
}
