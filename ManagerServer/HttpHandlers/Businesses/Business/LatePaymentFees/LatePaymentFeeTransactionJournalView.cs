using ManagerServer.Api.Businesses.Business.LatePaymentFees;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.LatePaymentFees
{
    [ProtoContract]
    [Title(nameof(Strings.LatePaymentFee), nameof(Strings.TransactionJournal))]
    internal sealed class LatePaymentFeeTransactionJournalView : DefaultView<GetLatePaymentFeeTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
