using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseQuotes
{
    [ProtoContract]
    [Guid("E3CC6497-0817-42E5-8656-787206BE386A")]
    [Title(nameof(Strings.PurchaseQuote), nameof(Strings.Lines))]
    [Guide("The **Purchase Quote Lines** report displays all individual line items from your purchase quotes in a consolidated view.")]
    [Guide("This report helps you analyze quoted items across all purchase quotes, making it easier to track what has been requested from suppliers and compare pricing across different quotes.")]
    [Guide("Each row in the table represents a single line item from a purchase quote, showing the item details, quantities, prices, and associated information.")]
    [Guide("Use this report to review all quoted items at once, identify frequently quoted products, or analyze pricing trends from your suppliers.")]
    [Columns]
    internal sealed class PurchaseQuoteLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.PurchaseQuote.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<PurchaseQuote>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PurchaseQuoteForm() { Business = Business, Key = x.PurchaseQuote.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PurchaseQuoteView() { Business = Business, Key = x.PurchaseQuote.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WhitespaceNoWrap]
        [WarnIfFutureDate, Center, MinWidth]
        [Guid("378A689C-CADB-4F32-A0FC-791FDF171D30")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("4B83A538-9D41-436F-9EE4-C7E9AD6DB788")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.PurchaseQuote.Reference).ToArray();
        }

        [Default]
        [Guid("5D0788E0-0E28-4513-8669-D0C159E737C6")]
        public string[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.Name).ToArray();
        }

        [Guid("B61A63B1-2A9C-4192-8D1B-D6B760FE1F25")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.PurchaseQuote.Description).ToArray();
        }

        [Guid("B01297E2-B1FE-45FA-B992-CEAC6EE82C72")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Guid("68697B08-6A14-4C8D-AE4C-E8B4860DB131")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Guid("774D0222-9589-4CC3-B0B1-AE71F5E54AF3")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value : default(decimal?)).ToArray();
        }

        [Default]
        [Guid("7EFA3604-6043-4C4D-9679-26E52A65197F")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetUnitPrice(x.Transaction).HasValue ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Guid("3B3F872E-D7EF-4907-9EFC-9F236F2F89EB")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("54C5223B-0180-466A-BB97-8722177A7908")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("B0EE8023-CA40-4508-A090-17A6A5757F59")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Guid("C25B82B5-C739-4126-8BC4-4C8BC8AE0E18")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Center]
        [Guid("31D0D2F2-37A4-4E5B-9A34-30BFF9684401")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => new Tuple<decimal, Currency>(x.TransactionAmount, x.TransactionCurrency)).ToArray();
        }
    }
}
