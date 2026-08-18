using System.Linq;
using System.Collections.Generic;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.JournalEntries
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("50902999-c655-4957-9004-2aa112de4738")]
    [Title(nameof(Strings.JournalEntries))]
    [Guide("The **Journal Entries** tab is designed for recording all accounting adjustments that do not fit into other tabs.")]
    [TabScreenshot("fa-balance-scale", nameof(Strings.JournalEntries))]
    [Guide("To add a new journal entry, click the **New Journal Entry** button.")]
    [HeroButtonScreenshot(nameof(Strings.JournalEntries), nameof(Strings.NewJournalEntry))]
    [LinkGuide("For more information, see:", typeof(JournalEntryForm))]
    [Header("Understanding the Columns")]
    [Guide("The **Journal Entries** tab includes several columns that display important information about your journal entries.")]
    [Columns]
    [Guide("To customize the visibility of columns, use the **Edit Columns** button.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("Learn more about customizing columns:", typeof(NakedObjectsWithEditColumns<JournalEntry>))]
    internal sealed class JournalEntries : NakedObjectsWithAutomaticRows<ManagerServer.Model.JournalEntry>
    {
        [Center]
        [Default]
        [MinWidth]
        [WarnIfFutureDate]
        [WhitespaceNoWrap]
        [Guid("ed23a0f2-7c27-47bd-a237-dcb69c6f36a0")]
        [Guide("The *Date* column displays the date when the journal entry was made.")]
        public DateTime[] GetDate(ManagerServer.Model.JournalEntry[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Center]
        [MinWidth]
        [PaddedSorting]
        [WarnIfNotUnique]
        [Guid("c5667daf-72b1-4e6e-ae6c-8513306acdb1")]
        [Guide("The *Reference* column displays the reference number for the journal entry.")]
        public string[] GetReference(ManagerServer.Model.JournalEntry[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("87a0afc6-fe9a-43ca-981e-c549aa2d15d6")]
        [Guide("The *Narration* column displays the description provided for the journal entry.")]
        public string[] GetNarration(ManagerServer.Model.JournalEntry[] rows)
        {
            return rows.Select(x => x.Narration).ToArray();
        }

        [Guid("65ce749f-036b-4045-9346-9d5b356fe716")]
        [Guide("The *Accounts* column displays a list of accounts, separated by commas, that are involved in the journal entry.")]
        public string[] GetAccounts(ManagerServer.Model.JournalEntry[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Select(x => x.Account).Distinct())).ToArray();
        }

        [Guid("a78ec9e5-e1f4-47c8-8d0b-7518ca06da3e")]
        public string[] GetProject(ManagerServer.Model.JournalEntry[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Where(x => x.Project != null).Select(x => x.Project.Name).Distinct())).ToArray();
        }

        [Sum]
        [Guid("db33e104-c1c8-476c-9dc0-d07d8a8ea5e4"), Default, Bold, Right, WhitespaceNoWrap]
        [Guide("The *Debit* column displays the sum of all debit amounts for the journal entry.")]
        public Tuple<decimal, ManagerServer.Model.Currency>[] GetDebit(ManagerServer.Model.JournalEntry[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();
            var values = new List<Tuple<decimal, ManagerServer.Model.Currency>>();
            foreach (var e in rows)
            {
                var generalLedgerTransactions = e.GetGeneralLedgerTransactions(database);
                var currency = database.SingleOrDefault<ManagerServer.Model.ForeignCurrency>(e.Currency) as ManagerServer.Model.Currency ?? baseCurrency;
                var debit = generalLedgerTransactions.Where(x => x.TransactionLine != null).Where(x => x.TransactionAmount > 0m).Sum(x => x.TransactionAmount);
                values.Add(new Tuple<decimal, ManagerServer.Model.Currency>(debit, currency));
            }
            return values.ToArray();
        }

        [Sum]
        [Guid("09c198eb-8c2a-4db4-b9d7-083434441a7e"), Default, Bold, Right, WhitespaceNoWrap]
        [Guide("The *Credit* column displays the sum of all credit amounts in the journal entry.")]
        public Tuple<decimal, ManagerServer.Model.Currency>[] GetCredit(ManagerServer.Model.JournalEntry[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();
            var values = new List<Tuple<decimal, ManagerServer.Model.Currency>>();
            foreach (var e in rows)
            {
                var generalLedgerTransactions = e.GetGeneralLedgerTransactions(database);
                var currency = database.SingleOrDefault<ManagerServer.Model.ForeignCurrency>(e.Currency) as ManagerServer.Model.Currency ?? baseCurrency;
                var credit = generalLedgerTransactions.Where(x => x.TransactionLine != null).Where(x => x.TransactionAmount < 0m).Sum(x => x.TransactionAmount) * -1m;
                values.Add(new Tuple<decimal, ManagerServer.Model.Currency>(credit, currency));
            }
            return values.ToArray();
        }

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("8e3e2f5c-630c-4172-86f7-b57b070f375e"), Default]
        [Guide("The *Status* column shows whether a journal entry is *Balanced* or *Unbalanced*.")]
        [Header("Understanding Entry Status")]
        [Guide("A *Balanced* entry occurs when the totals of the *Debit* and *Credit* columns are equal.")]
        [Guide("If an entry is *Unbalanced*, Manager automatically transfers the discrepancy to the *Suspense* account on the **Balance Sheet** report, ensuring that your financial statements remain balanced.")]
        [Header("Fixing Unbalanced Entries")]
        [Guide("To eliminate the *Suspense* account balance, ensure that all your journal entries are *Balanced*.")]
        public Status[] GetStatus(ManagerServer.Model.JournalEntry[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var values = new List<Status>();
            foreach (var e in rows)
            {
                var generalLedgerTransactions = e.GetGeneralLedgerTransactions(database);
                var balance = generalLedgerTransactions.Where(x => x.TransactionLine != null).Sum(x => x.TransactionAmount);
                if (balance == 0) values.Add(Status.Balanced);
                else values.Add(Status.Unbalanced);
            }
            return values.ToArray();
        }

        public enum Status
        {
            [Success] Balanced,
            [Danger] Unbalanced
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: new JournalEntryLines() { Business = Business }.ToUrl(), @class: "btn btn-xs")) Write(Strings.JournalEntries + " - " + Strings.Lines);
            base.OnFooterEndSection(context);
        }
    }
}
