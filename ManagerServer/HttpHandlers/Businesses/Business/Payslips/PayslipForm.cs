using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payslips
{
    [ProtoContract]
    [Title(nameof(Strings.Payslip), nameof(Strings.Edit))]
    [Guide("The `Payslip` form enables you to create detailed wage statements for employees, documenting their earnings, deductions, and net pay for each pay period.")]
    [Guide("Payslips serve as official records of employee compensation and create the necessary accounting entries to record wage expenses, tax liabilities, and amounts payable to employees.")]
    [Header("Purpose and Benefits")]
    [Guide("Each payslip provides transparency about gross wages, tax withholdings, benefit deductions, and employer contributions.")]
    [Guide("It documents the pay period, payment date, and a detailed breakdown of all compensation components.")]
    [Guide("The system automatically calculates net pay and creates a liability in the employee clearing account. This liability is cleared when you record the actual payment to the employee.")]
    [Header("Creating a Payslip")]
    [Guide("To create a payslip, start by selecting the employee and specifying the pay period dates.")]
    [Guide("Add earnings items such as regular wages, overtime pay, bonuses, or commissions with their respective amounts.")]
    [Guide("Each earnings item should reflect the actual work performed or compensation earned during the pay period.")]
    [Guide("Include all applicable deductions such as income tax, social security contributions, health insurance premiums, or retirement plan contributions.")]
    [Guide("These deductions reduce the employee's gross pay to calculate their net pay.")]
    [Guide("You can also record employer contributions that represent additional employment costs but don't affect the employee's net pay.")]
    [Guide("These might include employer-paid taxes, insurance contributions, or retirement plan matching.")]
    [Header("Form Fields")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.Payslip))]
    internal sealed class PayslipForm : NakedVueForm<ManagerServer.Model.Payslip>
    {
        protected override bool CanHaveImage() => true;

        protected override void OnSource(Payslip form, ManagerServer.Model.Object source)
        {
            if (source is Payslip payslip)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<Payslip>(Business, payslip.CustomFields);
            }
        }
    }
}