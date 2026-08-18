using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseInvoices
{
    [ProtoContract]
    [Guid("3281fcc3-a156-4ade-ab17-ae181447a8e7")]
    [Title(nameof(Strings.PurchaseInvoices), nameof(Strings.Lines))]
    [Guide("The **Purchase Invoices - Lines** screen displays all line items from every purchase invoice in your business. This comprehensive view allows you to analyze purchases at the line item level rather than by invoice totals.")]
    [Guide("This screen is particularly useful for:")]
    [Guide("• Analyzing spending patterns across all your purchases")]
    [Guide("• Searching for specific items or accounts across multiple invoices")]
    [Guide("• Generating detailed purchase reports at the line item level")]
    [Guide("• Tracking purchases by project, division, or other dimensions")]
    [Header("How to Access")]
    [Guide("To access this screen, navigate to the **Purchase Invoices** tab.")]
    [TabScreenshot("fa-file-invoice", nameof(Strings.PurchaseInvoices))]
    [Guide("Then click the **Purchase Invoices - Lines** button at the bottom of the screen.")]
    [SmallBottomButtonScreenshot("PurchaseInvoices-Lines")]
    [Header("Understanding the Information")]
    [Guide("Each row in this report represents a single line item from a purchase invoice. The report includes key information such as:")]
    [Guide("• *Issue date* and *due date* of the invoice")]
    [Guide("• *Supplier* name and invoice *reference*")]
    [Guide("• *Item* or *account* charged on each line")]
    [Guide("• *Quantity*, *unit price*, and *total amount*")]
    [Guide("• *Tax codes* and *tax amounts* applied")]
    [Guide("• *Project* and *division* allocations")]
    internal sealed class PurchaseInvoiceLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.PurchaseInvoice.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<PurchaseInvoice>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.PurchaseInvoice != null && x.TransactionLine != null && !x.IsCostOfGoodsSold).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PurchaseInvoiceForm() { Business = Business, Key = x.PurchaseInvoice.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PurchaseInvoiceView() { Business = Business, Key = x.PurchaseInvoice.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("eaefe7e1-815c-403c-a9d7-87e0669b4335")]
        public DateTime[] GetIssueDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.PurchaseInvoice.IssueDate).ToArray();
        }

        [Guid("369cf966-cdc6-4a8f-aa5d-6a092706dc5f")]
        public DateTime[] GetDueDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.PurchaseInvoice.GetDueDate()).ToArray();
        }

        [Default]
        [PaddedSorting]
        [Guid("3a9e23a9-5f9d-4fe4-af80-adacbbbd01ff")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.PurchaseInvoice.Reference).ToArray();
        }

        [Default]
        [Guid("19f8fdb6-5a29-41fb-8599-59ff55c66dc5")]
        public string[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.GetCodeAndName()).ToArray();
        }

        [Guid("9eb601a6-f094-4cf4-907a-11ad3c91a7dd")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.PurchaseInvoice.Description).ToArray();
        }

        [Guid("57f33159-8720-4fd8-80a9-eae69f8d27f2")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Default]
        [Guid("59caa54a-347b-4fbb-bfdc-0d535dc5b425")]
        public string[] GetAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Account).ToArray();
        }

        [Guid("24dc2b03-7448-4cb1-901f-55f728992ee3")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("eb63ef13-a561-41c9-b5b1-b5e26e5ee1b2")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.Qty).ToArray();
        }

        [Guid("15c9103a-99e9-46ae-82e2-21d67015a4e4")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetUnitPrice(x.Transaction).HasValue ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Right, Sum]
        [Guid("32f3aee5-b414-4ff4-a72d-f61f575395bc")]
        public Tuple<decimal, Currency>[] GetDiscount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetDiscountAmountWithCurrency()).ToArray();
        }

        [Guid("9ef32675-9e42-4c81-97ff-b289d10415d6")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("3b0c2877-b469-40dc-ad71-c7b947ac0253")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("a12441e7-cb5a-4a26-b21b-e5e0ef15d71e")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Guid("5885121d-e067-4da8-98a0-ba6c57cd899e")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("81270c94-bc81-4ed8-8c69-4be3e6a3390c")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => new Tuple<decimal, Currency>(x.TransactionAmount, x.TransactionCurrency)).ToArray();
        }
    }
}
