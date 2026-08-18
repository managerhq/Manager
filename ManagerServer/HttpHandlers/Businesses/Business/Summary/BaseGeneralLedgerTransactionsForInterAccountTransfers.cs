using System.Linq;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForInterAccountTransfers : BaseGeneralLedgerTransactionsForCurrencyGainsLossTransactions
    {
        protected override void InnerGet4(Context context)
        {
            var interAccountTransferAccount = ApplicationData.Businesses.Get(Business).Single<BalanceSheetInterAccountTransfers>();
            if (GetRoot().InterAccountTransferPair == null && GeneralLedgerAccount == interAccountTransferAccount.Key)
            {
                var accountBalances = GetGeneralLedgerTransactions()
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

            base.InnerGet4(context);
        }

        public sealed class InterAccountTransferPairBalance : IsInactive
        {
            public Tuple<BankOrCashAccount, BankOrCashAccount> AccountPair;
            public decimal Balance;

            bool IsInactive.IsInactive => AccountPair.Item1.Inactive || AccountPair.Item2.Inactive;
        }

        [Default]
        public string[] GetName(InterAccountTransferPairBalance[] rows)
        {
            return rows.Select(x => x.AccountPair.Item1.GetCodeAndName() + " ⟷ " + x.AccountPair.Item2.GetCodeAndName()).ToArray();
        }

        [Default, Right, Sum]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetBalance(InterAccountTransferPairBalance[] rows)
        {
            var referrer = ToUrl();
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => new Tuple<decimal, Currency, BusinessTemplate>(
                x.Balance,
                baseCurrency,
                GetHttpHandler(new Tuple<Guid, Guid>(x.AccountPair.Item1.Key, x.AccountPair.Item2.Key), referrer)
            )).ToArray();
        }

        private BusinessTemplate GetHttpHandler(Tuple<Guid, Guid> interAccountTransferPair, string referrer)
        {
            var businessTemplate = Serializer.NonGeneric.DeepClone(this) as BaseGeneralLedgerTransactionsInheritable;
            businessTemplate.InterAccountTransferPair = interAccountTransferPair;
            businessTemplate.Referrer = referrer;
            businessTemplate.SortBy = null;
            return businessTemplate;
        }
    }
}