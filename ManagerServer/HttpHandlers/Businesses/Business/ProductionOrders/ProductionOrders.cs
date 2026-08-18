using System.Collections.Generic;
using System.Linq;
using ManagerServer;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.ProductionOrders
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.ProductionOrders))]
    [Guid("0d929bb1-931d-4d35-83da-a39174e25241")]
    [Guide("The **Production Orders** tab is designed for manufacturing businesses. It enables them to oversee and track their production processes, managing the transformation of raw materials into finished goods.")]
    [TabScreenshot("fa-conveyor-belt", nameof(Strings.ProductionOrders))]
    [Guide("To create a new production order, click the **New Production Order** button.")]
    [HeroButtonScreenshot(nameof(Strings.ProductionOrders), nameof(Strings.NewProductionOrder))]
    [Guide("The **Production Orders** tab includes several columns:")]
    [Columns]
    internal sealed class ProductionOrders : NakedObjectsWithAutomaticRows<ManagerServer.Model.ProductionOrder>
    {
        [Default]
        [WarnIfFutureDate]
        [Center, MinWidth]
        [WhitespaceNoWrap]
        [Guid("34947391-d26a-4faa-87c9-32f3a8aff033")]
        [Guide("The date when the production order was created or executed.")]
        public DateTime[] GetDate(ProductionOrder[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [PaddedSorting]
        [WarnIfNotUnique]
        [Guid("1aec787d-16d9-405d-9467-7e424e990e04")]
        [Guide("A unique reference number that identifies this production order.")]
        public string[] GetReference(ProductionOrder[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("d9483df6-b27d-4587-9cd5-9bbb109c2ec1")]
        [Guide("A description of what is being produced or any notes about this production order.")]
        public string[] GetDescription(ProductionOrder[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("ec045885-97ca-47a4-94a4-affafffcb823")]
        [Guide("The *inventory location* where the finished goods will be stored after production.")]
        public string[] GetInventoryLocation(ProductionOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<CustomInventoryLocation>(x.InventoryLocation)?.Name).ToArray();
        }

        [Default]
        [Guid("be433952-3ee2-4e43-b23f-b50d33f798b9")]
        [Guide("The *inventory item* that will be produced as a result of this production order.")]
        public string[] GetFinishedInventoryItem(ProductionOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<InventoryItem>(x.FinishedInventoryItem)?.ItemName).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("ff3803f0-cd3f-4191-9953-250af363ea18")]
        [Guide("The quantity of finished goods to be produced by this production order.")]
        public decimal[] GetQty(ProductionOrder[] rows)
        {
            return rows.Select(x => x.Qty).ToArray();
        }

        [Default]
        [Right, Sum, Bold]
        [Guid("ff14c7fa-bebb-4679-abea-f1bba03161f2")]
        [Guide("The total cost of producing the finished goods, including all raw materials and allocated costs.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetTotalCost(ProductionOrder[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => x.CostOfSales(database).HasValue ? new Tuple<decimal, Currency, BusinessTemplate>(x.CostOfSales(database).Value, baseCurrency, new ProductionOrderCosts() { Business = Business, Transaction = x.Key, ReverseSign = true, Referrer = referrer }) : null).ToArray();
        }

        [Default]
        [Center, MinWidth]
        [Guid("2c9c8708-be0e-43a7-bb0b-1c9aa770dbf2")]
        [Guide("Shows whether the production order has been successfully completed.")]
        [Guide("A status of **Complete** means all required inventory items from the *bill of materials* were available and allocated.")]
        [Guide("A status of **Insufficient Quantity** indicates that some required materials were not available in sufficient quantities.")]
        public Status[] GetStatus(ProductionOrder[] rows)
        {
            return rows.Select(x => Status.Complete).ToArray();
            /*
            var database = Manager.ApplicationData.Businesses.Get(FileID);

            if (!database.Single<Manager.Model.InventoryAutomaticRevaluation>().Enabled)
            {
                return rows.Select(x => Status.Complete).ToArray();
            }
            else
            {
                var productionOrders = new Manager.Query.GeneralLedger.GeneralLedger(FileID).CalculateInventoryCostOfGoodsSold().Where(x => x.GeneralLedgerAccount.IsInventoryOnHand && x.ProductionOrder != null).GroupBy(x => x.ProductionOrder.Key).ToDictionary(x => x.Key, x => x.ToArray());
                var output = new List<Status>();
                foreach (var e in rows)
                {
                    var qty = 0m;
                    var costOfGoodsSoldQty = 0m;
                    if (productionOrders.TryGetValue(e.Key, out GeneralLedgerTransaction[] transactions))
                    {
                        qty = transactions.Where(x => x.InventoryCostDestination != null).Sum(x => x.Qty ?? 0m) * -1m;
                        costOfGoodsSoldQty = transactions.Where(x => x.IsCostOfGoodsSold).Sum(x => x.CostOfGoodsSoldQty ?? 0m) * -1m;
                    }

                    if (qty == costOfGoodsSoldQty) output.Add(Status.Complete);
                    else output.Add(Status.InsufficientQuantity);
                }
                return output.ToArray();
            }
            */
        }

        public enum Status
        {
            [Success] Complete,
            [Danger] InsufficientQuantity
        }
    }
}
