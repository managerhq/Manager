using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CashFlowStatementGroups.OperatingActivities
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.OperatingActivities))]
    [NewButton(nameof(Strings.NewGroup))]
    [Guide("The **Operating Activities** section of the *cash flow statement* shows cash flows from your primary business operations.")]
    [Guide("Operating activities include cash received from customers, cash paid to suppliers and employees, and other cash flows related to your core business activities.")]
    [Header("Purpose")]
    [Guide("Operating activity groups help organize and present cash flows in a meaningful way on your *cash flow statement*.")]
    [Guide("Common operating activities include cash receipts from sales, cash payments for inventory, wages, rent, and other operating expenses.")]
    [Header("Creating Groups")]
    [Guide("Click the **New Group** button to create custom groupings that match your business needs.")]
    [Guide("Each group will appear as a separate line item in the operating activities section of your *cash flow statement*.")]
    [Columns]
    internal sealed class OperatingActivities : PersistentObjectTable<ManagerServer.Model.CashFlowStatementOperatingActivityGroup>
    {
        [Guid("cd5e6174-e665-4cb1-a1c6-156cedc95100")]
        [Guide("Displays the name of each *operating activity group* that you have created.")]
        [Guide("This name will appear as a line item heading in the operating activities section of your *cash flow statement*.")]
        public string GetName(ManagerServer.Model.CashFlowStatementOperatingActivityGroup row) => row.Name;
    }
}
