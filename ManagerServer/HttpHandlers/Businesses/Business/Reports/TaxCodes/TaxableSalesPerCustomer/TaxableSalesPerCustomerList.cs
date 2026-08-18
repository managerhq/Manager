using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxableSalesPerCustomer
{
    [ProtoContract]
    [Title(nameof(Strings.TaxableSalesPerCustomer))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("The *Taxable Sales Per Customer* report provides a detailed summary of taxable transactions grouped by customer over a specified date range.")]
    [Guide("This report helps you analyze your sales revenue from each customer, showing the total taxable sales amount for tax reporting purposes.")]
    [Guide("To create a new report, go to the **Reports** tab, click **Taxable Sales Per Customer**, then click the **New Report** button.")]
    [Guide("You can create multiple reports covering different time periods or using different accounting methods to compare results.")]
    [HeroButtonScreenshot(title: nameof(Strings.TaxableSalesPerCustomer), name: nameof(Strings.NewReport))]
    internal sealed class TaxableSalesPerCustomerList : PersistentObjectTable<ManagerServer.Model.TaxableSalesPerCustomer>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("000b85b7-9745-4cae-aada-2019e6159beb")]
        public DateTime GetFromDate(ManagerServer.Model.TaxableSalesPerCustomer o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("06d3bb2a-6b61-4188-b749-8a99380bfe67")]
        public DateTime GetToDate(ManagerServer.Model.TaxableSalesPerCustomer o) => o.ToDate;

        [Guid("7c2982a4-d2b4-4f9b-b91b-793d97064b90")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.TaxableSalesPerCustomer o) => o.AccountingMethod;

        [Guid("ded111f1-d08b-4153-aea3-84bac82fe7b5")]
        public string GetDescription(ManagerServer.Model.TaxableSalesPerCustomer o) => o.Description;
    }
}