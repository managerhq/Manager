using System.Collections.Generic;
using System.Linq;
using ManagerServer;
using ManagerServer.Attributes;
using ManagerServer.Model;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.BankAndCashAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("efe0e273-fa0b-4824-a9d2-7225383d49de")]
    [Title(nameof(Strings.BankAndCashAccounts))]
    [Guide("The **Bank & Cash Accounts** tab is your central hub for managing all bank accounts, cash accounts, credit cards, and other financial accounts.")]
    [Header("Overview")]
    [TabScreenshot(icon: "fa-coins", name: "BankAndCashAccounts")]
    [Guide("From here you can monitor balances, import transactions, and track all money flowing in and out of your business.")]
    [Guide("If the **Bank & Cash Accounts** tab is not visible, you need to enable it in your tab settings.")]
    [LinkGuide("Learn how to enable tabs:", typeof(TabsForm))]
    [Header("Creating Bank and Cash Accounts")]
    [Guide("To add a new bank or cash account, click the **New Bank or Cash Account** button.")]
    [HeroButtonScreenshot(title: "BankAndCashAccounts", name: "NewBankOrCashAccount")]
    [LinkGuide("Learn more about account setup:", typeof(BankOrCashAccountForm))]
    [Header("Automatic Chart of Accounts Entries")]
    [Guide("When you create your first bank or cash account, Manager automatically adds two essential accounts to your **Chart of Accounts**:")]
    [Guide("• **Cash & Cash Equivalents** - A control account in the *Assets* section that shows the combined balance of all your bank and cash accounts.")]
    [Guide("• **Inter Account Transfers** - A special account in the *Equity* section used for transfers between your accounts. This ensures transfers are properly matched and do not affect your net position.")]
    [LinkGuide("Learn more about the chart of accounts:", typeof(Settings.ChartOfAccounts.ChartOfAccounts))]
    [Header("Setting Up Starting Balances")]
    [Guide("For existing bank accounts with current balances, enter starting balances through **Settings** → **Starting Balances**.")]
    [Guide("This ensures your Manager balances match your actual bank statements from day one.")]
    [LinkGuide("Learn how to set starting balances:", typeof(Settings.StartingBalances.BankAndCashAccounts.BankOrCashAccountStartingBalanceList))]
    [Header("Organizing Accounts with Control Accounts")]
    [Guide("By default, all bank and cash accounts are grouped under the **Cash & Cash Equivalents** control account.")]
    [Guide("This means your **Balance Sheet** shows one combined total rather than individual account balances.")]
    [Guide("You can organize accounts into logical groups by creating custom control accounts:")]
    [Guide("• Credit cards can be grouped under a *liability control account*.")]
    [Guide("• Term deposits can have their own *asset control account*.")]
    [Guide("• Bank loans can be separated as *liabilities*.")]
    [Guide("For maximum detail, create a control account for each bank account to show individual balances on financial statements.")]
    [LinkGuide("Learn about control accounts:", typeof(Settings.ControlAccounts.BankAndCashAccounts.BankAndCashAccountControlAccounts))]
    [Header("Importing and Syncing Transactions")]
    [Guide("Save time and reduce errors by importing bank statements instead of entering transactions manually.")]
    [Guide("Click the **Import Bank Statement** button to upload transaction files from your bank.")]
    [SmallBottomButtonScreenshot(name: "ImportBankStatement")]
    [LinkGuide("Learn about importing statements:", typeof(ImportBankStatement))]
    [Header("Customizing Display Columns")]
    [Guide("The **Bank & Cash Accounts** tab displays essential information about each account in customizable columns.")]
    [Columns]
    [Guide("Click **Edit Columns** to show or hide columns based on what information is most important for your business.")]
    [LinkGuide("Learn about customizing columns:", typeof(NakedObjectsWithEditColumns<>))]
    internal sealed class BankAndCashAccounts : NakedObjectsWithAutomaticRows<BankOrCashAccount>
    {
        protected override BankOrCashAccount[] OnGetRows(BankOrCashAccount[] rows)
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess)
            {
                var accounts = userPermissions.GetBankCashAccounts().ToList();
                var filter = true;
                if (accounts.Count == 0)
                {
                    filter = false;
                    foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.BankOrCashAccount>()) accounts.Add(e.Key);
                }
                if (filter) rows = rows.Where(x => accounts.Contains(x.Key)).ToArray();
            }

            return rows;
        }

        [WarnIfNotUnique]
        [Guid("bb413e74-5aed-4346-aa3a-123f729b143e")]
        [Guide("Shows the optional *Code* field for each bank or cash account.")]
        public string[] GetCode(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            return rows.Select(x => x.Code).ToArray();
        }

        [Default]
        [Guid("1276fd60-908a-489b-a7bd-c026987db9eb")]
        [Guide("Shows the *Name* field for each bank or cash account.")]
        public string[] GetName(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Guid("a1300b66-248d-4a38-80ab-a4c324ee76a8")]
        [Guide("Shows the control account where each bank or cash account appears on the **Balance Sheet**.")]
        [Guide("By default, bank and cash accounts are categorized under the **Cash & Cash Equivalents** account. You can create custom control accounts for more flexibility.")]
        [LinkGuide("Learn about control accounts:", typeof(Settings.ControlAccounts.BankAndCashAccounts.BankAndCashAccountControlAccounts))]
        public string[] GetControlAccount(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => (database.SingleOrDefault<ManagerServer.Model.ControlAccountForBankAccounts>(x.ControlAccount) as ManagerServer.Model.NamedObject ?? database.Single<ManagerServer.Model.BalanceSheetCashAtBankAccount>()).GetName()).ToArray();
        }

        [Guid("c41b69fa-dcdc-41a4-a7f5-17700e2f1a75")]
        [Guide("If you are using *Divisions*, this column displays the division assigned to each bank or cash account.")]
        [LinkGuide("Learn about divisions:", typeof(Settings.Divisions.Divisions))]
        public string[] GetDivision(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Division>(x.Division)?.Name).ToArray();
        }

        [Center, Sum]
        [Guid("5bc4536a-0504-47f6-8015-afde8a5456d3")]
        [Guide("The **Uncategorized Receipts** column displays the total number of *Receipts* linked to each bank account that have not been assigned a credit account.")]
        [Guide("This commonly occurs when importing bank statements. Click the displayed number to go to the **Uncategorized Receipts** page.")]
        [Guide("There you can categorize receipts in bulk by applying *Receipt Rules*.")]
        public Tuple<int, BusinessTemplate>[] GetUncategorizedReceipts(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var uncategorized = database.OfType<ManagerServer.Model.Receipt>().Where(x => x.ReceivedIn.HasValue && x.IsUncategorized()).GroupBy(x => x.ReceivedIn.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => uncategorized.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new Receipts.UncategorizedReceipts() { Business = Business, BankAccount = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center, Sum]
        [Guid("677f58de-7ec6-4ef2-ad43-420cf63fbb1f")]
        [Guide("The **Uncategorized Payments** column displays the count of *Payments* made through each bank account that lack an assigned debit account.")]
        [Guide("This typically occurs when importing bank statements. Click the number to go to the **Uncategorized Payments** screen.")]
        [Guide("There you can categorize payments in bulk using *Payment Rules*.")]
        public Tuple<int, BusinessTemplate>[] GetUncategorizedPayments(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var uncategorized = database.OfType<ManagerServer.Model.Payment>().Where(x => x.PaidFrom.HasValue && x.IsUncategorized()).GroupBy(x => x.PaidFrom.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => uncategorized.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new Payments.UncategorizedPayments() { Business = Business, BankAccount = x.Key, Referrer = referrer }) : null).ToArray();
        }

        private Dictionary<BankOrCashAccount, Balance> getBalances = null;
        public Dictionary<BankOrCashAccount, Balance> GetBalances(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            if (getBalances == null)
            {
                var referrer = this.ToUrl();
                var database = ApplicationData.Businesses.Get(Business);
                var baseCurrency = database.Single<BaseCurrency>();
                var output = new Dictionary<BankOrCashAccount, Balance>();

                var bankTransactionsByBankAccount = new Dictionary<Guid, List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>>();
                foreach (var e in database.OfType<Receipt>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount != null).GroupBy(x => x.BankAccount.Key))
                {
                    if (!bankTransactionsByBankAccount.ContainsKey(e.Key)) bankTransactionsByBankAccount.Add(e.Key, new());
                    bankTransactionsByBankAccount[e.Key].AddRange(e);
                }
                foreach (var e in database.OfType<Payment>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount != null).GroupBy(x => x.BankAccount.Key))
                {
                    if (!bankTransactionsByBankAccount.ContainsKey(e.Key)) bankTransactionsByBankAccount.Add(e.Key, new());
                    bankTransactionsByBankAccount[e.Key].AddRange(e);
                }
                foreach (var e in database.OfType<InterAccountTransfer>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount != null).GroupBy(x => x.BankAccount.Key))
                {
                    if (!bankTransactionsByBankAccount.ContainsKey(e.Key)) bankTransactionsByBankAccount.Add(e.Key, new());
                    bankTransactionsByBankAccount[e.Key].AddRange(e.AsEnumerable());
                }
                foreach (var e in database.OfType<JournalEntry>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount != null).GroupBy(x => x.BankAccount.Key))
                {
                    if (!bankTransactionsByBankAccount.ContainsKey(e.Key)) bankTransactionsByBankAccount.Add(e.Key, new());
                    bankTransactionsByBankAccount[e.Key].AddRange(e.AsEnumerable());
                }
                foreach (var e in database.OfType<BankOrCashAccountStartingBalance>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount != null).GroupBy(x => x.BankAccount.Key))
                {
                    if (!bankTransactionsByBankAccount.ContainsKey(e.Key)) bankTransactionsByBankAccount.Add(e.Key, new());
                    bankTransactionsByBankAccount[e.Key].AddRange(e.AsEnumerable());
                }

                foreach (var e in rows)
                {
                    var currency = database.SingleOrDefault<ForeignCurrency>(e.Currency) as ManagerServer.Model.Currency ?? baseCurrency;
                    var clearedBalance = 0m;
                    var pendingDeposits = 0m;
                    var pendingWithdrawals = 0m;

                    if (bankTransactionsByBankAccount.ContainsKey(e.Key))
                    {
                        clearedBalance = bankTransactionsByBankAccount[e.Key].Where(x => x.ClearDate.HasValue).Select(x => x.AccountAmount).SafeSum();
                        pendingDeposits = bankTransactionsByBankAccount[e.Key].Where(x => !x.ClearDate.HasValue && x.AccountAmount > 0m).Select(x => x.AccountAmount).SafeSum();
                        pendingWithdrawals = bankTransactionsByBankAccount[e.Key].Where(x => !x.ClearDate.HasValue && x.AccountAmount < 0m).Select(x => x.AccountAmount).SafeSum();
                    }

                    var actualBalance = clearedBalance + pendingDeposits + pendingWithdrawals;

                    var availableCredit = default(decimal?);
                    if (e.HasCreditLimit && e.CreditLimit > 0m) availableCredit = e.CreditLimit;
                    if (actualBalance < 0m) availableCredit += actualBalance;
                    if (availableCredit < 0m) availableCredit = 0m;

                    output.Add(e, new Balance()
                    {
                        ClearedBalance = new Tuple<decimal, Currency, BusinessTemplate>(clearedBalance, currency, new BankAccountTransactions() { BankAccount = e.Key, Business = Business, ClearedOnly = true, Referrer = referrer }),
                        PendingDeposits = pendingDeposits > 0m ? new Tuple<decimal, Currency, BusinessTemplate>(pendingDeposits, currency, new BankAccountTransactions() { BankAccount = e.Key, Business = Business, PendingDepositsOnly = true, Referrer = referrer }) : null,
                        PendingWithdrawals = pendingWithdrawals < 0m ? new Tuple<decimal, Currency, BusinessTemplate>(pendingWithdrawals, currency, new BankAccountTransactions() { BankAccount = e.Key, Business = Business, PendingWithdrawlsOnly = true, Referrer = referrer }) : null,
                        ActualBalance = new Tuple<decimal, Currency, BusinessTemplate>(actualBalance, currency, new BankAccountTransactions() { BankAccount = e.Key, Business = Business, Referrer = referrer }),
                        AvailableCredit = availableCredit.HasValue ? new Tuple<decimal, Currency>(availableCredit.Value, currency) : null
                    });
                }

                getBalances = output;
            }
            return getBalances;
        }

        [Right, Sum]
        [Guid("542f2b2c-df24-43b8-86eb-5d4df087587f")]
        [Guide("The **Cleared Balance** column displays the sum of all *Payments*, *Receipts*, and *Inter Account Transfers* recorded in each bank account that are marked as *Cleared*.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetClearedBalance(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].ClearedBalance).ToArray();
        }

        [Right, Sum]
        [Guid("624e2a46-7a7c-48a9-b947-fcf4087aaf80")]
        [Guide("The **Pending Deposits** column displays the sum of all *Receipts* and *Inter Account Transfers* recorded for each bank account that are flagged as *Pending*.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetPendingDeposits(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].PendingDeposits).ToArray();
        }

        [Right, Sum]
        [Guid("4c8470c9-91db-4170-acd4-3177bb21d590")]
        [Guide("The **Pending Withdrawals** column displays the sum of all *Payments* and *Inter Account Transfers* recorded in each bank account that are designated as *Pending*.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetPendingWithdrawals(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].PendingWithdrawals).ToArray();
        }

        [Right, Sum]
        [Guid("577ebfc7-f35a-4b02-8377-f94ba3dfe13a"), Default, Bold]
        [Guide("The **Actual Balance** column displays the sum of all *Payments*, *Receipts*, and *Inter Account Transfers* recorded for each bank account.")]
        [Guide("It equals the *Cleared Balance* plus *Pending Deposits* minus *Pending Withdrawals*.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetActualBalance(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var aggregations = database.GetGeneralLedgerTransactions().GetAggregations();
            return rows.Select(x => new Tuple<decimal, Currency, BusinessTemplate>(aggregations.GetBankOrCashAccountCurrencyAmount(x.Key, DateTime.MinValue, DateTime.MaxValue), database.SingleOrDefault<ForeignCurrency>(x.Currency) as Currency ?? baseCurrency, new BankAccountTransactions() { BankAccount = x.Key, Business = Business, Referrer = referrer })).ToArray();
        }

        [Right, Sum]
        [Guid("84c0c684-387f-40df-8c7b-0e572a0c0689")]
        public Tuple<decimal, Currency>[] GetAvailableCredit(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].AvailableCredit).ToArray();
        }

        public sealed class Balance
        {
            public Tuple<decimal, Currency, BusinessTemplate> ClearedBalance;
            public Tuple<decimal, Currency, BusinessTemplate> PendingDeposits;
            public Tuple<decimal, Currency, BusinessTemplate> PendingWithdrawals;
            public Tuple<decimal, Currency, BusinessTemplate> ActualBalance;
            public Tuple<decimal, Currency> AvailableCredit;
        }

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("1b6c3144-f009-4396-b056-e16ef1d9cb82")]
        [Guide("The **Last Bank Reconciliation** column displays the date of the most recent bank reconciliation for each bank account.")]
        [Guide("This helps ensure your reconciliations are up to date and do not lag behind.")]
        public DateTime?[] GetLastBankReconciliation(ManagerServer.Model.BankOrCashAccount[] rows)
        {
            var lastBankReconciliationDates = ApplicationData.Businesses.Get(Business).OfType<BankReconciliation>().Where(x => x.BankAccount.HasValue).GroupBy(x => x.BankAccount.Value).ToDictionary(x => x.Key, x => x.Max(y => y.Date));
            return rows.Select(x => lastBankReconciliationDates.TryGetValue(x.Key, out DateTime date) ? date as DateTime? : null).ToArray();
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: new ImportBankStatement() { Business = Business }.ToUrl(), @class: "btn btn-xs")) Write(Strings.ImportBankStatement);
            base.OnFooterEndSection(context);
        }
    }
}
