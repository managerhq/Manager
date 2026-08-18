using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPayslips
{
    [ProtoContract]
    [Title(nameof(Strings.RecurringPayslip))]
    [Guide("Set up recurring payslips for regular employee payments.")]
    [Guide("Automatically generate payslips for salaries, wages, and regular compensation.")]
    [Fields(typeof(ManagerServer.Model.RecurringPayslip))]
    internal sealed class RecurringPayslipForm : NakedVueForm<ManagerServer.Model.RecurringPayslip>
    {
        [ProtoMember(1)] public Guid? Payslip;

        protected override void OnSource(RecurringPayslip form, ManagerServer.Model.Object source)
        {
            if (source is ManagerServer.Model.Payslip payslip)
            {
                form.employee = payslip.employee;
                form.description = payslip.description;
                form.Earnings = payslip.Earnings;
                form.Deductions = payslip.Deductions;
                form.Contributions = payslip.Contributions;
                form.ShowTotalsForThePeriod = payslip.ShowTotalsForThePeriod;
                form.TotalsPeriodStart = payslip.TotalsPeriodStart;
                form.CustomFields = payslip.CustomFields;
                form.CustomTheme = payslip.CustomTheme;
            }
        }
    }
}