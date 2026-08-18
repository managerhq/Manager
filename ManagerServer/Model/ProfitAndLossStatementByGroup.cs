using System;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("41ec3bd5-c95a-49e5-973b-ee26509a6a8c")]
    public sealed class ProfitAndLossStatementByGroup : Object, IComparable<ProfitAndLossStatementByGroup>, IHasCustomTheme
    {
        [Guide("Select the group to report on. The report will list only the accounts in this group (and any nested subgroups).")]
        [ProtoMember(1), Autocomplete(typeof(ProfitAndLossStatementGroup))] public Guid? Group { get; set; }

        [Guide("Configure the report columns:")]
        [Fields(typeof(ProfitAndLossStatement.Period))]
        [Guide("You can also add comparative columns by clicking on `AddComparativeColumn` button.")]
        [ProtoMember(2), AddLineLabel(nameof(Strings.AddComparativeColumn)), Label(nameof(Strings.Columns))] public ProfitAndLossStatement.Period[] Periods { get; set; }

        [Guide("Choose the accounting method � either `AccrualBasis` or `CashBasis`.")]
        [ProtoMember(3)] public AccountingBasis AccountingMethod { get; set; }

        [Guide("Select this option to round figures to whole numbers on the report.")]
        [ProtoMember(4)] public Rounding Rounding { get; set; }

        [Guide("Select which subgroups should be collapsed. Collapsed groups will appear as regular accounts, making the report more concise.")]
        [ProtoMember(5), Autocomplete(typeof(ProfitAndLossStatementGroup))] public Guid[] GroupsToCollapse { get; set; }

        [Guide("Enter text to be displayed at the bottom of the report.")]
        [ProtoMember(6), Textarea, Long] public string Footer { get; set; }

        [Guide("If you use account codes, select this option to display them alongside account names.")]
        [ProtoMember(7)] public bool AccountCodes { get; set; }

        [Guide("Check this option to exclude accounts with a zero balance from the report.")]
        [ProtoMember(8)] public bool ExcludeZeroBalances { get; set; }

        [ProtoMember(9), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(10), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        public int CompareTo(ProfitAndLossStatementByGroup other)
        {
            if (other == null) return 1;
            return (other.Periods?[0]?.FromDate, other.Periods?[0]?.ToDate).CompareTo((this.Periods?[0]?.FromDate, this.Periods?[0]?.ToDate));
        }
    }
}
