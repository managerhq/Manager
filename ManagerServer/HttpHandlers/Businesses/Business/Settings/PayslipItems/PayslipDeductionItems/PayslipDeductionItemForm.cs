using System;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.PayslipItems.PayslipDeductionItems
{
    [ProtoContract]
    [Title(nameof(Strings.PayslipDeductionItem))]
    [Guide("Define deduction items for employee payslips.")]
    [Guide("Deductions include items like taxes, insurance premiums, or loan repayments.")]
    [Fields(typeof(ManagerServer.Model.PayslipDeductionItem))]
    internal sealed class PayslipDeductionItemForm : NakedVueForm<PayslipDeductionItem>
    {
    }
}