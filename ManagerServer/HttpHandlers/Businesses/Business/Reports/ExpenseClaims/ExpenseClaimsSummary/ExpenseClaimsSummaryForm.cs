using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ExpenseClaimsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.ExpenseClaimsSummary))]
    [Guide("The Expense Claims Summary form configures report parameters for expense claims.")]
    [Guide("Set date ranges to analyze expense claim balances and transactions.")]
    [Fields(typeof(ManagerServer.Model.ExpenseClaimsSummary))]
    internal sealed class ExpenseClaimsSummaryForm : NakedVueForm<ManagerServer.Model.ExpenseClaimsSummary>
    {
    }
}
