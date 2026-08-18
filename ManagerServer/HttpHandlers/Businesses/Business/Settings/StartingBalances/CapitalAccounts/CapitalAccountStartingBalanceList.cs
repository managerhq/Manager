using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.CapitalAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(CapitalAccounts))]
    [Guid("6a56cd26-be5e-4932-99dd-b93cb4a40045")]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.CapitalAccounts))]
    [Guide("This screen allows you to set up starting balances for capital accounts that you have created under the **Capital Accounts** tab.")]
    [Guide("To create a new starting balance for a capital account, click the **New Starting Balance** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.CapitalAccounts), name: nameof(Strings.NewStartingBalance))]
    [Guide("You will be taken to the *Starting Balance* screen for the *Capital Account*.")]
    [LinkGuide("For more information, see:", typeof(CapitalAccountStartingBalanceForm))]
    internal sealed class CapitalAccountStartingBalanceList : NakedObjectsWithAutomaticRows<CapitalAccountStartingBalance>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewStartingBalance);
        }

        [Default]
        [Guid("211612b3-579f-4497-ab66-674961225337")]
        public NamedObject[] GetCapitalAccount(CapitalAccountStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<CapitalAccount>(x.CapitalAccount)).ToArray();
        }

        [Default, Right, Bold, Sum]
        [Guid("b1936107-abcb-4354-a8d1-a3ba83727e2a")]
        public Tuple<decimal, Currency>[] GetBalance(CapitalAccountStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).SingleOrDefault(x => !x.IsBalancing)?.GetTransactionAmountWithCurrency()).ToArray();
        }
    }
}