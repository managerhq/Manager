using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomer
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByCustomer))]
    [Guide("The Sales Invoice Totals by Customer form configures customer sales analysis.")]
    [Guide("Set date ranges to analyze sales revenue broken down by customer.")]
    [Fields(typeof(ManagerServer.Model.SalesInvoiceTotalsByCustomer))]
    internal sealed class SalesInvoiceTotalsByCustomerForm : NakedVueForm<ManagerServer.Model.SalesInvoiceTotalsByCustomer>
    {
    }
}
