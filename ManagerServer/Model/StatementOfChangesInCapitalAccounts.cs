using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("19f661de-d63c-4d25-bd00-3e05578018b1")]
    public sealed class CapitalAccountsSummary : Object, IHasCustomTheme
    {
        [Guide("Enter a custom title for this report. Leave blank to use the default 'Capital Accounts Summary' title.")]
        [ProtoMember(3), Placeholder(nameof(Strings.CapitalAccountsSummary))] public string Title { get; set; }
        [Guide("Enter an optional description to appear under the report title.")]
        [ProtoMember(4)] public string Description { get; set; }
        [Guide("Select the start date for the reporting period.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("Select the end date for the reporting period.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Choose how to round monetary amounts in the report. This helps simplify large numbers.")]
        [ProtoMember(6)] public Rounding Rounding { get; set; }
        [Guide("Check this box to reverse the sign of all amounts. This can help match reporting conventions.")]
        [ProtoMember(7)] public bool ReverseSigns { get; set; }
        [Guide("Check this box to hide accounts with zero balances from the report.")]
        [ProtoMember(8)] public bool ExcludeZeroBalances { get; set; }
        [Guide("Enter optional text to appear at the bottom of the report.")]
        [ProtoMember(5), Textarea] public string Footer { get; set; }

        [ProtoMember(9), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(10), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}