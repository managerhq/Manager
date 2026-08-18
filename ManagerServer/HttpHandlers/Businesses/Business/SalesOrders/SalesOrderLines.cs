using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesOrders
{
    [ProtoContract]
    [Guid("7cd2bf35-f585-4601-8ff8-35fc547287db")]
    [Title(nameof(Strings.SalesOrders), nameof(Strings.Lines))]
    [Guide("The Sales Order Lines report shows all line items from sales orders.")]
    [Guide("View item details, quantities, prices, and totals across all orders.")]
    [Columns]
    internal sealed class SalesOrderLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.SalesOrder.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<SalesOrder>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null && !x.IsCostOfGoodsSold).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesOrderForm() { Business = Business, Key = x.SalesOrderAsTransaction.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesOrderView() { Business = Business, Key = x.SalesOrderAsTransaction.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("30b4356e-2304-4a42-8554-64572bddb2a6")]
        [Guide("The date when the sales order was created or issued. This represents when the customer placed their order.")]
        [Guide("This date is important for order tracking, aging reports, and determining when to fulfill the order. It may differ from the delivery or invoice date.")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SalesOrderAsTransaction.Date).ToArray();
        }

        [Default]
        [PaddedSorting]
        [Guid("0f6193a3-c987-421c-b5f2-7155b1eecd09")]
        [Guide("Unique reference number for the sales order. This helps identify and track specific orders throughout their lifecycle.")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SalesOrderAsTransaction.Reference).ToArray();
        }

        [Default]
        [Guid("b7964eb3-cd59-4c84-a060-526d2ce67e57")]
        [Guide("The customer who placed this sales order. This identifies who is purchasing the goods or services.")]
        [Guide("The customer selection determines billing address, payment terms, and any special pricing agreements. Ensure the correct customer is selected for accurate order processing.")]
        public string[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.Name).ToArray();
        }

        [Guid("39a596a0-52cc-4888-948b-b384ad47a6e5")]
        [Guide("A general description or summary of the entire sales order. This helps identify the order's purpose at a glance.")]
        [Guide("Use this to note special instructions, delivery requirements, or to summarize what the order contains. Examples: 'Monthly office supplies order', 'Custom furniture order - Johnson project'.")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SalesOrderAsTransaction.Description).ToArray();
        }

        [Default]
        [Guid("850478a7-956b-4b00-b8f8-fda30ace60cc")]
        [Guide("The specific product or service that the customer is ordering. Select from your inventory or non-inventory items list.")]
        [Guide("Selecting an item will automatically populate default settings like description, price, and tax code. For custom items not in your list, you can enter details manually in the description field.")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Guid("1808d3fa-e963-4359-a298-972dc2bbb714")]
        [Guide("A detailed description for this specific line item within the sales order. This describes what is being ordered on this line.")]
        [Guide("Include product specifications, customizations, or special requirements for this line item. Clear descriptions help warehouse staff fulfill orders correctly and reduce errors.")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Guid("72e164c5-8150-430d-bfc3-f84fd1484e35")]
        [Guide("The number of units ordered by the customer for this line item. This represents the quantity commitment before delivery.")]
        [Guide("Enter whole numbers or decimals as appropriate for the item. The quantity will be used to calculate the line total and track order fulfillment. Ensure the unit of measure matches your inventory settings.")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value*-1m : default(decimal?)).ToArray();
        }

        [Guid("cf215ae9-855b-453c-b3ec-00b21549302f")]
        [Guide("The selling price for one unit of the item. This is the agreed price with the customer before any discounts are applied.")]
        [Guide("The unit price should match your pricing agreements with the customer. If you have standard pricing, this will default from the item's settings but can be overridden for special pricing arrangements.")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetUnitPrice(x.Transaction).HasValue ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Guid("38a7ecdd-7e04-4fc1-890d-b440d4897976")]
        [Guide("The project that this line item should be allocated to for tracking purposes. This enables project-based profitability analysis.")]
        [Guide("Select a project if you want to track sales and costs by project. This helps determine project profitability and is useful for job costing or contract management.")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("6b3a5ed3-90ad-4df4-95d5-5340c120fcf3")]
        [Guide("The business division or department that this line item belongs to. This enables divisional reporting and analysis.")]
        [Guide("Use divisions to segment your business for better financial analysis. This helps track performance by business unit, location, or product line.")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("8394790c-0eb2-45e2-b37d-49d87c9e519e")]
        [Guide("The tax code that determines how this line item will be taxed. Different items may have different tax treatments.")]
        [Guide("Select the appropriate tax code based on the item type and customer location. The tax code determines the tax rate and whether the item is taxable, exempt, or zero-rated.")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Guid("5518eebd-5186-4f6e-b3b2-07eb4781cdf0")]
        [Guide("The tax amount calculated for this line item based on the selected tax code and line total. This shows the tax liability for this line.")]
        [Guide("This is automatically calculated as (quantity × unit price) × tax rate. The tax amount is added to the line subtotal to get the final line amount.")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value * -1m, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("ce601c96-fb7c-4d7d-96e7-b46d52b7c194")]
        [Guide("Total amount for the line item including tax. This is the final amount the customer will be charged for this line item.")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => new Tuple<decimal, Currency>(x.TransactionAmount*-1m, x.TransactionCurrency)).ToArray();
        }
    }
}