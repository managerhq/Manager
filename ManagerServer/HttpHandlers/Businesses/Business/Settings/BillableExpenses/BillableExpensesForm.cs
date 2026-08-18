using System;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.BillableExpenses
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Customers))]
    [Title(nameof(Strings.BillableExpenses))]
    [Guide("*Billable expenses* are expenses that a business incurs on behalf of its customers, expecting to be reimbursed later. These expenses might include materials, external services, or travel costs. You can track these expenses and bill the appropriate customer for them.")]
    [Header("Getting Started")]
    [Guide("To enable billable expenses, go to the **Settings** tab and click **Billable Expenses**.")]
    [SettingsItemScreenshot("fa-briefcase", nameof(Strings.BillableExpenses))]
    [Guide("Check the **Enabled** checkbox to activate this feature.")]
    [CheckboxScreenshot(name: nameof(Strings.Enabled))]
    [Header("Setting Up Customer Tracking")]
    [Guide("After enabling billable expenses, navigate to the **Customers** tab and click the **Edit Columns** button.")]
    [Guide("Enable the **Uninvoiced** column to monitor billable expenses that have not yet been invoiced to customers.")]
    [Header("How Billable Expenses Work")]
    [Guide("When you activate billable expenses, a new *Billable expenses* account is automatically added to your *chart of accounts*.")]
    [Guide("This account becomes available in various transactions including *payments*, *purchase invoices*, and *expense claims*.")]
    [Guide("To record a billable expense, select the **Billable expenses** account in your transaction, then choose the **Customer** to allocate the expense to.")]
    [SelectAccountScreenshot(accountName: nameof(Strings.BillableExpense), prepend: nameof(Strings.Customer))]
    [Header("Accounting Impact")]
    [Guide("*Billable expenses* is an asset account on the *balance sheet*. Recording new billable expenses does not affect your *profit and loss statement*.")]
    [Guide("This ensures that expenses to be reimbursed later do not inflate your income and expenses until they are actually invoiced to the customer.")]
    internal sealed class BillableExpensesForm : NakedVueForm<ManagerServer.Model.BillableExpenses>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            return !ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BillableExpenses>().Enabled;
        }
    }
}
