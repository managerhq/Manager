using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payslips
{
    [ProtoContract]
    [Title(nameof(Strings.Payslip))]
    [Guide("The `Payslip` view displays comprehensive payroll information for an employee, including earnings, deductions, and employer contributions.")]
    [Guide("This view provides a complete breakdown of an employee's pay for a specific period, showing how the net pay is calculated from gross earnings.")]
    [Header("Available Actions")]
    [Guide("From this view, you can perform several actions:")]
    [Guide("• `Edit` - Modify the payslip details")]
    [Guide("• `Print` - Generate a printable version of the payslip")]
    [Guide("• `Email` - Send the payslip directly to the employee's registered email address")]
    [Guide("• `New Payment` - Create a payment transaction for the net pay amount")]
    [Header("Payslip Components")]
    [Guide("The payslip displays the following key components:")]
    [Guide("• `Gross Pay` - Total earnings before any deductions")]
    [Guide("• `Deductions` - Amounts subtracted from gross pay (such as taxes, insurance, or retirement contributions)")]
    [Guide("• `Net Pay` - The final amount payable to the employee after all deductions")]
    [Guide("• `Employer Contributions` - Additional amounts paid by the employer (not deducted from employee pay)")]
    [Header("Period Totals")]
    [Guide("When enabled, the payslip can display cumulative totals for a specified period, showing year-to-date or custom period totals for all earnings, deductions, and contributions.")]
    [LinkGuide("To learn how to create and edit payslips, see:", typeof(PayslipForm))]
    internal sealed class PayslipView : TransactionView<ManagerServer.Model.Payslip>
    {
        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForPayslip>();
        }

        protected override string GetRecipient()
        {
            var business = ApplicationData.Businesses.Get(Business);
            return business.SingleOrDefault<Employee>(business.SingleOrDefault<Payslip>(Key)?.employee)?.Email;
        }

        protected override Type[] GetCopyToOptions()
        {
            return [ typeof(ManagerServer.Model.Payment), typeof(ManagerServer.Model.Payslip), typeof(ManagerServer.Model.RecurringPayslip) ];
        }

        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new PayslipTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}