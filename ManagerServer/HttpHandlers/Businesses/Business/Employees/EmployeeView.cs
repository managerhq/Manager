using System;
using ManagerServer.Api.Businesses.Business.Employees;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Employees
{
    [ProtoContract]
    [Title(nameof(Strings.Employee))]
    [Guide("The `Employee` view displays comprehensive information about an individual employee, including their personal details, employment information, and transaction history.")]
    [Guide("From this view, you can click `Edit` to modify employee details, view related transactions, or manage file attachments associated with the employee record.")]
    [LinkGuide("To learn about creating and editing employees, see:", typeof(EmployeeForm))]
    internal sealed class EmployeeView : DefaultView<GetEmployeeView>
    {
    }
}
