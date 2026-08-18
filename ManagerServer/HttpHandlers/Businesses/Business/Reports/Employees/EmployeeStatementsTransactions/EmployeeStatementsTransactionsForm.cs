using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.EmployeeStatementsTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.EmployeeStatements), nameof(Strings.Transactions))]
    [Guide("Employee statements with transactions provide a comprehensive record of all financial activities between your business and your employees. These statements show every transaction in chronological order, including payslips, expense claims, payments, and any other adjustments.")]
    [Guide("This report is essential for reconciling employee clearing accounts, verifying outstanding balances, and maintaining accurate records of amounts owed to or by employees. Each transaction is listed with its date, reference number, description, and the resulting balance after each entry.")]
    [Guide("Use employee transaction statements when you need to review the complete history with an employee, resolve discrepancies, or provide documentation for audit purposes. The statements can be customized to show transactions for specific date ranges and can include opening balances to provide a complete picture of the account status.")]
    [Fields(typeof(ManagerServer.Model.EmployeeStatementsTransactions))]
    internal sealed class EmployeeStatementsTransactionsForm : NakedVueForm<ManagerServer.Model.EmployeeStatementsTransactions>
    {
    }
}
