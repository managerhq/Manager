using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Helpers;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("80bdd6ea-c139-43a1-9de4-4f281c38970f")]
    [Title(nameof(Strings.InventoryItems))]
    [Guide("The **Inventory Items** tab functions as a module for creating, monitoring, and managing an inventory list.")]
    [TabScreenshot(icon: "fa-inventory", name: nameof(Strings.InventoryItems))]
    [Header("Getting Started")]
    [Guide("Click the **New Inventory Item** button to create a new inventory item.")]
    [HeroButtonScreenshot(title: nameof(Strings.InventoryItems), name: nameof(Strings.NewInventoryItem))]
    [LinkGuide("For more information, see:", typeof(InventoryItemForm))]
    [Guide("If you have created inventory items with existing quantities, you can set starting balances under **Settings**, then **Starting Balances**.")]
    [LinkGuide("For more information, see:", typeof(Settings.StartingBalances.InventoryItems.InventoryItemStartingBalanceList))]
    [Guide("By default, when you use the **Inventory Items** tab, all inventory purchases will debit your *Inventory on Hand* asset account and all inventory sales will credit your *Inventory Sales* income account.")]
    [Header("Understanding the Display")]
    [Guide("The **Inventory Items** tab features several columns:")]
    [Columns]
    [Guide("To customize visible columns, use the **Edit Columns** button.")]
    [LinkGuide("For more information, see:", typeof(NakedObjectsWithEditColumns<>))]    
    [Header("Using Advanced Queries")]
    [Guide("Utilize the **Advanced Queries** feature to organize inventory items by filtering, sorting, and grouping them within the **Inventory Items** screen.")]
    [Guide("For instance, if you want to display a list of inventory items that shows only the *Qty on Hand*, your advanced query might look like this:")]
    [AdvancedQuery(select: new[] { nameof(Strings.ItemCode), nameof(Strings.ItemName), nameof(Strings.QtyOnHand) }, where: new[] { nameof(Strings.QtyOnHand), nameof(Strings.IsNotEmpty), null })]
    [Guide("You can swap *Qty on Hand* for *Qty to Deliver* to see a list of inventory items that are awaiting delivery to customers. Alternatively, use *Qty to Receive* for items still to be received from suppliers, or *Qty to Order* to identify inventory items that need to be ordered from suppliers to restock.")]
    internal sealed class InventoryItems : NakedObjectsWithAutomaticRows<InventoryItem>
    {
        protected override void OnAfterHeader(Context context)
        {
            var unitCostColumn = context.Get<Column[]>().SingleOrDefault(x => x.Key == new Guid("4003bafc-5587-4a86-a9fd-0b3b679fac09"));
            if (unitCostColumn != null)
            {
                unitCostColumn.Action = new Tuple<string, HttpHandler, bool>(Strings.Recalculate, new Settings.InventoryUnitCosts.InventoryCostCorrection() { Business = Business, Referrer = this.ToUrl() }, false);
            }

            base.OnAfterHeader(context);
        }

        [WarnIfNotUnique]
        [Guid("72c52313-6054-4682-ad12-cc4d5676e5b8")]
        [Guide("Displays the code assigned to an inventory item.")]
        public string[] GetItemCode(ManagerServer.Model.InventoryItem[] rows)
        {
            return rows.Select(x => x.ItemCode).ToArray();
        }

        [Default]
        [Guid("63d7d695-75d2-4f7a-ab63-f38696dca522")]
        [Guide("Displays the name of the item as defined in the inventory item entry.")]
        public string[] GetItemName(ManagerServer.Model.InventoryItem[] rows)
        {
            return rows.Select(x => x.ItemName).ToArray();
        }

        [Guid("2cb46b27-1cdf-47d5-a919-00e320b9d849")]
        [Guide("Displays the valuation method for the inventory item. This is used when using the **Recalculate** button.")]
        //[LinkGuide("For more information see:", typeof(RecalculateInventoryUnitCost))]
        public string[] GetValuationMethod(ManagerServer.Model.InventoryItem[] rows)
        {
            return rows.Select(x => Strings.GetPropertyValue(x.ValuationMethod.ToString())).ToArray();
        }

        [Guid("786ad00c-b989-48a5-a208-0622a271e49c")]
        [Guide("Displays the control account associated with an inventory item. By default, inventory items are assigned to the *Inventory on Hand* control account. However, you have the option to set up custom control accounts as well.")]
        public string[] GetControlAccount(ManagerServer.Model.InventoryItem[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => (database.SingleOrDefault<ManagerServer.Model.ControlAccountForInventoryItems>(x.ControlAccount) as ManagerServer.Model.NamedObject ?? database.Single<ManagerServer.Model.BalanceSheetInventoryOnHandAccount>()).GetName()).ToArray();
        }

        [Guid("ca64ba9e-0401-4d7a-ae4c-1aae02f9d46f")]
        [Guide("Indicates the division associated with an inventory item. This column is pertinent for those utilizing divisional accounting.")]
        public string[] GetDivision(ManagerServer.Model.InventoryItem[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Division>(x.Division)?.GetName()).ToArray();
        }

        [Guid("ff189d12-320b-4bd9-a50d-e12b8ce1ffc3")]
        [Guide("Displays the description that has been set for the inventory item.")]
        public string[] GetDescription(ManagerServer.Model.InventoryItem[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.HasDefaultLineDescription ? x.DefaultLineDescription : null).ToArray();
        }

        [Guid("0a4e91ed-3484-4dc6-a7b3-6351b9b0e072")]
        [Guide("Displays the default selling price for the inventory item. This price is automatically used when creating sales transactions unless overridden.")]
        public Tuple<decimal, Currency>[] GetSalePrice(ManagerServer.Model.InventoryItem[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => x.HasDefaultSalesUnitPrice ? new Tuple<decimal, Currency>(x.DefaultSalesUnitPrice, baseCurrency) : null).ToArray();
        }

        [Guid("2c4b584b-34d0-4967-b74c-497c8662bb09")]
        [Guide("Displays the default purchase price for the inventory item. This price is automatically used when creating purchase transactions unless overridden.")]
        public Tuple<decimal, Currency>[] GetPurchasePrice(ManagerServer.Model.InventoryItem[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => x.HasDefaultPurchaseUnitPrice ? new Tuple<decimal, Currency>(x.DefaultPurchaseUnitPrice, baseCurrency) : null).ToArray();
        }

        [Guid("1bcd1f73-cad9-42ef-84f5-dec2748ce27d")]
        [Guide("Displays the unit of measurement for the inventory item, such as pieces, kilograms, or liters.")]
        public string[] GetUnitName(ManagerServer.Model.InventoryItem[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.UnitName).ToArray();
        }

        [Right, Default, Sum]
        [Guid("762ceb3b-9288-4392-b5f3-fa13d1a42b76")]
        [Guide("Displays the total quantity that has been acquired but not yet sold or written off.")]
        [Guide("All general ledger transactions are included.")]
        [Guide("*Delivery Notes* and *Goods Receipts* have no effect here because they are not general ledger transactions.")]
        [Guide("When you click on the *Qty Owned* figure, you will see a list of transactions that contribute to the *Qty Owned* balance.")]
        [LinkGuide("For more information, see:", typeof(InventoryItemQtyOwned))]
        public Tuple<decimal, BusinessTemplate>[] GetQtyOwned(ManagerServer.Model.InventoryItem[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var aggregations = database.GetGeneralLedgerTransactions().GetAggregations();

            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(aggregations.GetInventoryItemQtyOwned(x.Key, DateTime.MinValue, DateTime.MaxValue).TrimTrailingZeroes(), new InventoryItemQtyOwned() { Business = Business, InventoryItem = x.Key, Referrer = referrer })).ToArray();
        }

        
        private Dictionary<InventoryItem, Tuple<decimal, BusinessTemplate>> getQtyToDeliver = null;
        [Right, Sum]
        [Guid("d7217fa0-9789-446c-89d1-7318931bc729"), Name(nameof(Strings.QtyToDeliver))]
        [Guide("Tracks inventory items that have been sold but not yet delivered to customers.")]
        [Guide("Transactions that increase *Qty to Deliver*:")]
        [Guide("- *Sales Invoices*")]
        [Guide("Transactions that decrease *Qty to Deliver*:")]
        [Guide("- *Delivery Notes*")]
        [Guide("- *Credit Notes*")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyToDeliver(ManagerServer.Model.InventoryItem[] rows)
        {
            if (getQtyToDeliver == null)
            {
                var referrer = this.ToUrl();
                var output = new Dictionary<InventoryItem, Tuple<decimal, BusinessTemplate>>();
                var database = ApplicationData.Businesses.Get(Business);
                var inventoryItems = new HashSet<Guid>(rows.Select(x => x.Key));

                var inventoryTransactions = new List<Transaction>();
                inventoryTransactions.AddRange(database.OfType<DeliveryNote>());
                inventoryTransactions.AddRange(database.OfType<SalesInvoice>());
                inventoryTransactions.AddRange(database.OfType<CreditNote>());
                inventoryTransactions.AddRange(database.OfType<InventoryItemStartingBalance>());

                var inventoryQuantities = inventoryTransactions
                    .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                    .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                    .Where(x => x.InventoryItem != null && x.Customer != null && inventoryItems.Contains(x.InventoryItem.Key))
                    .GroupBy(x => x.InventoryItem)
                    .ToDictionary(x => x.Key.Key, x => x.Sum(y => y.QtyToDeliver));                

                foreach (var e in rows)
                {
                    var total = inventoryQuantities.TryGetValue(e.Key, out decimal value) ? value : 0m;

                    output.Add(e, new Tuple<decimal, BusinessTemplate>(total.TrimTrailingZeroes(), new InventoryItemQtyToDeliver() { Business = Business, InventoryItem = e.Key, Referrer = referrer }));
                }
                getQtyToDeliver = output;
            }
            return rows.Select(x => getQtyToDeliver[x]).ToArray();
        }
        
        private Dictionary<InventoryItem, Tuple<decimal, BusinessTemplate>> getQtyToReceive = null;
        [Right, Sum]
        [Guid("8c1bf2b0-b8b0-47bb-b9c4-690728458d6f"), Name(nameof(Strings.QtyToReceive))]
        [Guide("Tracks inventory items that have been purchased but not yet received from suppliers.")]
        [Guide("Transactions that increase *Qty to Receive*:")]
        [Guide("- *Purchase Invoices*")]
        [Guide("Transactions that decrease *Qty to Receive*:")]
        [Guide("- *Goods Receipts*")]
        [Guide("- *Debit Notes*")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyToReceive(ManagerServer.Model.InventoryItem[] rows)
        {
            if (getQtyToReceive == null)
            {
                var referrer = this.ToUrl();
                var output = new Dictionary<InventoryItem, Tuple<decimal, BusinessTemplate>>();
                var database = ApplicationData.Businesses.Get(Business);
                var inventoryItems = new HashSet<Guid>(rows.Select(x => x.Key));

                var inventoryTransactions = new List<Transaction>();
                inventoryTransactions.AddRange(database.OfType<GoodsReceipt>());
                inventoryTransactions.AddRange(database.OfType<PurchaseInvoice>());
                inventoryTransactions.AddRange(database.OfType<DebitNote>());
                inventoryTransactions.AddRange(database.OfType<InventoryItemStartingBalance>());

                var inventoryQuantities = inventoryTransactions
                    .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                    .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                    .Where(x => x.InventoryItem != null && x.Supplier != null && inventoryItems.Contains(x.InventoryItem.Key))
                    .GroupBy(x => x.InventoryItem)
                    .ToDictionary(x => x.Key.Key, x => x.Sum(y => y.QtyToReceive));                

                foreach (var e in rows)
                {
                    var total = inventoryQuantities.TryGetValue(e.Key, out decimal value) ? value : 0m;
                    output.Add(e, new Tuple<decimal, BusinessTemplate>(total.TrimTrailingZeroes(), new InventoryItemQtyToReceive() { Business = Business, InventoryItem = e.Key, Referrer = referrer }));
                }
                getQtyToReceive = output;
            }
            return rows.Select(x => getQtyToReceive[x]).ToArray();
        }

        private Dictionary<InventoryItem, Tuple<decimal, BusinessTemplate>> getQtyOnHand = null;
        [Right, Sum, WarnIfNegative]
        [Guid("ee701a02-7ce3-4e3e-80d2-9cde604dbf0a")]
        [Guide("Shows the physical quantity of inventory items currently in your possession.")]
        [Header("Transactions That Affect Qty on Hand")]
        [Guide("Transactions that increase *Qty on Hand*:")]
        [Guide("- *Goods Receipts*")]
        [Guide("- All other general ledger transactions (except those listed below)")]
        [Guide("Transactions that decrease *Qty on Hand*:")]
        [Guide("- *Delivery Notes*")]
        [Header("Transactions Excluded from Qty on Hand")]
        [Guide("The following transactions affect *Qty Owned* but NOT *Qty on Hand*:")]
        [Guide("- *Sales Invoices* (unless they are also acting as delivery notes)")]
        [Guide("- *Purchase Invoices* (unless they are also acting as goods receipts)")]
        [Guide("- *Credit Notes* (unless they are also acting as delivery notes)")]
        [Guide("- *Debit Notes* (unless they are also acting as goods receipts)")]
        [Header("Key Difference")]
        [Guide("*Delivery Notes* and *Goods Receipts* affect *Qty on Hand* but not *Qty Owned*, while *Sales Invoices*, *Purchase Invoices*, *Debit Notes* and *Credit Notes* affect *Qty Owned* but not *Qty on Hand*.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyOnHand(ManagerServer.Model.InventoryItem[] rows)
        {
            if (getQtyOnHand == null)
            {
                var database = ApplicationData.Businesses.Get(Business);
                var inventoryItems = new HashSet<Guid>(rows.Select(x => x.Key));

                var referrer = this.ToUrl();
                
                var inventoryTransactions = new List<Transaction>();
                inventoryTransactions.AddRange(database.OfType<JournalEntry>());
                inventoryTransactions.AddRange(database.OfType<Payment>());
                inventoryTransactions.AddRange(database.OfType<Receipt>());
                inventoryTransactions.AddRange(database.OfType<ExpenseClaim>());
                inventoryTransactions.AddRange(database.OfType<InventoryWriteOff>());
                inventoryTransactions.AddRange(database.OfType<ProductionOrder>());
                inventoryTransactions.AddRange(database.OfType<SalesInvoice>());
                inventoryTransactions.AddRange(database.OfType<CreditNote>());
                inventoryTransactions.AddRange(database.OfType<PurchaseInvoice>());
                inventoryTransactions.AddRange(database.OfType<DebitNote>());
                inventoryTransactions.AddRange(database.OfType<InventoryItemStartingBalance>());

                var rows2 = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
                rows2.AddRange(inventoryTransactions
                    .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                    .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                    .Where(x => x.InventoryItem != null)
                    .Where(x => inventoryItems.Contains(x.InventoryItem.Key))
                    .Where(x => x.QtyOnHand != 0m));

                rows2.AddRange(database.OfType<GoodsReceipt>()
                    .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                    .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                    .Where(x => x.InventoryItem != null)
                    .Where(x => inventoryItems.Contains(x.InventoryItem.Key))
                    .Where(x => x.Supplier != null)
                    .Where(x => x.QtyOnHand != 0m));

                rows2.AddRange(database.OfType<DeliveryNote>()
                    .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                    .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                    .Where(x => x.InventoryItem != null)
                    .Where(x => inventoryItems.Contains(x.InventoryItem.Key))
                    .Where(x => x.Customer != null)
                    .Where(x => x.QtyOnHand != 0m));

                var balances = rows2.GroupBy(x => x.InventoryItem.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.QtyOnHand));

                getQtyOnHand = rows.ToDictionary(x => x, x => new Tuple<decimal, BusinessTemplate>(
                    balances.TryGetValue(x.Key, out decimal value) ? value.TrimTrailingZeroes() : 0m,
                    new InventoryItemQtyOnHand() { Business = Business, InventoryItem = x.Key, Referrer = referrer }
                ));
            }
            return rows.Select(x => getQtyOnHand[x]).ToArray();
        }
        
        private Dictionary<InventoryItem, Tuple<decimal, BusinessTemplate>> getQtyReserved = null;
        [Right, Sum]
        [Guid("d6855f33-31b7-452e-9386-0834ea7995fe")]
        [Guide("Tracks inventory items that have been reserved for sales orders but not yet delivered.")]
        [Guide("Transactions that increase *Qty Reserved*:")]
        [Guide("- *Sales Orders*")]
        [Guide("Transactions that decrease *Qty Reserved*:")]
        [Guide("- *Delivery Notes* linked to *Sales Orders*")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyReserved(ManagerServer.Model.InventoryItem[] rows)
        {
            if (getQtyReserved == null)
            {
                var referrer = this.ToUrl();
                var output = new Dictionary<InventoryItem, Tuple<decimal, BusinessTemplate>>();
                var database = ApplicationData.Businesses.Get(Business);

                var activeSalesOrders = database.OfType<SalesOrder>().Where(x => !x.Cancelled).Select(x => x.Key).ToArray();
                var salesOrderQuantities = SalesOrders.SalesOrders.GetSalesOrderQuantities(database, inventoryItems: rows.Select(x => x.Key).ToArray(), salesOrders: activeSalesOrders);

                var balances = salesOrderQuantities
                    .GroupBy(x => x.InventoryItem)
                    .ToDictionary(x => x.Key.Key, x => x.Sum(y => y.QtyReserved));

                getQtyReserved = rows.ToDictionary(x => x, x => new Tuple<decimal, BusinessTemplate>(
                    balances.TryGetValue(x.Key, out decimal value) ? value.TrimTrailingZeroes() : 0m,
                    new InventoryItemQtyReserved() { Business = Business, InventoryItem = x.Key, Referrer = referrer }
                ));
            }
            return rows.Select(x => getQtyReserved[x]).ToArray();
        }
        
        private Dictionary<InventoryItem, decimal> getQtyAvailable = null;
        [Right, Sum]
        [Guid("de761437-1f2e-48d6-9dac-8a6cec3de7f5")]
        [Guide("Shows the quantity available for immediate sale and delivery.")]
        [Guide("Calculated as: *Qty on Hand* minus *Qty to Deliver* minus *Qty Reserved*")]
        public decimal[] GetQtyAvailable(ManagerServer.Model.InventoryItem[] rows)
        {
            if (getQtyAvailable == null)
            {
                var qtyOnHand = GetQtyOnHand(rows);
                var qtyToDeliver = GetQtyToDeliver(rows);
                var qtyReserved = GetQtyReserved(rows);

                var output = new Dictionary<InventoryItem, decimal>();
                for (int i = 0; i < rows.Length; i++)
                {
                    var qtyAvailable = 0m;
                    qtyAvailable += qtyOnHand[i].Item1;
                    qtyAvailable -= Math.Max(0m , qtyToDeliver[i].Item1);
                    qtyAvailable -= qtyReserved[i].Item1;
                    output.Add(rows[i], qtyAvailable);
                }
                getQtyAvailable = output;
            }
            return rows.Select(x => getQtyAvailable[x]).ToArray();
        }
        
        private Dictionary<InventoryItem, Tuple<decimal, BusinessTemplate>> getQtyOnOrder = null;
        [Right, Sum]
        [Guid("abb55efe-1b58-4db4-9ac5-4eaf1cccbd9a")]
        [Guide("Tracks inventory items that have been ordered from suppliers but not yet received or invoiced.")]
        [Guide("Each purchase order maintains its own *Qty on Order* balance.")]
        [Guide("Calculated as: *Qty Ordered* minus the higher of *Qty Invoiced* or *Qty Received*")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyOnOrder(ManagerServer.Model.InventoryItem[] rows)
        {
            if (getQtyOnOrder == null)
            {
                var referrer = this.ToUrl();
                var output = new Dictionary<InventoryItem, Tuple<decimal, BusinessTemplate>>();
                var database = ApplicationData.Businesses.Get(Business);

                var activePurchaseOrders = database.OfType<PurchaseOrder>().Where(x => !x.Cancelled).Select(x => x.Key).ToArray();
                var purchaseOrderQuantities = PurchaseOrders.PurchaseOrders.GetPurchaseOrderQuantities(database, inventoryItems: rows.Select(x => x.Key).ToArray(), purchaseOrders: activePurchaseOrders);

                var balances = purchaseOrderQuantities
                    .GroupBy(x => x.InventoryItem)
                    .ToDictionary(x => x.Key.Key, x => x.Sum(y => y.QtyOnOrder));

                getQtyOnOrder = rows.ToDictionary(x => x, x => new Tuple<decimal, BusinessTemplate>(
                    balances.TryGetValue(x.Key, out decimal value) ? value.TrimTrailingZeroes() : 0m,
                    new InventoryItemQtyOnOrder() { Business = Business, InventoryItem = x.Key, Referrer = referrer }
                ));
            }
            return rows.Select(x => getQtyOnOrder[x]).ToArray();
        }                
        
        private Dictionary<InventoryItem, decimal> getQtyToBeAvailable = null;
        [Right, Sum]
        [Guid("4dca8df8-6d38-4aac-aa7b-7d6c9e35ff61")]
        [Guide("Shows projected future stock levels after all pending transactions are completed.")]
        [Guide("Calculated as: *Qty Available* plus *Qty to Receive* (if positive) plus *Qty on Order*")]
        public decimal[] GetQtyToBeAvailable(ManagerServer.Model.InventoryItem[] rows)
        {
            if (getQtyToBeAvailable == null)
            {
                var qtyAvailable = GetQtyAvailable(rows);
                var qtyToReceive = GetQtyToReceive(rows);
                var qtyOnOrder = GetQtyOnOrder(rows);

                var output = new Dictionary<InventoryItem, decimal>();
                for (int i = 0; i < rows.Length; i++)
                {
                    var qtyToBeAvailable = 0m;
                    qtyToBeAvailable += qtyAvailable[i];
                    qtyToBeAvailable += Math.Max(0m, qtyToReceive[i].Item1);
                    qtyToBeAvailable += qtyOnOrder[i].Item1;
                    output.Add(rows[i], qtyToBeAvailable.TrimTrailingZeroes());
                }
                getQtyToBeAvailable = output;
            }
            return rows.Select(x => getQtyToBeAvailable[x]).ToArray();
        }

        [Right, Sum]
        [Guid("859e9dbe-fdfc-4e04-85e0-44041d4532f7")]
        [Guide("Shows the reorder point for each inventory item.")]
        [Guide("This value is set when editing the inventory item and represents the minimum quantity you want to maintain in stock.")]
        public decimal?[] GetQtyDesired(ManagerServer.Model.InventoryItem[] rows)
        {
            return rows.Select(x => x.GetQtyDesired()).ToArray();
        }

        [Bold]
        [Right, Sum]
        [Guid("7de575d6-863f-440b-838a-f0986ed419d0")]
        [Guide("Shows the quantity that needs to be ordered to maintain your desired stock levels.")]
        [Guide("This is the difference between *Qty Desired* and *Qty to Be Available* when the desired quantity is higher.")]
        [Guide("As you order and receive stock, this value will decrease until your stock levels meet the desired quantity.")]
        public decimal?[] GetQtyToOrder(ManagerServer.Model.InventoryItem[] rows)
        {
            var qtyToBeAvailable = GetQtyToBeAvailable(rows);

            var output = new List<decimal?>();
            for (int i = 0; i < rows.Length; i++)
            {
                var qtyDesired = rows[i].GetQtyDesired();
                if (qtyDesired.HasValue)
                {
                    var qtyToOrder = qtyDesired.Value - qtyToBeAvailable[i];
                    if (qtyToOrder <= 0m)
                    {
                        output.Add(null);
                    }
                    else
                    {
                        output.Add(qtyToOrder.TrimTrailingZeroes());
                    }
                }
                else
                {
                    output.Add(null);
                }                
            }
            return output.ToArray();
        }

        [Right, Default]
        [Guid("de062f5e-3691-4fe3-9361-5a9fa4dade1d")]
        [Guide("Shows the average cost per unit for each inventory item.")]
        [Guide("Calculated as: *Total Cost* divided by *Qty Owned*")]
        public Tuple<decimal, Currency>[] GetAverageCost(ManagerServer.Model.InventoryItem[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            var totalCosts = GetTotalCost(rows);
            var qtyOwned = GetQtyOwned(rows);

            var output = new Tuple<decimal, Currency>[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                if (qtyOwned[i].Item1 > 0m && totalCosts[i].Item1 > 0m)
                {
                    var averageCost = baseCurrency.Round(totalCosts[i].Item1 / qtyOwned[i].Item1);
                    output[i] = new Tuple<decimal, Currency>(averageCost, baseCurrency);
                }
            }

            return output;
        }

        [Right, Sum, Default]
        [Guid("4003bafc-5587-4a86-a9fd-0b3b679fac09"), Bold]
        [Guide("Shows the total value of inventory items currently in stock.")]
        [Guide("Click on any figure to view the transactions that make up the total cost.")]
        [Guide("The **Recalculate** button above this column allows you to recalculate inventory unit costs based on your selected valuation method.")]
        [LinkGuide("For more information, see:", typeof(Settings.InventoryUnitCosts.InventoryCostCorrection))]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetTotalCost(ManagerServer.Model.InventoryItem[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var aggregations = database.GetGeneralLedgerTransactions().GetAggregations();
            var balanceSheetInventoryOnHand = database.Single<BalanceSheetInventoryOnHandAccount>().Key;

            var output = new Tuple<decimal, Currency, BusinessTemplate>[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                var balance = aggregations.GetInventoryItemAmount(rows[i].Key, DateTime.MinValue, DateTime.MaxValue);
                if (balance < 0m)
                {
                    balance = 0m;
                }
                else
                {
                    var qty = aggregations.GetInventoryItemQtyOwned(rows[i].Key, DateTime.MinValue, DateTime.MaxValue);
                    if (qty <= 0m)
                    {
                        balance = 0m;
                    }
                    else
                    {
                        balance = balance.TrimTrailingZeroes();
                    }
                }

                var generalLedgerAccount = database.SingleOrDefault<ControlAccountForInventoryItems>(rows[i].ControlAccount)?.Key ?? balanceSheetInventoryOnHand;

                output[i] = new Tuple<decimal, Currency, BusinessTemplate>(balance, baseCurrency, new InventoryItemTransactions() { Business = Business, GeneralLedgerAccount = generalLedgerAccount, InventoryItemCost = rows[i].Key, To = DateTime.MaxValue, Referrer = referrer });
            }

            return output;
        }
    }
}
