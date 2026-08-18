using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.SpecialAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SpecialAccounts))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.SpecialAccounts))]
    [Guide("Special account control accounts help you manage and consolidate balances for custom-defined accounts that don't fit into standard categories.")]
    [Guide("These control accounts automatically summarize the total balances of all related special sub-accounts in your subsidiary ledger, providing a single consolidated view in your general ledger.")]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Columns]
    internal sealed class SpecialAccountControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForSpecialAccounts>
    {
        [Guid("d3f0ce55-f36d-4512-9c94-43c8104a8e9e")]
        [Guide("Control accounts are summary accounts in the general ledger that represent the total of all individual special account balances in the subsidiary ledger.")]
        [Header("What are Special Account Control Accounts?")]
        [Guide("A special account control account automatically consolidates balances for custom-defined accounts that don't fit standard categories. Common examples include:")]
        [Guide("• Project accounts for tracking individual project finances")]
        [Guide("• Trust accounts for holding client or third-party funds")]
        [Guide("• Escrow accounts for temporarily holding funds during transactions")]
        [Guide("• Any other specialized tracking requirements unique to your business")]
        [Header("Naming Your Control Accounts")]
        [Guide("When naming special account control accounts, use descriptive names that clearly identify the purpose or nature of the accounts being controlled. Good examples include:")]
        [Guide("• *Project Accounts* - for consolidating all project-related accounts")]
        [Guide("• *Client Trust Funds* - for summarizing all client trust account balances")]
        [Guide("• *Escrow Accounts* - for tracking all escrow-related transactions")]
        [Guide("• *Restricted Funds* - for accounts with specific usage limitations")]
        [Header("Benefits and Best Practices")]
        [Guide("Special account control accounts provide several key benefits:")]
        [Guide("• Flexible account management for unique business needs")]
        [Guide("• Automatic consolidation of specialized account categories")]
        [Guide("• Detailed subsidiary records while maintaining a simplified general ledger")]
        [Guide("• Customized financial tracking beyond standard accounting categories")]
        [Guide("**Best practice**: Create separate control accounts for different types of special accounts based on their purpose, legal requirements, or reporting needs. This ensures proper segregation, compliance with regulations, and accurate financial reporting for specialized transactions.")]
        public string GetName(ManagerServer.Model.ControlAccountForSpecialAccounts row) => row.Name;

        [Guid("0f9e964b-f5dc-48da-8233-7c1df7212277")]
        [Guide("The balance sheet group determines where this control account appears on your balance sheet. Select the appropriate group based on the nature of the special accounts being controlled.")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForSpecialAccounts row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}
