using ManagerServer.Api.Businesses.Business.Payments;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payments
{
    [ProtoContract]
    [Title(nameof(Strings.Payment), nameof(Strings.TransactionJournal))]
    internal sealed class PaymentTransactionJournalView : DefaultView<GetPaymentTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}