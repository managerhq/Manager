using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPayments
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Payments))]
    [Guid("194298ec-c398-47cc-8777-556409e6b46b")]
    [Title(nameof(Strings.RecurringPayments), nameof(Strings.Pending))]
    [Guide("The **Recurring Payments** screen manages scheduled payment transactions that are automatically created at regular intervals.")]
    [Guide("Use this feature to set up automatic payments for regular expenses such as rent, insurance premiums, loan repayments, or subscription fees.")]
    [Guide("The system will automatically generate *payment* transactions based on the schedule you define, saving you time and ensuring important payments are never forgotten.")]
    [Guide("Each recurring payment template specifies the *payee*, *amount*, *account*, and frequency of the payment.")]
    [Columns]
    internal sealed class RecurringPayments : NakedObjectsWithAutomaticRows<ManagerServer.Model.RecurringPayment>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("c4564c65-2e68-404b-a11b-0ace0d45b9e5")]
        [Guide("Displays the date when the next *payment* transaction will be automatically generated based on the recurring schedule.")]
        public DateTime?[] GetNextIssueDate(RecurringPayment[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("eb77d1b7-6851-4cb9-9999-b11a70b49558")]
        [Guide("Displays the *bank account* or *cash account* from which the payment will be made.")]
        [Guide("This account will be credited when the recurring payment is processed.")]
        public string[] GetAccount(RecurringPayment[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.PaidFrom)?.Name).ToArray();
        }

        [Default]
        [Guid("bfb00cd9-5055-422d-abb5-f3c3c21c42e4")]
        [Guide("Displays the description or reference of the recurring payment.")]
        [Guide("This helps identify the purpose of each recurring payment in the list.")]
        public string[] GetDescription(RecurringPayment[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("caba6ad1-f6e0-4df0-bbb9-94836335906f")]
        [Guide("Displays the amount of each recurring payment in the currency of the *bank account* or *cash account*.")]
        [Guide("The total at the bottom shows the sum of all pending recurring payments.")]
        public Tuple<decimal, Currency>[] GetAmount(RecurringPayment[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var payment = new ManagerServer.Model.Payment();
                Copy(e, payment);
                var balancingTransaction = payment.CreateGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing);

                output.Add(balancingTransaction?.GetReversedTransactionAmountWithCurrency());
            }
            return output.ToArray();
        }
    }
}
