using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.RealizedCurrencyGainsLosses
{
    [ProtoContract]
    internal sealed class GetRealizedCurrencyGainsLossesView : GetReportView<Model.RealizedCurrencyGainsLosses>
    {
        protected override string DefaultTitle => Strings.RealizedCurrencyGainsAndLosses;

        protected override ReportModel Build(Database business, Model.RealizedCurrencyGainsLosses report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Name = Strings.AcquisitionCost });
            model.Columns.Add(new Column { Name = Strings.SettlementAmount });
            model.Columns.Add(new Column { Name = Strings.RealizedGain, IsBold = true });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var baseCurrency = business.Single<BaseCurrency>();
            var transactions = new GeneralLedger(Business).Where(x => x.Date <= report.ToDate).ToArray();
            foreach (var e in transactions.Where(x => x.AccountCurrency is ForeignCurrency).GroupBy(x => x.AccountCurrency))
            {
                var foreignCurrencyBalance = 0m;
                var baseCurrencyBalance = 0m;

                foreach (var e2 in e.GroupBy(x => x.Date).OrderBy(x => x.Key))
                {
                    var foreignCurrencyMovement = e2.Sum(x => x.AccountAmount);

                    if (foreignCurrencyBalance != 0m && foreignCurrencyMovement != 0m && Math.Sign(foreignCurrencyBalance) != Math.Sign(foreignCurrencyMovement))
                    {
                        foreach (var e3 in e2.OrderByDescending(x => x.Transaction.CanBeRealizedCurrencyTransaction(business)).ThenByDescending(x => Math.Sign(x.AccountAmount) == Math.Sign(foreignCurrencyBalance)))
                        {
                            if (Math.Sign(e3.AccountAmount) == Math.Sign(foreignCurrencyBalance))
                            {
                                baseCurrencyBalance += e3.BaseAmount;
                                foreignCurrencyBalance += e3.AccountAmount;
                            }
                            else if (foreignCurrencyBalance == 0m)
                            {
                                baseCurrencyBalance += e3.BaseAmount;
                                foreignCurrencyBalance += e3.AccountAmount;
                            }
                            else if (!e3.Transaction.CanBeRealizedCurrencyTransaction(business))
                            {
                                baseCurrencyBalance += e3.BaseAmount;
                                foreignCurrencyBalance += e3.AccountAmount;
                            }
                            else if (e3.AccountAmount == 0m)
                            {
                                baseCurrencyBalance += e3.BaseAmount;
                            }
                            else
                            {
                                var unrealizedExchangeRate = baseCurrencyBalance / foreignCurrencyBalance;
                                var cost = baseCurrency.Round(e3.AccountAmount * unrealizedExchangeRate);
                                var gain = cost - e3.BaseAmount;

                                baseCurrencyBalance += cost;
                                foreignCurrencyBalance += e3.AccountAmount;

                                if (e2.Key >= report.FromDate)
                                {
                                    model.Rows.Items.Add(new Row
                                    {
                                        Name = e2.Key.ToLocalShortDisplayString(),
                                        Cells = new List<Cell>
                                        {
                                            Make(Math.Abs(cost)),
                                            Make(Math.Abs(e3.BaseAmount)),
                                            Make(gain)
                                        }
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        baseCurrencyBalance += e2.Sum(x => x.BaseAmount);
                        foreignCurrencyBalance += foreignCurrencyMovement;
                    }
                }
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
