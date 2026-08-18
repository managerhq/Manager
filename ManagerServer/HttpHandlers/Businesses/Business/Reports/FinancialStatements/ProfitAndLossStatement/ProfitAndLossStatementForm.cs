using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatement
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatement), nameof(Strings.Edit))]
    [Guide("The `ProfitAndLossStatement` shows your business performance over a period of time.")]
    [Guide("It displays revenues earned and expenses incurred to calculate net profit or loss.")]
    [Guide("Configure date ranges to analyze profitability for any period - monthly, quarterly, or annually.")]
    [Fields(typeof(ManagerServer.Model.ProfitAndLossStatement))]
    [Guide("Income accounts appear at the top, followed by expense accounts, with net profit/loss at the bottom.")]
    [Guide("Use comparative columns to analyze trends across multiple periods.")]
    [Guide("Filter by divisions or projects to analyze segment profitability.")]
    [Guide("To configure income and expense account categories, see:")]
    [LinkGuide("For more information see:", typeof(Settings.ChartOfAccounts.ChartOfAccounts))]
    [Guide("Divisions and projects enable detailed profitability analysis by business segment.")]
    [Guide("To set up tracking categories for detailed reporting, see:")]
    [LinkGuide("For more information see:", typeof(Settings.Divisions.Divisions))]
    [LinkGuide("For more information see:", typeof(Projects.Projects))]
    internal sealed class ProfitAndLossStatementForm : NakedVueForm<ManagerServer.Model.ProfitAndLossStatement>
    {
    }
}
