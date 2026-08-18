using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("2631d044-861d-4710-a871-d7a11461b4ba")]
    public sealed class Summary : Object
    {
        [Guide("Select `ShowBalancesForSpecifiedPeriod` to ensure the summary displays figures only for the specified period.")]
        [Guide("When this option is enabled, the `Summary` screen will provide a notification if there are any transactions dated after the specified period.")]
        [Guide("This helps clarify why newly added transactions do not impact the figures shown on the `Summary` screen.")]
        [Guide("Typically you will set `ShowBalancesForSpecifiedPeriod` once you've been using the program for more than one accounting period.")]
        [Guide("Under these circumstances, you would adjust the period on the `Summary` screen to reflect a single financial period, such as a financial year.")]
        [Guide("This means `ProfitAndLossStatement` figures in `Summary` tab won't be increasing perpetually but will be showing only for the current accounting period.")]
        [Guide("If you are migrating existing business to Manager.io, you should set `ShowBalancesForSpecifiedPeriod` right away to your current accounting period.")]
        [Guide("This is because migrating to Manager.io typically involves entering historical transactions to establish opening balances.")]
        [Guide("For example, when entering starting balances for customers, you'd enter all their unpaid invoices with historical dates. These unpaid invoices would credit your income accounts but you don't necessarily want to see this historical income in your `Summary` tab since this income belongs to past accounting periods.")]
        [ProtoMember(5)] public bool ShowBalancesForSpecifiedPeriod { get; set; }
        [ProtoMember(1), IfTrue(nameof(ShowBalancesForSpecifiedPeriod)), Prepend(nameof(Strings.FromDate)), NoLabel, NoWrap] public DateTime FromDate { get; set; }
        [ProtoMember(3), IfTrue(nameof(ShowBalancesForSpecifiedPeriod)), Prepend(nameof(Strings.ToDate)), NoLabel, NoWrap] public DateType ToDate { get; set; }
        [ProtoMember(4), IfTrue(nameof(ShowBalancesForSpecifiedPeriod)), NoLabel, IfEnum(nameof(ToDate), (int)DateType.Custom)] public DateTime ToDateValue { get; set; }

        [Guide("Tick the `ShowBalancesOnCashBasis` option if you prefer to omit unpaid invoices from your total amounts.")]
        [Guide("If you don't utilize the `SalesInvoices` or `PurchaseInvoices` tabs, selecting this option won't affect the figures displayed in the `Summary` tab because you have no invoices.")]
        [Guide("If you utilize the `SalesInvoices` or `PurchaseInvoices` tabs, the `Summary` screen will automatically adjust your figures through a `CashBasisAdjustment` entry, excluding your unpaid invoices from the totals. However, we advise against using this option, as even unpaid invoices are integral to your financial position and should not be omitted from your financial figures.")]
        [Guide("If you're uncertain about choosing this option, it's best to leave it unchecked. The `AccrualBasis` option accounts for unpaid invoices, ensuring that the `Assets` and `Liabilities` displayed on the `Summary` tab are accurate. The choice to check this option affects only how information is displayed on the `Summary` screen. Regardless of your selection, you can generate reports using either `AccrualBasis` or `CashBasis` under the `Reports` tab for comprehensive analysis.")]
        [ProtoMember(10)] public bool ShowBalancesOnCashBasis { get; set; }

        [Guide("If you wish to display account codes alongside account names, ensure `AccountCodes` is checked.")]
        [Guide("If you aren't utilizing account codes, selecting this option will not affect anything.")]
        [Guide("You can set account codes for individual accounts under your `ChartOfAccounts`.")]
        [ProtoMember(7)] public bool AccountCodes { get; set; }

        [Guide("Select `ExcludeZeroBalances` to conceal accounts that have zero balances. This feature is helpful if you possess numerous accounts with no activity. By enabling this option, your `Summary` screen becomes more streamlined and simpler to navigate.")]
        [ProtoMember(8)] public bool ExcludeZeroBalances { get; set; }

        [Guide("Enable the `GroupsToCollapse` option and then choose specific account groups to display them as ordinary accounts, omitting detailed information.")]
        [Guide("This feature comes in handy when you have numerous accounts, even when the `ExcludeZeroBalances` option is activated. It helps you further declutter and streamline the `Summary` screen by allowing you to collapse selected groups as if they were individual accounts.")]
        [Guide("You can create groups within your `ChartOfAccounts`.")]
        [ProtoMember(11), Label(nameof(Strings.GroupsToCollapse))] public bool HasGroupsToCollapse { get; set; }
        [ProtoMember(12), Autocomplete(typeof(ManagerServer.Model.ChartOfAccountsGroup)), IfTrue(nameof(HasGroupsToCollapse)), NoLabel] public Guid[] GroupsToCollapse { get; set; }

        [ProtoMember(6)] public AccountingBasis Obsolete_AccountingMethod { get; set; }
        [ProtoMember(9)] public bool Obsolete_NewSummary { get; set; }
    }
}
