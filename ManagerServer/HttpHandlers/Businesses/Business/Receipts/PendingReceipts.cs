using System;
using ManagerServer.Model;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Receipts
{
    [ProtoContract]
    [Guid("65A3B677-204C-46C2-9390-253246FD3F5B")]
    [Title(nameof(Strings.Receipts))]
    [Guide("The `Pending Receipts` screen displays recurring receipts that are ready to be processed.")]
    [Guide("Review receipts that are due based on their recurring schedules and create them when needed.")]
    [Columns]
    internal sealed class PendingReceipts : NakedObjectsOfPendingRecurringTransactions<ManagerServer.Model.RecurringReceipt>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("AA209B91-A91A-4455-9357-CB78D2B6B7CB")]
        [Guide("Displays the date when the recurring receipt is scheduled to be created.")]
        public DateTime?[] GetNextIssueDate(RecurringReceipt[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("EFF7AF3A-21C3-4D6A-BD27-05991C70A25E")]
        [Guide("Displays the bank or cash account that will receive the funds.")]
        public string[] GetAccount(RecurringReceipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.ReceivedIn)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("51D32A56-61C8-422A-9999-06B8E81E5798")]
        [Guide("Displays the description of the recurring receipt.")]
        public string[] GetDescription(RecurringReceipt[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }
    }
}
