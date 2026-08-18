using System;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Attributes;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("c6b97908-627b-453b-9b62-3e6308a96ace")]
    public sealed class AdvancedQuery : NamedObject
    {
        [Guide("Select the base table or report to query data from. This determines which data source your advanced query will analyze.")]
        [Guide("Each table or report has different columns available for selection, filtering, and sorting.")]
        [ProtoMember(1)] public Guid NakedTable { get; set; }
        [Guide("Enter a descriptive name for this advanced query. This name appears in the list of saved queries and helps you identify its purpose.")]
        [Guide("Use clear names like 'High-value customers' or 'Overdue invoices by region' for easy identification.")]
        [ProtoMember(2)] public string Name { get; set; }
        [Guide("Select which columns to display in your query results. These are the data fields that will appear in your custom report.")]
        [Guide("You can select multiple columns such as customer names, amounts, dates, or any other fields available in the source table.")]
        [ProtoMember(3)] public SelectLine[] Select { get; set; }
        [Guide("Check this box to enable filtering. Filters allow you to show only records that meet specific criteria.")]
        [Guide("For example, filter to show only invoices over $1,000 or customers from a specific region.")]
        [ProtoMember(7)] public bool HasWhere { get; set; }
        [Guide("Define filter conditions to limit which records appear in the results. Each filter consists of a column, operator, and value.")]
        [Guide("You can add multiple filters to create complex criteria, such as 'Amount greater than 1000 AND Status equals Unpaid'.")]
        [ProtoMember(4)] public WhereLine[] Where { get; set; }
        [Guide("Check this box to enable sorting. This controls the order in which records appear in your results.")]
        [Guide("You can sort by any column in ascending or descending order, such as newest dates first or highest amounts at the top.")]
        [ProtoMember(8)] public bool HasOrderBy { get; set; }
        [Guide("Specify how to sort the query results. You can sort by multiple columns to create a primary and secondary sort order.")]
        [Guide("For example, sort first by customer name alphabetically, then by invoice date within each customer.")]
        [ProtoMember(5)] public OrderByLine[] OrderBy { get; set; }
        [Guide("Check this box to enable grouping. Grouping aggregates data by specific columns to show totals, counts, or summaries.")]
        [Guide("This is useful for reports like 'Total sales by customer' or 'Invoice count by month'.")]
        [ProtoMember(9)] public bool HasGroupBy { get; set; }
        [Guide("Define which columns to group by. When grouping is enabled, the query will consolidate rows with the same values in these columns.")]
        [Guide("Numeric columns will be summed, while other columns will show the grouped value or count of records.")]
        [ProtoMember(10)] public GroupByLine[] GroupBy { get; set; }

        [ProtoMember(6)] public Guid? Obsolete_GroupBy { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.Unnamed;
            return Name;
        }

        [ProtoContract]
        public sealed class SelectLine
        {
            [Guide("Select a column to include in the query results. This dropdown shows all available columns from the selected table or report.")]
            [Guide("Each column you add here will appear as a separate column in your query results.")]
            [ProtoMember(1)] public Guid? Key { get; set; }
        }

        [ProtoContract]
        public sealed class WhereLine
        {
            [Guide("Select the column you want to filter on. This determines which field's values will be evaluated by your filter condition.")]
            [Guide("The available operators and value fields will adjust based on the data type of the selected column.")]
            [ProtoMember(1)] public Guid? Key { get; set; }
            [Guide("Choose the operator for text comparisons. Options include 'Contains', 'Does not contain', 'Is empty', and 'Is not empty'.")]
            [Guide("Text filters are case-insensitive and work with partial matches when using 'Contains'.")]
            [ProtoMember(2)] public StringOperator StringFilter { get; set; }
            [Guide("Choose the operator for decimal number comparisons. Options include 'Is less than', 'Is more than', 'Is zero', and 'Is not zero'.")]
            [Guide("Use this for filtering amounts, percentages, or any values with decimal places.")]
            [ProtoMember(3)] public DecimalOperator DecimalFilter { get; set; }
            [Guide("Choose the operator for whole number comparisons. Options include 'Is less than', 'Is more than', 'Is zero', and 'Is not zero'.")]
            [Guide("Use this for filtering quantities, counts, or any values without decimal places.")]
            [ProtoMember(4)] public NumberOperator NumberFilter { get; set; }
            [Guide("Choose the operator for yes/no (boolean) comparisons. Options are 'Is checked' and 'Is not checked'.")]
            [Guide("Use this for filtering checkboxes or true/false fields.")]
            [ProtoMember(5)] public BooleanOperator BooleanFilter { get; set; }
            [Guide("Choose the operator for date comparisons. Options include 'Is exactly', 'Is after', 'Is on or after', 'Is before', and 'Is before or on'.")]
            [Guide("Date filters help you find records within specific time periods or relative to certain dates.")]
            [ProtoMember(6)] public DateOperator DateFilter { get; set; }
            [Guide("Choose the operator for dropdown list comparisons. Options are 'Is' and 'Is not'.")]
            [Guide("Use this to filter based on specific selections from predefined lists like status, type, or category fields.")]
            [ProtoMember(12)] public EnumOperator EnumFilter { get; set; }

            [Guide("Enter the text value to compare against when using a text filter. This is the value that will be searched for in the selected column.")]
            [Guide("For 'Contains' filters, entering 'ABC' will match 'ABC Company', 'ABC123', or any text containing 'ABC'.")]
            [ProtoMember(7)] public string Text { get; set; }
            [Guide("Select the date to compare against when using a date filter. Use the date picker to choose the specific date for comparison.")]
            [Guide("This works with operators like 'Is after' or 'Is before' to find records in relation to this date.")]
            [ProtoMember(8)] public DateTime? Date { get; set; }
            [Guide("Enter the decimal number to compare against when using a decimal filter. Include decimal places as needed.")]
            [Guide("For example, enter '1000.50' to find all amounts greater than or less than this value.")]
            [ProtoMember(9)] public decimal? Decimal { get; set; }
            [Guide("Enter the whole number to compare against when using a number filter. This must be a whole number without decimals.")]
            [Guide("For example, enter '10' to find all quantities greater than or less than 10.")]
            [ProtoMember(10)] public int? Number { get; set; }
            [Guide("Select the dropdown list value to compare against when using an enum filter. Choose from the available options in the list.")]
            [Guide("This allows you to filter for specific predefined values like 'Active', 'Pending', or other status options.")]
            [ProtoMember(11)] public int? EnumValue { get; set; }

            public bool IsMatch(object value)
            {
                var value2 = value;

                if (value2 is string s)
                {
                    if (StringFilter == StringOperator.Contains && s.Contains(Text ?? string.Empty, StringComparison.OrdinalIgnoreCase)) return true;
                    else if (StringFilter == StringOperator.DoesNotContain && !s.Contains(Text ?? string.Empty, StringComparison.OrdinalIgnoreCase)) return true;
                    else if (StringFilter == StringOperator.IsEmpty && string.IsNullOrWhiteSpace(s)) return true;
                    else if (StringFilter == StringOperator.IsNotEmpty && !string.IsNullOrWhiteSpace(s)) return true;
                }
                else if (value2 is string[] stringArray)
                {
                    if (StringFilter == StringOperator.Contains && stringArray.Contains(Text)) return true;
                    else if (StringFilter == StringOperator.DoesNotContain && !stringArray.Contains(Text)) return true;
                    else if (StringFilter == StringOperator.IsEmpty && stringArray.All(x => string.IsNullOrWhiteSpace(x))) return true;
                    else if (StringFilter == StringOperator.IsNotEmpty && stringArray.Any(x => !string.IsNullOrWhiteSpace(x))) return true;
                }
                else if (value2 is DateTime date)
                {
                    if (!Date.HasValue) return true;
                    if (DateFilter == DateOperator.IsExactly && date == Date.Value) return true;
                    if (DateFilter == DateOperator.IsAfter && date > Date.Value) return true;
                    if (DateFilter == DateOperator.IsOnOrAfter && date >= Date.Value) return true;
                    if (DateFilter == DateOperator.IsBefore && date < Date.Value) return true;
                    if (DateFilter == DateOperator.IsBeforeOrOn && date <= Date.Value) return true;
                }
                else if (value2 is decimal d)
                {
                    if (DecimalFilter == DecimalOperator.IsZero)
                    {
                        if (d == 0m) return true;
                        else return false;
                    }
                    if (DecimalFilter == DecimalOperator.IsNotZero)
                    {
                        if (d != 0m) return true;
                        else return false;
                    }
                    if (!Decimal.HasValue) return true;
                    if (DecimalFilter == DecimalOperator.IsLessThan && d < Decimal.Value) return true;
                    if (DecimalFilter == DecimalOperator.IsMoreThan && d > Decimal.Value) return true;
                }
                else if (value2 is int i)
                {
                    if (NumberFilter == NumberOperator.IsZero)
                    {
                        if (i == 0m) return true;
                        else return false;
                    }
                    if (NumberFilter == NumberOperator.IsNotZero)
                    {
                        if (i != 0m) return true;
                        else return false;
                    }
                    if (!Number.HasValue) return true;
                    if (NumberFilter == NumberOperator.IsLessThan && i < Number.Value) return true;
                    if (NumberFilter == NumberOperator.IsMoreThan && i > Number.Value) return true;
                }
                else if (value2 is bool b)
                {
                    if (BooleanFilter == BooleanOperator.IsChecked && b) return true;
                    if (BooleanFilter == BooleanOperator.IsNotChecked && !b) return true;
                }
                else if (value2 is Enum)
                {
                    if (!EnumValue.HasValue) return true;
                    if (EnumFilter == EnumOperator.Is && (int)value2 == EnumValue.Value) return true;
                    if (EnumFilter == EnumOperator.IsNot && (int)value2 != EnumValue.Value) return true;
                }
                return false;
            }
        }        

        [ProtoContract]
        public sealed class OrderByLine
        {
            [Guide("Select the column to sort by. This determines which field's values will be used to order the query results.")]
            [Guide("You can sort by any column that appears in your query, including dates, amounts, names, or reference numbers.")]
            [ProtoMember(1)] public Guid? Key { get; set; }
            [Guide("Choose the sort direction. 'Ascending' sorts from lowest to highest (A-Z, oldest to newest, smallest to largest).")]
            [Guide("'Descending' sorts from highest to lowest (Z-A, newest to oldest, largest to smallest).")]
            [ProtoMember(2)] public SortOrder SortOrder { get; set; }
        }

        [ProtoContract]
        public sealed class GroupByLine
        {
            [Guide("Select the column to group by. Records with the same value in this column will be combined into a single row.")]
            [Guide("When grouping, numeric columns will show totals, while text columns will show the grouped value. This is perfect for summary reports.")]
            [ProtoMember(1)] public Guid? Key { get; set; }
        }

        public enum StringOperator : int
        {
            Contains = 0,
            DoesNotContain = 1,
            IsEmpty = 2,
            IsNotEmpty = 3
        }

        public enum DecimalOperator : int
        {
            IsLessThan = 0,
            IsMoreThan = 1,
            IsNotZero = 2,
            IsZero = 3
        }

        public enum NumberOperator : int
        {
            IsLessThan = 0,
            IsMoreThan = 1,
            IsNotZero = 2,
            IsZero = 3
        }

        public enum BooleanOperator : int
        {
            IsChecked = 0,
            IsNotChecked = 1
        }

        public enum DateOperator : int
        {
            IsExactly = 0,
            IsAfter = 1,
            IsOnOrAfter = 2,
            IsBefore = 3,
            IsBeforeOrOn = 4,
        }

        public enum EnumOperator : int
        {
            Is = 0,
            IsNot = 1
        }
    }
}