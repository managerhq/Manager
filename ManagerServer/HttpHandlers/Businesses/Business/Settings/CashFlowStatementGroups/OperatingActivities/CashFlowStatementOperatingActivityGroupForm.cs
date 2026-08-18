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
    [Title(nameof(Strings.OperatingActivities), nameof(Strings.Edit))]
    [Guide("The Operating Activities form allows you to create custom groupings for the *operating activities* section of your **Cash Flow Statement**.")]
    [Guide("Operating activities represent the primary revenue-generating activities of your business and other activities that are not investing or financing activities. These typically include cash receipts from sales of goods and services, cash payments to suppliers and employees, and other cash flows from your core business operations.")]
    [Guide("Custom groups help you organize and present your operating cash flows in a way that best reflects your business structure and reporting needs. Each group you create will appear as a separate line item in the operating activities section of your **Cash Flow Statement**.")]
    [Fields(typeof(ManagerServer.Model.CashFlowStatementOperatingActivityGroup))]
    internal sealed class CashFlowStatementOperatingActivityGroupForm : NakedVueForm<ManagerServer.Model.CashFlowStatementOperatingActivityGroup>
    {
    }
}
