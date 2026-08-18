using ManagerServer.Model;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    [Guid("733f5326-3bf6-4654-a09e-8d3854522e41")]
    [Title(nameof(Strings.SalesInvoices), nameof(Strings.Lines))]
    [Guide("This screen displays a list of sales invoice lines from all sales invoices. It's useful for summarizing, filtering, or quickly finding specific invoices based on their line items.")]
    [Guide("To reach the Sales Invoices - Lines screen, navigate to the **Sales Invoices** tab.")]
    [TabScreenshot("fa-file-invoice", nameof(Strings.SalesInvoices))]
    [Guide("Then, click the **Sales Invoices - Lines** button.")]
    [SmallBottomButtonScreenshot("SalesInvoices-Lines")]
    [Guide("The Sales Invoices - Lines screen displays all line items from your sales invoices in a detailed table format.")]
    [Columns]
    [Guide("Click the **Edit Columns** button to customize which columns are displayed.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("Learn more about customizing columns:", typeof(NakedObjectsWithEditColumns<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>))]
    [Guide("Use **Advanced Queries** to filter and analyze your data in powerful ways.")]
    [Guide("For example, you can analyze quantities sold by customer and item by grouping the data appropriately:")]
    [AdvancedQuery(select: new[] { nameof(Strings.Item), nameof(Strings.Customer), nameof(Strings.Qty), nameof(Strings.Amount) }, where: new[] { nameof(Strings.Item), nameof(Strings.IsNot), nameof(Strings.Empty) }, orderBy: new[] { nameof(Strings.Item), nameof(Strings.Ascending) }, groupBy: new[] { nameof(Strings.Item), nameof(Strings.Customer) })]
    internal sealed class SalesInvoiceLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.SalesInvoice.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<SalesInvoice>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null && !x.IsCostOfGoodsSold).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesInvoiceForm() { Business = Business, Key = x.SalesInvoice.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesInvoiceView() { Business = Business, Key = x.SalesInvoice.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WhitespaceNoWrap]
        [WarnIfFutureDate, Center, MinWidth]
        [Guid("ef3d6941-ff42-4547-bd97-f675d2359ce3")]
        [Guide("The **Issue Date** column shows when the invoice was issued.")]
        public DateTime[] GetIssueDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SalesInvoice.IssueDate).ToArray();
        }

        [Guid("c63075f2-040a-4c74-a6bf-cc4e27ba1a48")]
        [Guide("The **Due Date** column shows when payment is due for the invoice.")]
        public DateTime[] GetDueDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SalesInvoice.GetDueDate()).ToArray();
        }

        [Default]
        [PaddedSorting]
        [Guid("26e5fb83-f85c-4988-a20a-c7187008e511")]
        [Guide("The **Reference** column shows the unique reference number for each invoice.")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SalesInvoice.Reference).ToArray();
        }

        [Default]
        [Guid("46eab8be-d7da-4ca0-883f-ea0fc5bd5361")]
        [Guide("The **Customer** column shows the name of the customer for each line item.")]
        public string[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.GetCodeAndName()).ToArray();
        }

        [Guid("39a596a0-52cc-4888-948b-b384ad47a6e5")]
        [Guide("The **Description** column shows the overall description of the invoice.")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SalesInvoice.Description).ToArray();
        }

        [Guid("1e14e7f5-cc2c-4cf4-bc25-92980221fb83")]
        [Guide("The **Item** column shows the *inventory item* or *non-inventory item* for each line.")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Default]
        [Guid("e9385bff-e0c7-44f3-8adf-868bd922d492")]
        [Guide("The **Account** column shows the *income account* associated with each line item.")]
        public string[] GetAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Account).ToArray();
        }

        [Guid("1808d3fa-e963-4359-a298-972dc2bbb714")]
        [Guide("The **Line Description** column shows the specific description for each individual line item.")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("4bc7373c-c821-4d7b-a5f1-10e5c64804d9")]
        [Guide("The **Qty** column displays the quantity for each line item.")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value*-1m : default(decimal?)).ToArray();
        }

        [Right, Sum]
        [Guid("22eaf444-5c0d-443c-b229-490fee32248d")]
        [Guide("The **Unit Price** column displays the price per unit for each line item.")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetUnitPrice(x.Transaction).HasValue ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Guid("511c260e-d6db-4080-bed9-e4729fa976a8")]
        [Guide("The **Project** column shows the *project* associated with each line item.")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("4061aa7b-22e4-4d85-9704-35c5b5798686")]
        [Guide("The **Division** column shows the *division* associated with each line item.")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("fb8ab8a3-e0af-4df7-a5c2-b20107daf156")]
        [Guide("The **Tax Code** column shows the *tax code* applied to each line item.")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Right, Sum]
        [Guid("edb32c91-eaf1-4918-9acf-7b3e8e9023ae")]
        [Guide("The **Discount** column shows any discount amount applied to each line item.")]
        public Tuple<decimal, Currency>[] GetDiscount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetDiscountAmountWithCurrency()).ToArray();
        }

        [Right, Sum]
        [Guid("03b8a3a0-1d42-468f-8db1-83593b0f79bd")]
        [Guide("The **Tax Amount** column shows the calculated tax amount for each line item.")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value * -1m, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("5056fbb8-fa2a-485b-9020-e00c565c000e")]
        [Guide("The **Amount** column shows the total amount for each line item including any applicable taxes.")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => new Tuple<decimal, Currency>(x.TransactionAmount*-1m, x.TransactionCurrency)).ToArray();
        }
    }
}
