using ManagerServer.Api.Businesses.Business.BillableTime;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.BillableTime
{
    [ProtoContract]
    [Title(nameof(Strings.BillableTime), nameof(Strings.TransactionJournal))]
    internal sealed class BillableTimeTransactionJournalView : DefaultView<GetBillableTimeTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
