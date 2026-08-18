using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Investments
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("e1cf69b4-4b7b-4d52-ab63-59cd20330a50")]
    [Title(nameof(Strings.Investments))]
    [Guide("The `Investments` tab is where you manage all financial investments owned by your business, such as stocks, bonds, mutual funds, or other securities.")]
    [Guide("This tab provides a comprehensive view of your investment portfolio, tracking quantities owned, market values, and investment performance over time.")]
    [TabScreenshot(icon: "fa-chart-pie", name: "Investments")]
    [Header("Getting Started")]
    [Guide("To create a new investment, click the `New Investment` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.Investments), name: nameof(Strings.NewInvestment))]
    [Guide("If you already own investments when you begin using Manager, you can enter their existing quantities and cost basis through `Settings` → `Starting Balances` → `Investments`.")]
    [LinkGuide("Learn more:", typeof(Settings.StartingBalances.Investments.InvestmentStartingBalanceList))]
    [Header("Automatic Accounts")]
    [Guide("When you create your first investment, Manager automatically adds two essential accounts to your `Chart of Accounts`:")]
    [Guide("• `Investments` - A `Balance Sheet` account that shows the current market value of all your investments")]
    [Guide("• `Investment Gains (Losses)` - A `Profit and Loss Statement` account that captures both realized gains (from sales) and unrealized gains (from market value changes)")]
    [Guide("The `Investments` account balance is automatically calculated based on the market prices you enter in `Settings` → `Investment Market Prices`. This ensures your balance sheet always reflects current market values.")]
    [LinkGuide("Learn more:", typeof(Settings.InvestmentMarketPrices.InvestmentMarketPrices))]
    [Guide("The `Investment Gains (Losses)` account automatically captures the difference between your investments' market value and their cost basis. This includes:")]
    [Guide("• Realized gains/losses - Actual profits or losses when you sell investments")]
    [Guide("• Unrealized gains/losses - Paper profits or losses from market value changes on investments you still own")]
    [Guide("To analyze your investment performance in detail, use the `Investment Gains (Losses)` report under the `Reports` tab. This report separates realized gains (from completed sales) and unrealized gains (from market value changes).")]
    [Header("Recording Transactions")]
    [Guide("To record an investment purchase:")]
    [Guide("1. Go to the `Payments` tab and click `New Payment`")]
    [Guide("2. In the payment form, select `Investments` as the account")]
    [Guide("3. Choose the specific investment from the dropdown that appears")]
    [Guide("4. Enter the quantity purchased and the total amount paid")]
    [SelectAccountScreenshot(accountName: nameof(Strings.Investments), prepend: nameof(Strings.Investment))]
    [Guide("Important: To record the quantity of shares or units purchased, you must enable the `Qty` column by checking the `Qty` checkbox at the bottom of the payment form. This allows you to track both the amount paid and the number of units acquired.")]
    [CheckboxScreenshot("Column-Qty")]
    [Guide("To record an investment sale, use a `Receipt` transaction and select the `Investments` account. The process mirrors purchasing but records money coming in rather than going out. Enter a negative quantity to reduce your holdings.")]
    [Header("Investment List Columns")]
    [Guide("The `Investments` tab displays the following columns:")]
    [Columns]
    [Header("Foreign Currency Investments")]
    [Guide("Many investments are traded on foreign currency markets. In Manager, all investment values are displayed in your base currency, regardless of the market they trade on.")]
    [Guide("An investment is not a foreign currency. While an investment may trade on a foreign currency market, the same investment can trade on multiple markets across different currencies simultaneously (dual-listed companies, futures contracts, commodities, precious metals, etc.).")]
    [Guide("When a foreign currency weakens, the price of the investment typically rises to compensate for the forex loss, maintaining equilibrium. An investment's value might be rising in foreign currency terms but remain flat in your base currency.")]
    [Guide("This is why Manager tracks all investment performance in your base currency—it provides a consistent basis for evaluating returns across your entire portfolio.")]
    internal sealed class Investments : NakedObjectsWithAutomaticRows<Investment>
    {
        [WarnIfNotUnique]
        [Guid("6bd1b417-5f57-437d-8d55-51edd6c7786d")]        
        [Guide("The investment code or ticker symbol. This helps identify investments quickly and can be used for sorting and searching. Examples: AAPL for Apple stock or FUND001 for a mutual fund.")]
        public string[] GetCode(Investment[] rows)
        {
            return rows.Select(x => x.Code).ToArray();
        }

        [Default]
        [Guid("4b0ae605-109f-444d-b537-38ae50308b5c")]
        [Guide("The full name or description of the investment. This should clearly identify what the investment is, such as \"Apple Inc. Common Stock\" or \"Growth Fund Series A\".")]
        public string[] GetName(Investment[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Guid("f4a80d78-7c11-42dc-b755-6ac410f514ac")]
        [Guide("Shows which control account manages this investment. For most businesses, this will display `Investments`. Custom control accounts can be created to separate different types of investments (e.g., `Long-term Investments` vs `Trading Securities`).")]
        public string[] GetControlAccount(ManagerServer.Model.Investment[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => (database.SingleOrDefault<ManagerServer.Model.ControlAccountForInvestments>(x.ControlAccount) as ManagerServer.Model.NamedObject ?? database.Single<ManagerServer.Model.BalanceSheetInvestmentsAccount>()).GetName()).ToArray();
        }

        [Right]
        [Guid("2809d760-40e3-4cb8-882e-7a6d70626809"), Default, Bold]
        [Guide("The total quantity of shares, units, or other investment units currently owned. This is automatically calculated from all purchase and sale transactions. Click the quantity to see a detailed transaction history.")]
        public Tuple<decimal, BusinessTemplate>[] GetQty(Investment[] rows)
        {
            var referrer = this.ToUrl();

            var balances = GetBalances(rows);
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                balances[x].Qty,
                new InvestmentTransactions()
                {
                    To = DateTime.MaxValue,
                    Business = Business,
                    Investment = x.Key,
                    Referrer = referrer
                }
            )).ToArray();
        }

        [Default]
        [Right]
        [Guid("329e22d5-9452-489a-a5b0-5447880eca25")]
        [Guide("The current market price per unit of the investment. Click to update market prices.")]
        [LinkGuide("Learn more:", typeof(Settings.InvestmentMarketPrices.InvestmentMarketPrices))]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetMarketPrice(Investment[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var marketPrices = database.OfType<InvestmentMarketPrice>().Where(x => x.Investment.HasValue && x.MarketPrice > 0m).GroupBy(x => x.Investment).ToDictionary(x => x.Key, x => x.Last());

            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            var output = new List<Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>>();
            foreach (var e in rows)
            {
                var httpHandler = new Settings.InvestmentMarketPrices.InvestmentMarketPrices() { Business = Business, Investment = e.Key, Referrer = referrer };

                if (marketPrices.TryGetValue(e.Key, out InvestmentMarketPrice marketPrice))
                {                    
                    var foreignCurrency = database.SingleOrDefault<ForeignCurrency>(marketPrice.Currency);
                    var marketPrice1 = new Tuple<decimal, Currency>(marketPrice.MarketPrice, foreignCurrency as Currency ?? baseCurrency);
                    Tuple<decimal, Currency> marketPrice2 = null;
                    if (foreignCurrency != null)
                    {
                        marketPrice2 = new Tuple<decimal, Currency>(marketPrice.GetMarketPriceInBaseCurrency(baseCurrency) ?? 0, baseCurrency);
                    }
                    output.Add(new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(marketPrice1, marketPrice2, httpHandler));
                }
                else
                {
                    output.Add(new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(null, null, httpHandler));
                }

            }
            return output.ToArray();
        }

        private Dictionary<Investment, Balance> getBalances = null;
        public Dictionary<Investment, Balance> GetBalances(Investment[] rows)
        {
            if (getBalances == null)
            {
                var referrer = this.ToUrl();
                var database = ApplicationData.Businesses.Get(Business);
                var baseCurrency = database.Single<BaseCurrency>();
                var investymentMarketPrices = database.OfType<InvestmentMarketPrice>().Where(x => x.Investment.HasValue && x.MarketPrice > 0m).GroupBy(x => x.Investment).ToDictionary(x => x.Key, x => x.Last());

                var profitAndLossRealizedInvestmentGains = database.Single<ManagerServer.Model.ProfitAndLossStatementCapitalGainsOnInvestments>();

                var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsControlAccountForInvestments).GroupBy(x => x.Investment).ToDictionary(x => x.Key, x => x.ToArray());

                var output = new Dictionary<Investment, Balance>();
                foreach (var e in rows)
                {
                    var investmentTransactions = Array.Empty<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
                    if (transactions.ContainsKey(e)) investmentTransactions = transactions[e];

                    var qty = investmentTransactions.Sum(x => x.Qty ?? 0m);

                    var marketValue = 0m;

                    if (investymentMarketPrices.TryGetValue(e.Key, out var marketPrice))
                    {
                        var baseMarketPrice = marketPrice.MarketPrice;
                        var foreignCurrency = database.SingleOrDefault<ForeignCurrency>(marketPrice.Currency);
                        if (foreignCurrency != null)
                        {
                            baseMarketPrice = marketPrice.GetMarketPriceInBaseCurrency(baseCurrency) ?? 0m;
                        }

                        marketValue = baseCurrency.Round(baseMarketPrice * qty);
                    }
                    else
                    {
                        marketValue = investmentTransactions.Sum(x => x.BaseAmount);
                    }

                    output.Add(e, new Balance()
                    {
                        Qty = qty,
                        MarketValue = new Tuple<decimal, Currency>(marketValue, baseCurrency)
                    });
                }

                getBalances = output;
            }
            return getBalances;
        }        

        [Sum, Right]
        [Guid("353209d3-d329-4986-996d-682b1ec45dbe"), Default]
        [Guide("The current market value of your investment holdings, calculated by multiplying the quantity owned by the current market price. This total represents what your investments are worth if sold at current market prices.")]
        public Tuple<decimal, Currency>[] GetMarketValue(Investment[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].MarketValue).ToArray();
        }

        public sealed class Balance
        {
            public decimal Qty;
            public Tuple<decimal, Currency> MarketValue;
        }        
    }
}
