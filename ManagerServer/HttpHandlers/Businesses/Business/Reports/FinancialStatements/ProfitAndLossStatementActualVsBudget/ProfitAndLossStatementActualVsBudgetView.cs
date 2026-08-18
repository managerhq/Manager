using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatementActualVsBudget;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatementActualVsBudget
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatementActualVsBudget))]
    [Guide("The Profit and Loss Statement Actual vs Budget report compares performance against plans.")]
    [Guide("It shows actual income and expenses alongside budgeted amounts with variance analysis.")]
    [LinkGuide("For more information see:", typeof(ProfitAndLossStatementActualVsBudgetForm))]
    internal sealed class ProfitAndLossStatementActualVsBudgetView : DefaultView<GetProfitAndLossStatementActualVsBudgetView>
    {
    }
}