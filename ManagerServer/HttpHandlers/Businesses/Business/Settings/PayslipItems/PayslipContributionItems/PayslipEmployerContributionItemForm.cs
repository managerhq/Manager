using System;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.PayslipItems.PayslipContributionItems
{
    [ProtoContract]
    [Title(nameof(Strings.PayslipContributionItem))]
    [Guide("Define employer contribution items for payslips.")]
    [Guide("Contributions include items like retirement fund contributions or insurance premiums paid by the employer.")]
    [Fields(typeof(ManagerServer.Model.PayslipContributionItem))]
    internal sealed class PayslipEmployerContributionItemForm : NakedVueForm<PayslipContributionItem>
    {
    }
}
