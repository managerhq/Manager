using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.Employees
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Employees))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.Employees))]
    [Guide("Employee control accounts are special accounts in the general ledger that automatically consolidate all employee-related transactions into a single balance sheet account.")]
    [Guide("These accounts track amounts owed to or from employees, including salary advances, expense reimbursements, loans, and other employee-related financial transactions.")]
    [Guide("The system automatically maintains the balance of each *control account* by aggregating all individual employee balances from the subsidiary ledger.")]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Columns]
    internal sealed class EmployeeControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForEmployees>
    {
        [Guid("d626a606-ab1b-4318-8839-37e70dcc9d3d")]
        [Guide("The name identifies each employee control account in your chart of accounts.")]
        [Header("Overview")]
        [Guide("Control accounts are summary accounts in the general ledger that represent the total of all individual employee balances in the subsidiary ledger.")]
        [Guide("An employee control account automatically consolidates all amounts owed to or from employees into a single balance sheet account, tracking employee-related financial transactions such as salary advances, expense reimbursements, or loan repayments.")]
        [Header("Naming Guidelines")]
        [Guide("When naming employee control accounts, use descriptive names that clearly indicate the nature of employee transactions, such as *Employee Advances*, *Staff Loans*, *Expense Claims Payable*, or *Employee Clearing Account*.")]
        [Guide("Choose names that make it easy to identify the account's purpose when viewing financial reports or the chart of accounts.")]
        [Header("Benefits and Best Practices")]
        [Guide("Benefits include simplified payroll and expense management, automatic tracking of employee-related assets and liabilities, and the ability to maintain detailed employee transaction records while keeping the general ledger organized.")]
        [Guide("Best practice: Create separate control accounts for different types of employee transactions (advances vs. expense claims) or employee groups (permanent staff vs. contractors) to facilitate better analysis and internal control.")]
        public string GetName(ManagerServer.Model.ControlAccountForEmployees row) => row.Name;

        [Guid("9d83c7dd-947d-4de3-adc7-f906a98daaa9")]
        [Guide("The balance sheet group determines where this control account appears on your balance sheet.")]
        [Guide("Employee control accounts typically appear under *Current Assets* (for amounts owed by employees) or *Current Liabilities* (for amounts owed to employees), depending on the nature of the transactions.")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForEmployees row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}
