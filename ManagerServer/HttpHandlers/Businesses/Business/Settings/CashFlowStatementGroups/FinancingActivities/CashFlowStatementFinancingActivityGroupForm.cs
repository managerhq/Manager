using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CashFlowStatementGroups
{
    [ProtoContract]
    [Title(nameof(Strings.FinancingActivities), nameof(Strings.Edit))]
    [Guide("The Financing Activities form allows you to create and manage custom groups for organizing financing activities in your *cash flow statement*.")]
    [Guide("Financing activities are transactions that affect the long-term liabilities and equity of your business. These include activities that change the size and composition of the equity capital and borrowings.")]
    [Guide("Common examples of financing activities include:")]
    [Guide("• Proceeds from issuing shares or other equity instruments")]
    [Guide("• Cash payments to owners to acquire or redeem shares")]
    [Guide("• Proceeds from issuing debentures, loans, notes, bonds, and other borrowings")]
    [Guide("• Cash repayments of amounts borrowed")]
    [Guide("• Dividend payments to shareholders")]
    [Header("Creating Custom Groups")]
    [Guide("You can create custom groups to further categorize your financing activities for more detailed reporting. This helps organize your *cash flow statement* in a way that best represents your business structure.")]
    [Guide("Each group you create will appear as a separate line item under the Financing Activities section of your *cash flow statement*.")]
    [Fields(typeof(ManagerServer.Model.CashFlowStatementFinancingActivityGroup))]
    internal sealed class CashFlowStatementFinancingActivityGroupForm : NakedVueForm<ManagerServer.Model.CashFlowStatementFinancingActivityGroup>
    {
    }
}
