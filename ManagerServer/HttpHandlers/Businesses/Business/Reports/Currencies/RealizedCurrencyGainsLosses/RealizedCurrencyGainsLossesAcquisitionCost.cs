using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.Currencies.RealizedCurrencyGainsLosses
{
    [ProtoContract]
    [Title(nameof(Strings.RealizedCurrencyGainsAndLosses), nameof(Strings.AcquisitionCost))]
    [Guide("Shows how the acquisition cost of a realized currency gain or loss was calculated.")]
    [Guide("Acquisition cost is not taken from a single transaction. Every movement on the foreign currency account is pooled, and the cost released on settlement is the settled amount valued at the weighted average rate of that pool.")]
    [Guide("Rows are listed newest first, so the settlement being explained appears at the top and the movements that built the pool follow beneath it.")]
    [Columns]
    internal sealed class RealizedCurrencyGainsLossesAcquisitionCost : NakedObjectsWithSimpleSearch
    {
        [ProtoMember(1)] public Guid Currency;
        [ProtoMember(2)] public DateTime ToDate;
        [ProtoMember(3)] public int Realization;

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);

            var pool = new List<RealizedCurrencyGains.Step>();
            var found = false;

            foreach (var step in RealizedCurrencyGains.Replay(database, Business, ToDate).Where(x => x.Currency.Key == Currency))
            {
                pool.Add(step);

                if (step.RealizationIndex == Realization)
                {
                    found = true;
                    break;
                }
            }

            // Newest first, so the settlement being explained heads the list and the pool that built it follows.
            pool.Reverse();

            context.Set<Array>(found ? pool.ToArray() : Array.Empty<RealizedCurrencyGains.Step>());

            base.InnerGet4(context);
        }

        [Icon("fa-edit")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetEdit(RealizedCurrencyGains.Step[] rows)
        {
            var referrer = ToUrl();
            return rows.Select(x => TransactionViewer.GetEditHandler(Business, x.Entry.Transaction, referrer)).ToArray();
        }

        [Icon("fa-eye")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetView(RealizedCurrencyGains.Step[] rows)
        {
            var referrer = ToUrl();
            return rows.Select(x => TransactionViewer.GetViewHandler(Business, x.Entry.Transaction, referrer)).ToArray();
        }

        [Default, MinWidth, Center, WhitespaceNoWrap]
        [Guid("6d5992ab-ebe5-4649-b649-56e2f31fdc2c")]
        [Guide("Shows the date the movement hit the foreign currency account.")]
        public DateTime[] GetDate(RealizedCurrencyGains.Step[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("464ba67a-884f-49f8-9cb6-2fbe57616836")]
        [Guide("Shows the transaction behind the movement.")]
        public string[] GetTransaction(RealizedCurrencyGains.Step[] rows)
        {
            return rows.Select(x => x.Entry.Transaction?.GetTransactionName()).ToArray();
        }

        [Default, Right, WhitespaceNoWrap]
        [Guid("ded0c816-f682-40dd-9e03-16ae2a939834")]
        [Guide("Shows the amount this movement added to or removed from the pool, in the foreign currency.")]
        public Tuple<decimal, Currency>[] GetAmount(RealizedCurrencyGains.Step[] rows)
        {
            return rows.Select(x => new Tuple<decimal, Currency>(x.ForeignAmount, x.Entry.AccountCurrency)).ToArray();
        }

        [Default, Right, WhitespaceNoWrap]
        [Name(nameof(Strings.Balance))]
        [Guid("452a3423-3aaa-4140-9aa1-ff2945ecdf2d")]
        [Guide("Shows the pool after this movement — the foreign currency held, and what it cost in base currency. Dividing one by the other gives the weighted average rate used to value the next settlement.")]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>[] GetBalance(RealizedCurrencyGains.Step[] rows)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();

            return rows.Select(x => new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>(
                new Tuple<decimal, Currency>(x.ForeignBalance, x.Entry.AccountCurrency),
                new Tuple<decimal, Currency>(x.BaseBalance, baseCurrency)
            )).ToArray();
        }

        [Default, Right, Bold, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Guid("3755a2f3-438e-4def-841d-44b3c385677b")]
        [Guide("Shows the cost released from the pool when the settlement was made.")]
        public Tuple<decimal, Currency>[] GetAcquisitionCost(RealizedCurrencyGains.Step[] rows)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();

            return rows.Select(x => x.IsRealization ? new Tuple<decimal, Currency>(Math.Abs(x.Cost), baseCurrency) : null).ToArray();
        }

        [Default, Right, Bold, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Guid("aa699762-7a37-4cc7-ab54-f8bd3ac3a56d")]
        [Guide("Shows the gain or loss realized on settlement — the difference between the cost released from the pool and the amount actually settled.")]
        public Tuple<decimal, Currency>[] GetRealizedGain(RealizedCurrencyGains.Step[] rows)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();

            return rows.Select(x => x.IsRealization ? new Tuple<decimal, Currency>(x.Gain, baseCurrency) : null).ToArray();
        }
    }
}
