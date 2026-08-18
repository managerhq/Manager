using ManagerServer.Api.Businesses.Business.BankAndCashAccounts;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System;

namespace ManagerServer.HttpHandlers.Businesses.Business.BankAndCashAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.BankOrCashAccount), nameof(Strings.View))]
    [Guide("The bank or cash account view screen displays comprehensive details of an individual bank or cash account, including its current balance, recent transactions, and configuration settings.")]
    [Guide("To access this screen, navigate to the **Bank and Cash Accounts** tab and click the **View** button next to the account you want to examine.")]
    internal sealed class BankOrCashAccountView : DefaultView<GetBankOrCashAccountView>
    {
    }
}
