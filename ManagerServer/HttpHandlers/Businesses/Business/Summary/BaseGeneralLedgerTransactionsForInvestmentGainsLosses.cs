using System.Linq;
using ManagerServer;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Model.Enums;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForInvestmentGainsLosses : BaseGeneralLedgerTransactionsForInvestmentClosingBalanceTransactions
    {
        protected override void InnerGet4(Context context)
        {
            var investmentGainsLosses = ApplicationData.Businesses.Get(Business).Single<ProfitAndLossStatementCapitalGainsOnInvestments>();

            if (GeneralLedgerAccount == investmentGainsLosses.Key)
            {
                var database = ApplicationData.Businesses.Get(Business);
                var baseCurrency = database.Single<BaseCurrency>();

                var transactionByInvestment = new GeneralLedger(Business)
                    .Revaluate(GetRoot().From ?? DateTime.MinValue, GetRoot().To)
                    .Where(x => x.Date <= GetRoot().To)
                    .Where(x => x.GeneralLedgerAccount.IsControlAccountForInvestments)
                    .GroupBy(x => x.Investment.Key)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                var balances = GetGeneralLedgerTransactions()
                    .GroupBy(x => x.Investment)
                    .Select(x => new InvestmentGainLoss(baseCurrency)
                    {
                        Investment = x.Key,
                        Qty = transactionByInvestment.ContainsKey(x.Key.Key) ? transactionByInvestment[x.Key.Key].Sum(x => x.Qty ?? 0m) : 0m,
                        MarketValue = transactionByInvestment.ContainsKey(x.Key.Key) ? transactionByInvestment[x.Key.Key].Sum(x => x.BaseAmount) : 0m,
                        IncrementForThePeriod = x.Sum(y => y.BaseAmount)
                    })
                    .ToArray();

                context.Set<Array>(balances);
            }

            base.InnerGet4(context);
        }

        public sealed class InvestmentGainLoss
        {
            public BaseCurrency BaseCurrency { get; init; }

            public InvestmentGainLoss(BaseCurrency baseCurrency)
            {
                BaseCurrency = baseCurrency;
            }

            public Investment Investment;
            public decimal Qty;
            public decimal MarketValue;
            public decimal IncrementForThePeriod;

            public decimal ClosingBalance => MarketValue + IncrementForThePeriod;
        }

        [Default, WhitespaceNoWrap]
        public string[] GetInvestment(InvestmentGainLoss[] rows)
        {
            return rows.Select(x => x.Investment.GetCodeAndName()).ToArray();
        }

        [Default, Center, WhitespaceNoWrap, MinWidth]
        public Tuple<decimal, BusinessTemplate>[] GetQty(InvestmentGainLoss[] rows)
        {
            var referrer = ToUrl();

            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                x.Qty,
                GetHttpHandlerWithInvestment(x.Investment.Key, referrer)
            )).ToArray();
        }

        private BusinessTemplate GetHttpHandlerWithInvestment(Guid investment, string referrer)
        {
            var businessTemplate = Serializer.NonGeneric.DeepClone(this) as BaseGeneralLedgerTransactionsInheritable;
            businessTemplate.Investment = investment;
            businessTemplate.From = null;
            businessTemplate.GeneralLedgerAccount = null;
            businessTemplate.Referrer = referrer;
            businessTemplate.SortBy = null;
            return businessTemplate;
        }

        [Default, Right, WhitespaceNoWrap]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetMarketPrice(InvestmentGainLoss[] rows)
        {
            var referrer = ToUrl();
            return BaseGeneralLedgerTransactionsForInvestments.GetInvestmentMarketPrices(GetRoot().To, Business, rows.Select(x => x.Investment).ToArray(), referrer);
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency>[] GetMarketValue(InvestmentGainLoss[] rows)
        {
            return rows.Select(x => x.MarketValue != 0m ? new Tuple<decimal, Currency>(x.MarketValue, x.BaseCurrency) : null).ToArray();
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<DebitCreditAmount, Currency, BusinessTemplate>[] GetClosingBalance(InvestmentGainLoss[] rows)
        {
            var referrer = ToUrl();
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => new Tuple<DebitCreditAmount, Currency, BusinessTemplate>(
                new DebitCreditAmount(x.ClosingBalance),
                baseCurrency,
                GetHttpHandlerForClosingBalance(x.Investment.Key, referrer)
            )).ToArray();
        }

        private BusinessTemplate GetHttpHandlerForClosingBalance(Guid investment, string referrer)
        {
            var businessTemplate = Serializer.NonGeneric.DeepClone(this) as BaseGeneralLedgerTransactionsInheritable;
            businessTemplate.ClosingBalanceInvestment = investment;
            businessTemplate.Referrer = referrer;
            businessTemplate.SortBy = null;
            return businessTemplate;
        }

        [Default, Bold, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, DebitCredit>[] GetGainLoss(InvestmentGainLoss[] rows)
        {
            return rows.Select(x => new Tuple<decimal, DebitCredit>(
                Math.Abs(x.IncrementForThePeriod),
                x.IncrementForThePeriod >= 0 ? DebitCredit.Debit : DebitCredit.Credit
            )).ToArray();
        }
    }
}