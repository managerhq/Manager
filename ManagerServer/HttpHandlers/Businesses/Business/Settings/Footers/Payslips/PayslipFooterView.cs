using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.Payslips
{
    [ProtoContract]
    [Title(nameof(Strings.Payslip), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This screen displays the current *payslip footer* that will appear at the bottom of all payslips issued to employees.")]
    [Guide("The footer typically contains important information such as payment instructions, company policies, or legal disclaimers that need to be included on every payslip.")]
    [Guide("You can preview exactly how the footer will appear on printed or emailed payslips before making any changes.")]
    [LinkGuide("To edit the footer content, see:", typeof(PayslipFooterForm))]
    internal class PayslipFooterView : DefaultView<GetPayslipFooterView>
    {
    }
}
