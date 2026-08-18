using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CashFlowStatementGroups.InvestingActivities
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.InvestingActivities))]
    [NewButton(nameof(Strings.NewGroup))]
    [Guide("The **Investing Activities** screen allows you to create and manage custom groups for organizing investing activities on your *cash flow statement*.")]
    [Guide("Investing activities represent cash flows related to the acquisition and disposal of long-term assets and investments that are not included in cash equivalents.")]
    [Header("Understanding Investing Activities")]
    [Guide("In the *cash flow statement*, investing activities typically include transactions such as:")]
    [Guide("• Purchase or sale of property, plant, and equipment")]
    [Guide("• Purchase or sale of investments in securities")]
    [Guide("• Loans made to other entities or collections on those loans")]
    [Guide("• Acquisitions or disposals of businesses or subsidiaries")]
    [Header("Creating Custom Groups")]
    [Guide("Click the **New Group** button to create a custom investing activity group. Each group you create will appear as a separate line item in the investing activities section of your *cash flow statement*.")]
    [Guide("Custom groups help you organize similar investing transactions together, making your financial reports easier to understand and analyze.")]
    [Columns]
    internal sealed class InvestingActivities : PersistentObjectTable<ManagerServer.Model.CashFlowStatementInvestingActivityGroup>
    {
        [Guid("e1790139-8bb1-4a09-a84e-7ef05dac643f")]
        [Guide("Displays the name of each *investing activity group* that you have created. This name will appear as a line item in the investing activities section of your *cash flow statement*.")]
        public string GetName(ManagerServer.Model.CashFlowStatementInvestingActivityGroup row) => row.Name;
    }
}
