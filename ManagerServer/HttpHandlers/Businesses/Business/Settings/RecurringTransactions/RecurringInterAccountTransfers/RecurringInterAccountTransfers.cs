using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringInterAccountTransfers
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(InterAccountTransfers))]
    [Guid("435bc68f-a16d-492c-aa12-fef0d8832a09")]
    [Title(nameof(Strings.RecurringInterAccountTransfers), nameof(Strings.Pending))]
    [Guide("Recurring inter account transfers automatically move funds between your *bank accounts* and *cash accounts* on a regular schedule.")]
    [Guide("Use this feature to automate routine transfers such as monthly savings deposits, loan repayments, or regular fund allocations between accounts.")]
    [Guide("Each recurring transfer will be created automatically based on the schedule you define, saving time and ensuring consistency.")]
    [Columns]
    internal sealed class RecurringInterAccountTransfers : NakedObjectsWithAutomaticRows<ManagerServer.Model.RecurringInterAccountTransfer>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("c6bdebc3-10c7-4681-875a-7c7eca24475c")]
        [Guide("Displays the date when the next *inter account transfer* will be automatically created based on the recurring schedule.")]
        public DateTime?[] GetNextIssueDate(RecurringInterAccountTransfer[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("1967d2ea-3fd2-4e90-8770-64e426b33bae")]
        [Guide("Shows the source account from which funds will be transferred.")]
        public string[] GetPaidFrom(RecurringInterAccountTransfer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.PaidFrom)?.Name).ToArray();
        }

        [Default]
        [Guid("76db54aa-7842-4f61-a620-47edcdcfdf0e")]
        [Guide("Shows the destination account where funds will be received.")]
        public string[] GetReceivedIn(RecurringInterAccountTransfer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.ReceivedIn)?.Name).ToArray();
        }

        [Default]
        [Guid("cf8e6123-af35-4e04-a9d2-86ae6c116842")]
        [Guide("Shows the description or reference for the recurring transfer transaction.")]
        public string[] GetDescription(RecurringInterAccountTransfer[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Sum, Right]
        [Guid("194298ec-c398-47cc-8777-556409e6b46b")]
        [Guide("Shows the amount to be transferred in the currency of the source account.")]
        public Tuple<decimal, Currency>[] GetAmount(RecurringInterAccountTransfer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var bankOrCashAccount = database.SingleOrDefault<BankOrCashAccount>(e.PaidFrom);
                var currency = database.SingleOrDefault<ForeignCurrency>(bankOrCashAccount?.Currency) as Currency ?? baseCurrency;
                var amount = currency.Round(e.CreditAmount);
                output.Add(new Tuple<decimal, Currency>(amount, currency));
            }
            return output.ToArray();
        }
    }
}
