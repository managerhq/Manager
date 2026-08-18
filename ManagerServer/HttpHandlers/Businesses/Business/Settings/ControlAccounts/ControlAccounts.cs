using System;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.ControlAccounts))]
    [Guide("Control accounts allow you to customize how account balances are grouped and displayed on financial statements. Access this feature from the **Settings** tab to create and manage your own control accounts.")]
    [SettingsItemScreenshot("fa-object-group", nameof(Strings.ControlAccounts))]
    [Header("Understanding Control Accounts")]
    [Guide("Your business tracks balances across many different accounts: bank accounts, customers, suppliers, employees, capital accounts, fixed assets, intangible assets, and investments. Each account maintains a balance showing what you own, what others owe you, or what you owe to others.")]
    [Guide("The **Balance Sheet** report displays your assets and liabilities. However, since businesses typically have hundreds or thousands of individual accounts, showing every account separately would make financial statements unwieldy and difficult to read.")]
    [Guide("Control accounts solve this problem by combining similar accounts into single line items. For example, all customer balances appear under *Accounts receivable*, while all bank and cash accounts combine under *Cash and cash equivalents*. This keeps your **Balance Sheet** concise and easy to understand.")]
    [Header("Customizing Your Control Accounts")]
    [Guide("The default control account groupings work well for most businesses, but you can create custom control accounts to organize your accounts differently. This gives you complete flexibility over how information appears on your financial statements.")]
    [Guide("To create custom control accounts, first set up new control accounts for the account types you want to separate. Then assign individual accounts to your custom control accounts.")]
    [Header("Example: Fixed Assets")]
    [Guide("Instead of showing all fixed assets under a single *Fixed assets at cost* account, you can create separate control accounts for different asset categories:")]
    [Guide("• Machinery at cost\n• Vehicles at cost\n• Furniture at cost\n• Buildings at cost\n• Land at cost")]
    [Guide("After creating these control accounts, go to the **Fixed Assets** tab. When editing individual fixed assets, you'll see a new *Control account* field where you can specify which control account should include that asset.")]
    [Header("Example: Bank Accounts")]
    [Guide("You can also display bank accounts individually on your **Balance Sheet** instead of combining them. Simply create a control account for each bank account, then assign each bank account to its corresponding control account.")]
    [Guide("This approach is particularly useful when you need to show stakeholders the exact balance of specific bank accounts directly on financial statements.")]
    internal sealed class ControlAccounts : NakedNamespaces
    {
    }
}
