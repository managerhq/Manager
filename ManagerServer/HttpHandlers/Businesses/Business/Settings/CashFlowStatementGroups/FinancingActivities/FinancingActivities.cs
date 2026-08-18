using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CashFlowStatementGroups.FinancingActivities
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.FinancingActivities))]
    [NewButton(nameof(Strings.NewGroup))]
    [Guide("The **Financing Activities** screen allows you to create and manage groups for categorizing cash flows in the *cash flow statement*.")]
    [Guide("Financing activities represent cash flows between your business and its owners or lenders, showing how the business is funded.")]
    [Header("Overview")]
    [Guide("Financing activities include transactions that affect the long-term liabilities and equity of your business. Common examples include:")]
    [Guide("• Receiving cash from loans or issuing bonds")]
    [Guide("• Repaying loan principal amounts")]
    [Guide("• Receiving cash from issuing shares or capital contributions")]
    [Guide("• Paying dividends or making drawings to owners")]
    [Header("Setting Up Groups")]
    [Guide("Click the **New Group** button to create custom groups that match your business's financing structure.")]
    [Guide("Each group you create will appear as a separate line item in the financing activities section of your *cash flow statement*.")]
    [Guide("Groups help organize similar financing transactions together for clearer financial reporting.")]
    [Columns]
    internal sealed class FinancingActivities : PersistentObjectTable<ManagerServer.Model.CashFlowStatementFinancingActivityGroup>
    {
        [Guid("0dbd6e0c-ecd4-40d6-a0db-ba66ee2fa905")]
        [Guide("Displays the name of each financing activity group that you have created.")]
        [Guide("Group names should be descriptive to help identify the type of financing transactions they contain.")]
        public string GetName(ManagerServer.Model.CashFlowStatementFinancingActivityGroup row) => row.Name;
    }
}
