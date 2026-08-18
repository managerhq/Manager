using ManagerServer.Attributes;
using ManagerServer.Model;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.BalanceSheetAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.StartingBalance), nameof(Strings.BalanceSheetAccount), nameof(Strings.Edit))]
    [Guide("This form is the place where you can set up starting balance for balance sheet account.")]
    [Guide("The form includes the following fields:")]
    [Fields(typeof(BalanceSheetAccountStartingBalance))]
    internal sealed class BalanceSheetAccountStartingBalanceForm : NakedVueForm<BalanceSheetAccountStartingBalance>
    {
        protected override bool CanHaveImage() => true;
    }
}