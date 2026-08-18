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
    [Title(nameof(Strings.InvestingActivities), nameof(Strings.Edit))]
    [Guide("The Investing Activities form is used to create and edit custom groups for the investing activities section of your *cash flow statement*.")]
    [Guide("Investing activities typically include transactions involving the purchase and sale of long-term assets and investments that are not considered cash equivalents.")]
    [Guide("Examples of investing activities include:")]
    [Guide("• Purchase or sale of property, plant, and equipment")]
    [Guide("• Purchase or sale of investments")]
    [Guide("• Loans made to others and collections of those loans")]
    [Guide("• Acquisitions and disposals of businesses or subsidiaries")]
    [Header("Creating Custom Groups")]
    [Guide("When you create a custom investing activity group, you can organize similar types of investing cash flows together for clearer financial reporting.")]
    [Guide("Each group you create will appear as a separate line item in the investing activities section of your *cash flow statement*.")]
    [Fields(typeof(ManagerServer.Model.CashFlowStatementInvestingActivityGroup))]
    internal sealed class CashFlowStatementInvestingActivityGroupForm : NakedVueForm<ManagerServer.Model.CashFlowStatementInvestingActivityGroup>
    {
    }
}
