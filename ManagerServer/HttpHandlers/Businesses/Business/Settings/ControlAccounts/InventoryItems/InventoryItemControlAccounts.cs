using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.InventoryItems
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(InventoryItems))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.InventoryItems))]
    [Guide("Control accounts for inventory items help you manage and track the total value of your inventory on hand.")]
    [Guide("These accounts automatically summarize the value of all individual inventory items into single balance sheet accounts, providing a consolidated view of your inventory value while maintaining detailed item-level tracking.")]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Columns]
    internal sealed class InventoryItemControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForInventoryItems>
    {
        [Guid("0b82d878-ab8d-4c91-8c9c-69bffe5e061d")]
        [Guide("The name identifies each inventory control account in your chart of accounts.")]
        [Header("Understanding Control Accounts")]
        [Guide("Control accounts are summary accounts in the general ledger that represent the total value of all individual inventory items. They automatically consolidate the value of all inventory items into a single balance sheet account, tracking the total cost of goods held for sale or production based on your chosen valuation method (FIFO, average cost, etc.).")]
        [Guide("Benefits of using inventory control accounts include real-time inventory valuation, automatic updates for purchases and sales, simplified stock management, and the ability to maintain detailed item-level tracking while presenting summarized values in financial statements.")]
        [Header("Naming Guidelines")]
        [Guide("When naming inventory control accounts, use descriptive names that clearly identify the type of inventory. Common examples include *Merchandise Inventory*, *Raw Materials*, *Work in Progress*, *Finished Goods*, or *Spare Parts Inventory*.")]
        [Header("Best Practices")]
        [Guide("Create separate control accounts for different inventory categories (raw materials vs. finished goods) or locations (warehouse vs. consignment) to improve inventory analysis, facilitate physical counts, and enhance operational decision-making.")]
        [Guide("This separation allows you to track inventory values more precisely and provides better insights into your inventory composition and movement patterns.")]
        public string GetName(ManagerServer.Model.ControlAccountForInventoryItems row) => row.Name;

        [Guid("a019a297-3393-42ec-925d-daec7481a6ab")]
        [Guide("The balance sheet group determines where this inventory control account appears on your balance sheet.")]
        [Guide("Inventory accounts typically belong to the *Current Assets* group, as inventory is expected to be sold or used within one year. However, you can assign them to other asset groups if needed for specific reporting requirements.")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForInventoryItems row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}
