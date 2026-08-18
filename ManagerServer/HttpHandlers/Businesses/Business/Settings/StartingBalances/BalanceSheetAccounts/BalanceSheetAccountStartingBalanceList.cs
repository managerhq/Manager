using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.BalanceSheetAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("ebf9db17-460f-47d6-9a51-18b0987bfb6b")]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.BalanceSheetAccounts))]
    [Guide("This screen allows you to set up starting balances for custom balance sheet accounts that you have created under **Chart of Accounts**.")]
    [Guide("To create a new starting balance for a balance sheet account, click the **New Starting Balance** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.BalanceSheetAccounts), name: nameof(Strings.NewStartingBalance))]
    [Guide("You will be taken to the *Starting Balance* screen for the *Balance Sheet Account*.")]
    [LinkGuide("For more information, see:", typeof(BalanceSheetAccountStartingBalanceForm))]
    internal sealed class BalanceSheetAccountStartingBalanceList : NakedObjectsWithAutomaticRows<BalanceSheetAccountStartingBalance>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewStartingBalance);
        }

        [Default]
        [Guid("726006df-50ac-4f05-a51f-7ef042658d20")]
        public NamedObject[] GetBalanceSheetAccount(BalanceSheetAccountStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BalanceSheetAccount>(x.BalanceSheetAccount)).ToArray();
        }

        [Default, Right, Bold, Sum]
        [Guid("996c4d55-2a6e-408c-998b-9267b88c96f9")]
        public Tuple<decimal, Currency>[] GetDebit(BalanceSheetAccountStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            return rows.Select(x => x.DebitCredit == ManagerServer.Model.Enums.DebitCredit.Debit ? new Tuple<decimal, Currency>(baseCurrency.Round(x.StartingBalance), baseCurrency) : null).ToArray();
        }

        [Default, Right, Bold, Sum]
        [Guid("b2b63e86-4fca-4df9-a5df-6e8904a76517")]
        public Tuple<decimal, Currency>[] GetCredit(BalanceSheetAccountStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            return rows.Select(x => x.DebitCredit == ManagerServer.Model.Enums.DebitCredit.Credit ? new Tuple<decimal, Currency>(baseCurrency.Round(x.StartingBalance), baseCurrency) : null).ToArray();
        }
    }
}