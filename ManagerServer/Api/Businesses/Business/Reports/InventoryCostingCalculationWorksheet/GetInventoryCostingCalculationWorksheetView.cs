using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryCostingCalculationWorksheet;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryCostingCalculationWorksheet
{
    [ProtoContract]
    internal sealed class GetInventoryCostingCalculationWorksheetView : GetReportView<Model.InventoryCostingCalculationWorksheet>
    {
        protected override string DefaultTitle => Strings.InventoryCostingCalculationWorksheet;

        protected override ReportModel Build(Database business, Model.InventoryCostingCalculationWorksheet report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.As_at_XXX, report.Date.ToLocalShortDisplayString());
            model.Subtitle2 = Strings.GetPropertyValue(report.ValuationMethod.ToString());

            model.Columns.Add(new Column { Name = Strings.Qty, HideTotals = true });
            model.Columns.Add(new Column { Name = Strings.AverageCost, IsBold = true, HideTotals = true });
            model.Columns.Add(new Column { Name = Strings.TotalCost });

            Cell QtyCell(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Quantity, model.WholeNumbers, link);
            Cell CurrCell(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var items = report.ValuationMethod switch
            {
                ManagerServer.Model.Enums.InventoryValuationMethodWithoutManual.FirstInFirstOut => GetFirstInFirstOutUnitCosts(Business, report.Date),
                ManagerServer.Model.Enums.InventoryValuationMethodWithoutManual.WeightedAverageCost => GetWeightedAverageUnitCosts(Business, report.Date),
                _ => [],
            };

            foreach (var e in items.OrderBy(x => x.InventoryItem.GetCodeAndName()))
            {
                var totalCostLink = report.ValuationMethod switch
                {
                    ManagerServer.Model.Enums.InventoryValuationMethodWithoutManual.WeightedAverageCost => new Link(new InventoryCostingCalculationWorksheetTotalCostTransactions { Business = Business, Referrer = Referrer, Date = report.Date, InventoryItem = e.InventoryItem.Key }.ToUrl()),
                    _ => null,
                };

                // ExcludeIfZero = true in legacy; skip row when all cells are zero
                if (e.Qty == 0m && e.AverageCost == 0m && e.TotalCost == 0m) continue;

                var row = new Row
                {
                    Name = e.InventoryItem.NameWithCode,
                    Cells =
                    [
                        QtyCell(e.Qty, new Link(new InventoryCostingCalculationWorksheetQtyTransactions { Business = Business, Referrer = Referrer, To = report.Date, InventoryItemQty = e.InventoryItem.Key }.ToUrl())),
                        CurrCell(e.AverageCost),
                        CurrCell(e.TotalCost, totalCostLink),
                    ],
                };
                model.Rows.Items.Add(row);
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }

        public static Item[] GetWeightedAverageUnitCosts(string fileId, DateTime? date)
        {
            var database = ApplicationData.Instance.Businesses.Get(fileId);
            var baseCurrency = database.Single<BaseCurrency>();

            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(fileId)
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.Date <= (date ?? DateTime.MaxValue))
                .GroupBy(x => x.InventoryItem)
                .Select(x => new Tuple<InventoryItem, GeneralLedgerTransaction[]>(x.Key, x.OrderByDescending(x => x.Date).ToArray()))
                .ToArray();

            var output = new List<Item>();

            foreach (var e in transactions.OrderBy(x => x.Item1.NameWithCode))
            {
                var qty = e.Item2.Sum(x => x.Qty ?? 0m) / 1.000000000000000000000000000000000m;
                if (qty <= 0m) continue;

                var totalCost = e.Item2.Sum(x => x.BaseAmount);

                var averageCost = FindOptimalUnitPrice(totalCost, qty, baseCurrency);

                output.Add(new Item()
                {
                    Date = e.Item2.Max(x => x.Date),
                    InventoryItem = e.Item1,
                    Qty = qty,
                    AverageCost = averageCost,
                    TotalCost = totalCost
                });
            }

            return output.ToArray();
        }

        public static Item[] GetFirstInFirstOutUnitCosts(string fileId, DateTime? date)
        {
            var database = ApplicationData.Instance.Businesses.Get(fileId);
            var baseCurrency = database.Single<BaseCurrency>();

            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(fileId)
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.Date <= (date ?? DateTime.MaxValue))
                .ToArray();

            var averageCosts = database.OfType<InventoryUnitCost>()
                .Where(x => x.InventoryItem != null)
                .Where(x => x.Date <= (date ?? DateTime.MaxValue))
                .Where(x => x.UnitCost >= 0m)
                .GroupBy(x => x.InventoryItem.Value)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(x => x.Date).First().UnitCost);

            var output = new List<Item>();

            // First in, First out
            foreach (var e in transactions.GroupBy(x => x.InventoryItem.Key))
            {
                var inventoryItem = e.First().InventoryItem;

                var qty = e.Sum(x => x.Qty ?? 0m) / 1.000000000000000000000000000000000m;
                if (qty <= 0m) continue;

                var totalCost = GetTotalCostOnFirstInFirstOutBasis(baseCurrency, qty, e.ToArray(), averageCosts).Sum(x => x.Item2);

                var recalcuatedAverageCost = FindOptimalUnitPrice(totalCost, qty, baseCurrency);

                output.Add(new Item()
                {
                    Date = e.Max(x => x.Date),
                    AverageCost = recalcuatedAverageCost,
                    InventoryItem = inventoryItem,
                    Qty = qty,
                    TotalCost = totalCost
                });
            }

            return output.ToArray();
        }

        public static List<Tuple<decimal?, decimal, GeneralLedgerTransaction[]>> GetTotalCostOnFirstInFirstOutBasis(BaseCurrency baseCurrency, decimal qty, GeneralLedgerTransaction[] transactions, Dictionary<Guid, decimal> averageCosts)
        {
            var output = new List<Tuple<decimal?, decimal, GeneralLedgerTransaction[]>>();
            var totalCost = 0m;
            var qtyRemaining = qty;
            foreach (var e2 in transactions.GroupBy(x => x.Date).OrderByDescending(x => x.Key))
            {
                var transactionQty = 0m;
                var baseAmount = 0m;

                foreach (var e3 in e2)
                {
                    if ((e3.Qty ?? 0m) < 0m || e3.BaseAmount < 0m) continue;

                    transactionQty += e3.Qty ?? 0m;
                    baseAmount += e3.BaseAmount;

                    if (e3.Transaction is InventoryItemStartingBalance inventoryStartingBalance)
                    {
                        baseAmount = baseCurrency.Round(inventoryStartingBalance.AverageCost * transactionQty);
                    }
                    if (e3.Transaction is ProductionOrder productionOrder && e3.IsBalancing)
                    {
                        var baseAmount2 = 0m;
                        if (productionOrder.BillOfMaterials != null)
                        {
                            foreach (var e4 in productionOrder.BillOfMaterials)
                            {
                                if (!e4.BillOfMaterials.HasValue) continue;
                                if (e4.Qty <= 0m) continue;

                                if (averageCosts.TryGetValue(e4.BillOfMaterials.Value, out decimal averageCost))
                                {
                                    baseAmount2 += e4.Qty * averageCost;
                                }
                            }
                        }

                        baseAmount += baseCurrency.Round(baseAmount2);
                    }
                }

                if (qtyRemaining >= transactionQty)
                {
                    totalCost += baseAmount;
                    qtyRemaining -= transactionQty;
                    output.Add(new Tuple<decimal?, decimal, GeneralLedgerTransaction[]>(transactionQty, baseAmount, e2.ToArray()));
                }
                else
                {
                    var fraction = baseCurrency.Round(baseAmount / transactionQty * qtyRemaining);
                    totalCost += fraction;
                    output.Add(new Tuple<decimal?, decimal, GeneralLedgerTransaction[]>(qtyRemaining, fraction, e2.ToArray()));
                    qtyRemaining = 0m;
                }

                if (qtyRemaining <= 0m) break;
            }

            return output;
        }

        private static decimal FindOptimalUnitPrice(decimal total, decimal quantity, BaseCurrency baseCurrency)
        {
            for (int decimals = 0; decimals <= 28; decimals++)
            {
                var roundedUnitPrice = Math.Round(total / quantity, decimals);
                var calculatedTotal = baseCurrency.Round(roundedUnitPrice * quantity);

                if (calculatedTotal == total) return roundedUnitPrice;
            }

            return total / quantity;
        }

        public sealed class Item
        {
            public DateTime Date;
            public InventoryItem InventoryItem;
            public decimal Qty;
            public decimal AverageCost;
            public decimal TotalCost;
        }
    }
}
