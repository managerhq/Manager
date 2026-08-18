using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatement;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatement
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatement), nameof(Strings.View))]
    [Guide("The Profit & Loss Statement shows revenues and expenses for a period.")]
    [Guide("It calculates net profit or loss with income, costs, and expense categories.")]
    internal sealed class ProfitAndLossStatementView : DefaultView<GetProfitAndLossStatementView>
    {
    }
}