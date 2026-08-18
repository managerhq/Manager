using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.DebitNotes
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("03dcd586-7535-4f53-b70b-7da0f7f0f870")]
    [Title(nameof(Strings.DebitNotes))]
    [Guide("The `Debit Notes` tab is designed for creating and managing debit notes. These documents are issued by buyers to sellers to show that a specific amount has been deducted from the seller's account. They are often used in transactions involving returned goods.")]
    [TabScreenshot("fa-cut", nameof(Strings.DebitNotes))]
    [Guide("To create a new debit note, click the `New Debit Note` button.")]
    [HeroButtonScreenshot(nameof(Strings.DebitNotes), nameof(Strings.NewDebitNote))]
    [Guide("The `Debit Notes` tab features several columns:")]
    [Columns]
    internal class DebitNotes : NakedObjectsWithAutomaticRows<DebitNote>
    {
        [ProtoMember(1)] public Guid? Supplier;

        protected override DebitNote[] OnGetRows(DebitNote[] rows)
        {
            if (Supplier.HasValue) rows = rows.Where(x => x.Supplier == Supplier).ToArray();
            return rows;
        }

        [Default]
        [WarnIfFutureDate]
        [Center]
        [MinWidth]
        [WhitespaceNoWrap]
        [Guid("61294c82-3550-44aa-89fe-bd91e3df2518")]
        [Guide("The date when the debit note was issued to the supplier. This date is important for tracking when the deduction from the supplier's account was recorded.")]
        public DateTime[] GetDate(DebitNote[] rows)
        {
            return rows.Select(x => x.IssueDate).ToArray();
        }

        [PaddedSorting]
        [WarnIfNotUnique]
        [Guid("30a64830-627f-424b-9f98-9df85eca5b31")]
        [Guide("A unique reference number for this debit note. This helps identify and track the debit note in your records and when communicating with the supplier.")]
        public string[] GetReference(DebitNote[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("10173e4b-61e0-4e5b-a077-283be6d3b346")]
        [Guide("The supplier to whom the debit note was issued. This shows which supplier's account is being debited.")]
        public string[] GetSupplier(DebitNote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Supplier>(x.Supplier)?.GetCodeAndName()).ToArray();
        }

        [Guid("224ba963-d404-4c1c-89fa-dc5eebd17bc3")]
        [Guide("The reference number of the purchase invoice that this debit note relates to, if applicable. This links the debit note to the original purchase transaction.")]
        public string[] GetPurchaseInvoice(DebitNote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<PurchaseInvoice>(x.PurchaseInvoice)?.Reference).ToArray();
        }

        [Default]
        [Guid("faed3821-0772-497c-be0a-12adb4047472")]
        [Guide("A brief description explaining the reason for the debit note, such as returned goods, pricing adjustments, or quality issues.")]
        public string[] GetDescription(DebitNote[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Right]
        [Sum]
        [Default]
        [Guid("4ef717d0-be1f-4263-a20a-61188fa7e371")]
        [Guide("The total amount of the debit note. This represents the amount being deducted from the supplier's account.")]
        public Tuple<decimal, Currency>[] GetAmount(DebitNote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: new DebitNoteLines() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.DebitNotes + " - " + Strings.Lines);
            base.OnFooterEndSection(context);
        }
    }
}
