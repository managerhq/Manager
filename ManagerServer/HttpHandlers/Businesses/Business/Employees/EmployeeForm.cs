using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Employees
{
    [ProtoContract]
    [Title(nameof(Strings.Employee), nameof(Strings.Edit))]
    [Guide("The `Employee` form allows you to create and edit employee records in your business.")]
    [Guide("Employee records are essential for managing your workforce and processing payroll. Each employee record stores important personal and employment information that can be used across various features in the system.")]
    [Guide("You can include photos of employees by uploading an image file. This helps with identification and adds a personal touch to employee records.")]
    [Guide("The information entered here will be used in payslips, timesheets, expense claims, and other employee-related transactions throughout the system.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.Employee))]
    internal sealed class EmployeeForm : NakedVueForm<Employee>
    {
        protected override bool CanHaveImage()
        {
            return true;
        }
    }
}