using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerAccountView<T> : NakedObjectsWithSimpleSearch where T : BaseGeneralLedgerAccountView<T>, new()
    {
        [InheritedProtoMember(300)] public Guid? BalanceSheetAccount;
        [InheritedProtoMember(327)] public Guid? ProfitAndLossAccount;
        [InheritedProtoMember(301)] public DateTime? From;
        [InheritedProtoMember(302)] public DateTime To;
        [InheritedProtoMember(303)] public bool CashBasis;
        [InheritedProtoMember(305)] public bool Transactions;
        [InheritedProtoMember(306)] public Guid? SubAccount;
        [InheritedProtoMember(317)] public Guid? Division;
        [InheritedProtoMember(319)] public Guid? TaxCode;
        [InheritedProtoMember(332)] public string TaxComponent;
        [InheritedProtoMember(329)] public Guid? Investment; // This is used when drilling down into P&L unrealized investment gains (losses) account
        [InheritedProtoMember(320)] public bool? TaxTransactions;
        [InheritedProtoMember(321)] public bool? IsSale;
        [InheritedProtoMember(328)] public Tuple<Guid, Guid> InterAccountTransferPair;
        [InheritedProtoMember(330)] public bool BaseAmounts;
        [InheritedProtoMember(331)] public DateTime? AlternativeCurrencyRevaluationDate;        

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .DisposeFixedAssets()
                .DisposeIntangibleAssets();

            if (AlternativeCurrencyRevaluationDate.HasValue)
            {
                if (AlternativeCurrencyRevaluationDate.Value > DateTime.MinValue)
                {
                    generalLedger = generalLedger.Revaluate(From ?? DateTime.MinValue, AlternativeCurrencyRevaluationDate.Value.AddDays(-1));
                }
            }
            else
            {
                generalLedger = generalLedger.Revaluate(From ?? DateTime.MinValue, To);
            }

            if (CashBasis)
            {
                var dates = new List<DateTime>();
                if (From.HasValue && From.Value > DateTime.MinValue) dates.Add(From.Value.AddDays(-1));
                dates.Add(To);
                generalLedger = generalLedger.AutomaticallyMatchSalesInvoices().AutomaticallyMatchPurchaseInvoices().ConvertSalesInvoicesToCashBasis2(dates.ToArray()).ConvertPurchaseInvoicesToCashBasis2(dates.ToArray());
            }

            var transactions = generalLedger.Where(x => x.Date <= To);
            if (BalanceSheetAccount.HasValue) transactions = transactions.Where(x => x.BalanceSheetAccount.Key == BalanceSheetAccount.Value);
            if (ProfitAndLossAccount.HasValue) transactions = transactions.Where(x => x.ProfitAndLossAccount?.Key == ProfitAndLossAccount.Value);
            if (From.HasValue) transactions = transactions.Where(x => x.Date >= From);

            if (TaxCode.HasValue) transactions = transactions.Where(x => x.TaxCode?.Key == TaxCode.Value);
            if (TaxComponent != null) transactions = transactions.Where(x => x.TaxComponent == TaxComponent);
            if (TaxTransactions.HasValue) transactions = transactions.Where(x => x.IsTaxTransaction == TaxTransactions.Value);
            if (IsSale.HasValue) transactions = transactions.Where(x => x.IsSale == IsSale.Value);
            if (SubAccount.HasValue) transactions = transactions.Where(x => x.SubAccount?.Key == SubAccount.Value);
            if (Investment.HasValue) transactions = transactions.Where(x => x.Investment?.Key == Investment.Value);
            if (Division.HasValue) transactions = transactions.Where(x => x.Division?.Key == Division.Value);

            if (InterAccountTransferPair != null)
            {
                transactions = transactions.Where(x => x.InterAccountTransferPair?.Item1.Key == InterAccountTransferPair.Item1 && x.InterAccountTransferPair?.Item2.Key == InterAccountTransferPair.Item2);
            }

            var interAccountTransferAccount = database.Single<BalanceSheetInterAccountTransfers>();
            var investmentGainsLosses = database.Single<ProfitAndLossStatementCapitalGainsOnInvestments>();
            var currencyGainsLosses = database.Single<ProfitAndLossStatementAccountCurrencyGainsLosses>();

            if (!Transactions && transactions.All(x => x.SubAccount != null))
            {
                var accountBalances = transactions
                    .GroupBy(x => new { x.GeneralLedgerAccount, x.SubAccount })
                    .Select(x => new { Account = x.Key.SubAccount, AccountCurrency = x.First().AccountCurrency, BaseBalance = x.Sum(y => y.BaseAmount), AccountBalance = x.Sum(y => y.AccountAmount) })
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
            else if (!Transactions && BalanceSheetAccount == interAccountTransferAccount.Key)
            {
                var accountBalances = transactions
                    .GroupBy(x => x.InterAccountTransferPair)
                    .Select(x => new { x.Key, Balance = x.Sum(y => y.BaseAmount) })
                    .OrderByDescending(x => x.Balance != 0m)
                    .ThenByDescending(x => x.Balance)
                    .Select(x => new InterAccountTransferPairBalance()
                    {
                        AccountPair = x.Key,
                        Balance = x.Balance
                    })
                    .ToArray();

                context.Set<Array>(accountBalances);
            }
            /*
            else if (!Transactions && ProfitAndLossAccount == investmentGainsLosses.Key)
            {
                var baseCurrency = Manager.ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();

                var transactionByInvestment = generalLedger
                    .Where(x => x.GeneralLedgerAccount.HasInvestments)
                    .GroupBy(x => x.Investment.Key)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                var investmentQuantities = transactionByInvestment.ToDictionary(x => x.Key, x => x.Value.Sum(y => y.Qty ?? 0m));
                var investmentCost = transactionByInvestment.ToDictionary(x => x.Key, x => x.Value.Sum(y => y.BaseAmount));
                var investmentUnrealizedGain = generalLedger.Where(x => x.Date <= To && x.GeneralLedgerAccount.IsControlAccountForInvestments).GroupBy(x => x.Investment.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));

                var balances = transactions
                    .GroupBy(x => x.Investment)
                    .Select(x => new UnrealizedInvestmentGainLoss(baseCurrency)
                    {
                        Investment = x.Key,
                        Qty = investmentQuantities.ContainsKey(x.Key.Key) ? investmentQuantities[x.Key.Key] : 0m,
                        TotalCost = investmentCost.ContainsKey(x.Key.Key) ? investmentCost[x.Key.Key] : 0m,
                        ClosingUnrealizedGain = investmentUnrealizedGain.ContainsKey(x.Key.Key) ? investmentUnrealizedGain[x.Key.Key] : 0m,
                        IncrementForThePeriod = x.Sum(y => y.BaseAmount)
                    })
                    .ToArray();

                context.Set<Array>(balances);
            }
            */
            else if (!Transactions && ProfitAndLossAccount == currencyGainsLosses.Key)
            {
                var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();

                var balances = generalLedger
                    .Where(x => x.Date <= To)
                    //.Where(x => x.Transaction != null || x.Date < From)
                    .Where(x => x.AccountCurrency is ManagerServer.Model.ForeignCurrency)
                    .GroupBy(x => x.ForeignCurrencyAccount)
                    .Select(x => new CurrencyGainLoss(baseCurrency)
                    {
                        GeneralLedgerAccount = x.First().GeneralLedgerAccount,
                        ForeignCurrencyAccount = x.First().ForeignCurrencyAccount,
                        Currency = (ManagerServer.Model.ForeignCurrency)x.First().AccountCurrency,
                        CurrencyBalance = x.Sum(y => y.AccountAmount),
                        BaseBalance = x.Where(x => x.Transaction != null || x.Date < From).Sum(x => x.BaseAmount),
                        IncrementForThePeriod = x.Where(x => x.Transaction == null && x.Date > From).Sum(y => y.BaseAmount)*-1m
                    })
                    .Where(x => x.CurrencyBalance != 0m || x.BaseBalance != 0m || x.IncrementForThePeriod != 0m)
                    .OrderBy(x => x.GeneralLedgerAccount.GetCodeAndName())
                    .ThenBy(x => x.ForeignCurrencyAccount.GetCodeAndName())
                    .ToArray();
               
                context.Set<Array>(balances);
            }
            else
            {
                if (!BaseAmounts) transactions = transactions.Where(x => x.AccountAmount != 0m);
                context.Set<Array>(transactions.OrderByDescending(x => x.Date).ToArray());
            }

            var columns = context.Get<Column[]>();
            var rows = context.Get<Array>();
            foreach (var e in columns)
            {
                if (!e.CanEnsureCells(rows))
                {
                    e.Visible = false;
                }
            }

            base.InnerGet4(context);
        }

        [Default]
        public string[] GetName(InterAccountTransferPairBalance[] rows)
        {
            return rows.Select(x => x.AccountPair.Item1.GetCodeAndName() + " ⟷ " + x.AccountPair.Item2.GetCodeAndName()).ToArray();
        }

        [Default, Right, Sum]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetBalance(InterAccountTransferPairBalance[] rows)
        {
            var referrer = this.ToUrl();
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => new Tuple<decimal, Currency, BusinessTemplate>(
                x.Balance,
                baseCurrency,
                new T()
                {
                    Business = Business,
                    BalanceSheetAccount = BalanceSheetAccount,
                    Transactions = true,
                    To = To,
                    InterAccountTransferPair = new Tuple<Guid, Guid>(x.AccountPair.Item1.Key, x.AccountPair.Item2.Key),
                    Referrer = referrer,
                    CashBasis = CashBasis
                }
            )).ToArray();
        }

        [Icon("fa-edit")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => TransactionViewer.GetEditHandler(Business, x.Transaction, referrer)).ToArray();
        }

        [Icon("fa-eye")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => TransactionViewer.GetViewHandler(Business, x.Transaction, referrer)).ToArray();
        }

        [Default, MinWidth, Center, WhitespaceNoWrap]
        [Guid("594487b2-e46c-4a1d-8d14-5603d9015bf8")]
        public DateTime[] GetDate(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("3e220c05-5731-4103-aa3e-49903df09d97")]
        public string[] GetTransaction(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction?.GetTransactionName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("8ced7c1d-12e4-4332-a21d-7358e963cda7")]
        public string[] GetAccount(GeneralLedgerTransaction[] rows)
        {
            var retainedEarningsAccount = ApplicationData.Businesses.Get(Business).Single<BalanceSheetRetainedEarningsAccount>();
            if (BalanceSheetAccount != retainedEarningsAccount.Key) return rows.Select(x => default(string)).ToArray();
            return rows.Select(x => x.ProfitAndLossAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("a5251b08-c6c2-4139-8546-252b8dbec194")]
        public string[] GetBankOrCashAccount(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.BankAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("191ff2fb-95b3-4481-8563-f5ff9ed2c57c")]
        public string[] GetExpenseClaimPayer(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.ExpenseClaimPayer?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("7a07810d-68b6-44c8-9c96-68ec8c38128f")]
        public string[] GetCustomer(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("1c6af28e-0df4-4b04-9e2c-911610bcbc27")]
        public string[] GetSupplier(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("8fc60480-2341-471c-9ef3-45c597fad063")]
        public string[] GetEmployee(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Employee?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("18e57e7a-8e30-4a6d-ad62-dbb079fb4f55")]
        public string[] GetInventoryKit(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryKit?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("52abfeab-4e8c-4dcf-b4ee-e26367de142d")]
        public string[] GetInventoryItem(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryItem?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("7f925f01-de2c-4bbb-a6d7-ce73f6c189dc")]
        public string[] GetInvestment(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Investment?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("9dccabd1-c2fa-49a8-84e1-0da56af735b8")]
        public string[] GetFixedAsset(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.FixedAsset?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("3b7ea400-6f70-4a44-8ba7-bc9659c0c63f")]
        public string[] GetIntangibleAsset(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.IntangibleAsset?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("05b9dcb9-baf2-4b38-88cd-01ba5decd2f4")]
        public string[] GetCapitalAccount(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.CapitalAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("447c80e0-547c-4eaf-9a24-6ce098e7c181")]
        public string[] GetSpecialAccount(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SpecialAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("53680a9c-64d2-4ca9-b697-979b7db02e45")]
        public string[] GetDescription(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Default, HideColumnIfAllEmpty, MinWidth, WhitespaceNoWrap, Center]
        [Guid("b0740469-332c-43eb-82a4-e461b99794da")]
        public string[] GetTax(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Default, HideColumnIfAllEmpty, Center, WhitespaceNoWrap]
        [Guid("94114767-7485-427e-b924-6c4440782a76")]
        public decimal?[] GetQty(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty).ToArray();
        }

        [Default, HideColumnIfAllEmpty, Right, WhitespaceNoWrap]
        [Name(nameof(Strings.AcquisitionCost))]
        [Guid("de27fcdd-5aa9-4401-b845-4baa7b89a447")]
        public decimal?[] GetPurchaseCost(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.PurchaseCost).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Name(nameof(Strings.Amount))]
        [Guid("b42cd9f4-0d0d-41d2-bb22-6bb4740ae763")]
        public Tuple<decimal, Currency>[] GetTransactionAmount(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => !BaseAmounts && x.TransactionCurrency != x.AccountCurrency ? new Tuple<decimal, Currency>(Math.Abs(x.TransactionAmount), x.TransactionCurrency) : null).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Name(nameof(Strings.Amount))]
        [Guid("d8c17c80-18ad-43d4-b8be-685095f14185")]
        public Tuple<decimal, Currency>[] GetCurrencyAmount(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => BaseAmounts && x.AccountCurrency is not BaseCurrency ? new Tuple<decimal, Currency>(Math.Abs(x.AccountAmount), x.AccountCurrency) : null).ToArray();
        }

        [Default, Center, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("77a90bfc-c20a-47d2-b2b2-b0a70362682e")]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>[] GetExchangeRate(GeneralLedgerTransaction[] rows)
        {
            if (!BaseAmounts) return rows.Select(x => default(Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>)).ToArray();
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => GetExchangeRate(x.AccountAmount, x.BaseAmount, x.AccountCurrency, baseCurrency)).ToArray();
        }

        private Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>> GetExchangeRate(decimal accountAmount, decimal baseAmount, Currency accountCurrency, BaseCurrency baseCurrency)
        {
            if (accountAmount == 0m || baseAmount == 0m) return null;

            var x = Math.Abs(accountAmount) / Math.Abs(baseAmount);
            var y = Math.Abs(baseAmount) / Math.Abs(accountAmount);

            if (x > 1)
            {
                return new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>(
                    new Tuple<decimal, Currency>(1, baseCurrency),
                    new Tuple<decimal, Currency>(accountCurrency.Round(x), accountCurrency)
                );
            }
            else
            {
                return new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>(
                    new Tuple<decimal, Currency>(1, accountCurrency),
                    new Tuple<decimal, Currency>(baseCurrency.Round(y), baseCurrency)
                );
            }
        }

        [Default, Right, Bold, Sum, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Guid("4e1c8f59-15fb-4d5c-9b89-c9601f348618")]
        public Tuple<decimal, Currency>[] GetDebit(GeneralLedgerTransaction[] rows)
        {
            if (BaseAmounts)
            {
                var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
                return rows.Select(x => x.BaseAmount > 0m ? new Tuple<decimal, Currency>(x.BaseAmount, baseCurrency) : null).ToArray();
            }
            else
            {
                return rows.Select(x => x.AccountAmount > 0m ? new Tuple<decimal, Currency>(x.AccountAmount, x.AccountCurrency) : null).ToArray();
            }
        }

        [Default, Right, Bold, Sum, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Guid("ed407725-37be-4095-95e7-220de84ee99e")]
        public Tuple<decimal, Currency>[] GetCredit(GeneralLedgerTransaction[] rows)
        {
            if (BaseAmounts)
            {
                var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
                return rows.Select(x => x.BaseAmount < 0m ? new Tuple<decimal, Currency>(x.BaseAmount * -1m, baseCurrency) : null).ToArray();
            }
            else
            {
                return rows.Select(x => x.AccountAmount < 0m ? new Tuple<decimal, Currency>(x.AccountAmount * -1m, x.AccountCurrency) : null).ToArray();
            }
        }

        [Default, Right, WhitespaceNoWrap, RunningTotal2]
        public Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>[] GetBalance(GeneralLedgerTransaction[] rows)
        {
            var balance = 0m;
            if (BaseAmounts)
            {
                balance = rows.Sum(x => x.BaseAmount);
            }
            else
            {
                balance = rows.Sum(x => x.AccountAmount);
            }

            var output = new Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>[rows.Length];
            for (int i = 0; i < output.Length; i++)
            {
                if (balance >= 0m)
                {
                    output[i] = new Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>(balance, ManagerServer.Model.Enums.DebitCredit.Debit);
                }
                else
                {
                    output[i] = new Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>(balance * -1m, ManagerServer.Model.Enums.DebitCredit.Credit);
                }

                if (BaseAmounts)
                {
                    balance -= rows[i].BaseAmount;
                }
                else
                {
                    balance -= rows[i].AccountAmount;
                }
            }
            return output;
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
        public Tuple<decimal, Currency>[] GetBaseBalance(Balance[] rows)
        {
            if (rows.All(x => x.AccountBalance.Item2 is BaseCurrency || x.AccountBalance.Item1 == 0m)) return rows.Select(x => default(Tuple<decimal, Currency>)).ToArray();
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => new Tuple<decimal, Currency>(x.BaseBalance, baseCurrency)).ToArray();
        }

        [Default, Center, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("5dc10d5a-6a92-4df9-aa27-ec785c5e66eb")]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetExchangeRate(Balance[] rows)
        {
            if (rows.All(x => x.AccountBalance.Item2 is BaseCurrency || x.AccountBalance.Item1 == 0m)) return rows.Select(x => default(Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>)).ToArray();

            return GetExchangeRate(rows.Select(x => x.AccountBalance.Item2 as ForeignCurrency).ToArray());
        }

        [Default, Right, Sum, Bold]
        [Name(nameof(Strings.Balance))]
        [Guid("7e9914b0-e5f2-4c7c-a656-c5df277951d9")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetAccountBalance(Balance[] rows)
        {
            var referrer = this.ToUrl();

            return rows.Select(x => new Tuple<decimal, Currency, BusinessTemplate>(
                x.AccountBalance.Item1,
                x.AccountBalance.Item2,
                new T()
                {
                    Business = Business,
                    BalanceSheetAccount = BalanceSheetAccount,
                    ProfitAndLossAccount = ProfitAndLossAccount,
                    Transactions = true,
                    SubAccount = x.Account.Key,
                    From = From,
                    To = To,
                    Referrer = referrer,
                    CashBasis = CashBasis
                }
                )).ToArray();
        }

        public sealed class InterAccountTransferPairBalance : IsInactive
        {
            public Tuple<BankOrCashAccount, BankOrCashAccount> AccountPair;
            public decimal Balance;

            bool IsInactive.IsInactive => AccountPair.Item1.Inactive || AccountPair.Item2.Inactive;
        }

        public sealed class UnrealizedInvestmentGainLoss
        {
            public BaseCurrency BaseCurrency { get; init; }

            public UnrealizedInvestmentGainLoss(BaseCurrency baseCurrency)
            {
                BaseCurrency = baseCurrency;
            }

            public Investment Investment;
            public decimal Qty;
            public decimal MarketPrice => Qty != 0m ? BaseCurrency.Round(MarketValue / Qty) : 0m;
            public decimal MarketValue => Qty != 0m ? TotalCost + ClosingUnrealizedGain : 0m;
            public decimal AverageCost => Qty != 0m ? BaseCurrency.Round(TotalCost / Qty) : 0m;
            public decimal TotalCost;
            public decimal OpeningUnrealizedGain => ClosingUnrealizedGain - IncrementForThePeriod;
            public decimal IncrementForThePeriod;
            public decimal ClosingUnrealizedGain;
        }

        [Default, WhitespaceNoWrap]
        public string[] GetInvestment(UnrealizedInvestmentGainLoss[] rows)
        {
            return rows.Select(x => x.Investment.GetCodeAndName()).ToArray();
        }

        [Default, Center, WhitespaceNoWrap, MinWidth]
        public decimal?[] GetQty(UnrealizedInvestmentGainLoss[] rows)
        {
            return rows.Select(x => x.Qty != 0m ? x.Qty : default(decimal?)).ToArray();
        }

        [Default, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency>[] GetMarketPrice(UnrealizedInvestmentGainLoss[] rows)
        {
            return rows.Select(x => x.MarketPrice != 0m ? new Tuple<decimal, Currency>(x.MarketPrice, x.BaseCurrency) : null).ToArray();
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency>[] GetMarketValue(UnrealizedInvestmentGainLoss[] rows)
        {
            return rows.Select(x => x.MarketValue != 0m ? new Tuple<decimal, Currency>(x.MarketValue, x.BaseCurrency) : null).ToArray();
        }

        [Default, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency>[] GetAverageCost(UnrealizedInvestmentGainLoss[] rows)
        {
            return rows.Select(x => x.AverageCost != 0m ? new Tuple<decimal, Currency>(x.AverageCost, x.BaseCurrency) : null).ToArray();
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency>[] GetTotalCost(UnrealizedInvestmentGainLoss[] rows)
        {
            return rows.Select(x => x.TotalCost != 0m ? new Tuple<decimal, Currency>(x.TotalCost, x.BaseCurrency) : null).ToArray();
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency>[] GetOpeningUnrealizedGainsLosses(UnrealizedInvestmentGainLoss[] rows)
        {
            return rows.Select(x => new Tuple<decimal, Currency>(x.OpeningUnrealizedGain, x.BaseCurrency)).ToArray();
        }

        [Default, Bold, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetIncrementsOrDecrementsForThePeriod(UnrealizedInvestmentGainLoss[] rows)
        {
            var referrer = this.ToUrl();

            return rows.Select(x => new Tuple<decimal, Currency, BusinessTemplate>(
                x.IncrementForThePeriod,
                x.BaseCurrency,
                new T()
                {
                    Business = Business,
                    Transactions = true,
                    Investment = x.Investment.Key,
                    From = From,
                    To = To,
                    CashBasis = CashBasis,
                    Division = Division,
                    ProfitAndLossAccount = ProfitAndLossAccount,
                    Referrer = referrer
                })
            ).ToArray();
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency>[] GetClosingUnrealizedGainsLosses(UnrealizedInvestmentGainLoss[] rows)
        {
            return rows.Select(x => new Tuple<decimal, Currency>(x.ClosingUnrealizedGain, x.BaseCurrency)).ToArray();
        }

        public sealed class CurrencyGainLoss
        {
            public BaseCurrency BaseCurrency { get; init; }

            public CurrencyGainLoss(BaseCurrency baseCurrency)
            {
                BaseCurrency = baseCurrency;
            }

            public IGeneralLedgerAccount GeneralLedgerAccount;
            public NamedObject ForeignCurrencyAccount;
            public ForeignCurrency Currency;
            public decimal CurrencyBalance;
            public decimal BaseBalance;
            public decimal RevaluatedBalance => BaseBalance - IncrementForThePeriod;
            public decimal IncrementForThePeriod;
        }

        [Default]
        public string[] GetAccount(CurrencyGainLoss[] rows)
        {
            return rows.Select(x => $"{x.GeneralLedgerAccount.GetCodeAndName()} — {x.ForeignCurrencyAccount.GetCodeAndName()}").ToArray();
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetForeignBalance(CurrencyGainLoss[] rows)
        {
            var referrer = this.ToUrl();

            return rows.Select(x => new Tuple<decimal, Currency, BusinessTemplate>(
                x.CurrencyBalance,
                x.Currency,
                new T()
                {
                    Business = Business,
                    To = To,
                    BalanceSheetAccount = x.GeneralLedgerAccount.Key,
                    SubAccount = x.ForeignCurrencyAccount.Key,
                    Transactions = true,
                    Referrer = referrer
                }
            )).ToArray();
        }

        [Default, Center, HideColumnIfAllEmpty]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetExchangeRate(CurrencyGainLoss[] rows)
        {
            return GetExchangeRate(rows.Select(x => x.Currency).ToArray());
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency>[] GetAdjustedBalance(CurrencyGainLoss[] rows)
        {
            return rows.Select(x => new Tuple<decimal, Currency>(x.RevaluatedBalance, x.BaseCurrency)).ToArray();
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetCurrentBalance(CurrencyGainLoss[] rows)
        {
            var referrer = this.ToUrl();

            return rows.Select(x => new Tuple<decimal, Currency, BusinessTemplate>(
                x.BaseBalance,
                x.BaseCurrency,
                new T()
                {
                    Business = Business,
                    To = To,
                    BalanceSheetAccount = x.GeneralLedgerAccount.Key,
                    SubAccount = x.ForeignCurrencyAccount.Key,
                    Transactions = true,
                    BaseAmounts = true,
                    AlternativeCurrencyRevaluationDate = From.Value,
                    Referrer = referrer
                }
            )).ToArray();
        }

        [Default, Bold, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, DebitCredit>[] GetUnrealizedGains(CurrencyGainLoss[] rows)
        {
            var referrer = this.ToUrl();

            return rows.Select(x => new Tuple<decimal, DebitCredit>(
                Math.Abs(x.IncrementForThePeriod),
                x.IncrementForThePeriod >= 0 ? DebitCredit.Debit : DebitCredit.Credit
            )).ToArray();
        }
        
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetExchangeRate(ForeignCurrency[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            var exchangeRates = database.OfType<ExchangeRate>()
                .Where(x => x.Date <= To)
                .Where(x => x.Currency.HasValue)
                .Where(x => x.ExchangeRateValue > 0m)
                .OrderByDescending(x => x.Date)
                .GroupBy(x => x.Currency.Value)
                .ToDictionary(x => x.Key, x => x.First());

            var referrer = this.ToUrl();

            var output = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i] == null) continue;

                var row = rows[i];

                var httpHandler = new Settings.Currencies.ExchangeRates.ExchangeRates()
                {
                    Business = Business,
                    Date = To,
                    ForeignCurrency = row.Key,
                    Referrer = referrer
                };

                if (exchangeRates.TryGetValue(row.Key, out ExchangeRate exchangeRate))
                {
                    if (!exchangeRate.ExchangeRateIsInverse)
                    {
                        output[i] = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(
                            new Tuple<decimal, Currency>(1m, row),
                            new Tuple<decimal, Currency>(exchangeRate.ExchangeRateValue, baseCurrency),
                            httpHandler
                        );
                    }
                    else
                    {
                        output[i] = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(
                            new Tuple<decimal, Currency>(1m, baseCurrency),
                            new Tuple<decimal, Currency>(exchangeRate.ExchangeRateValue, row),
                            httpHandler
                        );
                    }
                }
                else
                {
                    output[i] = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(
                            new Tuple<decimal, Currency>(1m, baseCurrency),
                            new Tuple<decimal, Currency>(1m, row),
                            httpHandler
                        );
                }
            }

            return output;
        }
    }
}