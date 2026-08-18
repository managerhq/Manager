using System;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SupplierStatementsUnpaidInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.SupplierStatements), nameof(Strings.UnpaidInvoices))]
    [Guide("Supplier statements show unpaid purchase invoices for your suppliers, helping you track outstanding amounts owed.")]
    [Guide("This report displays all unpaid invoices for selected suppliers, including invoice dates, due dates, and amounts outstanding.")]
    [Guide("Use this report to review your payables, prepare payment runs, or send statements to suppliers for reconciliation purposes.")]
    [Fields(typeof(ManagerServer.Model.SupplierStatementsUnpaidInvoices))]
    internal sealed class SupplierStatementsUnpaidInvoicesForm : NakedVueForm<ManagerServer.Model.SupplierStatementsUnpaidInvoices>
    {
    }
}