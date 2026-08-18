using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomerStatementsUnpaidInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.CustomerStatementsUnpaidInvoices))]
    [Guide("The Customer Statements Unpaid Invoices form is used to configure report parameters.")]
    [Guide("Select customers to generate statements showing only unpaid invoices.")]
    [Fields(typeof(ManagerServer.Model.CustomerStatementsUnpaidInvoices))]
    internal sealed class CustomerStatementsUnpaidInvoicesForm : NakedVueForm<ManagerServer.Model.CustomerStatementsUnpaidInvoices>
    {
    }
}