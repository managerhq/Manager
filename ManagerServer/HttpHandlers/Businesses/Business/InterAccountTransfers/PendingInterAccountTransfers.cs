using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.InterAccountTransfers
{
    [ProtoContract]
    [Guid("167476EA-A318-4637-AC35-C05055AE5C71")]
    [Title(nameof(Strings.InterAccountTransfers))]
    [Guide("The **Pending Inter-account Transfers** screen displays recurring transfers that are scheduled to be automatically created.")]
    [Guide("These are transfers between *bank accounts* and *cash accounts* that have been set up to occur on a regular basis.")]
    [Guide("When the scheduled date arrives, the system will automatically create the transfer transaction.")]
    [Columns]
    internal sealed class PendingInterAccountTransfers : NakedObjectsOfPendingRecurringTransactions<ManagerServer.Model.RecurringInterAccountTransfer>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("485F0774-63BD-414E-9EC8-41915423F0C5")]
        [Guide("The **Next Issue Date** column shows when each recurring transfer is scheduled to be automatically created.")]
        [Guide("On this date, the system will generate the actual transfer transaction between the specified accounts.")]
        public DateTime?[] GetNextIssueDate(RecurringInterAccountTransfer[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("A7C6ADDA-A37E-4FDE-97CC-AA741559D4D6")]
        [Guide("The **Paid From** column displays the source account from which funds will be transferred.")]
        [Guide("This is the account that will be credited (reduced) when the transfer is created.")]
        public string[] GetPaidFrom(RecurringInterAccountTransfer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.PaidFrom)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("7D865DC1-745E-4383-94EB-941D6B18DC46")]
        [Guide("The **Received In** column displays the destination account where funds will be deposited.")]
        [Guide("This is the account that will be debited (increased) when the transfer is created.")]
        public string[] GetReceivedIn(RecurringInterAccountTransfer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.ReceivedIn)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("E5796997-1C9B-4281-8534-D11097FBDFCD")]
        [Guide("The **Description** column shows the description or narration for each recurring transfer.")]
        [Guide("This text will be included in the transfer transaction when it is automatically created.")]
        public string[] GetDescription(RecurringInterAccountTransfer[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Sum, Right]
        [Guid("70D4ED45-6EB3-4E0D-9ECA-85D8FD5616A4")]
        [Guide("The **Amount** column displays the amount to be transferred in the currency of the source account.")]
        [Guide("If the source and destination accounts use different currencies, the *exchange rate* will be applied when the transfer is created.")]
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