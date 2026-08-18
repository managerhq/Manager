using System;
using System.Linq;
using System.Collections.Generic;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Query;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.BankReconciliations
{
    [ProtoContract]
    [Title(nameof(Strings.BankReconciliation), nameof(Strings.Transactions))]
    [Guide("This screen shows bank reconciliation transactions for a specific *bank account* during the reconciliation process.")]
    [Guide("Bank reconciliation ensures your accounting records match your bank statements by comparing transactions and identifying discrepancies.")]
    [Header("Overview")]
    [Guide("The reconciliation process involves matching transactions recorded in Manager with those appearing on your bank statement.")]
    [Guide("This screen displays both cleared and uncleared transactions, helping you identify which payments and deposits have been processed by your bank.")]
    [Guide("Any discrepancies between your records and the bank statement will be highlighted for investigation.")]
    [Header("Working with Transactions")]
    [Guide("**Uncleared transactions** are those recorded in Manager but not yet appearing on your bank statement. These may include recent deposits or checks that haven't been processed.")]
    [Guide("To mark a transaction as cleared, click the **Edit** button next to it and update its status to *Cleared*, specifying the date it appeared on your bank statement.")]
    [Guide("The screen calculates *net movement* based on both your closing balances and individual transactions - these amounts must match for successful reconciliation.")]
    [Header("Reconciliation Tips")]
    [Guide("Compare each transaction on this screen with your bank statement line by line to ensure accuracy.")]
    [Guide("If you find missing transactions, you can add them using the **New Receipt** or **New Payment** buttons.")]
    [Guide("For transactions with incorrect amounts, use the **Edit** button to correct them before completing the reconciliation.")]
    [LinkGuide("For more information about bank reconciliation, see:", typeof(BankReconciliationForm))]
    internal sealed class BankReconciliationTransactions : BusinessTemplate
    {
        [ProtoMember(1)] public Guid BankAccount;
        [ProtoMember(2)] public DateTime Date;

        protected override void InnerGet2()
        {
            var bankAccount = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.BankOrCashAccount>(BankAccount);
            if (bankAccount == null)
            {
                return;
            }

            var bankReconciliationItems = this.GetBankReconciliationItems(BankAccount);
            var bankReconciliation = bankReconciliationItems.OrderByDescending(x => x.ClosingBalanceDate).FirstOrDefault(x => !x.IsReconciled && x.BankAccountID == BankAccount && x.ClosingBalanceDate <= Date);
            if (bankReconciliation == null)
            {
                return;
            }
            
            var currency = bankAccount.Currency;
            var cashTransactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount?.Key == bankAccount.Key).ToArray();
            var unclearedCashTransactions = cashTransactions.Where(x => !x.ClearDate.HasValue && x.Date <= bankReconciliation.ClosingBalanceDate).OrderBy(x => x.Date).ToArray();
            var clearedCashTransactions = cashTransactions.Where(x => x.ClearDate.HasValue && x.ClearDate <= bankReconciliation.ClosingBalanceDate).OrderBy(x => x.ClearDate).ToArray();
            if (bankReconciliation.OpeningBalanceDate.HasValue)
            {
                clearedCashTransactions = clearedCashTransactions.Where(x => x.ClearDate >= bankReconciliation.OpeningBalanceDate.Value).ToArray();
            }

            var currencies = ManagerServer.Query.Currencies.GetCurrencyProvider(Business);

            var referrer = this.ToUrl();

            using (Label(style: "line-height: 200%"))
            {
                var bankAccountName = @"<span style=""background-color: yellow; padding: 3px; font-weight: bold"">" + bankAccount.Name.Replace(" ", "&nbsp;") + @"</span>";
                if (!bankReconciliation.OpeningBalanceDate.HasValue)
                {
                    Write(@"For the <span style=""background-color: yellow; padding: 3px; font-weight: bold"">" + bankAccount.Name.Replace(" ", "&nbsp;") + @"</span> ");
                    Write(@"on <span style=""background-color: yellow; padding: 3px; font-weight: bold"">" + bankReconciliation.ClosingBalanceDate.ToString("dd MMMM yyyy").Replace(" ", "&nbsp;") + @"</span> you entered the following closing balance.");
                    Br();
                    Write("Check the balance to make sure you entered it correctly.");
                }
                else
                {
                    Write(@"For the <span style=""background-color: yellow; padding: 3px; font-weight: bold"">" + bankAccount.Name.Replace(" ", "&nbsp;") + @"</span> ");
                    Write(@"on <span style=""background-color: yellow; padding: 3px; font-weight: bold"">" + bankReconciliation.OpeningBalanceDate.Value.AddDays(-1).ToString("dd MMMM yyyy").Replace(" ", "&nbsp;") + @"</span> and <span style=""background-color: yellow; padding: 3px; font-weight: bold"">" + bankReconciliation.ClosingBalanceDate.ToString("dd MMMM yyyy").Replace(" ", "&nbsp;") + @"</span> you entered the following closing balances.");
                    Br();
                    Write("Check both balances to make sure you entered them correctly.");
                }
            }

            using (Div(@class: "card", style: "margin-top: 10px; min-width: 300px; width: 300px"))
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "card-title")) Write(Strings.ClosingBalances);
                }
                using (Table(@class: "card-table"))
                {
                    using (THead())
                    {
                        using (Tr())
                        {
                            using (Th()) { };
                            using (Th(style: "text-align: center")) Write(Strings.Date);
                            using (Th(style: "text-align: right")) Write(Strings.ClosingBalance);
                        }
                    }
                    if (bankReconciliation.OpeningBalanceDate.HasValue)
                    {
                        using (Tr())
                        {
                            using (Td(style: "width: 1px")) using (A(href: new BankReconciliationForm() { Business = Business, Key = bankReconciliation.OpeningBalanceKey, BankAccount = BankAccount, Date = bankReconciliation.OpeningBalanceDate.Value.AddDays(-1), Referrer = referrer }.ToUrl(), @class: "btn btn-xs")) Write(Strings.Edit);
                            using (Td(style: "text-align: center")) Write(bankReconciliation.OpeningBalanceDate.Value.AddDays(-1).ToLocalShortDisplayString());
                            using (Td(style: "text-align: right; font-weight: bold")) using (Span(style: (bankReconciliation.OpeningBalance < 0 ? "color: red" : null))) Write(bankReconciliation.OpeningBalance.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Long));
                        }
                    }
                    using (Tr())
                    {
                        using (Td(style: "width: 1px")) using (A(href: new BankReconciliationForm() { Business = Business, Key = bankReconciliation.ClosingBalanceKey, BankAccount = BankAccount, Date = bankReconciliation.ClosingBalanceDate, Referrer = referrer }.ToUrl(), @class: "btn btn-xs")) Write(Strings.Edit);
                        using (Td(style: "text-align: center")) Write(bankReconciliation.ClosingBalanceDate.ToLocalShortDisplayString());
                        using (Td(style: "text-align: right; font-weight: bold")) using (Span(style: (bankReconciliation.ClosingBalance < 0 ? "color: red" : null))) Write(bankReconciliation.ClosingBalance.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Long));
                    }
                    using (TFoot())
                    {
                        using (Tr())
                        {
                            var netMovementAsPerClosingBalances = bankReconciliation.ClosingBalance - bankReconciliation.OpeningBalance;
                            using (Th(colspan: 3, style: "text-align: right"))
                            {
                                using (Span(style: "color: #999")) Write(Strings.Net_movement);
                                Br();
                                using (Span(style: (netMovementAsPerClosingBalances < 0 ? "color: red" : "color: #000"))) Write(netMovementAsPerClosingBalances.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Long));
                            }
                        }
                    }
                }
            }

            if (unclearedCashTransactions.Any())
            {
                using (Label(style: "line-height: 200%"))
                {
                    Write(@"The next step is to check uncleared payments or deposits whether they have cleared on bank statement on or before <span style=""background-color: yellow; padding: 3px; font-weight: bold"">" + bankReconciliation.ClosingBalanceDate.ToShortDateString() + @"</span>.<br />If yes, edit transaction to set status to <span style=""background-color: yellow; padding: 3px; font-weight: bold"">Cleared</span> and specify date when transaction has appeared on your bank statement.");
                }

                using (Div(@class: "card", style: "margin-top: 10px"))
                {
                    using (Div(@class: "card-header"))
                    {
                        using (Span(@class: "card-title")) Write(Strings.Transactions);
                    }

                    var contact = unclearedCashTransactions.Any(x => !string.IsNullOrWhiteSpace(x.Contact));

                    using (Table(@class: "card-table"))
                    {
                        using (THead())
                        {
                            using (Tr())
                            {
                                using (Th(style: "text-align: center")) { };
                                using (Th(style: "text-align: center")) Write(Strings.Date);
                                using (Th(style: "text-align: center")) Write(Strings.Transaction);
                                if (contact) using (Th()) Write(Strings.Description);
                                using (Th()) Write(Strings.Contact);
                                using (Th(style: "text-align: right")) Write(Strings.Amount);
                            }
                        }
                        foreach (var e in unclearedCashTransactions)
                        {
                            using (Tr())
                            {
                                using (Td(style: "width: 1px"))
                                {
                                    var editHandler = TransactionViewer.GetEditHandler(Business, e.Transaction);
                                    editHandler.Referrer = referrer;
                                    using (A(href: editHandler.ToUrl(), @class: "btn btn-xs")) Write(Strings.Edit);                                    
                                }
                                using (Td(style: "text-align: center; width: 1px; white-space: nowrap")) Write(e.Date.ToLocalShortDisplayString());
                                using (Td(style: "text-align: center; width: 1px; white-space: nowrap; font-weight: bold")) Write(e.Transaction?.GetName());
                                using (Td()) Write(e.Description);
                                if (contact) using (Td()) Write(e.Contact);

                                using (Td(style: "font-weight: bold; width: 100px; white-space: nowrap; text-align: right"))
                                {
                                    using (Span(style: (e.AccountAmount < 0 ? "color: red" : null))) Write(e.AccountAmount.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Long));
                                }
                            }
                        }
                    }
                }
            }

            using (Label(style: "line-height: 200%"))
            {
                if (!bankReconciliation.OpeningBalanceDate.HasValue)
                {
                    Write(@"Check bank statement lines on your bank statement before <span style=""background-color: yellow; padding: 3px; font-weight: bold"">" + bankReconciliation.ClosingBalanceDate.ToString("dd MMMM yyyy").Replace(" ", "&nbsp;") + @"</span>.");
                }
                else
                {
                    Write(@"Check bank statement lines on your bank statement for the period from <span style=""background-color: yellow; padding: 3px; font-weight: bold"">" + bankReconciliation.OpeningBalanceDate.Value.AddDays(-1).ToString("dd MMMM yyyy").Replace(" ", "&nbsp;") + @"</span> to <span style=""background-color: yellow; padding: 3px; font-weight: bold"">" + bankReconciliation.ClosingBalanceDate.ToString("dd MMMM yyyy").Replace(" ", "&nbsp;") + @"</span>.");
                }
                Br();
                Write("Enter missing bank statement lines, fix incorrect amounts or delete transactions which don't appear on your bank statement.");
                Br();
                Write("Net movement as per your bank statement closing balances above must equal to net movement as per your bank statement transactions below.");
            }

            if (bankReconciliation.MiddleDate.HasValue)
            {
                using (Div(style: "background-color: #ffffdb; border: 1px solid #ddd; border-radius: 3px; padding: 5px 10px; margin-bottom: 10px; font-size: 11px; color: #333"))
                {
                    Write("You don't need to compare these transactions to your bank statement manually, enter closing balance as at <b>" + bankReconciliation.MiddleDate.Value.ToLocalShortDisplayString() + @"</b> to find discrepancy quicker.");
                    using (A(href: new BankReconciliationForm() { Business = Business, BankAccount = bankAccount.Key, Date = bankReconciliation.MiddleDate.Value, Referrer = referrer }.ToUrl(), @class: "btn btn-xs", style: "margin-left: 5px")) Write("Enter closing balance");
                }
            }

            var balance = bankReconciliation.OpeningBalance;

            using (Div(@class: "card", style: "margin-top: 10px"))
            {
                using (Div(@class: "card-header flex gap-2 items-center"))
                {
                    using (Span(@class: "card-title")) Write(Strings.Transactions);
                    using (A(href: new Receipts.ReceiptForm() { Business = Business, BankAccount = BankAccount, Date = bankReconciliation.ClosingBalanceDate, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.NewReceipt);
                    using (A(href: new Payments.PaymentForm() { Business = Business, BankAccount = BankAccount, Date = bankReconciliation.ClosingBalanceDate, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.NewPayment);
                }
                if (!clearedCashTransactions.Any())
                {
                    using (Div(@class: "card-inset", style: "text-align: center; padding: 50px"))
                    {
                        using (Span(style: "color: #ccc; font-size: 18px; font-weight: bold")) Write(Strings.Empty);
                    }
                }
                else
                {
                    var contact = clearedCashTransactions.Any(x => !string.IsNullOrWhiteSpace(x.Contact));

                    using (Table(@class: "card-table"))
                    {
                        using (THead())
                        {
                            using (Tr())
                            {
                                using (Th(style: "text-align: center")) { };
                                using (Th(style: "text-align: center")) Write(Strings.Date);
                                using (Th()) Write(Strings.Transaction);
                                if (contact) using (Th()) Write(Strings.Description);
                                using (Th()) Write(Strings.Contact);
                                using (Th(style: "text-align: right")) Write(Strings.Amount);
                            }
                        }
                        foreach (var e in clearedCashTransactions)
                        {
                            using (Tr())
                            {
                                using (Td(style: "width: 1px"))
                                {
                                    var editHandler = TransactionViewer.GetEditHandler(Business, e.Transaction);
                                    if (editHandler != null)
                                    {
                                        editHandler.Referrer = referrer;
                                        using (A(href: editHandler.ToUrl(), @class: "btn btn-xs")) Write(Strings.Edit);
                                    }
                                }
                                using (Td(style: "text-align: center; width: 1px; white-space: nowrap")) Write(e.ClearDate.ToLocalShortDisplayString());
                                using (Td()) Write(e.Transaction?.GetName());
                                using (Td()) Write(e.Description);
                                if (contact) using (Td()) Write(e.Contact);

                                using (Td(style: "font-weight: bold; width: 100px; white-space: nowrap; text-align: right"))
                                {
                                    using (Span(style: (e.AccountAmount < 0 ? "color: red" : null))) Write(e.AccountAmount.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Long));
                                    balance += e.AccountAmount;
                                }
                            }
                        }
                        using (TFoot())
                        {
                            using (Tr())
                            {
                                var netMovementAsPerTransactions = clearedCashTransactions.Sum(x => x.AccountAmount);
                                using (Th(colspan: 99, style: "text-align: right"))
                                {
                                    using (Span(style: "color: #999")) Write(Strings.Net_movement);
                                    Br();
                                    using (Span(style: (netMovementAsPerTransactions < 0 ? "color: red" : "color: #000"))) Write(netMovementAsPerTransactions.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Long));
                                }
                            }
                        }
                    }
                }
            }
        }

        private Item[] GetBankReconciliationItems(Guid bankAccount)
        {
            var bankReconciliationBalances = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.BankReconciliation>().Where(x => x.BankAccount == bankAccount).GroupBy(x => new { x.BankAccount.Value, x.Date }).Select(x => x.OrderBy(y => y.Timestamp).First()).Select(x => new BankReconciliationBalance() { Key = x.Key, BankAccount = x.BankAccount.Value, ClosingBalanceDate = x.Date, ClosingBalance = x.StatementBalance }).ToArray();
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount?.Key == bankAccount && x.ClearDate.HasValue).ToArray();

            var items = new List<Item>();
            foreach (var e in bankReconciliationBalances)
            {
                var item = new Item()
                {
                    BankAccountID = e.BankAccount,
                    ClosingBalanceKey = e.Key,
                    ClosingBalanceDate = e.ClosingBalanceDate,
                    ClosingBalance = e.ClosingBalance
                };

                var previousClosingBalanceItem = bankReconciliationBalances.Where(x => x.BankAccount == e.BankAccount && x.ClosingBalanceDate < e.ClosingBalanceDate).OrderByDescending(x => x.ClosingBalanceDate).FirstOrDefault();
                if (previousClosingBalanceItem != null)
                {
                    item.OpeningBalanceKey = previousClosingBalanceItem.Key;
                    item.OpeningBalance = previousClosingBalanceItem.ClosingBalance;
                    item.OpeningBalanceDate = previousClosingBalanceItem.ClosingBalanceDate.AddDays(1);
                }

                var transactionsInPeriod = transactions.Where(x => x.BankAccount.Key == e.BankAccount && x.ClearDate <= item.ClosingBalanceDate).OrderBy(x => x.ClearDate).ToArray();
                if (item.OpeningBalanceDate.HasValue) transactionsInPeriod = transactionsInPeriod.Where(x => x.ClearDate >= item.OpeningBalanceDate.Value).ToArray();

                item.NetMovementAsPerTransactions = transactionsInPeriod.Sum(x => x.AccountAmount);

                var transactionsWithinAPeriod = transactionsInPeriod.Where(x => x.ClearDate < item.ClosingBalanceDate).ToArray();
                if (item.OpeningBalanceDate.HasValue) transactionsWithinAPeriod = transactionsWithinAPeriod.Where(x => x.ClearDate > item.OpeningBalanceDate.Value).ToArray();

                if (transactionsWithinAPeriod.Length > 1)
                {
                    item.MiddleDate = transactionsWithinAPeriod.Skip(transactionsWithinAPeriod.Length / 2).First().ClearDate;
                }
                else if (transactionsInPeriod.Any())
                {
                    if (transactionsInPeriod.First().ClearDate != transactionsInPeriod.Last().ClearDate)
                    {
                        item.MiddleDate = transactionsInPeriod.First().ClearDate;
                    }
                }
                items.Add(item);
            }
            return items.OrderByDescending(x => x.ClosingBalanceDate).ToArray();
        }

        public sealed class BankReconciliationBalance
        {
            public Guid Key;
            public Guid BankAccount;
            public DateTime ClosingBalanceDate;
            public decimal ClosingBalance;
        }

        public sealed class Item
        {
            public Guid BankAccountID;

            public decimal OpeningBalance;
            public Guid? OpeningBalanceKey;
            public DateTime? OpeningBalanceDate;

            public decimal ClosingBalance;
            public Guid ClosingBalanceKey;
            public DateTime ClosingBalanceDate;

            public decimal NetMovementAsPerTransactions;
            public DateTime? MiddleDate;

            public bool IsReconciled
            {
                get
                {
                    return NetMovementAsPerTransactions == (ClosingBalance - OpeningBalance);
                }
            }
        }
    }
}