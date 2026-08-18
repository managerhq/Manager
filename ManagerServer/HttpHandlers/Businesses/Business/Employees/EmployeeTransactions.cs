using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.Employees
{
    [ProtoContract]
    [Title(nameof(Strings.Employees), nameof(Strings.Transactions))]
    [Guide("The *Employee Transactions* screen displays a complete history of all financial transactions for a specific employee.")]
    [Guide("This includes all movements in the employee's *clearing account*, such as payslips issued, expense claims submitted, and any other employee-related financial entries.")]
    [Guide("Transactions are listed in chronological order with the most recent entries appearing first. Each transaction shows the date, description, debit and credit amounts, and the running balance.")]
    [Guide("The balance shown represents the amount owed to or by the employee at any point in time. A positive balance indicates the business owes money to the employee, while a negative balance indicates the employee owes money to the business.")]
    [LinkGuide("For more information, see:", typeof(EmployeeForm))]
    internal sealed class EmployeeTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid Employee;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsEmployeeClearingAccount && x.Employee?.Key == Employee).OrderByDescending(x => x.Date).ToArray();
        }
    }
}
