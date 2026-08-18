using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.Payslips
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Payslips))]
    [Title(nameof(Strings.Payslip))]
    [Guide("Payslip footers are customizable text sections that appear at the bottom of employee payslips. These templates help you communicate important payroll information consistently across your organization.")]
    [Guide("You can create different footer templates for various purposes, such as standard payroll notices, executive compensation terms, or seasonal employee information. Select the appropriate footer when generating each payslip to ensure employees receive relevant information.")]
    [Columns]
    internal sealed class PayslipFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.PayslipFooter>
    {
        [Default]
        [Guide("Each footer template requires a descriptive name that helps you identify its purpose. Use clear, meaningful names that indicate the footer's content or intended use.")]
        [Header("Common Uses for Payslip Footers")]
        [Guide("**Confidentiality notices** - Legal disclaimers about the confidential nature of payroll information")]
        [Guide("**Contact information** - HR or payroll department contact details for employee queries")]
        [Guide("**Benefit reminders** - Important dates for benefits enrollment or changes")]
        [Guide("**Year-to-date summaries** - Notes about where employees can find cumulative pay information")]
        [Guide("**Company policies** - References to payroll policies or procedures")]
        [Header("Best Practices")]
        [Guide("Name your footer templates descriptively, such as *Standard Payroll Notice*, *Executive Compensation Terms*, or *Contractor Payment Information*.")]
        [Guide("Keep footer content concise but informative. Employees should be able to quickly find the information they need.")]
        [Guide("Update footer templates regularly to ensure the information remains current and relevant.")]
        public string[] GetName(ManagerServer.Model.PayslipFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}
