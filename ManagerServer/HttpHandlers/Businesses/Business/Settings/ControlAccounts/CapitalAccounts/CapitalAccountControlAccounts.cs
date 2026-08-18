using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.CapitalAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(CapitalAccounts))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.CapitalAccounts))]
    [Guide("Control accounts for capital accounts automatically consolidate individual owner equity balances into summary accounts in your general ledger.")]
    [Guide("These control accounts provide a single balance sheet line item that represents the combined equity of all partners, shareholders, or proprietors while maintaining detailed tracking of each individual's capital position.")]
    [Guide("To create a new control account, click the **New Control Account** button.")]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Columns]
    internal sealed class CapitalAccountControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForCapitalAccounts>
    {
        [Guid("b82a7f2e-b57b-41d3-9a36-25a45c7315d8")]
        [Guide("Control accounts are summary accounts in the general ledger that represent the total of all individual capital account balances.")]
        [Header("How Control Accounts Work")]
        [Guide("A capital account control account automatically consolidates all owners' equity accounts into a single balance sheet account. It tracks capital contributions, drawings, profit allocations, and ownership interests for partners, shareholders, or proprietors.")]
        [Guide("The control account balance is automatically updated whenever transactions affect individual capital accounts, ensuring your balance sheet always shows accurate equity totals.")]
        [Header("Naming Control Accounts")]
        [Guide("When naming capital account control accounts, use descriptive names that identify the type of equity being tracked. Common examples include *Partners' Capital*, *Shareholders' Equity*, *Members' Capital*, *Retained Earnings*, or *Owner's Equity*.")]
        [Guide("Choose names that clearly indicate the ownership structure of your business and match your legal entity type.")]
        [Header("Benefits")]
        [Guide("Control accounts simplify equity reporting by presenting a single line item on the balance sheet while maintaining detailed records for each owner. They automatically track capital movements and profit distributions, ensuring accurate partnership or shareholder management.")]
        [Guide("This approach provides consolidated equity positions for financial reporting while preserving the detailed information needed for owner statements and tax reporting.")]
        [Header("Best Practices")]
        [Guide("Create separate control accounts for different classes of equity, such as ordinary shares versus preference shares, or for different owner types like active partners versus silent partners.")]
        [Guide("This separation ensures accurate capital allocation, proper distribution calculations, and compliance with partnership agreements or corporate bylaws.")]
        public string GetName(ManagerServer.Model.ControlAccountForCapitalAccounts row) => row.Name;

        [Guid("65916268-9295-4cbf-b79d-9d22e69ae4ad")]
        [Guide("The balance sheet group determines where this control account appears on your balance sheet. This should typically be set to an equity or capital group to properly classify owner investments and retained earnings.")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForCapitalAccounts row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}
