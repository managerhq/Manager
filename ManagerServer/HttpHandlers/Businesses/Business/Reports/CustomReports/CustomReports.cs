using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomReports
{
    [ProtoContract]
    [Title(nameof(Strings.CustomReports))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("The **Custom Reports** screen allows you to create and manage custom reports tailored to your specific business needs.")]
    [Guide("Custom reports provide powerful data analysis capabilities beyond the standard reports available in the system.")]
    [Header("Overview")]
    [Guide("Custom reports enable you to build specialized reports that analyze your data in unique ways. You can define specific criteria, filters, and calculations to generate insights that standard reports might not provide.")]
    [Guide("Each custom report can be configured with date ranges, specific accounts, and various other parameters to focus on the exact information you need.")]
    [Header("Getting Started")]
    [Guide("To create a new custom report, click the **New Report** button. You can then define the report parameters, select data sources, and configure how the information should be displayed.")]
    [Guide("Once created, custom reports appear in this list where you can view, edit, or delete them as needed.")]
    [Columns]
    internal sealed class CustomReports : PersistentObjectTable<ManagerServer.Model.CustomReport>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("25f8698a-3cec-484d-8510-9ee3c0549d3f")]
        public DateTime? GetFromDate(ManagerServer.Model.CustomReport o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("02651684-bba9-4aee-aab6-0e6db0204134")]
        public DateTime? GetToDate(ManagerServer.Model.CustomReport o) => o.ToDate;

        [Guid("a0c73b10-ef7a-4561-b338-ab0f21f9899a")]
        public string GetName(ManagerServer.Model.CustomReport o) => string.IsNullOrEmpty(o.Name) ? Strings.CustomReport : o.Name;

        [Guid("663f2c7c-1db5-4e74-840c-815f2d825153")]
        public string GetDescription(ManagerServer.Model.CustomReport o) => o.Description;
    }
}