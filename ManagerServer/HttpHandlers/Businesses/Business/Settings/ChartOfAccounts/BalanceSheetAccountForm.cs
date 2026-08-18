using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.BalanceSheetAccount), nameof(Strings.Edit))]
    [Guide("The balance sheet account form is used to create or edit existing balance sheet accounts.")]
    [Guide("To create a new balance sheet account, navigate to the `Settings` tab, select `ChartOfAccounts`, and then click on `NewAccount` located in the `BalanceSheet` section of the chart of accounts.")]
    [Guide("The form includes the following fields:")]
    [Fields(typeof(ManagerServer.Model.BalanceSheetAccount))]
    [Guide("Balance sheet accounts at the top level cannot use foreign currencies. This is because these accounts must always appear in the base currency on financial statements, even if they were originally in a foreign currency. Therefore, if you need a custom balance sheet account that operates in a foreign currency, you should set it up as a `SpecialAccount` within the `SpecialAccounts` tab.")]
    [LinkGuide("For more information see:", typeof(SpecialAccounts.SpecialAccounts))]
    internal sealed class BalanceSheetAccountForm : NakedVueForm<BalanceSheetAccount>
    {        
    }
}
