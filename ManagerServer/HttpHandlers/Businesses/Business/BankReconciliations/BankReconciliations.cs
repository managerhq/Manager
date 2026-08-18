using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.BankReconciliations
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("7793a4e0-0945-437f-9c67-b25a5398d4cc")]
    [Title(nameof(Strings.BankReconciliations))]
    [Guide("The **Bank Reconciliations** tab helps you verify that your bank account records in Manager match your actual bank statements.")]
    [Guide("Regular reconciliations ensure accuracy and help identify missing transactions, errors, or fraudulent activity.")]
    [TabScreenshot("fa-clipboard-check", nameof(Strings.BankReconciliations))]
    [Header("Creating Bank Reconciliations")]
    [Guide("To start a new reconciliation, click the **New Bank Reconciliation** button.")]
    [HeroButtonScreenshot(nameof(Strings.BankReconciliations), nameof(Strings.NewBankReconciliation))]
    [LinkGuide("Learn about the reconciliation process:", typeof(BankReconciliationForm))]
    [Header("Understanding the Columns")]
    [Guide("The **Bank Reconciliations** tab displays the following information:")]
    [Columns]
    [Guide("Click **Edit Columns** to customize which columns are visible.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("Learn about column customization:", typeof(NakedObjectsWithEditColumns<BankReconciliations>))]
    internal sealed class BankReconciliations : NakedObjectsWithAutomaticRows<BankReconciliation>
    {
        [Default]
        [WarnIfFutureDate]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("311286db-dbe8-436e-8777-a9869731b387")]
        [Guide("The **Date** column shows when the bank reconciliation was performed.")]
        [Guide("This should match the statement date on your bank statement.")]
        public DateTime[] GetDate(BankReconciliation[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("1c4e76de-b978-46e5-bfb6-c66bbd8c2018")]
        [Guide("The **Bank Account** column shows which bank account is being reconciled.")]
        public string[] GetBankAccount(BankReconciliation[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.BankAccount)?.Name).ToArray();
        }

        [Default]
        [Bold, Right]
        [Guid("a6df8c80-ab7e-41be-993c-7cdd75dbc9ea")]
        [Guide("The **Statement Balance** column shows the closing balance from your bank statement.")]
        [Guide("This is the balance you enter when creating the reconciliation.")]
        public Tuple<decimal, Currency>[] GetStatementBalance(BankReconciliation[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var bankAccountCurrencies = database.OfType<BankOrCashAccount>().ToDictionary(x => x.Key, x => database.SingleOrDefault<ForeignCurrency>(x.Currency) as Currency ?? baseCurrency);
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var currency = e.BankAccount.HasValue && bankAccountCurrencies.TryGetValue(e.BankAccount.Value, out Currency value) ? value : baseCurrency;

                output.Add(new Tuple<decimal, Currency>(e.StatementBalance, currency));
            }
            return output.ToArray();
        }

        [Default]
        [Right]
        [Guid("306c57b8-c922-4a88-a52f-9a350d087269")]
        [Guide("The **Discrepancy** column shows the difference between your *statement balance* and the *calculated balance* from cleared transactions.")]
        [Guide("A zero discrepancy means your records match the bank statement perfectly.")]
        [Guide("Click on a non-zero discrepancy to see which transactions are causing the difference.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetDiscrepancy(BankReconciliation[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var bankAccountCurrencies = database.OfType<BankOrCashAccount>().ToDictionary(x => x.Key, x => database.SingleOrDefault<ForeignCurrency>(x.Currency) as Currency ?? baseCurrency);
            var discrepancies = GetDisprepancies(rows);
            var output = new List<Tuple<decimal, Currency, BusinessTemplate>>();
            foreach (var e in rows)
            {
                var discrepancy = discrepancies[e];
                var currency = e.BankAccount.HasValue && bankAccountCurrencies.TryGetValue(e.BankAccount.Value, out Currency value) ? value : baseCurrency;
                output.Add(e.BankAccount.HasValue ? new Tuple<decimal, Currency, BusinessTemplate>(discrepancy, currency, new BankReconciliationTransactions() { Business = Business, BankAccount = e.BankAccount.Value, Date = e.Date, Referrer = referrer }) : null);
            }

            return output.ToArray();
        }

        [Default]
        [MinWidth, Center, WhitespaceNoWrap]
        [Guid("6435b673-ac74-4228-8031-6dd600398105")]
        [Guide("The **Status** column indicates whether the bank account is reconciled:")]
        [Guide("• **Reconciled** - No discrepancy exists (perfect match)")]
        [Guide("• **Not Reconciled** - A discrepancy needs investigation")]
        public Status[] GetStatus(BankReconciliation[] rows)
        {
            var discrepancies = GetDisprepancies(rows);
            return rows.Select(x => discrepancies[x] == 0m ? Status.Reconciled : Status.NotReconciled).ToArray();
        }

        public enum Status
        {
            [Success] Reconciled,
            [Danger] NotReconciled
        }

        private Dictionary<BankReconciliation, decimal> disprepancies;
        public Dictionary<BankReconciliation, decimal> GetDisprepancies(BankReconciliation[] rows)
        {
            if (disprepancies == null)
            {
                var database = ApplicationData.Businesses.Get(Business);
                var bankTransactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount != null && x.ClearDate.HasValue).GroupBy(x => x.BankAccount.Key).ToDictionary(x => x.Key, x => x.ToArray());
                var output = new Dictionary<BankReconciliation, decimal>();
                foreach (var e in rows)
                {
                    var clearedBalance = 0m;
                    if (e.BankAccount.HasValue)
                    {
                        if (bankTransactions.ContainsKey(e.BankAccount.Value))
                        {
                            clearedBalance = bankTransactions[e.BankAccount.Value].Where(x => x.ClearDate <= e.Date).Sum(x => x.AccountAmount);
                        }
                    }

                    var discrepancy = e.StatementBalance - clearedBalance;
                    if (discrepancy < 0m) discrepancy *= -1m;

                    output.Add(e, discrepancy);
                }
                disprepancies = output;
            }
            return disprepancies;
        }
    }
}
