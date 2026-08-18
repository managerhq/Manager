using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.BankAndCashAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(BankAndCashAccounts))]
    [Guid("7717988d-72d1-45d4-968e-98ff704e9650")]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.BankAndCashAccounts))]
    [Guide("This screen allows you to set up starting balances for bank and cash accounts that you have created under the **Bank & Cash Accounts** tab.")]
    [Guide("To create a new starting balance for a bank or cash account, click the **New Starting Balance** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.BankAndCashAccounts), name: nameof(Strings.NewStartingBalance))]
    [Guide("You will be taken to the *Starting Balance* screen for your selected bank or cash account.")]
    [LinkGuide("For more information, see:", typeof(BankOrCashAccountStartingBalanceForm))]
    internal sealed class BankOrCashAccountStartingBalanceList : NakedObjectsWithAutomaticRows<BankOrCashAccountStartingBalance>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewStartingBalance);
        }

        [Default]
        [Guid("258d410f-115f-45e0-b747-3c51ff8e1b81")]
        public NamedObject[] GetBankOrCashAccount(BankOrCashAccountStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.BankOrCashAccount)).ToArray();
        }

        [Default, Right, Bold, Sum]
        [Guid("d5891808-7f7c-4e1d-97b8-7a3b7bf97cee")]
        public Tuple<decimal, Currency>[] GetClearedBalance(BankOrCashAccountStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).SingleOrDefault(x => !x.IsBalancing)?.GetTransactionAmountWithCurrency()).ToArray();
        }
    }
}