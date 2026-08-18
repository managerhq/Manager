using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxableSalesPerCustomer
{
    [ProtoContract]
    [Title(nameof(Strings.TaxableSalesPerCustomer))]
    [Guide("The **Taxable Sales Per Customer** report shows a summary of sales subject to tax, organized by customer.")]
    [Guide("This report helps you track which customers have generated taxable sales and the total amounts for each tax code.")]
    [Guide("Use this report to analyze your taxable revenue by customer, identify your largest taxable sales relationships, and ensure proper tax collection from each customer.")]
    [Fields(typeof(ManagerServer.Model.TaxableSalesPerCustomer))]
    internal sealed class TaxableSalesPerCustomerForm : NakedVueForm<ManagerServer.Model.TaxableSalesPerCustomer>
    {
    }
}
