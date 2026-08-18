using System.Linq;
using ManagerServer;
using ManagerServer.Model;
using ManagerServer.Globalization;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForSubaccount : BaseGeneralLedgerTransactionsForInvestmentTransactions
    {
        protected override void InnerGet4(Context context)
        {
            if (!GetRoot().Subaccount.HasValue && GeneralLedgerAccount.HasValue)
            {
                var eligibleAccounts = new List<Guid?>();
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetCashAtBankAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetAccountsReceivableAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetBillableExpensesAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetBillableTimeAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetWithholdingTaxReceivableAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetAccountsPayableAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetWithholdingTaxPayableAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetEmployeeClearingAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetSpecialAccountsAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetFixedAssetsAtCostAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetFixedAssetsAccumulatedDepreciationAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetIntangibleAssetsAtCostAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetCapitalAccountsAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).Single<BalanceSheetExpenseClaimsAccount>().Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).SingleOrDefault<ControlAccountForBankAccounts>(GeneralLedgerAccount.Value)?.Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).SingleOrDefault<ControlAccountForCustomers>(GeneralLedgerAccount.Value)?.Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).SingleOrDefault<ControlAccountForSuppliers>(GeneralLedgerAccount.Value)?.Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).SingleOrDefault<ControlAccountForEmployees>(GeneralLedgerAccount.Value)?.Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).SingleOrDefault<ControlAccountForSpecialAccounts>(GeneralLedgerAccount.Value)?.Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).SingleOrDefault<ControlAccountForFixedAssets>(GeneralLedgerAccount.Value)?.Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).SingleOrDefault<ControlAccountForFixedAssetsAccumulatedDepreciation>(GeneralLedgerAccount.Value)?.Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).SingleOrDefault<ControlAccountForIntangibleAssets>(GeneralLedgerAccount.Value)?.Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).SingleOrDefault<ControlAccountForIntangibleAssetsAccumulatedAmortization>(GeneralLedgerAccount.Value)?.Key);
                eligibleAccounts.Add(ApplicationData.Businesses.Get(Business).SingleOrDefault<ControlAccountForCapitalAccounts>(GeneralLedgerAccount.Value)?.Key);

                if (eligibleAccounts.Contains(GeneralLedgerAccount.Value))
                {
                    var accountBalances = GetGeneralLedgerTransactions()
                            .GroupBy(x => x.SubAccount)
                            .Select(x => new { Account = x.Key, x.First().AccountCurrency, BaseBalance = x.Select(y => y.BaseAmount).SafeSum(), AccountBalance = x.Select(y => y.AccountAmount).SafeSum(), Qty = x.Select(y => y.Qty ?? 0m).SafeSum() })
                            .OrderByDescending(x => Math.Abs(x.BaseBalance))
                            .ThenBy(x => x.Account.IsInactive())
                            .ThenBy(x => x.Account.GetCodeAndName())
                            .Select(x => new Balance()
                            {
                                Account = x.Account,
                                AccountBalance = new Tuple<decimal, Currency>(x.AccountBalance, x.AccountCurrency),
                                BaseBalance = x.BaseBalance,
                                IsInactive = x.Account.IsInactive()
                            })
                            .ToArray();

                    context.Set<Array>(accountBalances);
                }
            }

            base.InnerGet4(context);
        }

        public sealed class Balance : IsInactive
        {
            public NamedObject Account;
            public decimal BaseBalance;
            public Tuple<decimal, Currency> AccountBalance;
            public bool IsInactive;

            bool IsInactive.IsInactive => IsInactive;
        }

        [Default]
        [Guid("4efa27b7-1bfc-4de2-bd74-86abc2c65815")]
        public string[] GetName(Balance[] rows)
        {
            return rows.Select(x => x.Account.GetCodeAndName()).ToArray();
        }

        [Default, Right, Sum, HideColumnIfAllEmpty]
        [Name(nameof(Strings.Balance))]
        [Guid("c0f18bd8-f1dd-463e-a55e-2c6591d18463")]
        public Tuple<DebitCreditAmount, Currency>[] GetBaseBalance(Balance[] rows)
        {
            if (rows.All(x => x.AccountBalance.Item2 is BaseCurrency || x.AccountBalance.Item1 == 0m)) return null;
            var baseCurrency = ApplicationData.Instance.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => new Tuple<DebitCreditAmount, Currency>(new DebitCreditAmount(x.BaseBalance), baseCurrency)).ToArray();
        }

        [Default, Center, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("5dc10d5a-6a92-4df9-aa27-ec785c5e66eb")]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetExchangeRate(Balance[] rows)
        {
            if (rows.All(x => x.AccountBalance.Item2 is BaseCurrency || x.AccountBalance.Item1 == 0m)) return null;

            var referrer = ToUrl();
            return GetExchangeRates(GetTo(), Business, rows.Select(x => x.AccountBalance.Item2 as ForeignCurrency).ToArray(), referrer);
        }

        [Default, Right, Sum, Bold]
        [Name(nameof(Strings.Balance))]
        [Guid("7e9914b0-e5f2-4c7c-a656-c5df277951d9")]
        public Tuple<DebitCreditAmount, Currency, BusinessTemplate>[] GetAccountBalance(Balance[] rows)
        {
            var referrer = ToUrl();

            return rows.Select(x => new Tuple<DebitCreditAmount, Currency, BusinessTemplate>(
                new DebitCreditAmount(x.AccountBalance.Item1),
                x.AccountBalance.Item2,
                GetHttpHandlerWithSubaccount(x.Account.Key, referrer)
            )).ToArray();
        }

        private BusinessTemplate GetHttpHandlerWithSubaccount(Guid subaccount, string referrer)
        {
            var businessTemplate = Serializer.NonGeneric.DeepClone(this) as BaseGeneralLedgerTransactionsInheritable;
            businessTemplate.Subaccount = subaccount;
            businessTemplate.Referrer = referrer;
            businessTemplate.Term = null;
            businessTemplate.SortBy = null;
            return businessTemplate;
        }

        private DateTime GetTo()
        {
            return GetRoot().To;
        }

        public static Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetExchangeRates(DateTime date, string fileId, ForeignCurrency[] rows, string referrer)
        {
            var database = ApplicationData.Instance.Businesses.Get(fileId);
            var baseCurrency = database.Single<BaseCurrency>();

            var startingExchangeRates = database.Single<StartingExchangeRates>();

            var exchangeRates = database.OfType<ExchangeRate>()
                .Where(x => x.Date <= date)
                .Where(x => x.Currency.HasValue)
                .Where(x => x.ExchangeRateValue > 0m)
                .OrderByDescending(x => x.Date)
                .GroupBy(x => x.Currency.Value)
                .ToDictionary(x => x.Key, x => x.First());

            var output = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i] == null) continue;

                var row = rows[i];

                if (exchangeRates.TryGetValue(row.Key, out ExchangeRate exchangeRate))
                {
                    output[i] = GetExchangeRate(fileId, exchangeRate, date, referrer, baseCurrency, row);
                }
                else if (startingExchangeRates.GetExchangeRate(row) != null)
                {
                    var startingExchangeRate = startingExchangeRates.GetExchangeRate(row);
                    if (!startingExchangeRate.ExchangeRateIsInverse)
                    {
                        output[i] = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(
                            new Tuple<decimal, Currency>(1m, row),
                            new Tuple<decimal, Currency>(startingExchangeRate.ExchangeRate, baseCurrency),
                            null
                        );
                    }
                    else
                    {
                        output[i] = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(
                            new Tuple<decimal, Currency>(1m, baseCurrency),
                            new Tuple<decimal, Currency>(startingExchangeRate.ExchangeRate, row),
                            null
                        );
                    }
                }
                else
                {
                    output[i] = GetExchangeRate(fileId, null, date, referrer, baseCurrency, row);
                }
            }

            return output;
        }

        public static Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate> GetExchangeRate(string fileId, ExchangeRate exchangeRate, DateTime date, string referrer, BaseCurrency baseCurrency, ForeignCurrency foreignCurrency)
        {
            var httpHandler = new Settings.Currencies.ExchangeRates.ExchangeRateForm()
            {
                Business = fileId,
                Referrer = referrer
            };

            if (exchangeRate != null)
            {
                if (exchangeRate.Date == date)
                {
                    httpHandler.Key = exchangeRate.Key;
                }
                else
                {
                    httpHandler.Date = date;
                    httpHandler.ExchangeRateValue = exchangeRate.ExchangeRateValue;
                    httpHandler.ExchangeRateIsInverse = exchangeRate.ExchangeRateIsInverse;
                    httpHandler.ForeignCurrency = foreignCurrency.Key;
                }

                if (!exchangeRate.ExchangeRateIsInverse)
                {
                    return new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(
                        new Tuple<decimal, Currency>(1m, foreignCurrency),
                        new Tuple<decimal, Currency>(exchangeRate.ExchangeRateValue, baseCurrency),
                        httpHandler
                    );
                }
                else
                {
                    return new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(
                        new Tuple<decimal, Currency>(1m, baseCurrency),
                        new Tuple<decimal, Currency>(exchangeRate.ExchangeRateValue, foreignCurrency),
                        httpHandler
                    );
                }
            }
            else if (date == DateTime.MaxValue)
            {
                return new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(
                            new Tuple<decimal, Currency>(1m, baseCurrency),
                            new Tuple<decimal, Currency>(1m, foreignCurrency),
                            null
                        );
            }
            else
            {
                httpHandler.Date = date;
                httpHandler.ExchangeRateValue = 1m;
                httpHandler.ForeignCurrency = foreignCurrency.Key;

                return new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(
                            new Tuple<decimal, Currency>(1m, baseCurrency),
                            new Tuple<decimal, Currency>(1m, foreignCurrency),
                            httpHandler
                        );
            }
        }
    }
}