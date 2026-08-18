using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.RealizedCurrencyGainsLosses;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.Currencies.RealizedCurrencyGainsLosses
{
    [ProtoContract]
    [Title(nameof(Strings.RealizedCurrencyGainsAndLosses))]
    [Guide("The **Realized Currency Gains and Losses** report shows foreign exchange gains and losses that have been realized through actual transactions.")]
    [Guide("This report calculates the difference between the acquisition cost and settlement amount for foreign currency transactions. Gains or losses are realized when foreign currency balances are converted back to your base currency or used to settle transactions.")]
    [Header("How it Works")]
    [Guide("When you hold assets or liabilities in foreign currencies, their value fluctuates with exchange rates. These fluctuations create unrealized gains or losses.")]
    [Guide("A gain or loss becomes realized when you actually convert the foreign currency or use it in a transaction. The realized amount is the difference between what you originally paid for the currency (acquisition cost) and what you received when settling it (settlement amount).")]
    [Header("Report Contents")]
    [Guide("The report displays three columns for each realized currency transaction:")]
    [Guide("• **Acquisition Cost** - The original cost in base currency when the foreign currency was acquired")]
    [Guide("• **Settlement Amount** - The actual amount in base currency when the foreign currency was settled")]
    [Guide("• **Realized Gain** - The difference between settlement amount and acquisition cost (positive for gains, negative for losses)")]
    [LinkGuide("For more information, see:", typeof(RealizedCurrencyGainsLossesForm))]
    internal sealed class RealizedCurrencyGainsLossesView : DefaultView<GetRealizedCurrencyGainsLossesView>
    {
    }
}