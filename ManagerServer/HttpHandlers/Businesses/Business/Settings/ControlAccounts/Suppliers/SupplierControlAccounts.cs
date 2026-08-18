using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.Suppliers
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Suppliers))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.Suppliers))]
    [Guide("*Control accounts* for suppliers are summary accounts that automatically consolidate all amounts owed to suppliers into a single *balance sheet* account.")]
    [Guide("Use this screen to manage how supplier balances are grouped and displayed in your financial reports.")]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Columns]
    internal sealed class SupplierControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForSuppliers>
    {
        [Guid("ad22e7ae-cb8f-4f8f-bded-5265c9e660c1")]
        [Guide("The name identifies this *control account* in your *general ledger* and financial reports.")]
        [Header("Understanding Control Accounts")]
        [Guide("*Control accounts* are summary accounts in the *general ledger* that represent the total of all individual supplier balances. They automatically consolidate amounts owed to suppliers, providing a comprehensive view of your *accounts payable*.")]
        [Guide("The system automatically updates the *control account* balance whenever you create or modify transactions involving suppliers. This ensures your *general ledger* always reflects the total of detailed supplier records without manual reconciliation.")]
        [Header("Naming Your Control Accounts")]
        [Guide("Choose clear, descriptive names that identify the type of suppliers or payables being tracked. Common examples include:")]
        [Guide("• **Trade Payables** - For regular inventory and material suppliers")]
        [Guide("• **Accrued Expenses** - For services received but not yet invoiced")]
        [Guide("• **Vendor Deposits** - For advance payments to suppliers")]
        [Header("Best Practices")]
        [Guide("Consider creating separate *control accounts* for different supplier categories to improve cash flow management and financial analysis. For example:")]
        [Guide("• Group inventory suppliers separately from service providers")]
        [Guide("• Separate current payables from overdue amounts")]
        [Guide("• Distinguish between domestic and foreign suppliers if you deal in multiple currencies")]
        public string GetName(ManagerServer.Model.ControlAccountForSuppliers row) => row.Name;

        [Guid("b27ad422-3591-4e59-bfd0-3eb6f3ef949e")]
        [Guide("Select the *balance sheet* group where this *control account* should appear in financial reports. This determines how supplier balances are categorized on your *balance sheet*.")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForSuppliers row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}
