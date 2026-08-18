using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("fa775461-39a2-46a2-b022-adcad6c9b830")]
    public sealed class ReceiptsAndPaymentsSummary : Object, IHasCustomTheme
    {
        [Guide("Enter a custom title for the report, or leave blank to use the default title.")]
        [ProtoMember(5), Placeholder(nameof(Strings.ReceiptsAndPaymentsSummary))] public string Title { get; set; }
        [Guide("Add an optional description or subtitle that will appear under the report title.")]
        [ProtoMember(1), Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
        [Guide("Add one or more periods to compare receipts and payments across different time frames.")]
        [ProtoMember(2), AddLineLabel(nameof(Strings.AddComparativeColumn))] public Period[] Periods { get; set; }
        [Guide("Enter any footer text to appear at the bottom of the report.")]
        [ProtoMember(3), Textarea] public string Footer { get; set; }
        [Guide("Check to exclude accounts with zero net activity from the report.")]
        [ProtoMember(6)] public bool ExcludeZeroBalances { get; set; }
        [Guide("Check to display account codes alongside account names in the report.")]
        [ProtoMember(4)] public bool AccountCodes { get; set; }

        [ProtoMember(7), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(8), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [ProtoMember(2)] public DateTime FromDate { get; set; }
            [ProtoMember(3)] public DateTime ToDate { get; set; }
            [ProtoMember(4), Short, Placeholder(nameof(Strings.Automatic))] public string ColumnName { get; set; }
        }
    }
}
