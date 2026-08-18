using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesOrders
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("d4912688-adef-40fd-8cec-d67aab0f967a")]
    [Title(nameof(Strings.SalesOrders))]
    [Guide("The **Sales Orders** tab helps you record and monitor orders received from customers.")]
    [TabScreenshot("fa-shopping-basket", nameof(Strings.SalesOrders))]
    [Guide("Sales orders act as an umbrella over multiple invoices and delivery notes, allowing you to track complex fulfillment scenarios.")]
    [Header("When to Use Sales Orders")]
    [Guide("Use sales orders when there is a delay between order placement and invoice issuance, or when orders require multiple shipments or partial invoicing.")]
    [Guide("If customers receive invoices and deliveries immediately upon ordering, you may not need sales orders as they are fulfilled instantly.")]
    [Guide("Sales orders are particularly useful for tracking partial fulfillments, back orders, and monitoring overall order completion status.")]    
    [Header("Getting Started")]
    [Guide("Before creating sales orders, ensure customers are set up in the **Customers** tab, as every order must be linked to a customer.")]
    [Guide("To create a new sales order, click the **New Sales Order** button.")]
    [HeroButtonScreenshot(nameof(Strings.SalesOrders), nameof(Strings.NewSalesOrder))]
    [LinkGuide("For more information see:", typeof(SalesOrderForm))]
    [Header("Managing Columns")]
    [Guide("The **Sales Orders** tab displays information in customizable columns.")]
    [Columns]
    [Guide("Click the **Edit Columns** button to select which columns to display.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("Learn more:", typeof(NakedObjectsWithEditColumns<SalesOrder>))]
    [Header("Tracking Order Status")]
    [Guide("To monitor whether sales orders have been invoiced, enable the *Invoice Amount* and *Invoice Status* columns in **Edit Columns**.")]
    [Guide("If you use the **Inventory Items** tab, you can track delivery status by enabling the *Qty to Deliver* and *Delivery Status* columns.")]
    [Guide("An order is considered closed when its *Invoice Status* shows *Invoiced* and its *Delivery Status* shows *Delivered*.")]
    [Guide("Note that order status does not indicate whether the customer has paid. Payment tracking is handled in the **Sales Invoices** tab.")]
    [Guide("The primary purpose of tracking sales orders is to ensure that orders are accurately invoiced and fulfilled.")]
    [Header("Converting Orders to Invoices and Delivery Notes")]
    [Guide("To convert a sales order to an invoice, click **View** on the sales order, then click **Copy to** and select **New Sales Invoice**.")]
    [Guide("This method works best when one order will have exactly one invoice.")]
    [Guide("For orders requiring multiple deliveries, create a **New Delivery Note** linked to the sales order, detailing what is being delivered.")]
    [Guide("The delivery note can then be copied to a **New Sales Invoice**, maintaining proper linkage to the original order.")]
    [Header("Inventory Management")]
    [Guide("When a sales order is created, it increases *Qty Reserved* and decreases *Qty Available* under the **Inventory Items** tab.")]
    [Guide("This reserves inventory for the order without creating a delivery obligation.")]
    [Guide("Only when an invoice is issued does *Qty Reserved* decrease and *Qty to Deliver* increase, creating the actual delivery obligation.")]
    [Guide("This workflow supports businesses that dispatch goods only after payment, as invoices can be issued after payment is received.")]
    [Header("Filtering and Sorting")]
    [Guide("Use **Advanced Queries** to filter, sort, and group sales orders.")]
    [Guide("For example, you can display only sales orders with pending deliveries.")]
    [AdvancedQuery(select: [nameof(Strings.Date), nameof(Strings.Customer), nameof(Strings.QtyToDeliver)], where: [nameof(Strings.QtyToDeliver), nameof(Strings.IsNotZero), null])]
    [LinkGuide("Learn more:", typeof(NakedObjectsWithAdvancedQueries))]
    [Header("Managing Sales Orders")]
    [Guide("Sales orders can be edited at any time, even after partial invoicing or delivery.")]
    [Guide("To cancel an order, click **Edit** on the sales order and check the *Cancelled* checkbox.")]
    [Guide("To duplicate an order, click **View** on the sales order, then use the **Clone** button or **Copy to** option.")]
    [Guide("For recurring orders, go to **Settings** tab, then **Recurring Transactions**, then **Recurring Sales Orders**.")]
    [LinkGuide("Learn more about recurring transactions:", typeof(Settings.RecurringTransactions.RecurringSalesOrders.RecurringSalesOrders))]
    internal sealed class SalesOrders : NakedObjectsWithAutomaticRows<SalesOrder>
    {
        [ProtoMember(1)] public Guid? Customer;

        protected override SalesOrder[] OnGetRows(SalesOrder[] rows)
        {
            if (Customer.HasValue) rows = rows.Where(x => x.Customer == Customer).ToArray();
            return rows;
        }

        [Default]
        [WarnIfFutureDate]
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("0dbbd25c-0f41-47f1-a392-c1370ccc9672")]
        [Guide("The *Date* column shows the date of the sales order.")]
        public DateTime[] GetDate(SalesOrder[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [WarnIfNotUnique]
        [PaddedSorting]
        [Guid("916db179-c8ef-47fe-9ab3-d08e94513cd5")]
        [Guide("The *Reference* column displays the reference number of the sales order.")]
        public string[] GetReference(SalesOrder[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("be67f579-9b1f-4bca-991b-4a5abcb820ea")]
        [Guide("The *Customer* column displays the name of the customer who placed the sales order.")]
        public string[] GetCustomer(SalesOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.GetCodeAndName()).ToArray();
        }

        [Guid("d39096b9-2e5c-4822-94c8-427699c9e7c2")]
        [Guide("The *Sales Quote* column displays the reference number of an approved customer quote. Use this column only if you are using the **Sales Quotes** tab.")]
        public string[] GetSalesQuote(SalesOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<SalesQuote>(x.SalesQuote)?.GetName()).ToArray();
        }

        [Default]
        [Guid("72f722b5-6f50-45d3-ad28-c9a523713333")]
        [Guide("The *Description* column displays the description of the sales order.")]
        public string[] GetDescription(SalesOrder[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Right, Sum]
        [Guid("9788c257-1485-4d93-bda2-29eed6290295")]
        [Guide("The *Qty to Deliver* column shows the quantity of items that have been ordered but not yet delivered or invoiced.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyReserved(SalesOrder[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);

            var balances = GetSalesOrderQuantities(database, salesOrders: rows.Select(x => x.Key).ToArray())
                .GroupBy(x => x.SalesOrder.Key)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.QtyReserved));

            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                balances.TryGetValue(x.Key, out decimal value) ? value : 0,
                new SalesOrderQtyReserved() { Business = Business, SalesOrder = x.Key, Referrer = referrer }
            )).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("12104b52-ecce-45b7-8087-e8783d03d485")]
        [Guide("The *Order Amount* column displays the total amount of the sales order.")]
        public Tuple<decimal, Currency>[] GetOrderAmount(SalesOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }

        private Dictionary<SalesOrder, Invoice> getInvoices = null;
        public Dictionary<SalesOrder, Invoice> GetInvoiced(SalesOrder[] rows)
        {
            if (getInvoices == null)
            {
                var referrer = this.ToUrl();
                var database = ApplicationData.Businesses.Get(Business);
                var baseCurrency = database.Single<BaseCurrency>();

                var invoicedSalesOrders = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.SalesOrder != null && x.Transaction is SalesInvoice && x.IsBalancing).GroupBy(x => x.SalesOrder.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.AccountAmount));

                var output = new Dictionary<SalesOrder, Invoice>();
                foreach (var e in rows)
                {
                    var currency = database.SingleOrDefault<ForeignCurrency>(database.SingleOrDefault<Customer>(e.Customer)?.Currency) as Currency ?? baseCurrency;

                    decimal? invoiced = null;
                    if (invoicedSalesOrders.ContainsKey(e.Key)) invoiced = invoicedSalesOrders[e.Key];

                    var orderAmount = e.GetGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing)?.AccountAmount;
                    var status = InvoiceStatus.NotApplicable;

                    if (orderAmount != 0m)
                    {
                        status = InvoiceStatus.Uninvoiced;
                        if (invoiced.HasValue)
                        {
                            if (orderAmount <= invoiced.Value) status = InvoiceStatus.Invoiced;
                            else if (invoiced.Value > 0m) status = InvoiceStatus.PartiallyInvoiced;
                        }
                    }

                    output.Add(e, new Invoice()
                    {
                        InvoiceAmount = invoiced.HasValue ? new Tuple<decimal, Currency, BusinessTemplate>(invoiced.Value, currency, new SalesInvoices.SalesInvoices() { Business = Business, Customer = e.Customer, SalesOrder = e.Key, Referrer = referrer }) : null,
                        InvoiceStatus = status
                    });
                }
                getInvoices = output;
            }
            return getInvoices;
        }

        [Right, Sum]
        [Guid("10b75b0a-0c48-421d-aa0a-0355ee3a9947")]
        [Guide("The *Invoice Amount* column shows the total from all sales invoices linked to this sales order.")]
        [Guide("While typically one invoice is linked to one order, you may invoice customers in stages with multiple invoices.")]
        [Guide("This column helps ensure the total invoiced amount matches the order value.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetInvoiceAmount(SalesOrder[] rows)
        {
            var balances = GetInvoiced(rows);
            return rows.Select(x => balances[x].InvoiceAmount).ToArray();
        }

        [Center]
        [Guid("8364f509-1f09-4077-b038-6889f508afcd")]
        [Guide("The *Invoice Status* column displays the invoicing status of each order.")]
        [Guide("Possible statuses are: *Invoiced* (fully invoiced), *Partially Invoiced* (partially invoiced), *Uninvoiced* (not yet invoiced), or *Not Applicable* (when order amount is zero).")]
        [Guide("This helps you quickly identify which orders still need invoicing.")]
        public InvoiceStatus[] GetInvoiceStatus(SalesOrder[] rows)
        {
            var balances = GetInvoiced(rows);
            return rows.Select(x => balances[x].InvoiceStatus).ToArray();
        }

        [Center]
        [Guid("516f8260-113e-496d-bd33-701781448897")]
        [Guide("The *Delivery Status* column shows whether ordered items have been delivered.")]
        [Guide("Status is *Delivered* when all items have been delivered, or *Pending* when items remain to be delivered.")]
        public DeliveryStatus[] GetDeliveryStatus(SalesOrder[] rows)
        {
            return GetQtyReserved(rows).Select(x => x.Item1 != 0m ? DeliveryStatus.Pending : DeliveryStatus.Delivered).ToArray();
        }

        public sealed class Invoice
        {
            public Tuple<decimal, Currency, BusinessTemplate> InvoiceAmount;
            public InvoiceStatus InvoiceStatus;
        }

        public enum DeliveryStatus
        {
            NotApplicable,
            [Success] Delivered,
            [Danger] Pending
        }

        public enum InvoiceStatus
        {
            NotApplicable,
            [Success] Invoiced,
            [Warning] PartiallyInvoiced,
            [Danger] Uninvoiced
        }

        protected override void OnFooterEndSection(Context context)
        {
            if (!Customer.HasValue)
            {
                using (A(href: new SalesOrderLines() { Business = Business }.ToUrl(), @class: "btn btn-xs")) Write(Strings.SalesOrders + " - " + Strings.Lines);
            }
            base.OnFooterEndSection(context);
        }

        public static SalesOrderQty[] GetSalesOrderQuantities(Database database, Guid[] salesOrders = null, Guid[] inventoryItems = null)
        {
            var salesOrderKeys = new HashSet<Guid>(salesOrders);

            var transactions = new List<Transaction>();
            transactions.AddRange(database.OfType<SalesOrder>().Where(x => salesOrderKeys.Count == 0 || salesOrderKeys.Contains(x.Key)));
            transactions.AddRange(database.OfType<SalesInvoice>().Where(x => x.SalesOrder.HasValue && (salesOrderKeys.Count == 0 || salesOrderKeys.Contains(x.SalesOrder.Value))));
            transactions.AddRange(database.OfType<DeliveryNote>().Where(x => x.SalesOrder.HasValue && (salesOrderKeys.Count == 0 || salesOrderKeys.Contains(x.SalesOrder.Value))));

            return transactions
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem != null)
                .Where(x => (x.SalesOrderAsTransaction ?? x.SalesOrder) != null)
                .Where(x => inventoryItems == null || inventoryItems.Contains(x.InventoryItem.Key))
                .Where(x => x.Qty.HasValue)
                .GroupBy(x => new { SalesOrder = x.SalesOrderAsTransaction ?? x.SalesOrder, x.InventoryItem })
                .Select(x => new SalesOrderQty()
                {
                    SalesOrder = x.Key.SalesOrder,
                    InventoryItem = x.Key.InventoryItem,
                    QtyOrdered = x.Sum(y => y.QtyOrdered),
                    QtyDelivered = x.Sum(y => y.QtyDelivered),
                    QtyInvoiced = x.Sum(y => y.QtyInvoiced)
                }).ToArray();
        }

        public sealed class SalesOrderQty
        {
            public SalesOrder SalesOrder;
            public InventoryItem InventoryItem;

            public decimal QtyOrdered;
            public decimal QtyInvoiced;
            public decimal QtyDelivered;

            public decimal QtyReserved => Math.Max(0, QtyOrdered - Math.Max(QtyInvoiced, QtyDelivered));
        }
    }
}
