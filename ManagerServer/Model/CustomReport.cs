using System;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using System.Reflection;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("7df43b64-9aea-4b19-a60a-a56f2e390df4")]
    public sealed class CustomReport : Object, IHasCustomTheme
    {
        [Guide("Enter a descriptive name for this custom report. This name appears in the report list and at the top of the generated report.")]
        [Guide("Use clear names that describe the report's purpose, like 'Monthly expense analysis' or 'Customer payment history'.")]
        [ProtoMember(1), Placeholder(nameof(Strings.CustomReport))] public string Name { get; set; }
        [Guide("Enter an optional description to explain the purpose and content of this report. This helps other users understand what the report shows.")]
        [Guide("Include details about filters applied, data sources used, or specific business questions the report answers.")]
        [ProtoMember(2), Placeholder(nameof(Strings.Optional)), Long] public string Description { get; set; }
        [Guide("Select the starting date for the report period. Only transactions on or after this date will be included in the report.")]
        [Guide("This date filter applies to transaction dates, not creation or modification dates.")]
        [ProtoMember(3), NoWrap] public DateTime FromDate { get; set; }
        [Guide("Select the ending date for the report period. Only transactions on or before this date will be included in the report.")]
        [Guide("Set this to today's date for current reports, or use historical dates for period comparisons.")]
        [ProtoMember(4), NoWrap] public DateTime ToDate { get; set; }
        [Guide("Choose the accounting method for this report:")]
        [Guide("`AccrualBasis` - Recognizes income when earned and expenses when incurred, regardless of payment timing")]
        [Guide("`CashBasis` - Recognizes income when received and expenses when paid, following actual cash flow")]
        [Guide("This should match your business's accounting method for accurate financial reporting.")]
        [ProtoMember(5)] public AccountingBasis AccountingMethod { get; set; }
        [Guide("Select which fields from general ledger transactions to display as columns in your report.")]
        [Guide("You can choose transaction fields (date, amount, description) or related object fields (customer name, account name).")]
        [Guide("The order you select fields determines their column order in the report.")]
        [ProtoMember(6), FirstColumnLabel] public SelectElement[] Select { get; set; }
        [Guide("Check this box to enable filtering. Filters allow you to include only transactions that meet specific criteria.")]
        [Guide("Without filters, the report includes all general ledger transactions within the date range.")]
        [ProtoMember(7)] public bool HasWhere { get; set; }
        [Guide("Define one or more filter conditions. Only transactions matching ALL conditions will appear in the report.")]
        [Guide("Common filters include specific accounts, customers, amounts above thresholds, or transaction types.")]
        [Guide("Each filter consists of a field to check, an operator (equals, contains, greater than), and a comparison value.")]
        [ProtoMember(8), FirstColumnLabel, EmptyLabel, IfTrue(nameof(HasWhere))] public WhereElement[] Where { get; set; }
        [Guide("Check this box to enable custom sorting. This controls the order in which transactions appear in your report.")]
        [Guide("Without sorting, transactions appear in their natural database order.")]
        [ProtoMember(9)] public bool HasOrderBy { get; set; }
        [Guide("Specify sort fields and directions. You can sort by multiple fields to create primary and secondary sort orders.")]
        [Guide("For example, sort first by date (newest first), then by amount (largest first) for transactions on the same date.")]
        [ProtoMember(10), FirstColumnLabel, EmptyLabel, IfTrue(nameof(HasOrderBy))] public OrderByElement[] OrderBy { get; set; }
        [Guide("Check this box to enable grouping. This aggregates transactions by common values and shows subtotals.")]
        [Guide("Grouping transforms detailed transaction lists into summary reports with totals by category.")]
        [ProtoMember(11)] public bool HasGroupBy { get; set; }
        [Guide("Select fields to group by. Transactions with the same values in these fields will be combined.")]
        [Guide("Numeric fields will be totaled, while text fields show the group value. Perfect for summaries by account, customer, or time period.")]
        [Guide("Multiple grouping levels create hierarchical reports with subtotals at each level.")]
        [ProtoMember(12), FirstColumnLabel, EmptyLabel, IfTrue(nameof(HasGroupBy))] public GroupByElement[] GroupBy { get; set; }
        [Guide("Check this box to initially collapse all groups, showing only group headers and totals.")]
        [Guide("Users can expand individual groups to see transaction details. This creates cleaner summary reports while preserving detail access.")]
        [ProtoMember(13), IfTrue(nameof(HasGroupBy))] public bool GroupsToCollapse { get; set; }

        [ProtoMember(14), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(15), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class SelectElement
        {
            [Guide("Select the primary field from general ledger transactions.")]
            [ProtoMember(1), Short, MemberInfoAutocomplete(typeof(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction)), OnChangeSetNull(nameof(SelectSecondaryField)), OnChangeSetNull(nameof(SelectCustomField))] public MemberInfo SelectPrimaryField { get; set; }
            [Guide("If the primary field is an object, select a property from that object.")]
            [ProtoMember(2), Short, MemberInfoAutocomplete(nameof(SelectPrimaryField)), IfNotNull(nameof(SelectPrimaryField)), IfTrue(nameof(SelectPrimaryField), nameof(MemberInfo.IsObject)), NoLabel] public MemberInfo SelectSecondaryField { get; set; }
            [Guide("If selecting custom fields, choose which custom field to display.")]
            [ProtoMember(3), Autocomplete(typeof(ManagerServer.Model.CustomField), Filter = nameof(SelectCustomFieldFilter)), Short, IfTrue(nameof(SelectSecondaryField), nameof(MemberInfo.IsCustomFields)), NoLabel] public Guid? SelectCustomField { get; set; }
            [Guide("Enter an alias name for this column in the report.")]
            [ProtoMember(4), Prepend(nameof(Strings.Alias)), Placeholder(nameof(Strings.Optional)), EmptyLabel] public string DisplayName { get; set; }

            [Expression(IfNullThen, nameof(SelectPrimaryField), nameof(MemberInfo.ObjectKey)), Hidden] public object SelectCustomFieldFilter { get; set; }

            public string GetFullname()
            {
                var primaryName = SelectPrimaryField?.Name;
                var secondaryName = "."+ SelectSecondaryField?.Name;
                if (secondaryName.Length == 1) secondaryName = string.Empty;
                else if (secondaryName == ".CustomFields") secondaryName += "." + SelectCustomField?.ToString();
                return primaryName + secondaryName;
            }
        }

        [ProtoContract]
        public sealed class OrderByElement
        {
            [Guide("Select the primary field to sort by.")]
            [ProtoMember(1), MemberInfoAutocomplete(typeof(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction)), OnChangeSetNull(nameof(OrderBySecondaryField)), OnChangeSetNull(nameof(OrderByCustomField))] public MemberInfo OrderByPrimaryField { get; set; }
            [Guide("If the primary field is an object, select a property from that object.")]
            [ProtoMember(2), MemberInfoAutocomplete(nameof(OrderByPrimaryField)), IfNotNull(nameof(OrderByPrimaryField)), IfTrue(nameof(OrderByPrimaryField), nameof(MemberInfo.IsObject)), NoLabel] public MemberInfo OrderBySecondaryField { get; set; }
            [Guide("If sorting by custom fields, choose which custom field to sort by.")]
            [ProtoMember(3), Autocomplete(typeof(ManagerServer.Model.CustomField), Filter = nameof(OrderByCustomFieldFilter)), Short, IfTrue(nameof(OrderBySecondaryField), nameof(MemberInfo.IsCustomFields)), NoLabel] public Guid? OrderByCustomField { get; set; }
            [Guide("Select ascending or descending sort order.")]
            [ProtoMember(4)] public SortOrder SortOrder { get; set; }

            [Expression(IfNullThen, nameof(OrderByPrimaryField), nameof(MemberInfo.ObjectKey)), Hidden] public object OrderByCustomFieldFilter { get; set; }

            public string GetFullname()
            {
                var primaryName = OrderByPrimaryField?.Name;
                var secondaryName = "." + OrderBySecondaryField?.Name;
                if (secondaryName.Length == 1) secondaryName = string.Empty;
                else if (secondaryName == ".CustomFields") secondaryName += "." + OrderByCustomField?.ToString();
                return primaryName + secondaryName;
            }
        }

        [ProtoContract]
        public sealed class GroupByElement
        {
            [Guide("Select the primary field to group by.")]
            [ProtoMember(1), MemberInfoAutocomplete(typeof(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction)), OnChangeSetNull(nameof(GroupBySecondaryField)), OnChangeSetNull(nameof(GroupByCustomField))] public MemberInfo GroupByPrimaryField { get; set; }
            [Guide("If the primary field is an object, select a property from that object.")]
            [ProtoMember(2), MemberInfoAutocomplete(nameof(GroupByPrimaryField)), IfNotNull(nameof(GroupByPrimaryField)), IfTrue(nameof(GroupByPrimaryField), nameof(MemberInfo.IsObject)), NoLabel] public MemberInfo GroupBySecondaryField { get; set; }
            [Guide("If grouping by custom fields, choose which custom field to group by.")]
            [ProtoMember(3), Autocomplete(typeof(ManagerServer.Model.CustomField), Filter = nameof(GroupByCustomFieldFilter)), Short, IfTrue(nameof(GroupBySecondaryField), nameof(MemberInfo.IsCustomFields)), NoLabel] public Guid? GroupByCustomField { get; set; }

            [Expression(IfNullThen, nameof(GroupByPrimaryField), nameof(MemberInfo.ObjectKey)), Hidden] public object GroupByCustomFieldFilter { get; set; }

            public string GetFullname()
            {
                var primaryName = GroupByPrimaryField?.Name;
                var secondaryName = "." + GroupBySecondaryField?.Name;
                if (secondaryName.Length == 1) secondaryName = string.Empty;
                else if (secondaryName == ".CustomFields") secondaryName += "." + GroupByCustomField?.ToString();
                return primaryName + secondaryName;
            }
        }

        [ProtoContract]
        public sealed class WhereElement
        {
            [Guide("Select the primary field to filter on.")]
            [ProtoMember(1), Short, MemberInfoAutocomplete(typeof(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction)), OnChangeSetNull(nameof(WhereSecondaryField)), OnChangeSetNull(nameof(WhereCustomField)), OnChangeSetNull(nameof(Object))] public MemberInfo WherePrimaryField { get; set; }
            [Guide("If the primary field is an object, select a property from that object.")]
            [ProtoMember(2), Short, MemberInfoAutocomplete(nameof(WherePrimaryField)), IfNotNull(nameof(WherePrimaryField)), IfTrue(nameof(WherePrimaryField), nameof(MemberInfo.IsObject)), NoLabel] public MemberInfo WhereSecondaryField { get; set; }
            [Guide("If filtering by custom fields, choose which custom field to filter on.")]
            [ProtoMember(3), Autocomplete(typeof(ManagerServer.Model.CustomField), Filter = nameof(WhereCustomFieldFilter)), Short, IfTrue(nameof(WhereSecondaryField), nameof(MemberInfo.IsCustomFields)), NoLabel] public Guid? WhereCustomField { get; set; }

            [Expression(IfNullThen, nameof(WherePrimaryField), nameof(MemberInfo.ObjectKey)), Hidden] public object WhereCustomFieldFilter { get; set; }
            [Expression(IfNullThen, nameof(WhereSecondaryField), nameof(MemberInfo.ValueType), IfNullThen, nameof(WherePrimaryField), nameof(MemberInfo.ValueType)), Hidden] public object ObjectType { get; set; }
            [Expression(IfNullThen, nameof(WhereSecondaryField), nameof(MemberInfo.ObjectKey), IfNullThen, nameof(WherePrimaryField), nameof(MemberInfo.ObjectKey)), Hidden] public object ObjectKey { get; set; }

            [Guide("Select the operator for text comparisons.")]
            [ProtoMember(4), NoLabel, IfEnum(nameof(ObjectType), (int)MemberInfo.FieldType.String)] public StringOperator StringOperator { get; set; }
            [Guide("Enter the text value to compare against.")]
            [ProtoMember(5), NoLabel, IfEnum(nameof(StringOperator), (int)StringOperator.Contains, (int)StringOperator.DoesNotContain), Short] public string String { get; set; }
            [Guide("Select the operator for decimal comparisons.")]
            [ProtoMember(6), NoLabel, IfEnum(nameof(ObjectType), (int)MemberInfo.FieldType.Decimal)] public DecimalOperator DecimalOperator { get; set; }
            [Guide("Enter the decimal value to compare against.")]
            [ProtoMember(7), NoLabel, IfEnum(nameof(DecimalOperator), (int)DecimalOperator.IsLessThan, (int)DecimalOperator.IsMoreThan)] public decimal? Decimal { get; set; }
            [Guide("Select the operator for yes/no comparisons.")]
            [ProtoMember(8), NoLabel, IfEnum(nameof(ObjectType), (int)MemberInfo.FieldType.Boolean)] public BooleanOperator BooleanOperator { get; set; }
            [Guide("Select the operator for object comparisons.")]
            [ProtoMember(9), NoLabel, IfEnum(nameof(ObjectType), (int)MemberInfo.FieldType.Object)] public ObjectOperator ObjectOperator { get; set; }
            [Guide("Select the specific object to compare against.")]
            [ProtoMember(10), NoLabel, IfEnum(nameof(ObjectOperator), (int)ObjectOperator.Is, (int)ObjectOperator.IsNot), Autocomplete(null, Filter = nameof(ObjectKey)), Short] public Guid? Object { get; set; }
            [Guide("Select the operator for date comparisons.")]
            [ProtoMember(11), NoLabel, IfEnum(nameof(ObjectType), (int)MemberInfo.FieldType.Date)] public DateOperator DateOperator { get; set; }
            [Guide("Enter the start date for the date range.")]
            [ProtoMember(12), NoLabel, IfEnum(nameof(DateOperator), (int)DateOperator.IsBetween)] public DateTime StartDate { get; set; }
            [Guide("Enter the end date for the date range.")]
            [ProtoMember(13), NoLabel, IfEnum(nameof(DateOperator), (int)DateOperator.IsBetween)] public DateTime EndDate { get; set; }

            public string GetFullname()
            {
                var primaryName = WherePrimaryField?.Name;
                var secondaryName = "." + WhereSecondaryField?.Name;
                if (secondaryName.Length == 1) secondaryName = string.Empty;
                else if (secondaryName == ".CustomFields") secondaryName += "." + WhereCustomField?.ToString();
                return primaryName + secondaryName;
            }
        }

        public enum StringOperator : int
        {
            Contains = 1,
            DoesNotContain = 2,
            IsEmpty = 3,
            IsNotEmpty = 4
        }

        public enum DecimalOperator : int
        {
            IsLessThan = 1,
            IsMoreThan = 2,
            IsNotZero = 3,
            IsZero = 4
        }

        public enum BooleanOperator : int
        {
            IsChecked = 1,
            IsNotChecked = 2
        }

        public enum DateOperator : int
        {
            IsBetween = 1
        }

        public enum ObjectOperator : int
        {
            Is = 1,
            IsNot = 2,
            IsEmpty = 3,
            IsNotEmpty = 4
        }

        /*
        public sealed class DataItem
        {
            public Manager.Query.GeneralLedger.GeneralLedgerTransaction GeneralLedgerTransactions { get; set; }
            public Manager.Model.Customer Customers { get; set; }
            public Manager.Model.Supplier Suppliers { get; set; }
            public Manager.Model.AmortizationEntry AmortizationEntries { get; set; }
            public Manager.Model.BankAccount BankAccounts { get; set; }
            public Manager.Model.BankReconciliation BankReconciliations { get; set; }
            public Manager.Model.BillableTime BillableTime { get; set; }
            public Manager.Model.CapitalAccount CapitalAccounts { get; set; }
            public Manager.Model.CashAccount CashAccounts { get; set; }
            public Manager.Model.CreditNote CreditNotes { get; set; }
            public Manager.Model.DebitNote DebitNotes { get; set; }
            public Manager.Model.DeliveryNote DeliveryNotes { get; set; }
            public Manager.Model.DepreciationEntry DepreciationEntries { get; set; }
            public Manager.Model.Employee Employees { get; set; }
            public Manager.Model.ExpenseClaim ExpenseClaims { get; set; }
            public Manager.Model.FixedAsset FixedAssets { get; set; }
            public Manager.Model.GoodsReceipt GoodsReceipts { get; set; }
            public Manager.Model.IntangibleAsset IntangibleAssets { get; set; }
            public Manager.Model.InterAccountTransfer InterAccountTransfers { get; set; }
            public Manager.Model.InventoryItem InventoryItems { get; set; }
            public Manager.Model.InventoryTransfer InventoryTransfers { get; set; }
            public Manager.Model.InventoryWriteOff InventoryWriteOffs { get; set; }
            public Manager.Model.JournalEntry JournalEntries { get; set; }
            public Manager.Model.LatePaymentFee LatePaymentFees { get; set; }
            public Manager.Model.Payslip Payslips { get; set; }
            public Manager.Model.ProductionOrder ProductionOrders { get; set; }
            public Manager.Model.PurchaseInvoice PurchaseInvoices { get; set; }
            public Manager.Model.PurchaseOrder PurchaseOrders { get; set; }
            public Manager.Model.PurchaseQuote PurchaseQuotes { get; set; }
            public Manager.Model.Receipt Receipts { get; set; }
            public Manager.Model.Payment Payments { get; set; }
            public Manager.Model.SalesInvoice SalesInvoices { get; set; }
            public Manager.Model.SalesOrder SalesOrders { get; set; }
            public Manager.Model.SalesQuote SalesQuotes { get; set; }
            public Manager.Model.SpecialAccount SpecialAccounts { get; set; }
            public Manager.Model.ExchangeRate ExchangeRates { get; set; }
            public Manager.Model.ExpenseClaimsPayer ExpenseClaimPayers { get; set; }
            public Manager.Model.ForeignCurrency ForeignCurrencies { get; set; }
            public Manager.Model.InventoryKit InventoryKits { get; set; }
            public Manager.Model.InventoryLocation InventoryLocations { get; set; }
            public Manager.Model.NonInventoryItem NonInventoryItems { get; set; }
            public Manager.Model.TaxCode TaxCodes { get; set; }
            public Manager.Model.Division Divisions { get; set; }
        }
        */
    }
}