using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.AmortizationEntries
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(IntangibleAssets))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.AmortizationEntries))]
    [Guide("Control accounts for *accumulated amortization* track the total amortization charged against all intangible assets.")]
    [Guide("These accounts automatically summarize amortization entries from individual intangible assets into a single contra-asset account on the balance sheet.")]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Columns]
    internal sealed class IntangibleAssetAccumulatedAmortizationControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForIntangibleAssetsAccumulatedAmortization>
    {
        [Guid("b0dcd205-8bff-45c4-9b8d-a864a79bf982")]
        [Guide("The *name* identifies this control account in your chart of accounts.")]
        [Header("Understanding Control Accounts")]
        [Guide("Control accounts are summary accounts in the general ledger that represent the total accumulated amortization of all intangible assets.")]
        [Guide("An *intangible asset accumulated amortization control account* automatically consolidates all amortization entries into a single contra-asset account. This reduces the carrying value of intangible assets on the balance sheet to reflect their net book value.")]
        [Header("Naming Conventions")]
        [Guide("When naming accumulated amortization control accounts, use descriptive names that match the related intangible asset categories:")]
        [Guide("• Accumulated Amortization - Software")]
        [Guide("• Accumulated Amortization - Patents")]
        [Guide("• Accumulated Amortization - Licenses")]
        [Guide("• Accumulated Amortization - Intangibles")]
        [Header("Benefits")]
        [Guide("Using control accounts provides several advantages:")]
        [Guide("• Automatic tracking of intangible asset amortization")]
        [Guide("• Simplified calculation of net carrying values")]
        [Guide("• Detailed amortization schedules for each asset while presenting consolidated amounts")]
        [Guide("• Proper matching of costs with revenue generation periods")]
        [Header("Best Practices")]
        [Guide("Create separate accumulated amortization control accounts for each intangible asset control account category.")]
        [Guide("This approach helps maintain clear relationships between assets and their amortization, distinguishes between different amortization periods, and ensures compliance with accounting standards for intangible asset presentation.")]
        public string GetName(ManagerServer.Model.ControlAccountForIntangibleAssetsAccumulatedAmortization row) => row.Name;

        [Guid("27816390-c3e8-4554-a778-7093fafe3116")]
        [Guide("The *balance sheet group* determines where this control account appears on your balance sheet.")]
        [Guide("Accumulated amortization accounts are typically placed under the intangible assets section as contra-asset accounts.")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForIntangibleAssetsAccumulatedAmortization row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}
