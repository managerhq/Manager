using System;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("165c0392-9aad-4067-b855-a2393ead5df4")]
    public sealed class ProfitAndLossStatement : Object, IComparable<ProfitAndLossStatement>, IHasCustomTheme
    {
        [Guide("The title that will appear at the top of the report. Leave blank to use the default title.")]
        [ProtoMember(11), Placeholder(nameof(Strings.ProfitAndLossStatement))] public string Title { get; set; }
        [Guide("An optional description or subtitle that appears below the title. This can be used to provide additional context about the report.")]
        [ProtoMember(5), Long, Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
        [Guide("Define the time periods to include in the report. You can add multiple periods to create comparative columns showing different date ranges side by side.")]
        [ProtoMember(8), AddLineLabel(nameof(Strings.AddComparativeColumn))] public Period[] Periods { get; set; }
        [Guide("Choose whether to use accrual basis (recognizes revenue when earned and expenses when incurred) or cash basis (recognizes transactions only when cash changes hands) accounting.")]
        [ProtoMember(4)] public AccountingBasis AccountingMethod { get; set; }
        [Guide("Select how numbers should be rounded in the report. Options include showing full amounts, rounding to thousands, or rounding to millions.")]
        [ProtoMember(12)] public Rounding Rounding { get; set; }
        [Guide("Select account groups that should be collapsed (show only the group total) in the report. This helps create a more concise report by hiding individual account details for selected groups.")]
        [ProtoMember(15), Autocomplete(typeof(ProfitAndLossStatementGroup))] public Guid[] GroupsToCollapse { get; set; }
        [Guide("Optional footer text that appears at the bottom of the report. This can be used for notes, disclaimers, or additional information.")]
        [ProtoMember(7), Long, Textarea] public string Footer { get; set; }
        [Guide("Check this box to display account codes alongside account names in the report.")]
        [ProtoMember(9)] public bool AccountCodes { get; set; }
        [Guide("Check this box to hide accounts with zero balances from the report, making it more concise.")]
        [ProtoMember(13)] public bool ExcludeZeroBalances { get; set; }
        [ProtoMember(16), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(17), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [Guide("The starting date for this period.")]
            [ProtoMember(2)] public DateTime FromDate { get; set; }
            [Guide("The ending date for this period.")]
            [ProtoMember(3)] public DateTime ToDate { get; set; }
            [Guide("Optional division to filter the report. If selected, only transactions for this division will be included.")]
            [ProtoMember(4), Short, Autocomplete(typeof(Division))] public Guid? Division { get; set; }
            [Guide("Optional custom name for this column. If left blank, the date range will be used as the column header.")]
            [ProtoMember(5), Short, Placeholder(nameof(Strings.Optional))] public string ColumnName { get; set; }
        }

        [ProtoMember(2)] public DateTime Obsolete_From { get; set; }
        [ProtoMember(3)] public DateTime? Obsolete_To { get; set; }
        [ProtoMember(6)] public Guid? Obsolete_TrackingCode { get; set; }
        [ProtoMember(10)] internal ManagerServer.Model.Obsolete.Obsolete18.IncomeStatementType18 Obsolete_Type;
        [ProtoMember(14)] public string[] Obsolete_GroupsToCollapse { get; set; }

        int IComparable<ProfitAndLossStatement>.CompareTo(ProfitAndLossStatement other)
        {
            if (other == null) return 1;
            return (other.Periods?[0]?.FromDate, other.Periods?[0]?.ToDate, other.Description).CompareTo((this.Periods?[0]?.FromDate, this.Periods?[0]?.ToDate, this.Description));
        }
    }
}
