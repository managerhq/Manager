using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Divisions
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.Divisions))]
    [Guid("3667ceb4-c81a-4a90-9273-23ed3565d6c9")]
    [Guide("The *Divisions* feature enables you to track different segments of your business independently.")]
    [Guide("Each division can have its own income, expenses, assets, and liabilities for complete financial separation.")]
    [Guide("Common uses include geographic regions, product lines, departments, or business units.")]
    [SettingsItemScreenshot("fa-chart-pie", nameof(Strings.Divisions))]
    [Header("Creating Divisions")]
    [Guide("To create a new division, click the **New Division** button.")]
    [Guide("Give each division a clear name and optionally a code for quick identification.")]
    [HeroButtonScreenshot(title: nameof(Strings.Divisions), name: nameof(Strings.NewDivision))]
    [LinkGuide("For more information, see:", typeof(DivisionForm))]
    [Header("Assigning Divisions to Transactions")]
    [Guide("Once created, assign divisions to individual transactions like *Payments*, *Receipts*, and *Sales Invoices*.")]
    [Guide("This builds a complete picture of each division's financial performance.")]
    [Guide("Divisions can be assigned to transactions affecting profit & loss accounts or custom balance sheet accounts.")]
    [Guide("This allows tracking of divisional income, expenses, and custom assets or liabilities.")]
    [Header("Division Rules for Sub-Accounts")]
    [Guide("Sub-accounts like *Bank & Cash Accounts*, *Customers*, *Suppliers*, and *Fixed Assets* cannot have divisions assigned at the transaction level.")]
    [Guide("Instead, these accounts must be assigned to a division at the account level.")]
    [Guide("Sub-accounts must be wholly owned by a single division because their entire balance belongs to that division.")]
    [Guide("For example, a bank account balance cannot be split between divisions - the whole account belongs to one division.")]
    [Guide("This often means having separate bank accounts, customer accounts, or assets for each division.")]
    [Header("Interdivisional Transactions")]
    [Guide("Manager automatically handles cross-division transactions by creating *interdivisional loan accounts*.")]
    [Guide("Example: If Division A's bank account pays for Division B's expense, Manager tracks this as an interdivisional loan.")]
    [Guide("This ensures each division's financial position remains accurate even with shared resources.")]
    [Header("Divisional Reporting")]
    [Guide("Financial reports can be generated for individual divisions or compared side-by-side.")]
    [Guide("Both *Balance Sheet* and *Profit and Loss Statement* support divisional reporting.")]
    [Guide("Create comparative reports to analyze performance across divisions and identify top performers.")]
    [Header("Divisions vs Projects")]
    [Guide("Use divisions for permanent or long-term business segments like regions, departments, or product lines.")]
    [Guide("This differs from *Projects* which typically have start and end dates and are temporary in nature.")]
    [Guide("Divisions continue indefinitely until deactivated, while projects have defined lifecycles.")]
    [LinkGuide("For more information, see:", typeof(Projects.Projects))]
    internal sealed class Divisions : NakedObjectsWithAutomaticRows<ManagerServer.Model.Division>
    {
        [WarnIfNotUnique]
        [Guid("0626d2e9-b89b-4dae-b74b-646e345f7dde")]
        public string[] GetCode(ManagerServer.Model.Division[] rows)
        {
            return rows.Select(x => x.Code).ToArray();
        }

        [Default]
        [Guid("66fed5ea-c0ff-4498-a9fe-ef81473ae870")]
        public string[] GetName(ManagerServer.Model.Division[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }
    }
}
