using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxTotals
{
    [ProtoContract]
    internal sealed class GetTaxTotalsView : GetReportView<Model.TaxTotals>
    {
        protected override string DefaultTitle => Strings.TaxTotals;

        protected override ReportModel Build(Database business, Model.TaxTotals report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            var accountingMethods = business.OfType<ManagerServer.Model.SalesInvoice>().Any() || business.OfType<ManagerServer.Model.PurchaseInvoice>().Any();
            if (accountingMethods)
            {
                model.Subtitle2 = Strings.AccrualBasis;
                if (report.AccountingMethod == AccountingBasis.CashBasis) model.Subtitle2 = Strings.CashBasis;
            }

            model.Columns.Add(new Column { Key = "taxExclusiveTotal", Name = Strings.TaxExclusiveTotal });
            model.Columns.Add(new Column { Key = "taxAmount", Name = Strings.TaxAmount, IsBold = true });
            model.Columns.Add(new Column { Key = "taxInclusiveTotal", Name = Strings.TaxInclusiveTotal });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var division = business.SingleOrDefault<ManagerServer.Model.Division>(report.Division);
            if (division != null) model.Subtitle2 += " - " + division.Name;

            var totals = TaxTotalsCalculator.Calculate(Business, new Model.TaxTotals
            {
                FromDate = report.FromDate,
                ToDate = report.ToDate,
                AccountingMethod = report.AccountingMethod,
                Division = division?.Key,
            });

            foreach (var e in totals)
            {
                Link MakeLink(bool? taxTransactions, string taxComponent = null) => new Link(new HttpHandlers.Businesses.Business.Reports.TaxTotals.TaxTotalsTransactions { From = report.FromDate, To = report.ToDate, TaxTransactions = taxTransactions, TaxComponent = taxComponent, CashBasis = report.AccountingMethod == AccountingBasis.CashBasis, Business = Business, Referrer = Referrer, TaxCode = e.TaxCode.Key, Division = division?.Key }.ToUrl());

                if (e.Components != null)
                {
                    var rows = new Rows();
                    foreach (var c in e.Components)
                    {
                        rows.Items.Add(new Row
                        {
                            Name = c.Name,
                            Cells = new List<Cell>
                            {
                                Make(e.TaxExclusiveTotal, MakeLink(taxTransactions: false)),
                                Make(c.TaxAmount, MakeLink(taxTransactions: true, taxComponent: c.Name)),
                                Make(e.TaxExclusiveTotal + c.TaxAmount),
                            }
                        });
                    }

                    // The tax-exclusive base is shared by all components of a tax code, so the
                    // group total is set explicitly rather than auto-summed from component rows.
                    rows.TotalCells = new List<Cell>
                    {
                        Make(e.TaxExclusiveTotal, MakeLink(taxTransactions: false)),
                        Make(e.TaxAmount, MakeLink(taxTransactions: true)),
                        Make(e.TaxInclusiveTotal, MakeLink(taxTransactions: null)),
                    };

                    model.Rows.Items.Add(new Row { Name = e.TaxCode.Name, Rows = rows });
                }
                else
                {
                    model.Rows.Items.Add(new Row
                    {
                        Name = e.TaxCode.Name,
                        Cells = new List<Cell>
                        {
                            Make(e.TaxExclusiveTotal, MakeLink(taxTransactions: false)),
                            Make(e.TaxAmount, MakeLink(taxTransactions: true)),
                            Make(e.TaxInclusiveTotal, MakeLink(taxTransactions: null)),
                        }
                    });
                }
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
