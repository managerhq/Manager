using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ForecastProfitAndLossStatement
{
    [ProtoContract]
    [Title(nameof(Strings.ForecastProfitAndLossStatement), nameof(Strings.Transactions))]
    [Guide("Shows forecast transactions for a specific account within the date range.")]
    [Guide("Displays projected income and expense transactions from forecasts.")]
    [Columns]
    internal sealed class ForecastProfitAndLossStatementTransactions : ObjectTable<ManagerServer.Model.Forecast.ForecastTransaction>
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid Account;
        [ProtoMember(4)] public bool ReverseSign;

        protected override Forecast.ForecastTransaction[] GetObjects()
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BaseCurrency>();

            return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.Forecast>()
                .SelectMany(x => x.GetForecastTransactions(baseCurrency, From, To))
                .Where(x => x.Account == Account)
                .OrderBy(x => x.Date)
                .ToArray();
        }

        protected override BusinessTemplate GetEdit(Forecast.ForecastTransaction o, string referrer)
        {
            return new Settings.Forecasts.ForecastForm() { Business = Business, Key = o.Key, Referrer = referrer };
        }

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("85c8c90b-6339-46a9-9640-cf63c4d125d6")]
        public DateTime GetDate(Forecast.ForecastTransaction o) => o.Date;

        [Guid("02c7a605-6d0a-485c-9cf3-09553bf382b9")]
        public string GetDescription(Forecast.ForecastTransaction o) => o.Description;

        [Bold, Right, Sum, WhitespaceNoWrap]
        [Guid("9d97cdd9-db31-4ca0-bfac-a967b51714f9")]
        public decimal GetAmount(Forecast.ForecastTransaction o) => ReverseSign ? -o.Amount : o.Amount;        
    }
}