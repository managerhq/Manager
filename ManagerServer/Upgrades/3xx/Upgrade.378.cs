using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using System.Text;
using System.IO;
using ManagerServer.Model;
using System.Reflection;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade378(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var baseCurrency = objects.Single<BaseCurrency>();

            var journalEntryLines = new List<JournalEntry.Line>();

            foreach (var e in objects.OfType<BalanceSheetAccount>().ToArray())
            {
                var startingBalance = e.Obsolete_StartingBalance2;
                if (e.Obsolete_StartingBalanceType2 == Model.Enums.DebitCredit.Credit) startingBalance *= -1;

                if (startingBalance == 0m) continue;

                journalEntryLines.Add(new JournalEntry.Line()
                {
                    Account = e.Key,
                    Debit = baseCurrency.Round(startingBalance),
                    Division = e.Obsolete_Division
                });
            }

            foreach (var e in objects.OfType<BankOrCashAccount>().ToArray())
            {
                if (e.Obsolete_StartingBalance2 == 0m) continue;

                var transactionCurrency = objects.SingleOrDefault<ForeignCurrency>(e.Currency);
                var baseAmount = e.Obsolete_StartingBalance2;
                if (transactionCurrency != null)
                {
                    var exchangeRate = e.Obsolete_ExchangeRate2;
                    if (exchangeRate == 0m) exchangeRate = 1m;

                    if (e.Obsolete_ExchangeRateIsInverse2) baseAmount = baseCurrency.Round(e.Obsolete_StartingBalance2 / exchangeRate);
                    if (!e.Obsolete_ExchangeRateIsInverse2) baseAmount = baseCurrency.Round(e.Obsolete_StartingBalance2 * exchangeRate);
                }

                journalEntryLines.Add(new JournalEntry.Line()
                {
                    Account = typeof(BalanceSheetCashAtBankAccount).GetCustomAttribute<GuidAttribute>().Value,
                    BankOrCashAccount = e.Key,
                    Debit = baseAmount,
                    CurrencyAmount = e.Obsolete_StartingBalance2,
                });
            }

            foreach (var e in objects.OfType<CapitalAccount>().ToArray())
            {
                var startingBalance = e.Obsolete_StartingBalanceAmount2;
                if (e.Obsolete_StartingBalance2 == Model.Enums.StartingBalanceType.AmountToPay) startingBalance *= -1;

                if (startingBalance == 0m) continue;

                journalEntryLines.Add(new JournalEntry.Line()
                {
                    Account = objects.Single<BalanceSheetCapitalAccountsAccount>().Key,
                    CapitalAccount = e.Key,
                    Debit = startingBalance
                });
            }

            foreach (var e in objects.OfType<Customer>().ToArray())
            {
                if (e.Obsolete_StartingBalance2 <= 0m) continue;

                var transactionCurrency = objects.SingleOrDefault<ForeignCurrency>(e.Currency);
                var baseAmount = e.Obsolete_StartingBalance2;
                if (transactionCurrency != null)
                {
                    var exchangeRate = e.Obsolete_ExchangeRate2;
                    if (exchangeRate == 0m) exchangeRate = 1m;

                    if (e.Obsolete_ExchangeRateIsInverse2) baseAmount = baseCurrency.Round(e.Obsolete_StartingBalance2 / exchangeRate);
                    if (!e.Obsolete_ExchangeRateIsInverse2) baseAmount = baseCurrency.Round(e.Obsolete_StartingBalance2 * exchangeRate);
                }

                journalEntryLines.Add(new JournalEntry.Line()
                {
                    Account = objects.Single<BalanceSheetAccountsReceivableAccount>().Key,
                    AccountsReceivableCustomer = e.Key,
                    Debit = baseAmount * -1m,
                    CurrencyAmount = e.Obsolete_StartingBalance2,
                });
            }

            foreach (var e in objects.OfType<Employee>().ToArray())
            {
                var startingBalance = e.Obsolete_StartingBalanceAmount2;
                if (e.Obsolete_StartingBalance2 == Model.Enums.StartingBalanceType.AmountToPay) startingBalance *= -1;

                if (startingBalance == 0m) continue;

                var transactionCurrency = objects.SingleOrDefault<ForeignCurrency>(e.Currency);
                var baseAmount = startingBalance;
                if (transactionCurrency != null)
                {
                    var exchangeRate = e.Obsolete_ExchangeRate2;
                    if (exchangeRate == 0m) exchangeRate = 1m;

                    if (e.Obsolete_ExchangeRateIsInverse2) baseAmount = baseCurrency.Round(startingBalance / exchangeRate);
                    if (!e.Obsolete_ExchangeRateIsInverse2) baseAmount = baseCurrency.Round(startingBalance * exchangeRate);
                }

                journalEntryLines.Add(new JournalEntry.Line()
                {
                    Account = objects.Single<BalanceSheetEmployeeClearingAccount>().Key,
                    Employee = e.Key,
                    Debit = baseAmount,
                    CurrencyAmount = startingBalance,
                });
            }

            foreach (var e in objects.OfType<ExpenseClaimsPayer>().ToArray())
            {
                var startingBalance = e.Obsolete_StartingBalanceAmount2;
                if (e.Obsolete_StartingBalance2 == Model.Enums.StartingBalanceType.AmountToPay) startingBalance *= -1;

                if (startingBalance == 0m) continue;

                journalEntryLines.Add(new JournalEntry.Line()
                {
                    Account = objects.Single<BalanceSheetExpenseClaimsAccount>().Key,
                    ExpenseClaimPayer = e.Key,
                    Debit = startingBalance
                });
            }

            foreach (var e in objects.OfType<FixedAsset>().ToArray())
            {
                if (!e.Obsolete_StartingBalance2) continue;

                if (e.Obsolete_StartingBalanceAcquisitionCost2 != 0m)
                {
                    journalEntryLines.Add(new JournalEntry.Line()
                    {
                        Account = objects.Single<BalanceSheetFixedAssetsAtCostAccount>().Key,
                        FixedAsset = e.Key,
                        Debit = e.Obsolete_StartingBalanceAcquisitionCost2
                    });
                }

                if (e.Obsolete_StartingBalanceAccumulatedDepreciation2 != 0m)
                {
                    journalEntryLines.Add(new JournalEntry.Line()
                    {
                        Account = objects.Single<BalanceSheetFixedAssetsAccumulatedDepreciationAccount>().Key,
                        FixedAsset = e.Key,
                        Debit = e.Obsolete_StartingBalanceAccumulatedDepreciation2 * -1m
                    });
                }
            }

            foreach (var e in objects.OfType<IntangibleAsset>().ToArray())
            {
                if (e.Obsolete_StartingBalance2 != 0m)
                {
                    journalEntryLines.Add(new JournalEntry.Line()
                    {
                        Account = objects.Single<BalanceSheetIntangibleAssetsAtCostAccount>().Key,
                        IntangibleAsset = e.Key,
                        Debit = e.Obsolete_StartingBalance2
                    });
                }

                if (e.Obsolete_StartingBalanceAccumulatedAmortization2 != 0m)
                {
                    journalEntryLines.Add(new JournalEntry.Line()
                    {
                        Account = objects.Single<BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount>().Key,
                        IntangibleAsset = e.Key,
                        Debit = e.Obsolete_StartingBalanceAccumulatedAmortization2 * -1m
                    });
                }
            }

            foreach (var e in objects.OfType<InventoryItem>().ToArray())
            {
                if (e.Obsolete_StartingBalance2 != null)
                {
                    foreach (var e2 in e.Obsolete_StartingBalance2.Where(x => x.Qty > 0m))
                    {
                        var startingBalance = e.Obsolete_StartingBalanceAverageCost2 * e2.Qty;
                        if (startingBalance < 0m) startingBalance = 0m;
                        startingBalance = baseCurrency.Round(startingBalance);

                        journalEntryLines.Add(new JournalEntry.Line()
                        {
                            Account = objects.Single<BalanceSheetInventoryOnHandAccount>().Key,
                            Obsolete_InventoryItem = e.Key,
                            Qty = e2.Qty,
                            Debit = startingBalance,
                            InventoryLocation = e2.InventoryLocation
                        });
                    }
                }
            }

            foreach (var e in objects.OfType<Investment>().ToArray())
            {
                if (e.Obsolete_StartingBalance2 == 0m && e.Obsolete_StartingBalanceTotalCost2 == 0m) continue;

                journalEntryLines.Add(new JournalEntry.Line()
                {
                    Account = objects.Single<BalanceSheetInvestmentsAccount>().Key,
                    Investment = e.Key,
                    Qty = e.Obsolete_StartingBalance2,
                    Debit = e.Obsolete_StartingBalanceTotalCost2
                });
            }

            foreach (var e in objects.OfType<SpecialAccount>().ToArray())
            {
                var startingBalance = e.Obsolete_StartingBalance2;
                if (e.Obsolete_StartingBalanceType2 == Model.Enums.DebitCredit.Credit) startingBalance *= -1;

                if (startingBalance == 0m) continue;

                var transactionCurrency = objects.SingleOrDefault<ForeignCurrency>(e.Currency);
                var baseAmount = startingBalance;
                if (transactionCurrency != null)
                {
                    var exchangeRate = e.Obsolete_ExchangeRate2;
                    if (exchangeRate == 0m) exchangeRate = 1m;

                    if (e.Obsolete_ExchangeRateIsInverse2) baseAmount = baseCurrency.Round(startingBalance / exchangeRate);
                    if (!e.Obsolete_ExchangeRateIsInverse2) baseAmount = baseCurrency.Round(startingBalance * exchangeRate);
                }

                journalEntryLines.Add(new JournalEntry.Line()
                {
                    Account = objects.Single<BalanceSheetSpecialAccountsAccount>().Key,
                    SpecialAccount = e.Key,
                    Debit = baseAmount,
                    CurrencyAmount = startingBalance,
                });
            }

            foreach (var e in objects.OfType<Supplier>().ToArray())
            {
                if (e.Obsolete_StartingBalance2 <= 0m) continue;

                var transactionCurrency = objects.SingleOrDefault<ForeignCurrency>(e.Currency);
                var baseAmount = e.Obsolete_StartingBalance2;
                if (transactionCurrency != null)
                {
                    var exchangeRate = e.Obsolete_ExchangeRate2;
                    if (exchangeRate == 0m) exchangeRate = 1m;

                    if (e.Obsolete_ExchangeRateIsInverse2) baseAmount = baseCurrency.Round(e.Obsolete_StartingBalance2 / exchangeRate);
                    if (!e.Obsolete_ExchangeRateIsInverse2) baseAmount = baseCurrency.Round(e.Obsolete_StartingBalance2 * exchangeRate);
                }

                journalEntryLines.Add(new JournalEntry.Line()
                {
                    Account = objects.Single<BalanceSheetAccountsPayableAccount>().Key,
                    AccountsPayableSupplier = e.Key,
                    Debit = baseAmount,
                    CurrencyAmount = e.Obsolete_StartingBalance2,
                });
            }

            if (journalEntryLines.Count > 0)
            {
                var balance = journalEntryLines.Sum(x => x.Debit - x.Credit);
                if (balance != 0m)
                {
                    journalEntryLines.Add(new JournalEntry.Line()
                    {
                        Account = new Guid("74dfd025-d68e-4a99-9c78-5d43e17c0e09"), // Retained earnings
                        Debit = balance * -1m,
                    });
                }

                foreach (var e in journalEntryLines)
                {
                    if (e.Debit < 0m)
                    {
                        e.Credit = e.Debit * -1m;
                        e.Debit = 0m;
                    }

                    if (e.CurrencyAmount < 0m)
                    {
                        e.CurrencyAmount *= -1m;
                    }
                }

                return new[] {
                    new JournalEntry()
                    {
                        Key = new Guid("3273aded-a786-4116-8ba5-67fa800558ab"), // Starting balance journal entry key
                        Narration = "Starting balances",
                        Date = DateTime.MinValue.AddYears(1),
                        Lines = journalEntryLines.ToArray(),
                        QuantityColumn = journalEntryLines.Any(x => x.Qty != 0m)
                    }
                };
            }

            return null;
        }
    }
}
