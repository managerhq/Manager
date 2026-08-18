using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.ExpenseClaims
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of expense claims.")]
    [Guide("Use footers to add terms, conditions, or additional information to expense claims.")]
    [Fields(typeof(ManagerServer.Model.ExpenseClaimFooter))]
    internal sealed class ExpenseClaimFooterForm : NakedVueForm<ManagerServer.Model.ExpenseClaimFooter>
    {
    }
}