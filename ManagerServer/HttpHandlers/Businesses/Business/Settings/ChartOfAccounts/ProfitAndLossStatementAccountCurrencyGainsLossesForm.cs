using System;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.Account), nameof(Strings.CurrencyGainsLosses))]
    [Guide("This form allows to rename built-in `CurrencyGainsLosses` account.")]
    [Guide("To access this form, go to `Settings`, then `ChartOfAccounts`, then click `Edit` button for `CurrencyGainsLosses` account.")]
    [Guide("The form contains the following fields:")]
    [Fields(typeof(ProfitAndLossStatementAccountCurrencyGainsLosses))]
    [Guide("Click `Update` button to save your changes.")]
    [Guide("This account cannot be deleted, it is automatically added to your `ChartOfAccounts` when you have at least one foreign currency.")]
    [LinkGuide("For more information see:", typeof(Currencies.ForeignCurrencies.ForeignCurrencies))]
    internal sealed class ProfitAndLossStatementAccountCurrencyGainsLossesForm : NakedVueForm<ProfitAndLossStatementAccountCurrencyGainsLosses>
    {
    }
}