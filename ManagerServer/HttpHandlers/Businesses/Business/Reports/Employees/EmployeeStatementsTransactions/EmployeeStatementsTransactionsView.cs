using HttpFramework;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Api.Businesses.Business.Reports.EmployeeStatementsTransactions;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.EmployeeStatementsTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.EmployeeStatementsTransactions), nameof(Strings.View))]
    [Guide("The **Employee Statement - Transactions** view provides a comprehensive transaction history for a selected employee within a specified date range.")]
    [Guide("This statement displays all financial transactions between your business and the employee, including payslips, expense claims, payments, and journal entries.")]
    [Guide("Each transaction is listed chronologically with its date, description, debit amount, credit amount, and running balance.")]
    [Guide("The statement includes an *opening balance* at the start date and calculates a *closing balance* at the end date, giving you a complete picture of the employee clearing account status.")]
    [LinkGuide("To generate this statement, see:", typeof(EmployeeStatementsTransactionsForm))]
    internal sealed class EmployeeStatementsTransactionsView : DefaultView<GetEmployeeStatementsTransactionsView>
    {
        protected override bool CanHaveAttachments()
        {
            return false;
        }

        protected override void EditCloneButtons()
        {
            return;
        }

        protected override Guid? GetCustomTheme()
        {
            return ((IHasCustomTheme)ApplicationData.Businesses.Get(Business).Single<Model.EmployeeStatementsTransactions>()).GetCustomTheme();
        }

        protected override string GetRecipient()
        {
            return ApplicationData.Businesses.Get(Business).SingleOrDefault<Employee>(Key)?.Email;
        }
    }
}
