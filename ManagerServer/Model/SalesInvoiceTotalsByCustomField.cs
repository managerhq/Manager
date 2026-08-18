using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("8ea7ac48-0071-4e58-8647-c1f9b17f1dc6")]
    public sealed class SalesInvoiceTotalsByCustomField : Object, IHasCustomTheme
    {
        [Guide("Enter a custom name for this report, or leave blank to use the default title.")]
        [Guide("This report analyzes sales by grouping invoices according to custom field values.")]
        [ProtoMember(2), Placeholder(nameof(Strings.SalesInvoiceTotalsByCustomField))] public string Name { get; set; }
        [Guide("Select the custom field to group sales invoices by. The report will show totals for each unique value in this field.")]
        [Guide("For example, group by 'Product Category' to see sales per category, or by 'Sales Region' to compare regional performance.")]
        [Guide("Only custom fields assigned to sales invoices, invoice lines, customers, or inventory items are available.")]
        [ProtoMember(3), Autocomplete(typeof(CustomField), Filter = typeof(ManagerServer.Model.SalesInvoiceTotalsByCustomField))] public Guid? CustomField { get; set; }
        [Guide("Add one or more periods to compare sales totals across different time frames.")]
        [Guide("Each period creates a column showing totals for that date range, enabling trend analysis.")]
        [Guide("Common setups: monthly comparisons, year-over-year analysis, or custom reporting periods.")]
        [ProtoMember(1), AddLineLabel(nameof(Strings.AddComparativeColumn))] public Period[] Periods { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [ProtoMember(1)] public DateTime FromDate { get; set; }
            [ProtoMember(2)] public DateTime ToDate { get; set; }
            [ProtoMember(3), Short, Placeholder(nameof(Strings.Optional))] public string ColumnName { get; set; }
        }        
    }
}
