using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ExpenseClaimPayers
{
    [ProtoContract]
    [Title(nameof(Strings.ExpenseClaimPayers), nameof(Strings.Edit))]
    [Guide("Configure who can submit expense claims for reimbursement.")]
    [Guide("Expense claim payers can be employees or other entities that incur business expenses.")]
    [Fields(typeof(ManagerServer.Model.ExpenseClaimsPayer))]
    internal sealed class ExpenseClaimPayerForm : NakedVueForm<ManagerServer.Model.ExpenseClaimsPayer>
    {
    }
}