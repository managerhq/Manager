using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("06d97eb4-27de-41ee-80ef-47b8115b5b36")]
    public sealed class StatementOfChangesInEquity : Object, IHasCustomTheme
    {
        [Guide("Enter a custom title for the report, or leave blank to use the default title.")]
        [ProtoMember(6), Placeholder(nameof(Strings.StatementOfChangesInEquity))] public string Title { get; set; }
        [Guide("Add an optional description or subtitle that will appear under the report title.")]
        [ProtoMember(3), Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
        [Guide("Select whether to use accrual or cash basis accounting for this report.")]
        [ProtoMember(7)] public AccountingBasis AccountingMethod { get; set; }
        [Guide("Select how amounts should be rounded in the report.")]
        [ProtoMember(8)] public Rounding Rounding { get; set; }
        [Guide("Add one or more periods to compare equity changes across different time frames.")]
        [ProtoMember(5), AddLineLabel(nameof(Strings.AddComparativeColumn))] public Period[] Periods { get; set; }
        [Guide("Check to exclude equity accounts with no changes from the report.")]
        [ProtoMember(9)] public bool ExcludeZeroBalances { get; set; }
        [Guide("Enter any footer text to appear at the bottom of the report.")]
        [ProtoMember(4), Textarea] public string Footer { get; set; }

        [ProtoMember(10), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(11), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [ProtoMember(2)] public DateTime FromDate { get; set; }
            [ProtoMember(3)] public DateTime ToDate { get; set; }
            [ProtoMember(4), Short, Placeholder(nameof(Strings.Automatic))] public string ColumnName { get; set; }
        }

        [ProtoMember(1)] public DateTime Obsolete_From { get; set; }
        [ProtoMember(2)] public DateTime Obsolete_To { get; set; }
    }
}
