using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.DepreciationEntries
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(FixedAssets))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.DepreciationEntries))]
    [Guide("Control accounts for *depreciation entries* track the accumulated depreciation of *fixed assets* in your accounting system.")]
    [Guide("These accounts automatically consolidate all depreciation charged against fixed assets into summary accounts on your balance sheet.")]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Columns]
    internal sealed class FixedAssetAccumulatedDepreciationControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForFixedAssetsAccumulatedDepreciation>
    {
        [Guid("8bb55a43-9e6a-4012-9eae-d2cf124eceac")]
        [Guide("Control accounts are summary accounts in the *general ledger* that represent the total accumulated depreciation of all *fixed assets*.")]
        [Guide("These *contra-asset accounts* automatically consolidate all depreciation entries, reducing the carrying value of assets on the balance sheet to show their *net book value*.")]
        [Header("Naming Control Accounts")]
        [Guide("Use descriptive names that match your asset categories:")]
        [Guide("• Accumulated Depreciation - Buildings")]
        [Guide("• Accumulated Depreciation - Vehicles")]
        [Guide("• Accumulated Depreciation - Equipment")]
        [Guide("• Accumulated Depreciation - Property, Plant & Equipment")]
        [Header("Benefits")]
        [Guide("Control accounts provide automatic tracking of asset depreciation and simplified calculation of *net book values*.")]
        [Guide("They maintain detailed depreciation schedules for each asset while presenting consolidated amounts in financial statements.")]
        [Guide("This ensures compliance with accounting standards for asset valuation and reporting.")]
        [Header("Best Practices")]
        [Guide("Create separate *accumulated depreciation control accounts* for each *fixed asset control account* category.")]
        [Guide("This maintains clear relationships between assets and their depreciation, facilitates asset disposal calculations, and improves financial statement presentation.")]
        public string GetName(ManagerServer.Model.ControlAccountForFixedAssetsAccumulatedDepreciation row) => row.Name;

        [Guid("eccbcf6c-6682-4c3c-b6bf-fb04c767722c")]
        [Guide("Select the *balance sheet group* where this accumulated depreciation account will appear in your financial statements.")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForFixedAssetsAccumulatedDepreciation row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}
