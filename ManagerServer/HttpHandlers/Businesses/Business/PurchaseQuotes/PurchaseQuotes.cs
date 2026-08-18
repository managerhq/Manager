using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseQuotes
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("45b41778-691a-4d17-876b-0f1a77d3ceac")]
    [Title(nameof(Strings.PurchaseQuotes))]
    [Guide("The **Purchase Quotes** tab enables you to request and track quotes from various suppliers before deciding to make a purchase. It keeps all your *purchase quotes* organized in one place, making your procurement management more efficient and effective.")]
    [TabScreenshot("fa-drafting-compass", nameof(Strings.PurchaseQuotes))]
    [Guide("To create a new purchase quote, click the **New Purchase Quote** button.")]
    [HeroButtonScreenshot(nameof(Strings.PurchaseQuotes), nameof(Strings.NewPurchaseQuote))]
    [Guide("The **Purchase Quotes** tab displays information in several columns:")]
    [Columns]
    internal sealed class PurchaseQuotes : NakedObjectsWithAutomaticRows<PurchaseQuote>
    {
        [ProtoMember(1)] public Guid? Supplier;

        protected override PurchaseQuote[] OnGetRows(PurchaseQuote[] rows)
        {
            if (Supplier.HasValue) rows = rows.Where(x => x.Supplier == Supplier).ToArray();
            return rows;
        }

        [Default]
        [WarnIfFutureDate]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("eeb2c91e-2a2b-4a7f-82d1-40c220d3ea00")]
        [Guide("The date when the purchase quote was issued by the supplier.")]
        public DateTime[] GetDate(PurchaseQuote[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [PaddedSorting]
        [Guid("62920d60-5706-4cf8-b506-edfabf3bf4d4")]
        [Guide("The unique reference number assigned to identify this purchase quote.")]
        public string[] GetReference(PurchaseQuote[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("76e5c15a-86c4-4e29-af80-8003910882f3")]
        [Guide("The name of the supplier who provided this purchase quote.")]
        public string[] GetSupplier(PurchaseQuote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Supplier>(x.Supplier)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("734a2710-8610-49cd-b53a-b01b599163d3")]
        [Guide("A brief description or summary of what this purchase quote contains.")]
        public string[] GetDescription(PurchaseQuote[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("922b5938-a9f2-486f-bb45-5a368fcb5a13")]
        [Guide("The total amount of the purchase quote, including all items and any applicable taxes.")]
        public Tuple<decimal, Currency>[] GetAmount(PurchaseQuote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.RequestForQuotation ? null : x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetReversedTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }

        [Default]
        [MinWidth, Center]
        [Guid("24bbc73a-45e2-4d89-a6ba-7a55196cc9f4")]
        [Guide("The current status of the purchase quote. Possible values are **Active** (still under consideration), **Accepted** (converted to a purchase order or invoice), or **Cancelled** (no longer valid).")]
        public Status[] GetStatus(PurchaseQuote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var purchaseOrders = new HashSet<Guid>(database.OfType<PurchaseOrder>().Where(x => x.Supplier.HasValue && x.PurchaseQuote.HasValue).Select(x => x.PurchaseQuote.Value));
            var purchaseInvoices = new HashSet<Guid>(database.OfType<PurchaseInvoice>().Where(x => x.Supplier.HasValue && x.PurchaseQuote.HasValue).Select(x => x.PurchaseQuote.Value));
            var output = new List<Status>();
            foreach (var e in rows)
            {
                var status = Status.Active;
                if (e.Cancelled)
                {
                    status = Status.Cancelled;
                }
                else if (purchaseOrders.Contains(e.Key) || purchaseInvoices.Contains(e.Key))
                {
                    status = Status.Accepted;
                }                
                output.Add(status);
            }
            return output.ToArray();
        }

        public enum Status
        {
            [Success] Active,
            [Primary] Accepted,
            Cancelled
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: new PurchaseQuoteLines() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.PurchaseQuotes + " - " + Strings.Lines);
            base.OnFooterEndSection(context);
        }
    }
}
