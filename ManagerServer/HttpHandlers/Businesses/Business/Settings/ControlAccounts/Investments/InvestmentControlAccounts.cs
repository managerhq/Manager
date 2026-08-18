using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.Investments
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Investments))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.Investments))]
    [Guide("Investment control accounts help you manage and track your investment holdings in the accounting system.")]
    [Guide("These accounts automatically summarize the value of all your investments into a single balance sheet account, providing a consolidated view of your investment portfolio.")]
    [Guide("Each control account represents a collection of individual investments, making it easier to organize your financial statements while maintaining detailed records of each security.")]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Columns]
    internal sealed class InvestmentControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForInvestments>
    {
        [Guid("da80c3d2-5a80-4e73-9d61-7f85f3254855")]
        [Guide("Control accounts are summary accounts in the *general ledger* that represent the total value of all individual investments in the subsidiary ledger.")]
        [Guide("An investment control account automatically consolidates the value of all investment holdings into a single *balance sheet* account, tracking securities, bonds, shares, and other financial instruments at their current carrying value.")]
        [Header("Naming Your Investment Control Accounts")]
        [Guide("When naming investment control accounts, use descriptive names that identify the type or purpose of investments, such as 'Marketable Securities', 'Long-term Investments', 'Equity Investments', 'Bond Portfolio', or 'Strategic Holdings'.")]
        [Guide("Clear naming helps you quickly identify account purposes and simplifies financial reporting.")]
        [Header("Benefits of Using Investment Control Accounts")]
        [Guide("Investment control accounts provide streamlined portfolio management by automatically tracking all investment purchases and sales in one place.")]
        [Guide("They offer real-time valuation updates and allow you to maintain detailed investment records while presenting consolidated positions in your financial statements.")]
        [Header("Best Practices")]
        [Guide("Create separate control accounts for different investment classifications (such as trading, available-for-sale, or held-to-maturity) to facilitate proper accounting treatment.")]
        [Guide("Consider organizing control accounts by asset types (equities versus fixed income) to improve performance analysis and ensure regulatory compliance.")]
        public string GetName(ManagerServer.Model.ControlAccountForInvestments row) => row.Name;

        [Guid("401551b2-0875-4900-bb13-73f736c485ff")]
        [Guide("Specifies the *balance sheet* group where this investment control account will appear in your financial statements.")]
        [Guide("This determines whether your investments are classified as current assets (short-term investments) or non-current assets (long-term investments) on the *balance sheet*.")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForInvestments row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}
