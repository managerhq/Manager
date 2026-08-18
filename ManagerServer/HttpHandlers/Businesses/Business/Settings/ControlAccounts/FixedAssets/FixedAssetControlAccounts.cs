using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.FixedAssets
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(FixedAssets))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.FixedAssets))]
    [Guide("*Control accounts* for *fixed assets* automatically track the total cost of your assets in the general ledger.")]
    [Guide("When you record transactions for individual fixed assets, these control accounts summarize their combined purchase value on your balance sheet.")]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Columns]
    internal sealed class FixedAssetControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForFixedAssets>
    {
        [Guid("acb6d233-c61d-4820-9a0f-4c159e144fff")]
        [Guide("*Control accounts* are summary accounts in the general ledger that represent the total cost of all individual *fixed assets* in the subsidiary ledger.")]
        [Guide("A *fixed asset control account* automatically consolidates the purchase value of all fixed assets into a single balance sheet account, tracking the historical cost of property, plant, and equipment before depreciation.")]
        [Header("Naming Your Control Accounts")]
        [Guide("When naming *fixed asset control accounts*, use descriptive names that identify the category of assets, such as 'Property, Plant & Equipment', 'Motor Vehicles at Cost', 'Computer Equipment', or 'Furniture & Fixtures'.")]
        [Guide("This makes it easier to identify different asset categories on your balance sheet and in financial reports.")]
        [Header("Benefits and Best Practices")]
        [Guide("Benefits include organized asset management, simplified financial reporting, automatic tracking of asset acquisitions and disposals, and the ability to maintain detailed *asset registers* while keeping the chart of accounts concise.")]
        [Guide("Best practice: Create separate control accounts for different asset classes (buildings, vehicles, equipment) to facilitate *depreciation calculations*, insurance management, and compliance with accounting standards for asset classification and presentation.")]
        public string GetName(ManagerServer.Model.ControlAccountForFixedAssets row) => row.Name;

        [Guid("ba5fc85e-8763-4399-b162-3d294c388428")]
        [Guide("Select the *balance sheet group* where this control account should appear in your financial statements.")]
        [Guide("This determines where the total value of your fixed assets will be displayed on the balance sheet.")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForFixedAssets row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}
