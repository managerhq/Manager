using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxSummary
{
    [ProtoContract]
    [Title(nameof(Strings.TaxSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("The **Tax Summary** report provides a summary of tax amounts collected and paid during a specific period.")]
    [Guide("This report helps you understand your tax liabilities and tax assets, showing the net amount owed to or receivable from tax authorities.")]
    [Guide("To create a new tax summary report, go to the **Reports** tab, click **Tax Summary**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.TaxSummary), name: nameof(Strings.NewReport))]
    internal sealed class TaxSummaryList : PersistentObjectTable<ManagerServer.Model.TaxSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("000b85b7-9745-4cae-aada-2019e6159beb")]
        public DateTime GetFromDate(ManagerServer.Model.TaxSummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("b15710a6-34cc-478c-8363-b9ee54983ad6")]
        public DateTime GetToDate(ManagerServer.Model.TaxSummary o) => o.ToDate;

        [Guid("f55305ec-6976-4d48-889b-abb53544ec49")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.TaxSummary o) => o.AccountingMethod;

        [Guid("371dbaff-4dfd-4d99-ae0b-b033d4189f5e")]
        public string GetDescription(ManagerServer.Model.TaxSummary o) => o.Description;
    }
}