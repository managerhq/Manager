using ManagerServer;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoices), nameof(Strings.Reconciliation))]
    [Guide("This screen finds accounts receivables transactions unallocated to specific invoice and suggests allocation.")]
    internal sealed class SalesInvoiceReconciliation : Table<SalesInvoiceReconciliation.Row>
    {
        protected override Row[] GetObjects()
        {
            return AutomaticallyMatchSalesInvoices();
        }

        protected override BusinessTemplate GetEdit(Row o, string referrer)
        {
            return base.GetEdit(o, referrer);
        }

        protected override BusinessTemplate GetView(Row o, string referrer)
        {
            return base.GetView(o, referrer);
        }

        public record Row
        {
            [MinWidth, Center, WhitespaceNoWrap] public DateTime Date { get; set; }
            public string Transaction { get; set; }
            public Customer Customer { get; set; }
            [HideColumnIfAllEmpty] public string Description { get; set; }
            public SalesInvoice SalesInvoice { get; set; }
            [Bold, Right, WhitespaceNoWrap] public CurrencyAmount Credit { get; set; }
        }

        private Row[] AutomaticallyMatchSalesInvoices(Guid[] customers = null)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var salesInvoiceBalances = new Dictionary<SalesInvoice, BalanceDue>();
            var customerTransactionsToAllocate = new Dictionary<Customer, List<TransactionToAllocate>>();

            foreach (var e in new GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsAccountsReceivable))
            {
                if (e.Customer == null) continue;
                if (customers != null && !customers.Contains(e.Customer.Key)) continue;

                if (e.SalesInvoice != null)
                {
                    if (!salesInvoiceBalances.ContainsKey(e.SalesInvoice)) salesInvoiceBalances.Add(e.SalesInvoice, new BalanceDue());
                    salesInvoiceBalances[e.SalesInvoice].Amount += e.AccountAmount;
                }
                else
                {
                    if (!customerTransactionsToAllocate.ContainsKey(e.Customer)) customerTransactionsToAllocate.Add(e.Customer, new List<TransactionToAllocate>());
                    customerTransactionsToAllocate[e.Customer].Add(new TransactionToAllocate() { Transaction = e, Amount = e.AccountAmount * -1m });
                }
            }

            var rows = new List<Row>();

            var salesInvoicesSortedByCustomer = salesInvoiceBalances.Keys.Where(x => x.Customer.HasValue).GroupBy(x => x.Customer.Value).ToDictionary(x => x.Key, x => x.OrderBy(y => y.GetDueDate()).ThenBy(y => y.Reference).ToArray());
            foreach (var e in customerTransactionsToAllocate.Where(x => x.Value.Any()).Select(x => x.Key))
            {
                if (customers != null && !customers.Contains(e.Key)) continue;

                var stack = new Stack<TransactionToAllocate>();
                foreach (var e2 in customerTransactionsToAllocate[e].OrderBy(x => x.Transaction.Date).ThenBy(x => x.Amount < 0m))
                {
                    if (e2.Amount > 0m)
                    {
                        stack.Push(e2);
                    }
                    else if (e2.Amount < 0m)
                    {
                        while (true)
                        {
                            if (!stack.Any()) break;
                            var previous = stack.Peek();
                            if (previous.Amount + e2.Amount > 0m)
                            {
                                previous.Amount += e2.Amount;
                                break;
                            }
                            else
                            {
                                stack.Pop();
                                e2.Amount += previous.Amount;
                                if (e2.Amount == 0m) break;
                            }
                        }
                    }
                }

                var customerTransactions = new Queue<TransactionToAllocate>(stack.OrderBy(x => x.Transaction.Date));

                if (salesInvoicesSortedByCustomer.ContainsKey(e.Key))
                {
                    foreach (var e2 in salesInvoicesSortedByCustomer[e.Key])
                    {
                        while (true)
                        {
                            if (!customerTransactions.Any()) break;

                            var balance = salesInvoiceBalances[e2];
                            if (balance.Amount <= 0m) break;

                            var transaction = customerTransactions.Peek();
                            var amount = 0m;
                            if (balance.Amount >= transaction.Amount)
                            {
                                customerTransactions.Dequeue();
                                amount = transaction.Amount;
                            }
                            else
                            {
                                transaction.Amount -= balance.Amount;
                                amount = balance.Amount;
                            }

                            rows.Add(new Row()
                            {
                                Date = transaction.Transaction.Date,
                                Transaction = transaction.Transaction.Transaction.GetTransactionName(),
                                Customer = e,
                                SalesInvoice = e2,
                                Credit = new CurrencyAmount(amount, transaction.Transaction.AccountCurrency)
                            });                            

                            salesInvoiceBalances[e2].Amount -= amount;
                        }
                    }
                }
            }

            return rows.ToArray();
        }

        private class BalanceDue
        {
            public decimal Amount;
        }

        private class TransactionToAllocate
        {
            public GeneralLedgerTransaction Transaction;
            public decimal Amount;
        }
    }
}
