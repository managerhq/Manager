using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Model.Master;
using ManagerServer.Api.Businesses.Business.Reports.BalanceSheet;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BalanceSheet
{
    [ProtoContract]
    [Title(nameof(Strings.BalanceSheet))]
    [Guide("The Balance Sheet report shows the financial position of your business at a specific date.")]
    [Guide("It displays assets, liabilities, and equity with comparative periods.")]
    [LinkGuide("For more information see:", typeof(BalanceSheetForm))]
    internal sealed class BalanceSheetView : DefaultView<GetBalanceSheetView>
    {
    }
}