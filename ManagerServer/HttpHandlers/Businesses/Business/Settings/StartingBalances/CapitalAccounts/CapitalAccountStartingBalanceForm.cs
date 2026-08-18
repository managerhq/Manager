using ManagerServer.Attributes;
using ManagerServer.Model;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.CapitalAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.StartingBalance), nameof(Strings.CapitalAccount), nameof(Strings.Edit))]
    [Guide("This form is the place where you can set up starting balance for capital account.")]
    [Guide("The form includes the following fields:")]
    [Fields(typeof(CapitalAccountStartingBalance))]
    internal sealed class CapitalAccountStartingBalanceForm : NakedVueForm<CapitalAccountStartingBalance>
    {
        protected override bool CanHaveImage() => true;
    }
}