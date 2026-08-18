using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payments
{
    [ProtoContract]
    [Guid("AF5B6294-9E87-4AB5-8BB3-6ECA1BC890D0")]
    [Title(nameof(Strings.Payments))]
    [Guide("The `Pending Payments` screen displays recurring payments that are due to be processed based on their scheduled dates.")]
    [Guide("This screen helps you manage recurring payments by showing which payments need to be created and processed.")]
    [Guide("Click on any payment in the list to create it immediately, or use batch operations to process multiple payments at once.")]
    [Columns]
    internal sealed class PendingPayments : NakedObjectsOfPendingRecurringTransactions<ManagerServer.Model.RecurringPayment>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("52F66333-404C-4F08-B0F6-15B24266E35F")]
        [Guide("Shows the scheduled date when this recurring payment is due to be created.")]
        public DateTime?[] GetNextIssueDate(RecurringPayment[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("98C4B7E7-4187-47DC-B937-56D2FB33A5A2")]
        [Guide("Shows the bank or cash account from which the payment will be made.")]
        public string[] GetAccount(RecurringPayment[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.PaidFrom)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("131F02EF-277A-4CBF-A217-3C5323BB94C1")]
        [Guide("Shows the description of the payment as defined in the recurring payment template.")]
        public string[] GetDescription(RecurringPayment[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }
    }
}
