using System;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("7b4f463a-470d-44c4-9e75-fafc630b5851")]
    public sealed class BalanceSheet : Object, IComparable<BalanceSheet>, IHasCustomTheme
    {
        [Guide("By default, the report is named `BalanceSheet`, but you can change the title here.")]
        [ProtoMember(10), Placeholder(nameof(Strings.BalanceSheet))] public string Title { get; set; }
        
        [Guide("Enter a description for the report. This helps differentiate between various `BalanceSheet` reports in the list.")]
        [ProtoMember(5), Long, Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
        
        [Guide("Configure the report columns:")]
        [Fields(typeof(Period))]
        [Guide("You can also add comparative columns by clicking on `AddComparativeColumn` button.")]
        [ProtoMember(7), AddLineLabel(nameof(Strings.AddComparativeColumn)), Label(nameof(Strings.Columns))] public Period[] Periods { get; set; }
        
        [Guide("Choose the accounting method � either `AccrualBasis` or `CashBasis`.")]
        [ProtoMember(4)] public AccountingBasis AccountingMethod { get; set; }
        
        [Guide("Select this option to round figures to whole numbers on the report.")]
        [ProtoMember(11)] public Rounding Rounding { get; set; }
        
        [Guide("Choose the layout for the balance sheet report.")]
        [ProtoMember(12)] public BalanceSheetLayout Layout { get; set; }
        
        [Guide("Select which groups should be collapsed. Collapsed groups will appear as regular accounts, making the report more concise.")]
        [ProtoMember(16), Autocomplete(typeof(BalanceSheetAbstractGroup))] public Guid[] GroupsToCollapse { get; set; }
        
        [Guide("Enter text to be displayed at the bottom of the report.")]
        [ProtoMember(6), Textarea, Long] public string Footer { get; set; }
        
        [Guide("If you use account codes, select this option to display them alongside account names.")]
        [ProtoMember(8)] public bool AccountCodes { get; set; }
        
        [Guide("Check this option to exclude accounts with a zero balance from the report.")]
        [ProtoMember(13)] public bool ExcludeZeroBalances { get; set; }

        [ProtoMember(17), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(18), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoMember(9)] internal ManagerServer.Model.Obsolete.Obsolete18.BalanceSheetType18 Obsolete_Type;
        [ProtoMember(15)] public string[] Obsolete_GroupsToCollapse { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [Guide("Specify the date for which the balance sheet figures should be calculated.")]
            [ProtoMember(2)] public DateTime Date { get; set; }
            
            [Guide("If you use `Divisions`, select the appropriate one here to create a divisional balance sheet.")]
            [ProtoMember(4), Autocomplete(typeof(Division)), Short] public Guid? Division { get; set; }
            
            [Guide("Enter a name for the column. If left empty, the system will use the `Date`.")]
            [ProtoMember(3), Short, Placeholder(nameof(Strings.Optional))] public string ColumnName { get; set; }
        }

        [ProtoMember(2)] public DateTime Obsolete_Date { get; set; }

        public int CompareTo(BalanceSheet other)
        {
            if (other == null) return 1;
            return (other.Periods?[0]?.Date, other.Description).CompareTo((this.Periods?[0]?.Date, this.Description));
        }
    }
}
