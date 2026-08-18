using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseOrders
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("446ddae4-2f0a-4a55-aa28-0502b393d360")]
    [Title(nameof(Strings.PurchaseOrders))]
    [Guide("The **Purchase Orders** tab allows you to create, document, and monitor your orders to suppliers. You can use this tab simply to generate purchase orders. Additionally, you have the option to track the accuracy of invoicing and delivery for your orders.")]
    [TabScreenshot("fa-shopping-cart", nameof(Strings.PurchaseOrders))]
    [Header("Getting Started")]
    [Guide("To add a new purchase order, click the **New Purchase Order** button.")]
    [HeroButtonScreenshot(nameof(Strings.PurchaseOrders), nameof(Strings.NewPurchaseOrder))]
    [LinkGuide("For more information, see:", typeof(PurchaseOrderForm))]
    [Header("Understanding the Display")]
    [Guide("The **Purchase Orders** tab displays several columns.")]
    [Columns]
    [Guide("Click the **Edit Columns** button to choose which columns you want to display.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("For more information, see:", typeof(NakedObjectsWithEditColumns<>))]
    [Guide("The Purchase Orders screen shows a list of all purchase orders. If you wish to view individual lines across all purchase orders, click the **Purchase Orders - Lines** button in the bottom-right corner.")]
    [SmallBottomButtonScreenshot(name: "PurchaseOrders-Lines")]
    [LinkGuide("For more information, see:", typeof(PurchaseOrderLines))]
    [Header("Tracking Invoice and Delivery Status")]
    [Guide("To monitor if your purchase orders are accurately invoiced by suppliers, go to **Edit Columns** and turn on the *Invoice Amount* and *Invoice Status* columns.")]
    [Guide("If you're utilizing the **Inventory Items** tab and purchasing inventory items, you have the option to monitor the delivery status for each order. To do so, click the **Edit Columns** button and activate the *Qty to Receive* and *Delivery Status* columns.")]
    [Guide("It's important to note that the payment status to the supplier is not tracked within the order itself. This information can be found under the **Purchase Invoices** tab. The main goal of tracking purchase orders is to ensure that individual orders are accurately invoiced or fulfilled.")]
    [Header("Using Advanced Queries")]
    [Guide("Utilize **Advanced Queries** to organize, filter, and categorize purchase orders on the Purchase Orders screen.")]
    [Guide("For example, you can display only those purchase orders for which you are still awaiting delivery from the supplier.")]
    [AdvancedQuery(select: new[] { nameof(Strings.Date), nameof(Strings.Supplier), nameof(Strings.QtyToReceive) }, where: [nameof(Strings.QtyToReceive), nameof(Strings.IsNotZero), null])]
    [LinkGuide("For more information, see:", typeof(NakedObjectsWithAdvancedQueries))]
    internal sealed class PurchaseOrders : NakedObjectsWithAutomaticRows<PurchaseOrder>
    {
        [ProtoMember(1)] public Guid? Supplier;

        protected override PurchaseOrder[] OnGetRows(PurchaseOrder[] rows)
        {
            if (Supplier.HasValue) rows = rows.Where(x => x.Supplier == Supplier).ToArray();
            return rows;
        }

        [Default]
        [WarnIfFutureDate]
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("1e354a36-3dc6-4fca-9ba4-53ec4c71c846")]
        [Guide("The *Date* column displays the issuance date of the purchase order to the supplier.")]
        public DateTime[] GetDate(PurchaseOrder[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [WarnIfNotUnique]
        [PaddedSorting]
        [Guid("4982128c-5ba1-4f59-a29e-1d08b157dadf")]
        [Guide("The *Reference* column displays the reference number associated with your purchase order.")]
        public string[] GetReference(PurchaseOrder[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("895a4366-1c0e-4b1c-90d4-b1971c8dce0d")]
        [Guide("The *Supplier* column displays the name of the supplier to whom the purchase order was issued.")]
        public string[] GetSupplier(PurchaseOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Supplier>(x.Supplier)?.GetCodeAndName()).ToArray();
        }

        [Guid("a1abd9d2-106b-4a38-87ce-80c7a6eb5ba0")]
        [Guide("The *Purchase Quote* column displays the reference number of a quote from a supplier that has been approved. This column is applicable only if you are utilizing the **Purchase Quotes** tab.")]
        [LinkGuide("For more information, see:", typeof(PurchaseQuotes.PurchaseQuotes))]
        public string[] GetPurchaseQuote(PurchaseOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<PurchaseQuote>(x.PurchaseQuote)?.GetName()).ToArray();
        }

        [Default]
        [Guid("77624b46-6bca-454d-aa03-87a271645787")]
        [Guide("The *Description* column displays the description of the purchase order.")]
        public string[] GetDescription(PurchaseOrder[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("b8c688bb-8bd7-4e3c-b24b-91ec5a004b74")]
        [Guide("The *Order Amount* column displays the total amount of the purchase order.")]
        public Tuple<decimal, Currency>[] GetOrderAmount(PurchaseOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetReversedTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }

        [Right, Sum]
        [Guid("7f127b7b-21a0-42a0-aef2-e1f809d94f34")]
        [Guide("The *Qty On Order* column shows the total quantity which has been ordered without being invoiced or received.")]
        [Guide("It's important to note that *Qty On Order* can be decreased by either a *purchase invoice* or a *goods receipt*. That is, either by the supplier sending an invoice or by shipping the goods.")]
        [Guide("In other words, *Qty On Order* tracks quantity of inventory items that have been ordered but haven't been received or invoiced yet.")]
        [Guide("Once inventory items on order have been invoiced, they have been purchased from the accounting point of view and the supplier owes the shipment regardless of any order.")]
        [Guide("Similarly, once inventory items on order have been received, from the accounting point of view you have a negative quantity balance with the supplier, which means the supplier will send the invoice regardless of the order. This is common when customers can make many small orders which the supplier ships continuously but invoices at specific intervals in bulk.")]
        [Guide("If you want to track quantities received and invoiced on purchase order column, then use **Edit Columns** button to disable *Qty On Order* column.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyOnOrder(PurchaseOrder[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);

            var balances = GetPurchaseOrderQuantities(database, purchaseOrders: rows.Select(x => x.Key).ToArray())
                .GroupBy(x => x.PurchaseOrder.Key)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.QtyOnOrder));

            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                balances.TryGetValue(x.Key, out decimal value) ? value : 0,
                new PurchaseOrderQtyOnOrder() { Business = Business, PurchaseOrder = x.Key, Referrer = referrer }
            )).ToArray();
        }

        [Center]
        [Guid("9f10d2da-a65f-4690-b5b4-d2e955cfb3d6")]
        [Guide("The *Delivery Status* column shows whether the ordered items have been fully delivered. It displays *Delivered* when all items have been received, and *Pending* when items are still awaiting delivery.")]
        public DeliveryStatus[] GetDeliveryStatus(PurchaseOrder[] rows)
        {
            return GetQtyOnOrder(rows).Select(x => x.Item1 != 0m ? DeliveryStatus.Pending : DeliveryStatus.Delivered).ToArray();
        }

        private Dictionary<PurchaseOrder, Invoice> getInvoiced = null;
        public Dictionary<PurchaseOrder, Invoice> GetInvoiced(PurchaseOrder[] rows)
        {
            if (getInvoiced == null)
            {
                var referrer = this.ToUrl();
                var database = ApplicationData.Businesses.Get(Business);
                var baseCurrency = database.Single<BaseCurrency>();

                var invoicedPurchaseOrders = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.PurchaseOrder != null && x.Transaction is PurchaseInvoice && x.IsBalancing).GroupBy(x => x.PurchaseOrder.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.AccountAmount) * -1m);

                var output = new Dictionary<PurchaseOrder, Invoice>();
                foreach (var e in rows)
                {
                    var currency = database.SingleOrDefault<ForeignCurrency>(database.SingleOrDefault<Supplier>(e.Supplier)?.Currency) as Currency ?? baseCurrency;

                    decimal? invoiced = null;
                    if (invoicedPurchaseOrders.ContainsKey(e.Key)) invoiced = invoicedPurchaseOrders[e.Key];

                    var status = InvoiceStatus.NotApplicable;
                    var orderAmount = e.GetGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing)?.AccountAmountMultipliedByNegativeOne;

                    if (orderAmount != 0m)
                    {
                        status = InvoiceStatus.Uninvoiced;
                        if (invoiced.HasValue)
                        {
                            if (e.GetGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing)?.AccountAmountMultipliedByNegativeOne <= invoiced.Value) status = InvoiceStatus.Invoiced;
                            else if (invoiced.Value > 0m) status = InvoiceStatus.PartiallyInvoiced;
                        }
                    }

                    output.Add(e, new Invoice()
                    {
                        InvoiceAmount = invoiced.HasValue ? new Tuple<decimal, Currency, BusinessTemplate>(invoiced.Value, currency, new PurchaseInvoices.PurchaseInvoices() { Business = Business, Supplier = e.Supplier, PurchaseOrder = e.Key, Referrer = referrer }) : null,
                        InvoiceStatus = status
                    });
                }
                getInvoiced = output;
            }
            return getInvoiced;
        }

        [Right, Sum]
        [Guid("35d42d77-d8de-4c67-9359-a9068d2f8bcb")]
        [Guide("The *Invoice Amount* column shows the total amount from all *purchase invoices* linked to a single purchase order. Normally, you would link just one invoice to one order. However, there are cases where a supplier might bill you in parts, issuing several invoices for a single order. This feature ensures that the combined total of all these invoices matches the total order amount.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetInvoiceAmount(PurchaseOrder[] rows)
        {
            var balances = GetInvoiced(rows);
            return rows.Select(x => balances[x].InvoiceAmount).ToArray();
        }

        [Center]
        [Guid("944d34bf-f4d6-4534-a7d2-3928223268a1")]
        [Guide("The *Invoice Status* column can be set to *Invoiced*, *Partially Invoiced*, or *Uninvoiced*. This feature allows you to quickly identify which orders are awaiting invoicing and which orders have been completely invoiced.")]
        public InvoiceStatus[] GetInvoiceStatus(PurchaseOrder[] rows)
        {
            var balances = GetInvoiced(rows);
            return rows.Select(x => balances[x].InvoiceStatus).ToArray();
        }

        public sealed class Invoice
        {
            public Tuple<decimal, Currency, BusinessTemplate> InvoiceAmount;
            public InvoiceStatus InvoiceStatus;
        }

        public enum InvoiceStatus
        {
            NotApplicable,
            [Success] Invoiced,
            [Warning] PartiallyInvoiced,
            [Danger] Uninvoiced
        }

        public enum DeliveryStatus
        {
            NotApplicable,
            [Success] Delivered,
            [Danger] Pending
        }

        protected override void OnFooterEndSection(Context context)
        {
            if (!Supplier.HasValue)
            {
                using (A(href: new PurchaseOrderLines() { Business = Business }.ToUrl(), @class: "btn btn-xs")) Write(Strings.PurchaseOrder + " - " + Strings.Lines);
            }
            base.OnFooterEndSection(context);
        }

        public static PurchaseOrderQty[] GetPurchaseOrderQuantities(Database database, Guid[] purchaseOrders = null, Guid[] inventoryItems = null)
        {
            var purchaseOrderKeys = new HashSet<Guid>(purchaseOrders);

            var transactions = new List<Transaction>();
            transactions.AddRange(database.OfType<PurchaseOrder>().Where(x => purchaseOrderKeys.Count == 0 || purchaseOrderKeys.Contains(x.Key)));
            transactions.AddRange(database.OfType<PurchaseInvoice>().Where(x => x.PurchaseOrder.HasValue && (purchaseOrderKeys.Count == 0 || purchaseOrderKeys.Contains(x.PurchaseOrder.Value))));
            transactions.AddRange(database.OfType<GoodsReceipt>().Where(x => x.PurchaseOrder.HasValue && (purchaseOrderKeys.Count == 0 || purchaseOrderKeys.Contains(x.PurchaseOrder.Value))));

            return transactions
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem != null)
                .Where(x => (x.PurchaseOrderAsTransaction ?? x.PurchaseOrder) != null)
                .Where(x => inventoryItems == null || inventoryItems.Contains(x.InventoryItem.Key))
                .Where(x => x.Qty.HasValue)
                .GroupBy(x => new { PurchaseOrder = x.PurchaseOrderAsTransaction ?? x.PurchaseOrder, x.InventoryItem })
                .Select(x => new PurchaseOrderQty()
                {
                    PurchaseOrder = x.Key.PurchaseOrder,
                    InventoryItem = x.Key.InventoryItem,
                    QtyOrdered = x.Sum(y => y.QtyOrdered),
                    QtyReceived = x.Sum(y => y.QtyDelivered),
                    QtyInvoiced = x.Sum(y => y.QtyInvoiced)
                }).ToArray();
        }

        public sealed class PurchaseOrderQty
        {
            public PurchaseOrder PurchaseOrder;
            public InventoryItem InventoryItem;

            public decimal QtyOrdered;
            public decimal QtyInvoiced;
            public decimal QtyReceived;

            public decimal QtyOnOrder => Math.Max(0, QtyOrdered - Math.Max(QtyInvoiced, QtyReceived));
        }
    }
}
