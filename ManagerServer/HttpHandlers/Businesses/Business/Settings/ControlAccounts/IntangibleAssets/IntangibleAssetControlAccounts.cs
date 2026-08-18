using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.IntangibleAssets
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(IntangibleAssets))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.IntangibleAssets))]
    [Guide("Control accounts for *intangible assets* help you manage non-physical assets at their historical cost.")]
    [Guide("These accounts automatically summarize the total purchase value of all your *intangible assets* in a single *balance sheet* account.")]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Columns]
    internal sealed class IntangibleAssetControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForIntangibleAssets>
    {
        [Guid("f402ea84-8382-4cec-91b7-08396a14ad5a")]
        [Guide("Control accounts are summary accounts in the *general ledger* that represent the total cost of all individual *intangible assets* in the subsidiary ledger.")]
        [Guide("An *intangible asset control account* automatically consolidates the purchase value of all intangible assets into a single *balance sheet* account.")]
        [Header("What Are Intangible Assets?")]
        [Guide("Intangible assets are non-physical assets that provide value to your business, such as:")]
        [Guide("• Patents and trademarks")]
        [Guide("• Software licenses")]
        [Guide("• Goodwill")]
        [Guide("• Customer relationships")]
        [Guide("• Copyrights and intellectual property")]
        [Header("Naming Your Control Accounts")]
        [Guide("Use descriptive names that clearly identify the type of intangible assets being tracked:")]
        [Guide("• 'Intellectual Property' for patents and copyrights")]
        [Guide("• 'Software Licenses' for purchased software")]
        [Guide("• 'Patents & Trademarks' for registered intellectual property")]
        [Guide("• 'Goodwill' for business acquisition premiums")]
        [Guide("• 'Customer Relationships' for acquired customer bases")]
        [Header("Benefits")]
        [Guide("Using control accounts for intangible assets provides:")]
        [Guide("• Organized management of non-physical assets")]
        [Guide("• Simplified financial reporting")]
        [Guide("• Automatic tracking of acquisitions and write-offs")]
        [Guide("• Detailed subsidiary records while keeping the *general ledger* streamlined")]
        [Header("Best Practices")]
        [Guide("Create separate control accounts for different intangible asset categories to facilitate:")]
        [Guide("• *Amortization* scheduling (finite life vs. indefinite life assets)")]
        [Guide("• Impairment testing requirements")]
        [Guide("• Compliance with accounting standards")]
        [Guide("• Distinction between purchased and internally developed assets")]
        public string GetName(ManagerServer.Model.ControlAccountForIntangibleAssets row) => row.Name;

        [Guid("94233c7d-e3fa-4d0c-ba51-999be7f715db")]
        [Guide("The *balance sheet* group where this control account will appear in your financial reports.")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForIntangibleAssets row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}
