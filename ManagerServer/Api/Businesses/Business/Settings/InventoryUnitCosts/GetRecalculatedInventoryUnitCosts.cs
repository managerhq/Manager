using ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryUnitCosts;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Settings.InventoryUnitCosts
{
    [ProtoContract]
    internal sealed class GetRecalculatedInventoryUnitCosts : AuthorizedEndpoint<InventoryUnitCost[]>
    {
        [Description("Recalculate inventory unit costs from this date forward. If omitted, recalculates from the beginning of time.")]
        [ProtoMember(1)] public DateTime? FromDate { get; set; }

        public override InventoryUnitCost[] AuthorizedHandle()
        {
            var fromDate = FromDate ?? DateTime.MinValue;

            var database = ApplicationData.Instance.Businesses.Get(Business);

            var baseCurrency = database.Single<BaseCurrency>();

            var aggregations = database.GetGeneralLedgerTransactions().GetAggregations();

            var inventoryItems = database.GetGeneralLedgerTransactions()
                .GetAll()
                .Where(x => x.Value.ContainsGeneralLedgerTransaction.Value)
                .Where(x => x.Value.ContainsInventoryOnHandTransaction.Value)
                .Where(x => x.Value.MaxDate.Value >= fromDate)
                .SelectMany(x => x.Value.GetLines())
                .Where(x => x.Date >= fromDate)
                .Where(x => x.Transaction.IsGeneralLedgerTransaction())
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .GroupBy(x => x.InventoryItem.Key)
                .ToFrozenDictionary(x => x.Key, x => x.GroupBy(x => x.Date).ToFrozenDictionary(x => x.Key, x => x.ToArray()));

            var inventoryUnitCosts = database.OfType<InventoryUnitCost>().Where(x => x.InventoryItem.HasValue && x.Date < fromDate).ToList();

            var discrepancies = new Dictionary<Guid, decimal>();

            for (int i = 0; i < 20; i++)
            {
                foreach (var e in inventoryItems)
                {
                    discrepancies[e.Key] = Recalculate(aggregations, e.Value, inventoryUnitCosts, fromDate, e.Key, baseCurrency);
                }

                if (discrepancies.All(x => x.Value == 0m)) break;
            }

            var output = Reduce(inventoryUnitCosts).Where(x => x.Date >= fromDate).ToArray();

            return output;
        }

        private static decimal Recalculate(GeneralLedgerAggregations aggregations, FrozenDictionary<DateTime, GeneralLedgerTransaction[]> generalLedgerTransactions, List<InventoryUnitCost> inventoryUnitCosts, DateTime fromDate, Guid inventoryItem, BaseCurrency baseCurrency)
        {
            var balance = 0m;
            var qty = 0m;
            if (fromDate > DateTime.MinValue)
            {
                balance = aggregations.GetInventoryItemAmount(inventoryItem, DateTime.MinValue, fromDate.AddDays(-1));
                qty = aggregations.GetInventoryItemQtyOwned(inventoryItem, DateTime.MinValue, fromDate.AddDays(-1));
            }

            var pending = new Queue<GeneralLedgerTransaction[]>();

            var discrepancy = 0m;

            foreach (var e in generalLedgerTransactions.OrderBy(x => x.Key))
            {
                foreach (var e2 in e.Value.Where(x => !x.IsCostOfGoodsSold))
                {
                    if (e2.Qty.HasValue) qty += e2.Qty.Value;

                    if (e2.Transaction is not ProductionOrder)
                    {
                        balance += e2.BaseAmount;
                    }
                    else
                    {
                        foreach (var e3 in e2.ContraTransactions)
                        {
                            if (!e3.IsCostOfGoodsSold)
                            {
                                balance -= e3.BaseAmount;
                            }
                            else if (e3.Qty.HasValue)
                            {
                                var inventoryUnitCost = GetInventoryUnitCost(inventoryUnitCosts, e3.InventoryItem.Key, e3.Date);
                                if (inventoryUnitCost != 0m)
                                {
                                    balance -= baseCurrency.Round(e3.Qty.Value * inventoryUnitCost);
                                }
                            }
                        }
                    }
                }

                if (e.Value.Any(x => x.IsCostOfGoodsSold))
                {
                    pending.Enqueue([.. e.Value.Where(x => x.IsCostOfGoodsSold)]);
                }

                if (inventoryItem == new Guid("c7837e33-9b55-4677-9b7b-a79b70a7518d") && e.Key.Year == 2025)
                {
                    // This is edge-case demonstrated by ANMOL FEEDS (2025-03-09) within inventory item "85 - Mash Chilka"
                    // Findings: inventory write-offs must not be positive
                    // Implications: production orders probably also cannot produce negative quantities in finished item and credit notes should not be cost of sales
                }

                while (pending.Count > 0 && pending.Peek().Sum(x => x.Qty.Value) + qty >= 0m)
                {
                    var costOfSalesTransactions = pending.Dequeue();

                    var inventoryUnitCost = GetInventoryUnitCost(inventoryUnitCosts, inventoryItem, costOfSalesTransactions[0].Date);

                    var unitCost = inventoryUnitCost;

                    if (qty > 0m)
                    {
                        if (balance == 0m) unitCost = 0m;
                        else if (balance > 0m) unitCost = FindOptimalUnitPrice(balance, qty, baseCurrency);
                    }

                    foreach (var e2 in costOfSalesTransactions)
                    {
                        var originalCalculateAmount = baseCurrency.Round(inventoryUnitCost * e2.Qty.Value);
                        var calculatedAmount = baseCurrency.Round(unitCost * e2.Qty.Value);
                        discrepancy += Math.Abs(calculatedAmount - originalCalculateAmount);

                        SetInventoryUnitCost(inventoryUnitCosts, e2.InventoryItem.Key, e2.Date, unitCost);

                        balance += calculatedAmount;
                        qty += e2.Qty.Value;
                    }
                }
            }

            // This is needed in case cost of sale loop never runs due to negative inventory
            if (pending.Any() && inventoryUnitCosts.Count == 0)
            {
                if (balance > 0m && qty > 0m)
                {
                    var costOfSalesTransactions = pending.Dequeue();
                    var unitCost = (balance / qty);
                    SetInventoryUnitCost(inventoryUnitCosts, inventoryItem, costOfSalesTransactions.First().Date, unitCost);
                }
            }

            return discrepancy;
        }

        public static decimal GetInventoryUnitCost(List<InventoryUnitCost> inventoryUnitCosts, Guid inventoryItem, DateTime date)
        {
            var index = inventoryUnitCosts.BinarySearch(new InventoryUnitCost() { Date = date, InventoryItem = inventoryItem });
            if (index >= 0) return inventoryUnitCosts[index].UnitCost;
            var insertPoint = ~index;
            if (insertPoint == 0) return 0m;
            var inventoryUnitCost = inventoryUnitCosts[insertPoint - 1];
            if (inventoryUnitCost.InventoryItem == inventoryItem) return inventoryUnitCost.UnitCost;
            return 0m;
        }

        private static void SetInventoryUnitCost(List<InventoryUnitCost> inventoryUnitCosts, Guid inventoryItem, DateTime date, decimal unitCost)
        {
            var index = inventoryUnitCosts.BinarySearch(new InventoryUnitCost() { Date = date, InventoryItem = inventoryItem });
            if (index >= 0)
            {
                inventoryUnitCosts[index].UnitCost = unitCost;
                return;
            }
            var insertPoint = ~index;
            inventoryUnitCosts.Insert(insertPoint, new InventoryUnitCost() { InventoryItem = inventoryItem, Date = date, UnitCost = unitCost });
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

        private static List<InventoryUnitCost> Reduce(IEnumerable<InventoryUnitCost> inventoryUnitCosts)
        {
            var output = new List<InventoryUnitCost>();
            foreach (var e in inventoryUnitCosts)
            {
                if (output.Count > 0)
                {
                    if (output[^1].InventoryItem == e.InventoryItem.Value && output[^1].UnitCost == e.UnitCost) continue;
                }

                output.Add(e);
            }
            return output;
        }
    }
}
