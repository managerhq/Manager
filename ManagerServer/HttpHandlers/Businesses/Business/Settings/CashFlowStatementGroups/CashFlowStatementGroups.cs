using System;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CashFlowStatementGroups
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(BankAndCashAccounts))]
    [Title(nameof(Strings.CashFlowStatementGroups))]
    [Guide("*Cash Flow Statement Groups* allow you to organize accounts into meaningful categories on your *cash flow statement*, making it easier to understand and analyze your cash flows.")]
    [SettingsItemScreenshot("fa-list", nameof(Strings.CashFlowStatementGroups))]
    [Header("Why Use Cash Flow Statement Groups")]
    [Guide("Without *cash flow statement groups*, the report displays individual accounts exactly as they appear in your *chart of accounts*. This can result in a lengthy report with excessive detail that becomes difficult to read and analyze.")]
    [Guide("By grouping related accounts together, you can create a more concise and meaningful cash flow statement. For example, expense accounts such as telephone, printing, and computer equipment can all be grouped under a common category like \"Payments to suppliers\".")]
    [Header("Setting Up Groups")]
    [Guide("To create cash flow statement groups, go to the **Settings** tab and click **Cash Flow Statement Groups**.")]
    [Guide("After creating groups, navigate to **Chart of Accounts** and edit each account. A new field will appear where you can select which *cash flow statement group* the account belongs to.")]
    internal sealed class CashFlowStatementGroups : NakedNamespaces
    {
    }
}
