using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.Currencies.RealizedCurrencyGainsLosses;
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

            foreach (var step in RealizedCurrencyGains.Replay(business, Business, report.ToDate))
            {
                if (!step.IsRealization) continue;
                if (step.Date < report.FromDate) continue;

                var viewHandler = ManagerServer.HttpHandlers.Businesses.Business.TransactionViewer.GetViewHandler(Business, step.Entry.Transaction, Referrer);
                var settlementLink = viewHandler != null ? new Link(viewHandler.ToUrl()) : null;

                var acquisitionCostLink = new Link(new RealizedCurrencyGainsLossesAcquisitionCost
                {
                    Business = Business,
                    Referrer = Referrer,
                    Currency = step.Currency.Key,
                    ToDate = report.ToDate,
                    Realization = step.RealizationIndex
                }.ToUrl());

                model.Rows.Items.Add(new Row
                {
                    Name = step.Date.ToLocalShortDisplayString() + " — " + step.Currency.GetCode() + (step.Entry.Transaction != null ? " — " + step.Entry.Transaction.GetTransactionName() : null),
                    Cells = new List<Cell>
                    {
                        Make(Math.Abs(step.Cost), acquisitionCostLink),
                        Make(Math.Abs(step.Entry.BaseAmount), settlementLink),
                        Make(step.Gain)
                    }
                });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
