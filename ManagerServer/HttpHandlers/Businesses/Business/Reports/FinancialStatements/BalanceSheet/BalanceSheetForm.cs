using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BalanceSheet
{
    [ProtoContract]
    [Title(nameof(Strings.BalanceSheet), nameof(Strings.Edit))]
    [Guide("The `BalanceSheet` report shows your financial position at a specific point in time.")]
    [Guide("It displays what your business owns (assets), what it owes (liabilities), and the owner's equity.")]
    [Guide("To create a new balance sheet report, navigate to the `Reports` tab.")]
    [TabScreenshot("fa-print", nameof(Strings.Reports))]
    [Guide("Click on `BalanceSheet` to view existing reports or create new ones.")]
    [Guide("Click the `NewReport` button to create a customized balance sheet.")]
    [HeroButtonScreenshot(title: nameof(Strings.BalanceSheet), name: nameof(Strings.NewReport))]
    [Guide("Configure your balance sheet using these options:")]
    [Fields(typeof(ManagerServer.Model.BalanceSheet))]
    [Guide("The balance sheet follows the fundamental accounting equation: Assets = Liabilities + Equity.")]
    [Guide("Use date filters to view your financial position as of any specific date.")]
    [Guide("Accounts are organized into groups based on your chart of accounts structure.")]
    [Guide("To customize how accounts appear on the balance sheet, see:")]
    [LinkGuide("For more information see:", typeof(Settings.ChartOfAccounts.ChartOfAccounts))]
    [Guide("Starting balances ensure your balance sheet reflects accurate opening positions.")]
    [Guide("To set up or adjust starting balances for your accounts, see:")]
    [LinkGuide("For more information see:", typeof(Settings.StartingBalances.StartingBalances))]
    internal sealed class BalanceSheetForm : NakedVueForm<ManagerServer.Model.BalanceSheet>
    {
    }
}
