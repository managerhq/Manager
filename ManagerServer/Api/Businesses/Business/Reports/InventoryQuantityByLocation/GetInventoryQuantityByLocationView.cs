using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryQuantityByLocation;
using System;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryQuantityByLocation
{
    [ProtoContract]
    internal sealed class GetInventoryQuantityByLocationView : GetReportView<Model.InventoryQuantityByLocation>
    {
        protected override string DefaultTitle => Strings.InventoryQuantityByLocation;

        protected override ReportModel Build(Database business, Model.InventoryQuantityByLocation report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.As_at_XXX, report.Date.ToLocalShortDisplayString());

            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).ToList();
            transactions.AddRange(business.OfType<ManagerServer.Model.GoodsReceipt>().SelectMany(x => x.GetGeneralLedgerTransactions(business)));
            transactions.AddRange(business.OfType<ManagerServer.Model.DeliveryNote>().SelectMany(x => x.GetGeneralLedgerTransactions(business)));
            transactions.AddRange(business.OfType<ManagerServer.Model.InventoryTransfer>().SelectMany(x => x.GetGeneralLedgerTransactions(business)));

            transactions = transactions
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.Date <= report.Date)
                .ToList();

            var inventoryItems = transactions
                .GroupBy(x => new { x.InventoryItem, x.InventoryLocation })
                .Select(x => new Tuple<ManagerServer.Model.InventoryItem, ManagerServer.Model.CustomInventoryLocation, decimal>(x.Key.InventoryItem, x.Key.InventoryLocation, x.Sum(y => y.QtyOnHand)))
                .Where(x => x.Item3 != 0m)
                .ToArray();

            var locations = inventoryItems.Select(x => x.Item2).Distinct().ToArray();

            if (report.CustomInventoryLocations) locations = business.OfType<ManagerServer.Model.CustomInventoryLocation>().Where(x => report.InventoryLocations != null && report.InventoryLocations.Contains(x.Key)).ToArray();

            foreach (var e in locations)
            {
                if (e == null) model.Columns.Add(new Column { Name = Strings.Unspecified });
                else model.Columns.Add(new Column { Key = e.Key.ToString(), Name = e.Name });
            }

            if (!report.CustomInventoryLocations) model.Columns.Add(new Column { Name = Strings.Total, IsBold = true });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Quantity, model.WholeNumbers, link);

            foreach (var e in inventoryItems.Select(x => x.Item1).Distinct().OrderBy(x => x.NameWithCode))
            {
                var total = 0m;
                var cells = new System.Collections.Generic.List<Cell>();

                foreach (var e2 in locations)
                {
                    var inventoryQty = inventoryItems.SingleOrDefault(x => x.Item1 == e && x.Item2 == e2);
                    cells.Add(Make(inventoryQty?.Item3, new Link(new InventoryQuantityByLocationTransactions { Business = Business, Referrer = Referrer, InventoryItem = e.Key, InventoryLocation = e2?.Key, Date = report.Date }.ToUrl())));
                    if (inventoryQty != null) total += inventoryQty.Item3;
                }

                if (!report.CustomInventoryLocations) cells.Add(Make(total));

                // ExcludeIfZero = true in legacy; skip when all cells zero
                var allZero = cells.All(c => (c.Value ?? 0m) == 0m);
                if (allZero) continue;

                model.Rows.Items.Add(new Row { Name = e.NameWithCode, Cells = cells });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
