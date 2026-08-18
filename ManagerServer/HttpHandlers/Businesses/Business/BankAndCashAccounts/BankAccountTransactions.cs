using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using ManagerComponents;
using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business.BankAndCashAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.BankOrCashAccount), nameof(Strings.Transactions))]
    [Guide("The bank account transactions screen displays all transactions for a specific bank or cash account. This comprehensive view helps you track money flowing in and out of your accounts.")]
    [Guide("Each transaction shows its date, type, related account, description, and amount. Deposits appear as positive amounts while withdrawals show as negative amounts in red.")]
    [Guide("The running balance column (when displayed) shows your account balance after each transaction, making it easy to track your financial position over time.")]
    [Header("View Options")]
    [Guide("You can filter the transaction list using different views:")]
    [Guide("**Actual Balance** - Shows all transactions regardless of their cleared status. This represents the true balance according to your records.")]
    [Guide("**Cleared Balance** - Displays only transactions that have been marked as cleared, typically after reconciling with your bank statement. When viewing cleared transactions, you can also filter by date to see the cleared balance as of a specific date.")]
    [Guide("**Pending Deposits** - Shows only uncleared deposits awaiting confirmation. From this view, you can click **New Receipt** to record additional pending deposits.")]
    [Guide("**Pending Withdrawals** - Shows only uncleared payments and withdrawals. From this view, you can click **New Payment** to record additional pending payments.")]
    [Header("Transaction Details")]
    [Guide("The transaction table provides comprehensive information about each entry:")]
    [Columns]
    internal sealed class BankAccountTransactions : ObjectTable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        [ProtoMember(1)] public Guid BankAccount;
        [ProtoMember(2)] public bool PendingDepositsOnly;
        [ProtoMember(3)] public bool PendingWithdrawlsOnly;
        [ProtoMember(4)] public bool ClearedOnly;
        [ProtoMember(5)] public DateTime? Date;

        protected override ManagerComponents.HeaderButton GetPrimaryButton()
        {
            if (ClearedOnly)
            {
                return new ManagerComponents.HeaderButton()
                {
                    Text = Strings.ImportBankStatement,
                    Url = new ImportBankStatement() { Business = Business, BankAccount = BankAccount, Referrer = this.ToUrl() }.ToUrl()
                };
            }
            if (PendingDepositsOnly)
            {
                return new ManagerComponents.HeaderButton()
                {
                    Text = Strings.NewReceipt,
                    Url = new Receipts.ReceiptForm() { Business = Business, BankAccount = BankAccount, Pending = true, Referrer = this.ToUrl() }.ToUrl()
                };
            }
            if (PendingWithdrawlsOnly)
            {
                return new HeaderButton()
                {
                    Text = Strings.NewPayment,
                    Url = new Payments.PaymentForm() { Business = Business, BankAccount = BankAccount, Pending = true, Referrer = this.ToUrl() }.ToUrl()
                };
            }
            return null;
        }

        protected override GeneralLedgerTransaction[] GetObjects()
        {
            var bankTransactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount?.Key == BankAccount)
                .ToArray();

            if (ClearedOnly)
            {
                bankTransactions = bankTransactions.Where(x => x.ClearDate.HasValue).OrderByDescending(x => x.ClearDate).ThenBy(x => x.AccountAmount > 0m).ThenByDescending(x => x.Transaction?.GetName()).ToArray();
                if (Date.HasValue)
                {
                    bankTransactions = bankTransactions.Where(x => x.ClearDate <= Date.Value).ToArray();
                }
            }
            else if (PendingDepositsOnly)
            {
                bankTransactions = bankTransactions.Where(x => !x.ClearDate.HasValue && x.AccountAmount > 0m).OrderByDescending(x => x.Date).ThenByDescending(x => x.Transaction?.GetName()).ToArray();
            }
            else if (PendingWithdrawlsOnly)
            {
                bankTransactions = bankTransactions
                    .Where(x => !x.ClearDate.HasValue && x.AccountAmount < 0m)
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(x => x.Transaction?.GetName())
                    .ToArray();
            }
            else
            {
                bankTransactions = bankTransactions.OrderByDescending(x => x.Date).ThenBy(x => x.AccountAmount > 0m).ThenByDescending(x => x.Transaction?.GetName()).ToArray();
            }

            return bankTransactions.ToArray();
        }

        protected override bool GetAttachment(GeneralLedgerTransaction o)
        {
            var key = o.Transaction?.Key;
            if (key == null) return false;
            var database = ApplicationData.Businesses.Get(Business);
            var attachments = database.OfType<Attachment>();
            var index = Array.BinarySearch(attachments, new Attachment() { Object = o.Transaction?.Key });
            if (index >= 0) return true;
            return false;
        }

        protected override BusinessTemplate GetEdit(GeneralLedgerTransaction o, string referrer)
        {
            return TransactionViewer.GetEditHandler(Business, o.Transaction, referrer);
        }

        protected override BusinessTemplate GetView(GeneralLedgerTransaction o, string referrer)
        {
            return TransactionViewer.GetViewHandler(Business, o.Transaction, referrer);
        }

        [Center, MinWidth, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Guid("12bce55e-c38c-4cc0-969e-f6d928f4137e")]
        public DateTime? GetDate(GeneralLedgerTransaction o)
        {
            if (o.ClearDate.HasValue)
            {
                if (o.ClearDate > DateTime.MinValue) return o.ClearDate.Value;
                return null;
            }
            else if (o.Date > DateTime.MinValue)
            {
                return o.Date;
            }
            else
            {
                return null;
            }
        }

        [Guid("3c3e6329-9bd3-40fd-a30f-3e2d85b683d0")]
        public string GetTransaction(GeneralLedgerTransaction o) => o.Transaction?.GetTransactionName();

        [Guid("143715ff-cbd9-45f1-a0ae-3943be3cba82")]
        public string GetAccount(GeneralLedgerTransaction o)
        {
            var account = string.Empty;
            if (o.Transaction is Receipt || o.Transaction is Payment)
            {
                account = string.Join(", ", o.ContraTransactions.Where(x => !x.IsTaxTransaction).Select(x => x.Account).Distinct());
            }
            if (o.Transaction is InterAccountTransfer)
            {
                account = o.ContraTransactions[0].Account;
            }
            return account;
        }

        [HideColumnIfAllEmpty]
        [Guid("510846ce-649c-4369-8f3a-5259de5fd501")]
        public string GetDescription(GeneralLedgerTransaction o) => string.Join(" — ", new[] { o.Description, o.Contact }.Where(x => !string.IsNullOrWhiteSpace(x)));

        [Bold, WhitespaceNoWrap, Sum, Right, TabularNums, RedIfNegative]
        [Guid("3e13bff6-a505-49a3-a5c0-2b998b4fa5c1")]
        public Tuple<decimal, string> GetAmount(GeneralLedgerTransaction o) => new(o.AccountAmount, o.AccountAmount.ToCurrencyString(o.AccountCurrency, CurrencySymbol.Short));

        protected override decimal? GetBalanceMovement(GeneralLedgerTransaction o) => o.AccountAmount;
    }
}