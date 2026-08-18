using ManagerServer.Api.Businesses.Business.Payslips;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payslips
{
    [ProtoContract]
    [Title(nameof(Strings.Payslip), nameof(Strings.TransactionJournal))]
    internal sealed class PayslipTransactionJournalView : DefaultView<GetPayslipTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
