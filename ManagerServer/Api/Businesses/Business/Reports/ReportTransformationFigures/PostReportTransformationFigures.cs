using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Helpers;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.Api.Businesses.Business.Reports.ReportTransformationFigures
{
    internal sealed class PostReportTransformationFigures : AuthorizedEndpoint<PostReportTransformationFigures.Response>
    {
        private static readonly Guid HideIfEmptyMarker       = new("c9d0844e-7628-4bf4-899c-155be1577983");
        private static readonly Guid SetZeroIfNegativeMarker = new("1a94b65c-4869-4138-acc1-49d16bbfeed6");
        private static readonly Guid ReverseSignMarker       = new("0b3fe333-755b-42c0-b921-2835e39e50f0");
        private static readonly Guid TaxSalesMarker          = new("211bb6c2-9ca7-4cda-a099-99bcde19a173");
        private static readonly Guid TaxPurchasesMarker      = new("89c4e9b6-f555-4243-8432-680a1cc97a61");
        private static readonly Guid FromDateLastJulyMarker  = new("7d3ddc8b-49f1-4064-997a-430367e54055");
        private static readonly Guid EmployeeNameMarker      = new("db71c44c-ec5a-4701-aa54-67ada72aff1a");
        private static readonly Guid EmployeeEmailMarker     = new("f66ab672-c1c6-4280-9439-bdb0a72b7619");
        private static readonly Guid SupplierNameMarker      = new("22ec22e1-8ed2-4cba-a5b9-533a1e451977");
        private static readonly Guid BusinessNameMarker      = new("ce6302f8-0b02-42d8-b6b7-850063e4bbe0");
        private static readonly Guid FromDateMarker          = new("cef33379-d1b3-4172-b090-0fc24cf978da");
        private static readonly Guid ToDateMarker            = new("8ba7e5e7-8f74-443a-b7ee-d8539b12e7e2");

        // Bound from request JSON body.
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public AccountingBasis AccountingMethod { get; set; }
        public Guid? Employee { get; set; }
        public int Columns { get; set; }
        public Item[] Items { get; set; }

        // Optional grouping. ForEachSupplier/ForEachEmployee are computed per supplier/employee that has
        // any transaction in [FromDate, ToDate]. SupplierCustomField + Value narrows suppliers to those
        // whose classic custom field has the given value (used e.g. by TPAR for "reportable" suppliers).
        public Item[] ForEachSupplier { get; set; }
        public Guid? SupplierCustomField { get; set; }
        public string SupplierCustomFieldValue { get; set; }
        public Item[] ForEachEmployee { get; set; }

        public override Response AuthorizedHandle()
        {
            var database = GetApplicationData().Businesses.Get(Business);
            var businessDetails = database.Single<BusinessDetails>();

            GeneralLedgerTransaction[] transactions;
            if (AccountingMethod == AccountingBasis.CashBasis)
            {
                transactions = new GeneralLedger(Business)
                    .AutomaticallyMatchSalesInvoices()
                    .ConvertSalesInvoicesToCashBasis2(FromDate.AddDays(-1), ToDate)
                    .AutomaticallyMatchPurchaseInvoices()
                    .ConvertPurchaseInvoicesToCashBasis2(FromDate.AddDays(-1), ToDate)
                    .ToArray();
            }
            else
            {
                transactions = new GeneralLedger(Business).ToArray();
            }

            Model.Employee employee = null;
            if (Employee.HasValue)
            {
                employee = database.SingleOrDefault<Model.Employee>(Employee.Value);
                transactions = transactions.Where(x => x.Employee != null && x.Employee.Key == Employee.Value).ToArray();
            }

            var rows = new List<Row>();
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    rows.Add(BuildRow(item, transactions, businessDetails, employee, supplier: null, database));
                }
            }

            var supplierGroups = BuildSupplierGroups(transactions, businessDetails, database);
            var employeeGroups = BuildEmployeeGroups(transactions, businessDetails, database);

            return new Response
            {
                Rows = rows.ToArray(),
                SupplierGroups = supplierGroups,
                EmployeeGroups = employeeGroups,
                Decimals = database.Single<BaseCurrency>().GetDecimalPlaces(),
            };
        }

        private Group[] BuildSupplierGroups(GeneralLedgerTransaction[] transactions, BusinessDetails businessDetails, Database database)
        {
            if (ForEachSupplier == null || ForEachSupplier.Length == 0) return null;

            var groups = new List<Group>();
            foreach (var grp in transactions.Where(x => x.Supplier != null).GroupBy(x => x.Supplier).OrderBy(g => g.Key.Name))
            {
                if (SupplierCustomField.HasValue)
                {
                    if (grp.Key.CustomFields == null) continue;
                    if (!grp.Key.CustomFields.TryGetValue(SupplierCustomField.Value, out var v)) continue;
                    if (v != SupplierCustomFieldValue) continue;
                }
                var supplierTransactions = grp.ToArray();
                if (!supplierTransactions.Any(x => x.Date >= FromDate && x.Date <= ToDate)) continue;

                var rows = new List<Row>();
                foreach (var item in ForEachSupplier)
                {
                    rows.Add(BuildRow(item, supplierTransactions, businessDetails, employee: null, supplier: grp.Key, database));
                }
                groups.Add(new Group { Key = grp.Key.Key, Name = grp.Key.NameWithCode, Rows = rows.ToArray() });
            }
            return groups.ToArray();
        }

        private Group[] BuildEmployeeGroups(GeneralLedgerTransaction[] transactions, BusinessDetails businessDetails, Database database)
        {
            if (ForEachEmployee == null || ForEachEmployee.Length == 0) return null;

            var groups = new List<Group>();
            foreach (var grp in transactions.Where(x => x.Employee != null).GroupBy(x => x.Employee).OrderBy(g => g.Key.Name))
            {
                var employeeTransactions = grp.ToArray();
                if (!employeeTransactions.Any(x => x.Date >= FromDate && x.Date <= ToDate)) continue;

                var rows = new List<Row>();
                foreach (var item in ForEachEmployee)
                {
                    rows.Add(BuildRow(item, employeeTransactions, businessDetails, employee: grp.Key, supplier: null, database));
                }
                groups.Add(new Group { Key = grp.Key.Key, Name = grp.Key.NameWithCode, Rows = rows.ToArray() });
            }
            return groups.ToArray();
        }

        private Row BuildRow(Item item, GeneralLedgerTransaction[] transactions, BusinessDetails businessDetails, Model.Employee employee, Model.Supplier supplier, Database database)
        {
            var row = new Row { Name = item.Name };

            if (!string.IsNullOrWhiteSpace(item.Name))
            {
                var trimmed = item.Name.Trim();
                var lines = trimmed.Split('\n');
                if (lines.Length > 0 && lines[lines.Length - 1].All(c => c == '-'))
                {
                    row.Name = string.Join('\n', lines.Take(lines.Length - 1));
                    row.IsHeader = true;
                    return row;
                }
            }

            var cells = new List<Cell>(Columns);
            if (Columns >= 1) cells.Add(BuildCell(item.Column1, transactions, businessDetails, employee, supplier, database));
            if (Columns >= 2) cells.Add(BuildCell(item.Column2, transactions, businessDetails, employee, supplier, database));
            if (Columns >= 3) cells.Add(BuildCell(item.Column3, transactions, businessDetails, employee, supplier, database));
            if (Columns >= 4) cells.Add(BuildCell(item.Column4, transactions, businessDetails, employee, supplier, database));
            if (Columns >= 5) cells.Add(BuildCell(item.Column5, transactions, businessDetails, employee, supplier, database));
            row.Cells = cells.ToArray();
            return row;
        }

        private Cell BuildCell(Guid[] figures, GeneralLedgerTransaction[] transactions, BusinessDetails businessDetails, Model.Employee employee, Model.Supplier supplier, Database database)
        {
            if (figures == null || figures.Length == 0) return new Cell();

            var hideIfEmpty       = figures.Contains(HideIfEmptyMarker);
            var setZeroIfNegative = figures.Contains(SetZeroIfNegativeMarker);
            var reverseSign       = figures.Contains(ReverseSignMarker);
            var isSale            = figures.Contains(TaxSalesMarker);
            var isPurchase        = figures.Contains(TaxPurchasesMarker);

            var fromDate = FromDate;
            if (figures.Contains(FromDateLastJulyMarker))
            {
                fromDate = new DateTime(ToDate.Year, 7, 1);
                if (fromDate > ToDate) fromDate = new DateTime(ToDate.Year - 1, 7, 1);
            }

            var values = new List<object>();
            var reportingCategories = new List<Guid>();
            var cellKey = ComputeCellKey(figures);

            foreach (var e in figures)
            {
                // Built-in markers — handled regardless of whether a backing entity exists.
                if (e == EmployeeNameMarker)  { values.Add(employee?.Name);   continue; }
                if (e == EmployeeEmailMarker) { values.Add(employee?.Email);  continue; }
                if (e == SupplierNameMarker)  { values.Add(supplier?.Name);   continue; }
                if (e == BusinessNameMarker)  { values.Add(businessDetails.Name); continue; }
                if (e == FromDateMarker)      { values.Add(fromDate);         continue; }
                if (e == ToDateMarker)        { values.Add(ToDate);           continue; }

                var figure = database.SingleOrDefault<NamedObject>(e);
                if (figure == null && !string.IsNullOrWhiteSpace(businessDetails.Obsolete_Country))
                {
                    figure = Localizations.Localizations.Get(businessDetails.Obsolete_Country).SingleOrDefault(x => x.Key == e) as NamedObject;
                }

                // Known figure types with non-sum semantics.
                if (figure is ReportTransformationLabel label)
                {
                    values.Add(label.Name);
                    continue;
                }
                if (figure is CustomField customField)
                {
                    var v = ResolveCustomFieldValue(customField, employee, supplier, businessDetails);
                    if (v != null) values.Add(v);
                    continue;
                }

                if (e == HideIfEmptyMarker)
                {
                    continue;
                }

                // Default: treat as a reporting-category Guid and sum any transactions tagged
                // with it. Works for both legacy entity-backed categories (TaxCodeReportingCategory,
                // TaxAmountReportingCategory, …) and self-contained extension Guids that never
                // existed as entities — the engine no longer requires a NamedObject to be present.
                var trxs = transactions.Where(x => x.Date >= fromDate && x.Date <= ToDate && (x.ReportingCategory == e || x.ReportingCategoryReversed == e));
                if (isSale) trxs = trxs.Where(x => x.TaxCode == null || x.IsSale);
                if (isPurchase) trxs = trxs.Where(x => x.TaxCode == null || !x.IsSale);
                var amount = trxs.Sum(x => x.BaseAmount);
                if (figure is TaxCodeReversedReportingCategory) amount *= -1m;
                values.Add(amount);
                reportingCategories.Add(e);
            }

            var cell = new Cell { Key = cellKey };

            if (values.Count == 0)
            {
                cell.Hidden = hideIfEmpty;
                return cell;
            }

            if (values.Any(x => x is string))
            {
                cell.Text = string.Join(' ', values.Select(x => x?.ToString() ?? string.Empty));
                if (hideIfEmpty && string.IsNullOrWhiteSpace(cell.Text)) cell.Hidden = true;
                return cell;
            }

            if (values.All(x => x is decimal))
            {
                var total = values.Sum(x => (decimal)x);
                if (reverseSign) total *= -1m;
                if (setZeroIfNegative && total < 0m) total = 0m;
                cell.Number = total;
                if (hideIfEmpty && total == 0m)
                {
                    cell.Hidden = true;
                }
                else if (reportingCategories.Count > 0)
                {
                    cell.DrillDownUrl = BuildDrillDownUrl(reportingCategories.ToArray(), fromDate, supplier?.Key, employee?.Key, reverseSign, isSale, isPurchase);
                }
                return cell;
            }

            if (values.Count == 1 && values[0] is DateTime dt)
            {
                cell.Text = dt.ToLocalShortDisplayString();
                return cell;
            }

            cell.Text = string.Join(' ', values.Select(x => x?.ToString() ?? string.Empty));
            return cell;
        }

        // XOR-hash of figure GUIDs (sorted ascending, HideIfEmpty marker excluded) — matches legacy
        // BaseReportTransformationView.GetCell so existing custom scripts that select cells by class
        // name keep working.
        private static Guid? ComputeCellKey(Guid[] figures)
        {
            if (figures == null) return null;
            Guid? key = null;
            foreach (var e in figures.OrderBy(x => x))
            {
                if (e == HideIfEmptyMarker) continue;
                key = key.HasValue ? Xor(key.Value, e) : e;
            }
            return key;
        }

        private static Guid Xor(Guid a, Guid b)
        {
            var ab = a.ToByteArray();
            var bb = b.ToByteArray();
            var cb = new byte[ab.Length];
            for (var i = 0; i < cb.Length; i++) cb[i] = (byte)(ab[i] ^ bb[i]);
            return new Guid(cb);
        }

        // Returns the string value to display for a CustomField figure based on which entity owns it.
        // CustomField.Contains(Type) checks the field's Placement against the given entity type's GUID.
        private static object ResolveCustomFieldValue(CustomField field, Model.Employee employee, Model.Supplier supplier, BusinessDetails businessDetails)
        {
            string raw = null;
            if (employee != null && field.Contains(typeof(Model.Employee)) && employee.CustomFields != null
                && employee.CustomFields.TryGetValue(field.Key, out var fromEmployee))
            {
                raw = fromEmployee;
            }
            if (raw == null && supplier != null && field.Contains(typeof(Model.Supplier)) && supplier.CustomFields != null
                && supplier.CustomFields.TryGetValue(field.Key, out var fromSupplier))
            {
                raw = fromSupplier;
            }
            if (raw == null && field.Contains(typeof(BusinessDetails)) && businessDetails.CustomFields != null
                && businessDetails.CustomFields.TryGetValue(field.Key, out var fromBusiness))
            {
                raw = fromBusiness;
            }
            if (raw == null) return null;

            if (field.Type == CustomFieldStyle.Date
                && DateTime.TryParseExact(raw, "yyyy-M-d", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate;
            }
            return raw;
        }

        // Builds a URL into the legacy ReportTransformationFigures HTML handler so cells stay
        // drill-downable during the migration. Replace once the legacy handler is removed.
        private string BuildDrillDownUrl(Guid[] reportingCategories, DateTime fromDate, Guid? supplierKey, Guid? employeeKey, bool reverseSign, bool isSale, bool isPurchase)
        {
            var handler = new HttpHandlers.Businesses.Business.Reports.ReportTransformationReports.ReportTransformationReportFigures
            {
                Business = Business,
                From = fromDate,
                To = ToDate,
                Employee = employeeKey ?? Employee,
                Supplier = supplierKey,
                AccountingMethod = AccountingMethod,
                ReportingCategories = reportingCategories,
            };
            if (reverseSign) handler.ReverseSigns = true;
            if (isSale) handler.Sales = true;
            if (isPurchase) handler.Purchases = true;
            return handler.ToUrl();
        }

        public sealed class Item
        {
            public string Name { get; set; }
            public Guid[] Column1 { get; set; }
            public Guid[] Column2 { get; set; }
            public Guid[] Column3 { get; set; }
            public Guid[] Column4 { get; set; }
            public Guid[] Column5 { get; set; }
        }

        public sealed class Response
        {
            public Row[] Rows { get; set; }
            public Group[] SupplierGroups { get; set; }
            public Group[] EmployeeGroups { get; set; }
            // Number of fractional digits the business's base currency uses (e.g. 2 for USD, 3 for BHD).
            public int Decimals { get; set; }
        }

        public sealed class Group
        {
            public Guid Key { get; set; }
            public string Name { get; set; }
            public Row[] Rows { get; set; }
        }

        public sealed class Row
        {
            public string Name { get; set; }
            public bool IsHeader { get; set; }
            public Cell[] Cells { get; set; }
        }

        public sealed class Cell
        {
            // Stable hash of the figure GUIDs in this cell; used by per-report custom scripts to
            // locate specific cells in the rendered HTML via getElementsByClassName.
            public Guid? Key { get; set; }
            public decimal? Number { get; set; }
            public string Text { get; set; }
            public bool Hidden { get; set; }
            public string DrillDownUrl { get; set; }
        }
    }
}
