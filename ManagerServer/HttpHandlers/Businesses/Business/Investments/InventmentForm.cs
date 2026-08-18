using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Investments
{
    [ProtoContract]
    [Title(nameof(Strings.Investment), nameof(Strings.Edit))]
    [Guide("The `Investment` form allows you to create new investment records or modify existing ones in your portfolio.")]
    [Guide("Use this form to record details about your stocks, bonds, mutual funds, exchange-traded funds (ETFs), and other financial instruments.")]
    [Header("Key Information")]
    [Guide("Each investment record maintains important information such as the investment name, number of units held, and current market value.")]
    [Guide("The system automatically calculates your investment position based on the number of units and their market price.")]
    [Header("Tracking Performance")]
    [Guide("You can track both realized and unrealized gains or losses by updating market prices regularly.")]
    [Guide("Realized gains occur when you sell investments, while unrealized gains reflect changes in market value for investments you still hold.")]
    [Fields(typeof(ManagerServer.Model.Investment))]
    internal sealed class InventmentForm : NakedVueForm<ManagerServer.Model.Investment>
    {
    }
}