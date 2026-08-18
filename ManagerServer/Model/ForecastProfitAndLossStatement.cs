using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("a513a0b4-24f2-49ac-a820-d116ab9d198a")]
    public sealed class ForecastProfitAndLossStatement : Object, IHasCustomTheme
    {
        [Guide("Enter a title for this forecast report. Leave blank to use the default title.")]
        [ProtoMember(1), Placeholder(nameof(Strings.ForecastProfitAndLossStatement))] public string Title { get; set; }
        [Guide("Enter an optional description for this forecast configuration.")]
        [ProtoMember(2), Long, Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
        [Guide("Define the forecast periods. You can add multiple periods for comparison.")]
        [ProtoMember(3), AddLineLabel(nameof(Strings.AddComparativeColumn))] public Period[] Periods { get; set; }
        [Guide("Enter text to appear at the bottom of the forecast report.")]
        [ProtoMember(4), Long, Textarea] public string Footer { get; set; }
        [Guide("Check this box to display account codes alongside account names.")]
        [ProtoMember(5)] public bool AccountCodes { get; set; }
        [Guide("Check this box to hide accounts with zero forecasted balances.")]
        [ProtoMember(6)] public bool ExcludeZeroBalances { get; set; }

        [ProtoMember(7), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(8), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [Guide("Enter the start date for this forecast period.")]
            [ProtoMember(2)] public DateTime FromDate { get; set; }
            [Guide("Enter the end date for this forecast period.")]
            [ProtoMember(3)] public DateTime ToDate { get; set; }
            [Guide("Enter a custom name for this column. If blank, the period dates will be shown.")]
            [ProtoMember(4), Short, Placeholder(nameof(Strings.Optional))] public string ColumnName { get; set; }
        }
    }
}
