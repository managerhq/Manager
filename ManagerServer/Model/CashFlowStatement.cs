using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("2a9a4b8e-8b06-4819-adee-4b766a55119c")]
    public sealed class CashFlowStatement : Object, IHasCustomTheme
    {
        [Guide("Enter an optional description for this report configuration.")]
        [ProtoMember(1)] public string Description { get; set; }
        [Guide("Select the method for preparing the cash flow statement: Direct or Indirect method.")]
        [ProtoMember(5)] public CashFlowStatementMethod Method { get; set; }
        [Guide("Define the time periods to display. You can add multiple periods for comparison.")]
        [ProtoMember(2), AddLineLabel(nameof(Strings.AddComparativeColumn))] public Period[] Periods { get; set; }
        [Guide("Enter text to display at the bottom of the report.")]
        [ProtoMember(3), Textarea] public string Footer { get; set; }
        [Guide("Check this box to hide items with zero balances from the report.")]
        [ProtoMember(4)] public bool ExcludeZeroBalances { get; set; }
        [Guide("Check this box to round amounts to whole numbers on the report.")]
        [ProtoMember(6)] public bool RoundDecimals { get; set; }

        [ProtoMember(7), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(8), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [Guide("Enter the start date for this period.")]
            [ProtoMember(2)] public DateTime FromDate { get; set; }
            [Guide("Enter the end date for this period.")]
            [ProtoMember(3)] public DateTime ToDate { get; set; }
            [Guide("Enter a custom name for this column. If blank, the period dates will be shown.")]
            [ProtoMember(4), Short, Placeholder(nameof(Strings.Optional))] public string ColumnName { get; set; }
        }
    }
}
