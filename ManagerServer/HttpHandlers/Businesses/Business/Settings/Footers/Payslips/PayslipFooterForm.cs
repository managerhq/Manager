using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.Payslips
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of payslips.")]
    [Guide("Use footers to add terms, conditions, or additional information to payslips.")]
    [Fields(typeof(ManagerServer.Model.PayslipFooter))]
    internal sealed class PayslipFooterForm : NakedVueForm<ManagerServer.Model.PayslipFooter>
    {
    }
}