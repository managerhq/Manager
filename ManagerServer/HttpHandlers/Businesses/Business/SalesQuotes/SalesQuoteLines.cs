using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesQuotes
{
    [ProtoContract]
    [Guid("df668540-84d8-42df-8ab6-4caf18aec126")]
    [Title(nameof(Strings.SalesQuote), nameof(Strings.Lines))]
    [Guide("The `Sales Quote Lines` report displays all line items from your sales quotes in a detailed list format.")]
    [Guide("This report helps you analyze quoted items, quantities, prices, and totals across all quotes to track sales opportunities and pricing patterns.")]
    [Columns]
    internal sealed class SalesQuoteLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.SalesQuote.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<SalesQuote>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null && !x.IsCostOfGoodsSold).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesQuoteForm() { Business = Business, Key = x.SalesQuote.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesQuoteView() { Business = Business, Key = x.SalesQuote.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("1dc3d0ad-2a65-4276-a640-89ec953bc82a")]
        [Guide("The date when the sales quote was issued to the customer.")]
        [Guide("Use this date to track quote validity periods and monitor conversion times from quote to order. Most quotes expire after a set period and may require price updates.")]
        public DateTime[] GetIssueDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SalesQuote.IssueDate).ToArray();
        }

        [Default]
        [PaddedSorting]
        [Guid("fe0ba4a4-5c16-425a-a24e-83ca387ec3a0")]
        [Guide("The unique reference number for this sales quote.")]
        [Guide("Use a consistent numbering system like `QUO-2024-001` to track quotes easily. Reference numbers are essential for customer communications and converting quotes to orders.")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SalesQuote.Reference).ToArray();
        }

        [Default]
        [Guid("be6a4779-0bcd-4bf0-b1cb-b272f6a30580")]
        [Guide("The customer or prospect receiving this quote.")]
        [Guide("Select the correct customer to apply their specific pricing and terms. For new prospects, create a customer record first to maintain a complete quote history.")]
        public string[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.Name).ToArray();
        }

        [Guid("36a7d2f8-43ce-4688-8345-789889d68417")]
        [Guide("A summary of what this quote covers.")]
        [Guide("Include project names or service descriptions like `Annual maintenance contract` or `Office renovation - Phase 1`. This description appears on the quote document sent to customers.")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SalesQuote.Description).ToArray();
        }

        [Default]
        [Guid("4a756319-d9a4-4c3c-a9f4-1620a5d07741")]
        [Guide("The product or service being quoted on this line.")]
        [Guide("Select from your `Inventory Items` or `Non-inventory Items` list. Choosing an item automatically fills in its description, price, and tax code. For custom items, leave this blank and use the line description field.")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Guid("148bd0fe-d61c-4073-8d07-c5820df263dd")]
        [Guide("A detailed description for this line item.")]
        [Guide("Include model numbers, specifications, or service details. Clear descriptions help customers understand exactly what they are being quoted and reduce follow-up questions.")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Guid("3e25e522-857c-4cda-a9c0-7fc0b8b41cfc")]
        [Guide("The quantity of units being quoted.")]
        [Guide("Enter the quantity based on customer requirements. This multiplies with the unit price to calculate the line total. Ensure the unit of measure matches your item settings.")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value*-1m : default(decimal?)).ToArray();
        }

        [Guid("11b09590-9ff1-42dc-a891-962b21cffbc4")]
        [Guide("The price per unit for this item.")]
        [Guide("You can override the default item price for special quotes. Consider costs, market rates, and volume discounts when setting prices for different customers and quantities.")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetUnitPrice(x.Transaction).HasValue ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Guid("2c6d5a60-5c26-4633-9d15-b89f37557f90")]
        [Guide("The project this line item relates to.")]
        [Guide("Assign line items to projects to analyze project feasibility and profitability before work begins. This helps with project-based pricing and resource planning.")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("e2fbda27-32c9-40da-b3f8-82148bc92023")]
        [Guide("The division or department responsible for this line item.")]
        [Guide("Use divisions to track quote activity by business segment. This helps analyze which divisions generate the most opportunities and their conversion rates.")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("0d4e3016-cd40-4dc1-a616-450be4d612f4")]
        [Guide("The tax code applied to this line item.")]
        [Guide("Select the appropriate tax code based on the item type and customer tax status. This ensures the quote displays accurate tax amounts and total prices.")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Guid("b747e321-347d-4696-900c-456937fe5272")]
        [Guide("The calculated tax amount for this line item.")]
        [Guide("Automatically calculated as (quantity × unit price) × tax rate. This shows customers the tax component of their quoted price separately from the base amount.")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value * -1m, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("0cc897f3-9631-48ce-b22d-baefdbb080b1")]
        [Guide("The total amount for this line item including tax.")]
        [Guide("Calculated as (quantity × unit price) + tax amount. This is the final amount the customer would pay for this line item if they accept the quote.")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => new Tuple<decimal, Currency>(x.TransactionAmount*-1m, x.TransactionCurrency)).ToArray();
        }
    }
}