using ManagerServer.Api.Businesses.Business.WithholdingTaxReceipts;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.WithholdingTaxReceipts
{
    [ProtoContract]
    [Title(nameof(Strings.WithholdingTaxReceipt), nameof(Strings.TransactionJournal))]
    internal sealed class WithholdingTaxReceiptTransactionJournalView : DefaultView<GetWithholdingTaxReceiptTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
