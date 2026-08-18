using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.AgedPayables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.AgedPayables
{
    [ProtoContract]
    internal sealed class GetAgedPayablesView : GetReportModelEndpoint<Model.AgedPayables>
    {
        protected override string DefaultTitle => Strings.AgedPayables;

        protected override V2.ReportModel2 Build(Database business, Model.AgedPayables report)
        {
            var date = (report.Date == ManagerServer.Model.Enums.DateType.Today ? DateTime.Today : report.CustomDate);
            var days30 = date.SafeAddDays(-30);
            var days60 = date.SafeAddDays(-60);
            var days90 = date.SafeAddDays(-90);

            var model = new V2.ReportModel2();
            model.Subtitles.Add(string.Format(Strings.As_at_XXX, date.ToLocalShortDisplayString()));

            var suppliers = business.OfType<ManagerServer.Model.Supplier>().Select(x => new Item() { Supplier = x, Currency = x.Currency }).ToDictionary(x => x.Supplier.Key);

            var purchaseInvoices = business.OfType<ManagerServer.Model.PurchaseInvoice>().Where(x => x.Supplier.HasValue && suppliers.ContainsKey(x.Supplier.Value)).ToDictionary(x => x.Key);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchPurchaseInvoices().Where(x => x.Date <= date && x.GeneralLedgerAccount.IsAccountsPayable).ToArray();
            var purchaseInvoiceBalances = transactions.Where(x => x.PurchaseInvoice != null).GroupBy(x => x.PurchaseInvoice).Select(x => new { x.Key, Balance = x.Sum(y => y.AccountAmount) * -1m }).Select(x => new Invoice() { Key = x.Key.Key, Balance = x.Balance, Supplier = x.Key.Supplier.Value, Reference = x.Key.Reference, IssueDate = x.Key.IssueDate, DueDate = x.Key.GetDueDate() }).ToArray();
            foreach (var e in purchaseInvoiceBalances.GroupBy(x => x.Supplier))
            {
                var supplier = suppliers[e.Key];
                supplier.Invoices = e.ToArray();
            }
            foreach (var e in transactions.Where(x => x.Supplier != null && x.PurchaseInvoice == null).GroupBy(x => x.Supplier))
            {
                var balance = e.Sum(y => y.AccountAmount);
                if (balance != 0m)
                {
                    var supplier = suppliers[e.Key.Key];
                    supplier.Credit = balance;
                }
            }
            foreach (var e in suppliers.Keys.ToArray())
            {
                if (suppliers[e].Invoices == null && suppliers[e].Credit == 0m) suppliers.Remove(e);
            }

            if (report.Division.HasValue)
            {
                foreach (var e in suppliers.Keys.ToArray())
                {
                    if (suppliers[e].Supplier.Division != report.Division.Value) suppliers.Remove(e);
                }
                var divisionName = business.SingleOrDefault<ManagerServer.Model.Division>(report.Division)?.Name;
                if (!string.IsNullOrWhiteSpace(divisionName)) model.Subtitles.Add(divisionName);
            }

            var foreignCurrencies = business.OfType<ManagerServer.Model.ForeignCurrency>().ToDictionary(x => x.Key);
            var foreignCurrency = suppliers.Values.Any(x => x.Currency.HasValue && foreignCurrencies.ContainsKey(x.Currency.Value));

            var showCredits = suppliers.Any(x => x.Value.Credit != 0m);

            model.Columns.Add(new V2.Column { Name = Strings.Current });
            model.Columns.Add(new V2.Column { Name = "1-30 " + Strings.Days });
            model.Columns.Add(new V2.Column { Name = "31-60 " + Strings.Days });
            model.Columns.Add(new V2.Column { Name = "61-90 " + Strings.Days });
            model.Columns.Add(new V2.Column { Name = "90+ " + Strings.Days });
            if (showCredits) model.Columns.Add(new V2.Column { Name = Strings.Less + ": " + Strings.Credit });
            model.Columns.Add(new V2.Column { Name = Strings.Total, IsBold = true });

            static V2.Cell Make(decimal? v, Link link = null) => new V2.Cell { Value = v, Link = link, Style = V2.NumberStyle.Currency };

            var view = suppliers.Values.OrderByDescending(x => x.Total).ThenBy(x => x.Supplier.NameWithCode).ToArray();
            if (report.SortBy == ManagerServer.Model.Enums.SortBy.Name) view = suppliers.Values.OrderBy(x => x.Supplier.NameWithCode).ToArray();

            var currencies = ManagerServer.Query.Currencies.GetCurrencyProvider(Business);

            foreach (var e in view.GroupBy(x => x.Currency).OrderByDescending(x => x.Count()))
            {
                var rows = new List<V2.Row>();

                foreach (var e2 in e)
                {
                    if (report.ShowInvoices)
                    {
                        var supplierItems = new List<V2.Row>();

                        if (e2.Invoices != null)
                        {
                            foreach (var e3 in e2.Invoices.OrderByDescending(x => x.DueDate))
                            {
                                decimal? current = null;
                                decimal? days30amount = null;
                                decimal? days60amount = null;
                                decimal? days90amount = null;
                                decimal? days90plusAmount = null;

                                if (e3.DueDate >= date) current = e3.Balance;
                                if (e3.DueDate < date && e3.DueDate >= days30) days30amount = e3.Balance;
                                if (e3.DueDate < days30 && e3.DueDate >= days60) days60amount = e3.Balance;
                                if (e3.DueDate < days60 && e3.DueDate >= days90) days90amount = e3.Balance;
                                if (e3.DueDate < days90) days90plusAmount = e3.Balance;

                                var rowTotal = (current ?? 0m) + (days30amount ?? 0m) + (days60amount ?? 0m) + (days90amount ?? 0m) + (days90plusAmount ?? 0m);
                                if (rowTotal == 0m) continue; // ExcludeIfZero equivalent

                                var link = new Link(new ManagerServer.HttpHandlers.Businesses.Business.PurchaseInvoices.PurchaseInvoiceView() { Business = Business, Referrer = Referrer, Key = e3.Key }.ToUrl());
                                var cells = new List<V2.Cell>
                                {
                                    Make(current, link),
                                    Make(days30amount, link),
                                    Make(days60amount, link),
                                    Make(days90amount, link),
                                    Make(days90plusAmount, link),
                                };
                                if (showCredits) cells.Add(new V2.Cell());
                                cells.Add(Make(rowTotal));

                                supplierItems.Add(new V2.Row
                                {
                                    Name = e3.IssueDate.ToShortDateString() + " — " + Strings.Invoice + " #" + e3.Reference,
                                    Cells = cells,
                                });
                            }
                        }

                        if (e2.Credit != 0m)
                        {
                            var creditCells = new List<V2.Cell>
                            {
                                new V2.Cell(),
                                new V2.Cell(),
                                new V2.Cell(),
                                new V2.Cell(),
                                new V2.Cell(),
                            };
                            if (showCredits) creditCells.Add(Make(e2.Credit));
                            creditCells.Add(Make(e2.Credit * -1m));
                            supplierItems.Add(new V2.Row { Name = Strings.AvailableCredit, Cells = creditCells });
                        }

                        if (supplierItems.Count == 0) continue;

                        // TODO: ViewModel lacks Row.Url (for subreport drill-through); Url = GetSubreportUrl(...) is dropped.
                        rows.Add(new V2.Row
                        {
                            Name = e2.Supplier.NameWithCode,
                            Rows = supplierItems,
                        });
                    }
                    else
                    {
                        decimal current = 0m;
                        decimal days30amount = 0m;
                        decimal days60amount = 0m;
                        decimal days90amount = 0m;
                        decimal days90plusAmount = 0m;

                        if (e2.Invoices != null)
                        {
                            foreach (var e3 in e2.Invoices)
                            {
                                if (e3.DueDate >= date) current += e3.Balance;
                                else if (e3.DueDate >= days30) days30amount += e3.Balance;
                                else if (e3.DueDate >= days60) days60amount += e3.Balance;
                                else if (e3.DueDate >= days90) days90amount += e3.Balance;
                                else days90plusAmount += e3.Balance;
                            }
                        }

                        var bucketsTotal = current + days30amount + days60amount + days90amount + days90plusAmount;
                        if (bucketsTotal == 0m && e2.Credit == 0m) continue;

                        Link BucketLink(int bucket) => new Link(new AgedPayablesInvoices { Business = Business, Referrer = Referrer, Supplier = e2.Supplier.Key, Date = date, Bucket = bucket }.ToUrl());

                        var cells = new List<V2.Cell>
                        {
                            Make(current == 0m ? (decimal?)null : current, BucketLink(0)),
                            Make(days30amount == 0m ? (decimal?)null : days30amount, BucketLink(1)),
                            Make(days60amount == 0m ? (decimal?)null : days60amount, BucketLink(2)),
                            Make(days90amount == 0m ? (decimal?)null : days90amount, BucketLink(3)),
                            Make(days90plusAmount == 0m ? (decimal?)null : days90plusAmount, BucketLink(4)),
                        };
                        if (showCredits) cells.Add(Make(e2.Credit == 0m ? (decimal?)null : e2.Credit));
                        cells.Add(Make(bucketsTotal - e2.Credit));

                        rows.Add(new V2.Row
                        {
                            Name = e2.Supplier.NameWithCode,
                            Cells = cells,
                        });
                    }
                }

                if (rows.Count == 0) continue;

                if (foreignCurrency)
                {
                    model.Rows.Add(new V2.Row
                    {
                        Name = currencies.Get(e.Key).GetCode(),
                        Rows = rows,
                    });
                }
                else
                {
                    model.Rows.AddRange(rows);
                    var total = V2.Row.Combine(rows.ToArray());
                    total.IsBold = true;
                    model.Rows.Add(total);
                }
            }

            model.Format();
            return model;
        }

        private sealed class Item
        {
            public ManagerServer.Model.Supplier Supplier;
            public Guid? Currency;
            public Invoice[] Invoices;
            public decimal Credit;
            public decimal Total
            {
                get
                {
                    if (Invoices == null) return 0m;
                    return Invoices.Sum(x => x.Balance) - Credit;
                }
            }
        }

        private sealed class Invoice
        {
            public Guid Key;
            public string Reference;
            public DateTime IssueDate;
            public DateTime DueDate;
            public decimal Balance;
            public Guid Supplier;
        }
    }
}
