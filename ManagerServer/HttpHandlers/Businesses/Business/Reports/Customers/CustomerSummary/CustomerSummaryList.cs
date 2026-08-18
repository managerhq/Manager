using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomerSummary
{
    [ProtoContract]
    [Title(nameof(Strings.CustomerSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`CustomerSummary` provides overview of your customer interactions and transactions to effectively manage your customer relationships and financial performance.")]
    [Guide("To create a new `CustomerSummary`, go to `Reports` tab, click `CustomerSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.CustomerSummary), name: nameof(Strings.NewReport))]
    internal sealed class CustomerSummaryList : PersistentObjectTable<ManagerServer.Model.CustomerSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("145534f1-ff3f-4338-95ab-14ca8a07f8e0")]
        public DateTime? GetFromDate(ManagerServer.Model.CustomerSummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("1286a8f8-0619-4cda-8f8b-29862f62147c")]
        public DateTime? GetToDate(ManagerServer.Model.CustomerSummary o) => o.ToDate;

        [Guid("4fced1a6-0a1e-4d78-8b88-8a3c4d9c0dde")]
        public ManagerServer.Model.Division GetDivision(ManagerServer.Model.CustomerSummary o) => ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Division>(o.Division);

        [Guid("ffbed90d-22d2-4d16-bb9d-f54ec2ebdaa2")]
        public string GetDescription(ManagerServer.Model.CustomerSummary o) => string.Empty;        
    }
}