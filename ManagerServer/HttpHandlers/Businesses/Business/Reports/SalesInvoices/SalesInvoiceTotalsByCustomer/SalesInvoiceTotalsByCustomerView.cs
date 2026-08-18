using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using System.Security.Cryptography.X509Certificates;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByCustomer;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomer
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByCustomer))]
    [Guide("The Sales Invoice Totals by Customer report shows revenue by customer.")]
    [Guide("It displays total sales amounts for each customer over specified periods.")]
    [LinkGuide("For more information see:", typeof(SalesInvoiceTotalsByCustomerForm))]
    internal sealed class SalesInvoiceTotalsByCustomerView : DefaultView<GetSalesInvoiceTotalsByCustomerView>
    {
    }
}