using ManagerServer.Api.Businesses.Business.ExpenseClaims;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.ExpenseClaims
{
    [ProtoContract]
    [Title(nameof(Strings.ExpenseClaim), nameof(Strings.TransactionJournal))]
    internal sealed class ExpenseClaimTransactionJournalView : DefaultView<GetExpenseClaimTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
