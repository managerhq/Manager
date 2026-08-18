using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.JournalEntries
{
    [ProtoContract]
    [Guid("7d8cdb37-7920-4244-824c-649ee042f444")]
    [Title(nameof(Strings.JournalEntries), nameof(Strings.Lines))]
    [Guide("The **Journal Entries - Lines** screen displays all individual lines from journal entries across your business. This view is useful for reviewing, filtering, and analyzing specific journal entry lines without opening each complete entry.")]
    [Guide("To access this screen, navigate to the **Journal Entries** tab.")]
    [TabScreenshot(icon: "fa-balance-scale", name: nameof(Strings.JournalEntries))]
    [Guide("Then click the **Journal Entries - Lines** button at the bottom of the screen.")]
    [SmallBottomButtonScreenshot(name: nameof(Strings.JournalEntries)+"-"+nameof(Strings.Lines))]
    [Guide("The screen displays journal entry lines in a table format with multiple columns showing key information from each line.")]
    [Columns]
    [Guide("Click **Edit Columns** to customize which columns appear in your view.")]
    [SmallBottomButtonScreenshot(name: nameof(Strings.EditColumns))]
    internal sealed class JournalEntryLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.JournalEntry.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<JournalEntry>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null && !x.IsCostOfGoodsSold).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new JournalEntryForm() { Business = Business, Key = x.JournalEntry.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new JournalEntryView() { Business = Business, Key = x.JournalEntry.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("5ff4bb15-570d-4873-99c3-4196d5b122f0")]
        [Guide("The date when the journal entry was recorded. This date determines which *accounting period* the entry affects for financial reporting.")]
        [Guide("Use the date when the economic event occurred, not the date you enter it into the system. This ensures accurate period-based financial reporting.")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.JournalEntry.Date).ToArray();
        }

        [PaddedSorting]
        [Guid("a3cad68d-c4f0-4382-9477-c5448e3a7234")]
        [Guide("A unique *reference number* or code that identifies this journal entry. References help you quickly locate specific entries.")]
        [Guide("Use meaningful references such as 'ADJ-2024-001' or brief descriptions. Clear references make entries easier to find and understand later.")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.JournalEntry.Reference).ToArray();
        }

        [Guid("000b2402-652a-4906-a4fc-ebc9faf6e28f")]
        [Guide("A detailed explanation of the journal entry's purpose. The *narration* describes what business transaction or adjustment this entry represents.")]
        [Guide("Include key details such as the transaction type, reason for adjustment, supporting document references, or relevant context. For example: 'To record monthly depreciation expense for office equipment - March 2024'.")]
        public string[] GetNarration(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.JournalEntry.Narration).ToArray();
        }

        [Default]
        [Guid("23d64bae-1be9-4b29-82f8-2917e85edeba")]
        [Guide("The *general ledger account* affected by this line of the journal entry. Each line debits or credits a specific account.")]
        [Guide("Select the appropriate account from your *chart of accounts*. Remember that every journal entry must balance - total debits must equal total credits.")]
        public string[] GetAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Account).ToArray();
        }

        [Guid("f660a30c-7b98-4587-a02c-cfcdf1520286")]
        [Guide("A description for this specific line item. This explains what this particular debit or credit represents within the journal entry.")]
        [Guide("Add details about this line's purpose, such as 'Q1 depreciation expense' or 'Inventory count adjustment'. Line descriptions complement the overall *narration*.")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Guid("92a1de37-ec99-468f-8126-d98b4d7bcd12")]
        [Guide("The quantity of units affected by this journal entry line. This field is used when adjusting inventory quantities or other countable items.")]
        [Guide("Only enter quantities when the adjustment involves countable items like inventory. This maintains accurate quantity records alongside financial values.")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value*-1m : default(decimal?)).ToArray();
        }

        [Guid("e302c8f3-6bb9-492e-a8f6-c0d7a014e0cc")]
        [Guide("The project to which this journal entry line is allocated. Use this field to track adjustments by project.")]
        [Guide("Assign lines to projects when making project-related adjustments or accruals. This ensures project profitability reports include all relevant entries.")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("2359d47d-ca80-4caa-a519-423b9918269d")]
        [Guide("The division or department to which this journal entry line applies. Use this to track adjustments by organizational unit.")]
        [Guide("Assign lines to divisions when making department-specific adjustments. This ensures divisional reports include all relevant journal entries.")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("86977e1a-9780-4d1d-8d1f-c82abdcb7d6f")]
        [Guide("The *tax code* applied to this journal entry line. Use this field when making tax-related adjustments.")]
        [Guide("Select the appropriate tax code for tax adjustments or corrections. The tax code determines how this line affects *tax reports* and calculations.")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Guid("6fcb4d46-9cb9-4ff9-945f-992f7de9a461")]
        [Guide("The *tax amount* component of this journal entry line. Use this when recording tax adjustments or corrections.")]
        [Guide("Enter tax amounts when correcting tax calculations or making tax-specific adjustments. This ensures accurate *tax reporting* and liability tracking.")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value * -1m, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("274dbcaf-ad5d-4f8f-9362-717b34708400")]
        [Guide("The debit amount for this journal entry line. In *double-entry accounting*, debits are recorded on the left side.")]
        [Guide("Debits increase *asset* and *expense* accounts, and decrease *liability*, *equity*, and *income* accounts. The total of all debits must equal the total of all credits.")]
        public Tuple<decimal, Currency>[] GetDebit(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.Debit.HasValue ? new Tuple<decimal, Currency>(x.Debit.Value, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("54356aa9-1256-4382-906a-86de06a049d2")]
        [Guide("The credit amount for this journal entry line. In *double-entry accounting*, credits are recorded on the right side.")]
        [Guide("Credits increase *liability*, *equity*, and *income* accounts, and decrease *asset* and *expense* accounts. The total of all credits must equal the total of all debits.")]
        public Tuple<decimal, Currency>[] GetCredit(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.Credit.HasValue ? new Tuple<decimal, Currency>(x.Credit.Value, x.TransactionCurrency) : null).ToArray();
        }
    }
}