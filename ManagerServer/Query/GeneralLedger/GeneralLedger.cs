using ManagerServer.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ManagerServer.Query.GeneralLedger
{
    public sealed class GeneralLedger : IEnumerable<GeneralLedgerTransaction>
    {
        public string FileID { get; private set; }

        private ImmutableDictionary<Guid, GeneralLedgerTransactionContainer> startingTransactions;
        private GeneralLedgerTransaction[] newTransactions;
        private GeneralLedger parent;

        private GeneralLedgerAggregations aggregations;

        public GeneralLedger(string fileId)
        {
            FileID = fileId;
            var database = ApplicationData.Instance.Businesses.Get(fileId);
            startingTransactions = database.GetGeneralLedgerTransactions().GetAll();
            aggregations = database.GetGeneralLedgerTransactions().GetAggregations();
        }

        private GeneralLedger(GeneralLedger parent, GeneralLedgerTransaction[] newTransactions)
        {
            this.FileID = parent.FileID;
            this.parent = parent;
            this.aggregations = new GeneralLedgerAggregations(parent.aggregations);
            this.aggregations.Update(newTransactions, false);
            this.newTransactions = newTransactions;
        }

        public GeneralLedgerAggregations GetAggregations()
        {
            return aggregations;
        }

        public GeneralLedgerTransaction[] GetTransactions()
        {
            if (newTransactions == null && startingTransactions != null)
            {
                newTransactions = startingTransactions
                    .SelectMany(x => x.Value.GetLines())
                    .Where(x => x.Transaction.IsGeneralLedgerTransaction())
                    .ToArray();
            }

            return newTransactions.Concat(parent?.GetTransactions() ?? []).ToArray();
        }

        public GeneralLedger AutomaticallyMatchSalesInvoices(Guid[] customers = null)
        {
            var database = ApplicationData.Instance.Businesses.Get(FileID);
            var salesInvoiceBalances = new Dictionary<SalesInvoice, BalanceDue>();
            var customerTransactionsToAllocate = new Dictionary<Customer, List<TransactionToAllocate>>();

            foreach (var e in GetTransactions().Where(x => x.GeneralLedgerAccount.IsAccountsReceivable))
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

            var list = new LinkedList<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

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

                            var date = transaction.Transaction.Date;
                            if (date < e2.IssueDate) date = e2.IssueDate;

                            var transaction1 = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                transaction: transaction.Transaction.Transaction,
                                generalLedgerAccount: database.Single<BalanceSheetAccountsReceivableAccount>(),
                                customer: e,
                                salesInvoice: e2,
                                transactionAmount: amount * -1,
                                transactionCurrency: transaction.Transaction.AccountCurrency,
                                accountAmount: amount * -1,
                                date: date,
                                originalDate: transaction.Transaction.Date
                            );
                            var transaction2 = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                transaction: transaction.Transaction.Transaction,
                                generalLedgerAccount: database.Single<BalanceSheetAccountsReceivableAccount>(),
                                customer: e,
                                transactionAmount: amount,
                                transactionCurrency: transaction.Transaction.AccountCurrency,
                                accountAmount: amount,
                                date: date,
                                originalDate: transaction.Transaction.Date
                            );

                            list.AddLast(transaction1);
                            list.AddLast(transaction2);

                            salesInvoiceBalances[e2].Amount -= amount;
                        }
                    }
                }
            }

            if (list.Any())
            {
                return new GeneralLedger(this, list.ToArray());
            }

            return this;
        }

        public GeneralLedger AutomaticallyMatchPurchaseInvoices(Guid[] suppliers = null)
        {
            var database = ApplicationData.Instance.Businesses.Get(FileID);
            var purchaseInvoiceBalances = new Dictionary<PurchaseInvoice, BalanceDue>();
            var supplierTransactionsToAllocate = new Dictionary<Supplier, List<TransactionToAllocate>>();

            foreach (var e in GetTransactions().Where(x => x.GeneralLedgerAccount.IsAccountsPayable))
            {
                if (e.Supplier == null) continue;
                if (suppliers != null && !suppliers.Contains(e.Supplier.Key)) continue;

                if (e.PurchaseInvoice != null)
                {
                    if (!purchaseInvoiceBalances.ContainsKey(e.PurchaseInvoice)) purchaseInvoiceBalances.Add(e.PurchaseInvoice, new BalanceDue());
                    purchaseInvoiceBalances[e.PurchaseInvoice].Amount -= e.AccountAmount;
                }
                else
                {
                    if (!supplierTransactionsToAllocate.ContainsKey(e.Supplier)) supplierTransactionsToAllocate.Add(e.Supplier, new List<TransactionToAllocate>());
                    supplierTransactionsToAllocate[e.Supplier].Add(new TransactionToAllocate() { Transaction = e, Amount = e.AccountAmount });
                }
            }

            var list = new LinkedList<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            var purchaseInvoicesSortedBySupplier = purchaseInvoiceBalances.Keys.Where(x => x.Supplier.HasValue).GroupBy(x => x.Supplier.Value).ToDictionary(x => x.Key, x => x.OrderBy(y => y.GetDueDate()).ThenBy(y => y.Reference).ToArray());
            foreach (var e in supplierTransactionsToAllocate.Where(x => x.Value.Any()).Select(x => x.Key))
            {
                if (suppliers != null && !suppliers.Contains(e.Key)) continue;

                var stack = new Stack<TransactionToAllocate>();
                foreach (var e2 in supplierTransactionsToAllocate[e].OrderBy(x => x.Transaction.Date).ThenBy(x => x.Amount < 0m))
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

                var supplierTransactions = new Queue<TransactionToAllocate>(stack.OrderBy(x => x.Transaction.Date));

                if (purchaseInvoicesSortedBySupplier.ContainsKey(e.Key))
                {
                    foreach (var e2 in purchaseInvoicesSortedBySupplier[e.Key])
                    {
                        while (true)
                        {
                            if (!supplierTransactions.Any()) break;

                            var balance = purchaseInvoiceBalances[e2];
                            if (balance.Amount <= 0m) break;

                            var transaction = supplierTransactions.Peek();
                            var amount = 0m;
                            if (balance.Amount >= transaction.Amount)
                            {
                                supplierTransactions.Dequeue();
                                amount = transaction.Amount;
                            }
                            else
                            {
                                transaction.Amount -= balance.Amount;
                                amount = balance.Amount;
                            }

                            var date = transaction.Transaction.Date;
                            if (date < e2.IssueDate) date = e2.IssueDate;

                            var transaction1 = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                transaction: transaction.Transaction.Transaction,
                                generalLedgerAccount: database.Single<BalanceSheetAccountsPayableAccount>(),
                                supplier: e,
                                purchaseInvoice: e2,
                                transactionAmount: amount,
                                transactionCurrency: transaction.Transaction.AccountCurrency,
                                accountAmount: amount,
                                date: date,
                                originalDate: transaction.Transaction.Date
                            );
                            var transaction2 = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                transaction: transaction.Transaction.Transaction,
                                generalLedgerAccount: database.Single<BalanceSheetAccountsPayableAccount>(),
                                supplier: e,
                                transactionAmount: amount * -1,
                                transactionCurrency: transaction.Transaction.AccountCurrency,
                                accountAmount: amount * -1,
                                date: date,
                                originalDate: transaction.Transaction.Date
                            );

                            list.AddLast(transaction1);
                            list.AddLast(transaction2);

                            purchaseInvoiceBalances[e2].Amount -= amount;
                        }
                    }
                }
            }

            if (list.Any())
            {
                return new GeneralLedger(this, list.ToArray());
            }

            return this;
        }

        public GeneralLedger DisposeFixedAssets()
        {
            var database = ApplicationData.Instance.Businesses.Get(FileID);

            if (database.GetCount<FixedAsset>() == 0) return this;

            var baseCurrency = database.Single<BaseCurrency>();
            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
            foreach (var e in database.UnorderedOfType<FixedAsset>().Where(x => x.DisposedFixedAsset && x.DisposalDate.HasValue))
            {
                var date = e.DisposalDate.Value;
                var purchaseCost = database.GetGeneralLedgerTransactions().GetAggregations().GetFixedAssetAmount(e.Key, DateTime.MinValue, date);
                var accumulatedDepreciation = database.GetGeneralLedgerTransactions().GetAggregations().GetDepreciationAmount(e.Key, DateTime.MinValue, date);
                var bookValue = purchaseCost + accumulatedDepreciation;

                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    generalLedgerAccount: database.Single<BalanceSheetFixedAssetsAtCostAccount>(),
                    fixedAsset: e,
                    transactionAmount: purchaseCost * -1,
                    transactionCurrency: baseCurrency,
                    date: date,
                    trackingCode: database.SingleOrDefault<Division>(e.Division),
                    isFixedAssetDisposalTransaction: true
                ));
                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    generalLedgerAccount: database.Single<BalanceSheetFixedAssetsAccumulatedDepreciationAccount>(),
                    fixedAsset: e,
                    transactionAmount: accumulatedDepreciation * -1,
                    transactionCurrency: baseCurrency,
                    date: date,
                    trackingCode: database.SingleOrDefault<Division>(e.Division),
                    isFixedAssetDisposalTransaction: true
                ));
                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    generalLedgerAccount: database.SingleOrDefault<ProfitAndLossStatementAccount>(e.CustomExpenseAccountForDisposal) as IGeneralLedgerAccount ?? database.Single<ProfitAndLossStatementAccountFixedAssetLossOnDisposal>(),
                    fixedAsset: e,
                    transactionAmount: bookValue,
                    transactionCurrency: baseCurrency,
                    date: date,
                    trackingCode: database.SingleOrDefault<Division>(e.Division),
                    isFixedAssetDisposalTransaction: true
                ));
            }
            if (list.Count > 0)
            {
                return new GeneralLedger(this, list.ToArray());
            }
            return this;
        }

        public GeneralLedger DisposeIntangibleAssets()
        {
            var database = ApplicationData.Instance.Businesses.Get(FileID);

            if (database.GetCount<IntangibleAsset>() == 0) return this;

            if (!database.UnorderedOfType<IntangibleAsset>().Any(x => x.DisposedIntangibleAsset && x.DisposalDate.HasValue)) return this;

            var baseCurrency = database.Single<BaseCurrency>();
            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
            foreach (var e in database.UnorderedOfType<IntangibleAsset>().Where(x => x.DisposedIntangibleAsset && x.DisposalDate.HasValue))
            {
                var date = e.DisposalDate.Value;
                var acquisitionCost = database.GetGeneralLedgerTransactions().GetAggregations().GetIntangibleAssetAmount(e.Key, DateTime.MinValue, date);
                var amortization = database.GetGeneralLedgerTransactions().GetAggregations().GetAmortizationAmount(e.Key, DateTime.MinValue, date);
                var bookValue = acquisitionCost + amortization;

                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    generalLedgerAccount: database.Single<BalanceSheetIntangibleAssetsAtCostAccount>(),
                    intangibleAsset: e,
                    transactionAmount: acquisitionCost * -1,
                    transactionCurrency: baseCurrency,
                    date: date,
                    trackingCode: database.SingleOrDefault<Division>(e.Division),
                    isIntangibleAssetDisposalTransaction: true
                ));
                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    generalLedgerAccount: database.Single<BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount>(),
                    intangibleAsset: e,
                    transactionAmount: amortization * -1,
                    transactionCurrency: baseCurrency,
                    date: date,
                    trackingCode: database.SingleOrDefault<Division>(e.Division),
                    isIntangibleAssetDisposalTransaction: true
                ));
                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    generalLedgerAccount: database.SingleOrDefault<ProfitAndLossStatementAccount>(e.CustomExpenseAccountForDisposal) as IGeneralLedgerAccount ?? database.Single<ProfitAndLossStatementAccountIntangibleAssetsGainsLossOnDisposal>(),
                    intangibleAsset: e,
                    transactionAmount: bookValue,
                    transactionCurrency: baseCurrency,
                    date: date,
                    trackingCode: database.SingleOrDefault<Division>(e.Division),
                    isIntangibleAssetDisposalTransaction: true
                ));
            }
            if (list.Count > 0)
            {
                return new GeneralLedger(this, list.ToArray());
            }
            return this;
        }

        public GeneralLedger Revaluate(DateTime asOf)
        {
            return RevaluateInventoryItems(asOf).RevaluateInvestments(asOf).RevaluateForeignExchangeAccounts(asOf);
        }

        public GeneralLedger Revaluate(DateTime from, DateTime to)
        {
            return RevaluateInventoryItems(from, to).RevaluateInvestments(from, to).RevaluateForeignExchangeAccounts(from, to);
        }

        private GeneralLedger RevaluateInventoryItems(DateTime from, DateTime to)
        {
            var generalLedger = this;
            if (from > DateTime.MinValue)
            {
                if (from.AddDays(-1) < to) generalLedger = generalLedger.RevaluateInventoryItems(from.AddDays(-1));
            }
            generalLedger = generalLedger.RevaluateInventoryItems(to);
            return generalLedger;
        }

        private GeneralLedger RevaluateInventoryItems(DateTime date)
        {
            var database = ApplicationData.Instance.Businesses.Get(FileID);
            if (database.GetCount<InventoryItem>() == 0) return this;

            var list = new List<GeneralLedgerTransaction>();
            var baseCurrency = database.Single<BaseCurrency>();

            foreach (var e in database.UnorderedOfType<InventoryItem>())
            {
                var balance = aggregations.GetInventoryItemAmount(e.Key, DateTime.MinValue, date);
                var qty = aggregations.GetInventoryItemQtyOwned(e.Key, DateTime.MinValue, date);

                if (qty <= 0m || balance < 0m)
                {
                    if (balance != 0m)
                    {
                        var division = database.SingleOrDefault<Division>(e.Division);

                        list.Add(new GeneralLedgerTransaction(
                            database: database,
                            date: date,
                            generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                            inventoryItem: e,
                            trackingCode: division,
                            transactionAmount: -balance,
                            transactionCurrency: baseCurrency
                        ));

                        list.Add(new GeneralLedgerTransaction(
                            database: database,
                            date: date,
                            qty: qty,
                            generalLedgerAccount: database.Single<BalanceSheetNegativeInventoryClearing>(),
                            inventoryItem: e,
                            trackingCode: division,
                            transactionAmount: balance,
                            transactionCurrency: baseCurrency
                        ));

                        if (date != DateTime.MaxValue)
                        {
                            list.Add(new GeneralLedgerTransaction(
                                database: database,
                                date: date.SafeAddDays(1),
                                generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                                inventoryItem: e,
                                trackingCode: division,
                                transactionAmount: balance,
                                transactionCurrency: baseCurrency,
                                isBalancing: true
                            ));

                            list.Add(new GeneralLedgerTransaction(
                                database: database,
                                date: date.SafeAddDays(1),
                                generalLedgerAccount: database.Single<BalanceSheetNegativeInventoryClearing>(),
                                inventoryItem: e,
                                trackingCode: division,
                                qty: -qty,
                                transactionAmount: -balance,
                                transactionCurrency: baseCurrency,
                                isBalancing: true
                            ));
                        }
                    }
                }
            }

            if (list.Count > 0)
            {
                return new GeneralLedger(this, list.ToArray());
            }
            else
            {
                return this;
            }
        }

        private GeneralLedger RevaluateInvestments(DateTime? from, DateTime to)
        {
            var generalLedger = this;
            if (from.HasValue && from.Value > DateTime.MinValue)
            {
                generalLedger = generalLedger.RevaluateInvestments(from.Value.AddDays(-1));
            }
            generalLedger = generalLedger.RevaluateInvestments(to);
            return generalLedger;
        }

        private GeneralLedger RevaluateInvestments(DateTime date)
        {
            var database = ApplicationData.Instance.Businesses.Get(FileID);

            if (database.GetCount<Investment>() == 0) return this;

            var list = new List<GeneralLedgerTransaction>();
            var baseCurrency = ApplicationData.Instance.Businesses.Get(FileID).Single<BaseCurrency>();
            var investmentMarketPrices = database.UnorderedOfType<InvestmentMarketPrice>().Where(x => x.Investment.HasValue && x.Date <= date && x.MarketPrice > 0m).OrderByDescending(x => x.Date).GroupBy(x => x.Investment.Value).ToDictionary(x => x.Key, x => x.First());

            foreach (var e in GetTransactions().Where(x => x.Date <= date && x.GeneralLedgerAccount.IsControlAccountForInvestments).GroupBy(x => x.Investment))
            {
                if (investmentMarketPrices.TryGetValue(e.Key.Key, out InvestmentMarketPrice investmentMarketPrice))
                {
                    var marketPriceInBaseCurrency = investmentMarketPrice.GetMarketPriceInBaseCurrency(baseCurrency);

                    if (marketPriceInBaseCurrency.HasValue)
                    {
                        // We won't trigger revaluation unless market price is entered. Even if closing quantity is zero because there is a risk of doubling up gains / losses.

                        var balance = e.Sum(x => x.BaseAmount);
                        var qty = e.Sum(x => x.Qty ?? 0m);

                        var revaluatedBalance = qty * marketPriceInBaseCurrency.Value;

                        var investmentGain = revaluatedBalance - balance;

                        if (investmentGain != 0m)
                        {
                            var transaction = new GeneralLedgerTransaction(
                                database: database,
                                date: date,
                                generalLedgerAccount: e.First().GeneralLedgerAccount,
                                investment: e.Key,
                                transactionAmount: investmentGain,
                                transactionCurrency: baseCurrency,
                                investmentMarketPrice: investmentMarketPrice
                            );

                            list.Add(transaction);

                            list.Add(new GeneralLedgerTransaction(
                                database: database,
                                date: date,
                                generalLedgerAccount: database.Single<ProfitAndLossStatementCapitalGainsOnInvestments>(),
                                investment: e.Key,
                                contraTransactions: new GeneralLedgerTransaction[] { transaction },
                                transactionAmount: investmentGain * -1m,
                                transactionCurrency: baseCurrency,
                                investmentMarketPrice: investmentMarketPrice
                            ));
                        }
                    }
                }
            }

            if (list.Any())
            {
                return new GeneralLedger(this, list.ToArray());
            }
            else
            {
                return this;
            }
        }

        private GeneralLedger RevaluateForeignExchangeAccounts(DateTime? from, DateTime to)
        {
            var generalLedger = this;
            if (from.HasValue && from.Value > DateTime.MinValue)
            {
                generalLedger = generalLedger.RevaluateForeignExchangeAccounts(from.Value.AddDays(-1));
            }
            generalLedger = generalLedger.RevaluateForeignExchangeAccounts(to);
            return generalLedger;
        }

        private GeneralLedger RevaluateForeignExchangeAccounts(DateTime date)
        {
            var database = ApplicationData.Instance.Businesses.Get(FileID);

            if (database.GetCount<ForeignCurrency>() == 0) return this;

            var list = new List<GeneralLedgerTransaction>();
            var baseCurrency = ApplicationData.Instance.Businesses.Get(FileID).Single<BaseCurrency>();
            var startingExchangeRates = database.Single<StartingExchangeRates>();
            var exchangeRates = database.UnorderedOfType<ExchangeRate>().Where(x => x.Currency.HasValue && x.Date <= date && x.ExchangeRateValue > 0m).OrderByDescending(x => x.Date).GroupBy(x => x.Currency).ToDictionary(x => x.Key.Value, x => x.First());

            foreach (var e in GetTransactions().Where(x => x.Date <= date && x.AccountCurrency is ForeignCurrency).GroupBy(x => new { x.GeneralLedgerAccount, x.ForeignCurrencyAccount }))
            {
                var foreignCurrency = (ForeignCurrency)e.First().AccountCurrency;
                var division = e.First().Division;
                var baseBalance = e.Select(x => x.BaseAmount).SafeSum();;
                var accountBalance = e.Select(x => x.AccountAmount).SafeSum();

                var adjustedBaseBalance = accountBalance;
                if (exchangeRates.TryGetValue(foreignCurrency.Key, out ExchangeRate exchangeRate))
                {
                    if (!exchangeRate.ExchangeRateIsInverse)
                    {
                        adjustedBaseBalance = baseCurrency.Round(accountBalance * exchangeRate.ExchangeRateValue);
                    }
                    else
                    {
                        adjustedBaseBalance = baseCurrency.Round(accountBalance / exchangeRate.ExchangeRateValue);
                    }
                }
                else
                {
                    var startingExchangeRate = startingExchangeRates.GetExchangeRate(foreignCurrency);
                    if (startingExchangeRate != null)
                    {
                        if (!startingExchangeRate.ExchangeRateIsInverse)
                        {
                            adjustedBaseBalance = baseCurrency.Round(accountBalance * startingExchangeRate.ExchangeRate);
                        }
                        else
                        {
                            adjustedBaseBalance = baseCurrency.Round(accountBalance / startingExchangeRate.ExchangeRate);
                        }
                    }
                }
                var foreignExchangeGain = adjustedBaseBalance - baseBalance;

                if (foreignExchangeGain != 0m)
                {
                    var transaction = new GeneralLedgerTransaction(
                        database: database,
                        date: date,
                        generalLedgerAccount: e.Key.GeneralLedgerAccount,
                        supplier: e.Key.ForeignCurrencyAccount as Supplier,
                        customer: e.Key.ForeignCurrencyAccount as Customer,
                        specialAccount: e.Key.ForeignCurrencyAccount as SpecialAccount,
                        bankAccount: e.Key.ForeignCurrencyAccount as BankOrCashAccount,
                        employee: e.Key.ForeignCurrencyAccount as Employee,
                        trackingCode: division,
                        transactionAmount: 0m,
                        transactionCurrency: foreignCurrency,
                        accountAmount: 0m,
                        baseAmount: foreignExchangeGain
                    );

                    list.Add(transaction);

                    list.Add(new GeneralLedgerTransaction(
                        database: database,
                        date: date,
                        generalLedgerAccount: database.Single<ProfitAndLossStatementAccountCurrencyGainsLosses>(),
                        contraTransactions: new GeneralLedgerTransaction[] { transaction },
                        transactionAmount: 0m,
                        transactionCurrency: foreignCurrency,
                        trackingCode: division,
                        baseAmount: foreignExchangeGain*-1m
                    ));
                }
            }

            if (list.Any())
            {
                return new GeneralLedger(this, list.ToArray());
            }
            else
            {
                return this;
            }
        }

        public GeneralLedger ConvertSalesInvoicesToCashBasis2(params DateTime[] dates)
        {
            if (!GetTransactions().Any(x => x.Transaction is SalesInvoice)) return this;

            var list = new LinkedList<GeneralLedgerTransaction>();

            var database = ApplicationData.Instance.Businesses.Get(FileID);

            var salesInvoiceLines = GetTransactions().Where(x => x.Transaction is SalesInvoice).GroupBy(x => x.SalesInvoiceAsTransaction.Key).ToDictionary(x => x.Key, x => x.Where(y => !y.IsBalancing && !y.GeneralLedgerAccount.IsAccountsReceivable).ToArray());
            // line above -> we need !y.GeneralLedgerAccount.IsAccountsReceivable to handle withholding tax - https://forum.manager.io/t/withholding-tax-confusion/60288
            var salesInvoiceTotals = salesInvoiceLines.ToDictionary(x => x.Key, x => x.Value.Sum(y => y.TransactionAmount)*-1m);

            foreach (var date in dates)
            {
                foreach (var e in GetTransactions().Where(x => x.Date <= date && x.GeneralLedgerAccount.IsAccountsReceivable && x.SalesInvoice != null).GroupBy(x => x.SalesInvoice))
                {
                    var transactions = e.ToArray();

                    var balance = e.Sum(x => x.AccountAmount);
                    if (balance != 0m)
                    {
                        var firstTransaction = e.First();

                        var latePaymentFees = e.Where(x => x.Transaction is LatePaymentFee).Sum(x => x.AccountAmount);
                        if (latePaymentFees != 0m)
                        {
                            if (balance >= latePaymentFees)
                            {
                                balance -= latePaymentFees;
                            }
                            else
                            {
                                latePaymentFees = balance;
                                balance = 0m;
                            }

                            list.AddLast(new GeneralLedgerTransaction(
                                database: database,
                                date: date,
                                generalLedgerAccount: database.Single<ProfitAndLossStatementAccountLatePaymentFees>(),
                                transactionAmount: latePaymentFees,
                                transactionCurrency: firstTransaction.TransactionCurrency,
                                customer: firstTransaction.Customer,
                                salesInvoice: e.Key,
                                cashBasisAdjustment: true
                            ));

                            list.AddLast(new GeneralLedgerTransaction(
                                database: database,
                                date: date,
                                generalLedgerAccount: firstTransaction.GeneralLedgerAccount,
                                customer: firstTransaction.Customer,
                                salesInvoice: firstTransaction.SalesInvoice,
                                transactionAmount: latePaymentFees * -1m,
                                transactionCurrency: firstTransaction.TransactionCurrency,
                                cashBasisAdjustment: true
                            ));
                        }

                        if (balance != 0m && salesInvoiceTotals.ContainsKey(e.Key.Key))
                        {
                            var total = 0m;
                            var salesInvoiceTotal = salesInvoiceTotals[e.Key.Key];
                            if (salesInvoiceTotal != 0m)
                            {
                                var multiplier = balance / salesInvoiceTotal;
                                for (int i = 0; i < salesInvoiceLines[e.Key.Key].Length; i++)
                                {
                                    var salesInvoiceLine = salesInvoiceLines[e.Key.Key][i];

                                    var transactionAmount = salesInvoiceLine.TransactionCurrency.Round(salesInvoiceLine.TransactionAmount * multiplier);

                                    var transaction = new GeneralLedgerTransaction(
                                        database: database,
                                        date: date,
                                        generalLedgerAccount: salesInvoiceLine.GeneralLedgerAccount,
                                        inventoryItem: salesInvoiceLine.InventoryItem,
                                        nonInventoryItem: salesInvoiceLine.NonInventoryItem,
                                        inventoryKit: salesInvoiceLine.InventoryKit,
                                        fixedAsset: salesInvoiceLine.FixedAsset,
                                        intangibleAsset: salesInvoiceLine.IntangibleAsset,
                                        specialAccount: salesInvoiceLine.SpecialAccount,
                                        capitalAccount: salesInvoiceLine.CapitalAccount,
                                        transactionLine: salesInvoiceLine.TransactionLine,
                                        trackingCode: salesInvoiceLine.Division,
                                        transactionAmount: transactionAmount * -1,
                                        transactionCurrency: salesInvoiceLine.TransactionCurrency,
                                        transaction: salesInvoiceLine.Transaction,
                                        isTaxTransaction: salesInvoiceLine.IsTaxTransaction,
                                        customer: salesInvoiceLine.Customer,
                                        taxCode: salesInvoiceLine.TaxCode,
                                        taxComponent: salesInvoiceLine.TaxComponent,
                                        reportingCategory: salesInvoiceLine.ReportingCategory,
                                        reportingCategoryReversed: salesInvoiceLine.ReportingCategoryReversed,
                                        isReversedTaxTransaction: salesInvoiceLine.IsReversedTaxTransaction,
                                        salesInvoice: e.Key,
                                        cashBasisAdjustment: true
                                    );

                                    list.AddLast(transaction);

                                    total += transaction.TransactionAmount;
                                }

                                if (balance != total)
                                {
                                    var transaction = new GeneralLedgerTransaction(
                                        database: database,
                                        date: date,
                                        generalLedgerAccount: database.Single<ProfitAndLossStatementAccountRoundingExpense>(),
                                        customer: firstTransaction.Customer,
                                        salesInvoice: e.Key,
                                        transactionAmount: balance - total,
                                        transactionCurrency: firstTransaction.AccountCurrency,
                                        transaction: e.Key,
                                        cashBasisAdjustment: true
                                    );

                                    list.AddLast(transaction);

                                    total += transaction.TransactionAmount;
                                }

                                var contraTransaction = new GeneralLedgerTransaction(
                                        database: database,
                                        date: date,
                                        generalLedgerAccount: firstTransaction.GeneralLedgerAccount,
                                        customer: firstTransaction.Customer,
                                        salesInvoice: e.Key,
                                        transactionAmount: total * -1m,
                                        transactionCurrency: firstTransaction.AccountCurrency,
                                        transaction: e.Key,
                                        cashBasisAdjustment: true
                                    );

                                list.AddLast(contraTransaction);
                            }
                        }
                    }
                }

                var baseCurrency = database.Single<BaseCurrency>();
                foreach (var e in GetTransactions().Where(x => x.Date <= date && x.Transaction is BillableTime).GroupBy(x => new { x.GeneralLedgerAccount, x.Customer }))
                {
                    var baseAmount = e.Sum(x => x.BaseAmount);
                    list.AddLast(new GeneralLedgerTransaction(
                        database: database,
                        date: date,
                        generalLedgerAccount: e.Key.GeneralLedgerAccount,
                        customer: e.Key.Customer,
                        transactionAmount: baseAmount * -1m,
                        transactionCurrency: baseCurrency,
                        cashBasisAdjustment: true
                    ));
                }
            }

            foreach (var e in list.ToArray())
            {
                if (e.Date == DateTime.MaxValue) continue;
                list.AddLast(new GeneralLedgerTransaction(
                    database: database,
                    date: e.Date.AddDays(1),
                    generalLedgerAccount: e.GeneralLedgerAccount,
                    customer: e.Customer,
                    salesInvoice: e.SalesInvoice,
                    inventoryItem: e.InventoryItem,
                    nonInventoryItem: e.NonInventoryItem,
                    inventoryKit: e.InventoryKit,
                    fixedAsset: e.FixedAsset,
                    intangibleAsset: e.IntangibleAsset,
                    specialAccount: e.SpecialAccount,
                    capitalAccount: e.CapitalAccount,
                    transactionLine: e.TransactionLine,
                    trackingCode: e.Division,
                    transactionAmount: e.TransactionAmount * -1,
                    transactionCurrency: e.TransactionCurrency,
                    transaction: e.Transaction,
                    isTaxTransaction: e.IsTaxTransaction,
                    taxCode: e.TaxCode,
                    taxComponent: e.TaxComponent,
                    reportingCategory: e.ReportingCategory,
                    reportingCategoryReversed: e.ReportingCategoryReversed,
                    isReversedTaxTransaction: e.IsReversedTaxTransaction,
                    cashBasisAdjustment: e.CashBasisAdjustment
                ));
            }

            return new GeneralLedger(this, list.ToArray());
        }

        public GeneralLedger ConvertPurchaseInvoicesToCashBasis2(params DateTime[] dates)
        {
            if (!GetTransactions().Any(x => x.Transaction is PurchaseInvoice)) return this;

            var list = new LinkedList<GeneralLedgerTransaction>();

            var database = ApplicationData.Instance.Businesses.Get(FileID);

            var purchaseInvoiceLines = GetTransactions().Where(x => x.Transaction is PurchaseInvoice).GroupBy(x => x.PurchaseInvoiceAsTransaction).ToDictionary(x => x.Key, x => x.Where(y => !y.IsBalancing && !y.GeneralLedgerAccount.IsAccountsPayable).ToArray());
            // line above -> we need !y.GeneralLedgerAccount.IsAccountsPayable to handle withholding tax - https://forum.manager.io/t/withholding-tax-confusion/60288
            var purchaseInvoiceTotals = purchaseInvoiceLines.ToDictionary(x => x.Key, x => x.Value.Sum(y => y.TransactionAmount) * -1m);

            foreach (var date in dates)
            {
                foreach (var e in GetTransactions().Where(x => x.Date <= date && x.GeneralLedgerAccount.IsAccountsPayable && x.PurchaseInvoice != null).GroupBy(x => x.PurchaseInvoice))
                {
                    var transactions = e.ToArray();

                    var balance = e.Sum(x => x.AccountAmount);
                    if (balance != 0m)
                    {
                        var firstTransaction = e.First();

                        if (balance != 0m)
                        {
                            var total = 0m;
                            var purchaseInvoiceTotal = purchaseInvoiceTotals[e.Key];
                            if (purchaseInvoiceTotal != 0m)
                            {
                                var multiplier = balance / purchaseInvoiceTotal;
                                for (int i = 0; i < purchaseInvoiceLines[e.Key].Length; i++)
                                {
                                    var purchaseInvoiceLine = purchaseInvoiceLines[e.Key][i];

                                    var transactionAmount = purchaseInvoiceLine.TransactionCurrency.Round(purchaseInvoiceLine.TransactionAmount * multiplier);

                                    var transaction = new GeneralLedgerTransaction(
                                        database: database,
                                        date: date,
                                        generalLedgerAccount: purchaseInvoiceLine.GeneralLedgerAccount,
                                        inventoryItem: purchaseInvoiceLine.InventoryItem,
                                        nonInventoryItem: purchaseInvoiceLine.NonInventoryItem,
                                        inventoryKit: purchaseInvoiceLine.InventoryKit,
                                        fixedAsset: purchaseInvoiceLine.FixedAsset,
                                        intangibleAsset: purchaseInvoiceLine.IntangibleAsset,
                                        specialAccount: purchaseInvoiceLine.SpecialAccount,
                                        capitalAccount: purchaseInvoiceLine.CapitalAccount,
                                        transactionLine: purchaseInvoiceLine.TransactionLine,
                                        trackingCode: purchaseInvoiceLine.Division,
                                        transactionAmount: transactionAmount * -1,
                                        transactionCurrency: purchaseInvoiceLine.TransactionCurrency,
                                        transaction: purchaseInvoiceLine.Transaction,
                                        isTaxTransaction: purchaseInvoiceLine.IsTaxTransaction,
                                        supplier: purchaseInvoiceLine.Supplier,
                                        taxCode: purchaseInvoiceLine.TaxCode,
                                        taxComponent: purchaseInvoiceLine.TaxComponent,
                                        reportingCategory: purchaseInvoiceLine.ReportingCategory,
                                        reportingCategoryReversed: purchaseInvoiceLine.ReportingCategoryReversed,
                                        isReversedTaxTransaction: purchaseInvoiceLine.IsReversedTaxTransaction,
                                        purchaseInvoice: e.Key,
                                        cashBasisAdjustment: true
                                    );

                                    list.AddLast(transaction);

                                    total += transaction.TransactionAmount;
                                }

                                var contraTransaction = new GeneralLedgerTransaction(
                                        database: database,
                                        date: date,
                                        generalLedgerAccount: firstTransaction.GeneralLedgerAccount,
                                        supplier: firstTransaction.Supplier,
                                        purchaseInvoice: e.Key,
                                        transactionAmount: total * -1m,
                                        transactionCurrency: firstTransaction.AccountCurrency,
                                        transaction: e.Key,
                                        cashBasisAdjustment: true
                                    );

                                list.AddLast(contraTransaction);
                            }
                        }
                    }
                }                
            }

            foreach (var e in list.ToArray())
            {
                if (e.Date == DateTime.MaxValue) continue;
                list.AddLast(new GeneralLedgerTransaction(
                    database: database,
                    date: e.Date.AddDays(1),
                    generalLedgerAccount: e.GeneralLedgerAccount,
                    supplier: e.Supplier,
                    purchaseInvoice: e.PurchaseInvoice,
                    inventoryItem: e.InventoryItem,
                    nonInventoryItem: e.NonInventoryItem,
                    inventoryKit: e.InventoryKit,
                    fixedAsset: e.FixedAsset,
                    intangibleAsset: e.IntangibleAsset,
                    specialAccount: e.SpecialAccount,
                    capitalAccount: e.CapitalAccount,
                    transactionLine: e.TransactionLine,
                    trackingCode: e.Division,
                    transactionAmount: e.TransactionAmount * -1,
                    transactionCurrency: e.TransactionCurrency,
                    transaction: e.Transaction,
                    isTaxTransaction: e.IsTaxTransaction,
                    taxCode: e.TaxCode,
                    taxComponent: e.TaxComponent,
                    reportingCategory: e.ReportingCategory,
                    reportingCategoryReversed: e.ReportingCategoryReversed,
                    isReversedTaxTransaction: e.IsReversedTaxTransaction,
                    cashBasisAdjustment: e.CashBasisAdjustment
                ));
            }

            return new GeneralLedger(this, list.ToArray());
        }

        public IEnumerator<GeneralLedgerTransaction> GetEnumerator()
        {
            return GetTransactions().AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetTransactions().GetEnumerator();
        }

        internal class BalanceDue
        {
            public decimal Amount;
        }

        internal class TransactionToAllocate
        {
            public GeneralLedgerTransaction Transaction;
            public decimal Amount;
        }
    }
}