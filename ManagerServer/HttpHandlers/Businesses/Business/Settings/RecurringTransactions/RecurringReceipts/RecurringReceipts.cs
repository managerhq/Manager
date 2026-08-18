using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringReceipts
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Receipts))]
    [Guid("32ba6a92-7bea-4d8d-a1d5-7c820701417f")]
    [Title(nameof(Strings.RecurringReceipts), nameof(Strings.Pending))]
    [Guide("Recurring receipts are automated receipt transactions that repeat on a schedule you define. This eliminates the need to manually enter regular income such as rent payments, subscription fees, or recurring service charges.")]
    [Guide("Each recurring receipt will automatically generate a new receipt transaction on its scheduled date. The system creates these transactions in the background, ensuring your cash flow records stay up to date without manual intervention.")]
    [Guide("To create a recurring receipt, click the **New Recurring Receipt** button. You can set the frequency (daily, weekly, monthly, etc.), specify the amount, and choose which bank or cash account will receive the funds.")]
    [Columns]
    internal sealed class RecurringReceipts : NakedObjectsWithAutomaticRows<ManagerServer.Model.RecurringReceipt>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("dcdfc81a-7b33-4d6f-975d-b0236455690a")]
        [Guide("Displays the date when the system will automatically generate the next receipt transaction. This date updates automatically based on the recurring schedule you've configured.")]
        public DateTime?[] GetNextIssueDate(RecurringReceipt[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("d6018b39-f40e-4889-8e3e-020983f9c640")]
        [Guide("Shows the *bank account* or *cash account* that will receive the funds when each receipt is generated. This must match one of your configured accounts in the **Bank & Cash Accounts** tab.")]
        public string[] GetAccount(RecurringReceipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.ReceivedIn)?.Name).ToArray();
        }

        [Default]
        [Guid("f0970a5e-8ce4-4fba-ac87-894f52be8351")]
        [Guide("Displays the description that will appear on each generated receipt. Use descriptive text to identify the source of income, such as \"Monthly rent from tenant\" or \"Quarterly subscription payment\".")]
        public string[] GetDescription(RecurringReceipt[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("3e76d260-7f30-4175-ab1c-2596f5032672")]
        [Guide("Shows the total amount of each recurring receipt in the currency of the receiving account. If the receipt includes multiple line items, this displays their combined total.")]
        public Tuple<decimal, Currency>[] GetAmount(RecurringReceipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var receipt = new ManagerServer.Model.Receipt();
                Copy(e, receipt);
                var balancingTransaction = receipt.CreateGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing);

                output.Add(balancingTransaction?.GetTransactionAmountWithCurrency());
            }
            return output.ToArray();
        }
    }
}
