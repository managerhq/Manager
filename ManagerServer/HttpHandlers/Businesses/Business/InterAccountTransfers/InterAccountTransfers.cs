using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.InterAccountTransfers
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("1268dbf7-7c59-46be-a1bc-8438cfa0e71c")]
    [Title(nameof(Strings.InterAccountTransfers))]
    [Guide("Inter-account transfers allow you to record the movement of money between your bank and cash accounts within the same business.")]
    [Guide("Use this feature whenever you need to move funds from one account to another, such as depositing cash into a bank account, withdrawing cash from the bank, or transferring money between different bank accounts.")]
    [TabScreenshot("fa-arrow-alt-to-right", nameof(Strings.InterAccountTransfers))]
    [Header("Creating Transfers")]
    [Guide("To create a new inter-account transfer, click the **New Inter Account Transfer** button.")]
    [HeroButtonScreenshot(nameof(Strings.InterAccountTransfers), nameof(Strings.NewInterAccountTransfer))]
    [Guide("You can also convert existing *Payment* and *Receipt* pairs into inter-account transfers.")]
    [Guide("This is particularly useful when importing bank statements. The import process may create separate payments and receipts that actually represent transfers between your accounts. These can be easily converted to proper inter-account transfers for cleaner record-keeping.")]
    [LinkGuide("Learn how to convert payment/receipt pairs:", typeof(NewInterAccountTransfers))]
    [Header("Working with the Transfer List")]
    [Guide("The table below displays all your inter-account transfers with key information organized in columns.")]
    [Columns]
    [Guide("You can customize which columns are visible by clicking the **Edit Columns** button to show only the information you need.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("Learn more about customizing columns:", typeof(NakedObjectsWithEditColumns<InterAccountTransfer>))]
    internal sealed class InterAccountTransfers : NakedObjectsWithAutomaticRows<ManagerServer.Model.InterAccountTransfer>
    {
        protected override InterAccountTransfer[] OnGetRows(InterAccountTransfer[] rows)
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess)
            {
                var accounts = userPermissions.GetBankCashAccounts().ToList();
                var filter = true;
                if (accounts.Count == 0)
                {
                    filter = false;
                    foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.BankOrCashAccount>()) accounts.Add(e.Key);
                }
                if (filter) rows = rows.Where(x => (x.ReceivedIn.HasValue && accounts.Contains(x.ReceivedIn.Value)) || (x.PaidFrom.HasValue && accounts.Contains(x.PaidFrom.Value))).ToArray();
            }
            return rows;
        }

        [Default]
        [WarnIfNotUnique]
        [Center, MinWidth]
        [WhitespaceNoWrap]
        [Guid("59a25e36-d0db-410a-b2e4-6c703365a398")]
        [Guide("The **Date** column shows when the transfer between accounts occurred.")]
        [Guide("This date is important for *bank reconciliation* and tracking the timing of fund movements.")]
        public DateTime[] GetDate(InterAccountTransfer[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [PaddedSorting]
        [Guid("433dd21c-d229-4e85-b93e-4c276e0b65d4")]
        [Guide("The **Reference** column displays the unique reference number for each inter-account transfer.")]
        [Guide("Reference numbers help you identify and track specific transfers, especially when reconciling bank statements.")]
        public string[] GetReference(InterAccountTransfer[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("12841af2-21b7-4833-b25d-41f7938b156f")]
        [Guide("The **Paid From** column shows the bank or cash account from which money was withdrawn.")]
        [Guide("This is the source account that will have its balance reduced by the transfer amount.")]
        public string[] GetPaidFrom(InterAccountTransfer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.PaidFrom)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("71798b01-60f6-49c1-b75c-63f75e27e1ed")]
        [Guide("The **Received In** column displays the bank or cash account where money was deposited.")]
        [Guide("This is the destination account that will have its balance increased by the transfer amount.")]
        public string[] GetReceivedIn(InterAccountTransfer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.ReceivedIn)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("4ab13b19-574c-431c-8ba7-f17ef4786c46")]
        [Guide("The **Description** column contains optional notes or details about the transfer.")]
        [Guide("Use this field to record the reason for the transfer or any other relevant information for future reference.")]
        public string[] GetDescription(InterAccountTransfer[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Right, Sum]
        [Default]
        [Guid("a6bd4337-d39d-4cb6-9023-c9b050487e73")]
        [Guide("The **Amount** column shows the monetary value of each transfer.")]
        [Guide("The total sum of all transfers is displayed at the bottom of this column, helping you see the overall movement of funds.")]
        public Tuple<decimal, Currency>[] GetAmount(InterAccountTransfer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.TransactionAmount > 0m)?.GetTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }
    }
}
