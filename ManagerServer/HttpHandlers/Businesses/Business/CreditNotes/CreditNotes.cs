using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.CreditNotes
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("7eeefeb5-3321-40b5-872b-879568e68f01")]
    [Title(nameof(Strings.CreditNotes))]
    [Guide("The `Credit Notes` tab manages credit notes issued to customers for returns, refunds, or invoice corrections.")]
    [Guide("Credit notes are essentially negative invoices that reduce the amount customers owe you.")]
    [Guide("Use credit notes when you need to provide a full or partial credit against an existing sales invoice, or to record a standalone credit.")]
    [TabScreenshot("fa-cut", nameof(Strings.CreditNotes))]
    [Header("Creating Credit Notes")]
    [Guide("To issue a credit note to a customer, click the `New Credit Note` button.")]
    [Guide("You can create credit notes linked to specific sales invoices or as standalone credits.")]
    [HeroButtonScreenshot(nameof(Strings.CreditNotes), nameof(Strings.NewCreditNote))]
    [LinkGuide("For detailed information on creating credit notes, see:", typeof(CreditNoteForm))]
    [Header("Managing Credit Notes")]
    [Guide("The `Credit Notes` tab displays all your credit notes in a table format with the following columns:")]
    [Columns]
    [Guide("Click `Edit Columns` to customize which columns appear in the table and their order.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("To learn more about customizing columns, see:", typeof(NakedObjectsWithEditColumns<CreditNotes>))]
    internal class CreditNotes : NakedObjectsWithAutomaticRows<CreditNote>
    {
        [ProtoMember(1)] public Guid? Customer;

        protected override CreditNote[] OnGetRows(CreditNote[] rows)
        {
            if (Customer.HasValue) rows = [.. rows.Where(x => x.Customer == Customer)];
            return rows;
        }

        [Center]
        [Default]
        [MinWidth]
        [WhitespaceNoWrap]
        [WarnIfFutureDate]
        [Guid("547643c1-a534-4190-a72e-411ca4f46237")]
        [Guide("The `Date` column shows when the credit note was issued.")]
        public DateTime[] GetDate(CreditNote[] rows)
        {
            return rows.Select(x => x.IssueDate).ToArray();
        }

        [PaddedSorting]
        [WarnIfNotUnique]
        [Guide("The `Reference` column displays the unique reference number for each credit note.")]
        [Guid("c6e180d1-8dd5-4d57-befa-65a677caee5c")]
        public string[] GetReference(CreditNote[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("7a044000-b5f0-4ef6-a6e0-f3cebe224f3e")]
        [Guide("The `Customer` column shows who received this credit note.")]
        public string[] GetCustomer(CreditNote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return [.. rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.GetCodeAndName())];
        }

        [Guid("5dc597eb-88cb-4ef2-9244-1c968e812d9e")]
        [Guide("The `Sales Invoice` column shows the original invoice being credited, if applicable.")]
        [Guide("This column will be empty for standalone credit notes that are not linked to a specific invoice.")]
        public string[] GetSalesInvoice(CreditNote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return [.. rows.Select(x => database.SingleOrDefault<SalesInvoice>(x.SalesInvoice)?.Reference)];
        }

        [Default]
        [Guid("4bd6aa24-6a22-46c2-ad12-5eed2f15ef48")]
        [Guide("The `Description` column shows the reason for issuing the credit note.")]
        public string[] GetDescription(CreditNote[] rows)
        {
            return [.. rows.Select(x => x.Description)];
        }

        [Default]
        [Right, Sum]
        [HideColumnIfAllEmpty]
        [Guid("5ce718a2-1e15-46a5-8e57-a65089894543")]
        [Guide("The `Cost of Sales` column shows the inventory cost reversed when items are returned.")]
        [Guide("This column only appears when inventory items are included in the credit note.")]
        [Guide("Click the amount to view detailed cost calculations.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetCostOfSales(CreditNote[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return [.. rows.Select(x => x.CostOfSales(database).HasValue ? new Tuple<decimal, Currency, BusinessTemplate>(x.CostOfSales(database).Value * -1m, baseCurrency, new CreditNoteCosts() { Business = Business, Transaction = x.Key, Referrer = referrer }) : null)];
        }

        [Sum]
        [Right]
        [Bold]
        [Default]
        [Guid("dfabc6ee-a5bb-41eb-859d-9d142f0ed13a")]
        [Guide("The `Amount` column displays the total credit amount issued to the customer.")]
        public Tuple<decimal, Currency>[] GetAmount(CreditNote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetReversedTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: new CreditNoteLines() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.CreditNotes + " - " + Strings.Lines);
            base.OnFooterEndSection(context);
        }
    }
}
