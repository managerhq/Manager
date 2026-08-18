using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.TaxableSalesPerCustomer;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxableSalesPerCustomer
{
    [ProtoContract]
    [Title(nameof(Strings.TaxableSalesPerCustomer))]
    [Guide("The **Taxable Sales Per Customer** report provides a comprehensive breakdown of taxable sales organized by customer and tax code.")]
    [Guide("This report displays three key columns for each customer: *net sales* (the amount before tax), *tax on sales* (the tax amount collected), and *total sales* (the combined amount including tax).")]
    [Header("Report Structure")]
    [Guide("Sales are grouped by tax code, with customers listed under each applicable tax code. This organization helps you quickly identify which tax codes apply to which customers.")]
    [Guide("The report automatically calculates totals for each column, giving you a complete overview of your taxable sales activity during the selected period.")]
    [Header("Uses and Benefits")]
    [Guide("Use this report to analyze taxable revenue patterns by customer, verify tax collection accuracy, and identify your primary sources of taxable income.")]
    [Guide("The tax amounts shown in bold make it easy to focus on the tax liability portion of each transaction, which is particularly useful for tax reporting and compliance purposes.")]
    [LinkGuide("To configure report parameters, see:", typeof(TaxableSalesPerCustomerForm))]
    internal sealed class TaxableSalesPerCustomerView : DefaultView<GetTaxableSalesPerCustomerView>
    {
    }
}