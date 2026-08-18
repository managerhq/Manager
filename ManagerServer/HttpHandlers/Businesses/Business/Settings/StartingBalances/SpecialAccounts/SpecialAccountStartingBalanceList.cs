using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.SpecialAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SpecialAccounts))]
    [Guid("f4a987cf-80fd-4c4a-9fab-bfbaf1b82e97")]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.SpecialAccounts))]
    [Guide("This screen allows you to set up starting balances for special accounts that you have created under the **Special Accounts** tab.")]
    [Guide("To create a new starting balance for a special account, click the **New Starting Balance** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.SpecialAccounts), name: nameof(Strings.NewStartingBalance))]
    [Guide("You will be taken to the *Starting Balance* screen for the *Special Account*.")]
    [LinkGuide("For more information, see:", typeof(SpecialAccountStartingBalanceForm))]
    internal sealed class SpecialAccountStartingBalanceList : NakedObjectsWithAutomaticRows<SpecialAccountStartingBalance>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewStartingBalance);
        }

        [Default]
        [Guid("e02b68f1-d90b-4602-a647-7bbe9fa657d7")]
        public NamedObject[] GetSpecialAccount(SpecialAccountStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<SpecialAccount>(x.SpecialAccount)).ToArray();
        }

        [Default, Right, Bold, Sum]
        [Guid("be3b4e73-fbe7-4802-b263-3795925ab5a2")]
        public Tuple<decimal, Currency>[] GetBalance(SpecialAccountStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).SingleOrDefault(x => !x.IsBalancing)?.GetTransactionAmountWithCurrency()).ToArray();
        }
    }
}