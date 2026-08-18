using System;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("25c7aa0e-536c-42be-a75c-1b5ac71e955a")]
    public sealed class ProfitAndLossStatementActualVsBudget : Object, IComparable<ProfitAndLossStatementActualVsBudget>, IHasCustomTheme
    {
        [Guide("The title that will appear at the top of the report. Leave blank to use the default title.")]
        [ProtoMember(5), Placeholder(nameof(Strings.ProfitAndLossStatementActualVsBudget))] public string Title { get; set; }
        [Guide("The starting date for the comparison period. Both actual and budget figures will be calculated from this date.")]
        [ProtoMember(1), NoWrap] public DateTime FromDate { get; set; }
        [Guide("The ending date for the comparison period. Both actual and budget figures will be calculated up to this date.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Choose whether to use accrual basis or cash basis accounting for calculating actual amounts. This should match your regular accounting method.")]
        [ProtoMember(9)] public AccountingBasis AccountingMethod { get; set; }
        [Guide("Optional division to filter the report. If selected, only transactions and budgets for this division will be included.")]
        [ProtoMember(4), Autocomplete(typeof(Division))] public Guid? Division { get; set; }
        [Guide("Enter budget amounts for each account. The report will compare these budgeted amounts against actual results.")]
        [ProtoMember(3)] public BudgetItem[] Lines { get; set; }
        [Guide("Optional footer text that appears at the bottom of the report. Use this for notes about budget assumptions or variance explanations.")]
        [ProtoMember(7), Textarea, Long] public string Footer { get; set; }
        [Guide("Check this box to hide accounts where both actual and budget amounts are zero, making the report more concise.")]
        [ProtoMember(6)] public bool ExcludeZeroBalances { get; set; }
        [Guide("Check this box to round amounts to whole numbers, removing decimal places for a cleaner presentation.")]
        [ProtoMember(8)] public bool RoundDecimals { get; set; }

        [ProtoMember(10), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(11), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class BudgetItem
        {
            [Guide("Select the revenue or expense account to budget for.")]
            [ProtoMember(1), Autocomplete(typeof(ManagerServer.Model.IProfitAndLossAccount))] public Guid? Account { get; set; }
            [Guide("Enter the budgeted amount for this account. Use positive numbers for revenue and negative numbers for expenses.")]
            [ProtoMember(2), Sum] public decimal Amount { get; set; }
        }

        int IComparable<ProfitAndLossStatementActualVsBudget>.CompareTo(ProfitAndLossStatementActualVsBudget other)
        {
            if (other == null) return 1;
            return (other.FromDate, other.ToDate).CompareTo((this.FromDate, this.ToDate));
        }
    }
}
