using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.Customers
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Customers))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.Customers))]
    [Guide("Customer control accounts automatically track and summarize the total amounts owed by all your customers.")]
    [Guide("Instead of having individual accounts receivable entries cluttering your general ledger, control accounts consolidate all customer balances into a single, manageable account.")]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Columns]
    internal sealed class CustomerControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForCustomers>
    {
        [Guid("08c2bb49-079d-4232-b514-d4e5cec04d07")]
        [Guide("The name identifies this control account in your *chart of accounts* and financial reports.")]
        [Header("Understanding Control Accounts")]
        [Guide("Control accounts are special *general ledger* accounts that automatically summarize all individual customer balances from your *subsidiary ledger*.")]
        [Guide("When you record transactions for any customer, the system automatically updates the relevant control account, ensuring your *balance sheet* always shows accurate total receivables.")]
        [Header("Naming Your Control Accounts")]
        [Guide("Choose descriptive names that clearly identify the purpose of each control account, such as:")]
        [Guide("• **Trade Receivables** - For regular customer sales")]
        [Guide("• **Customer Deposits** - For prepayments received from customers")]
        [Guide("• **International Receivables** - For foreign customer accounts")]
        [Header("Benefits and Best Practices")]
        [Guide("Control accounts provide automatic reconciliation between your detailed customer records and general ledger, eliminating manual work and reducing errors.")]
        [Guide("For better financial analysis, create separate control accounts for different customer groups (domestic vs. international) or payment terms (current vs. overdue).")]
        [Guide("This approach keeps your *chart of accounts* clean while maintaining complete customer detail in the subsidiary records.")]
        public string GetName(ManagerServer.Model.ControlAccountForCustomers row) => row.Name;

        [Guid("0b5b8815-f352-4fa9-91e6-86c2d3fa85eb")]
        [Guide("Select the *balance sheet* group where this control account will appear in your financial reports.")]
        [Guide("Customer control accounts typically belong under **Assets** in a group like **Current Assets** or **Accounts Receivable**.")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForCustomers row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}
