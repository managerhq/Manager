using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.Payments
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Payments))]
    [Title(nameof(Strings.Payment))]
    [Guide("Payment footers allow you to add customized text at the bottom of payment forms to document important information about each transaction.")]
    [Guide("You can create multiple footer templates for different payment scenarios, such as bank transfers, checks, or electronic payments.")]
    [Guide("Each footer template can be selected when recording a payment, ensuring the appropriate documentation appears on the payment record.")]
    [Columns]
    internal sealed class PaymentFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.PaymentFooter>
    {
        [Default]
        [Header("Understanding Payment Footers")]
        [Guide("Footers are text sections that appear at the bottom of your payment forms, providing space to document important payment details and authorization information.")]
        [Guide("Common uses for payment footers include:")]
        [Guide("• Payment authorization codes or approval signatures")]
        [Guide("• Bank transfer reference numbers and wire instructions")]
        [Guide("• Remittance advice for suppliers")]
        [Guide("• Internal control notes or approval workflows")]
        [Guide("• Check numbers or electronic payment confirmations")]
        [Header("Creating Footer Templates")]
        [Guide("You can create multiple footer templates and select the appropriate one when recording each payment transaction.")]
        [Guide("This flexibility allows you to maintain proper documentation standards while accommodating different payment methods, approval requirements, or supplier preferences.")]
        [Guide("When creating a footer template, enter a descriptive name that clearly identifies its purpose. Examples include:")]
        [Guide("• *Wire Transfer Instructions* - for payments requiring bank routing details")]
        [Guide("• *Check Payment Authorization* - for payments made by check")]
        [Guide("• *ACH Payment Confirmation* - for electronic transfers")]
        [Guide("• *Petty Cash Reimbursement* - for small cash payments")]
        public string[] GetName(ManagerServer.Model.PaymentFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}
