using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using HttpFramework;
using ManagerServer.Model;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payments
{
    [ProtoContract]
    [Title(nameof(Strings.Payment))]
    [Guide("The `Payment` view displays complete details of a payment transaction. This includes the payee information, payment date, reference number, and all line items associated with the transaction.")]
    [Guide("This is a read-only view of the payment that provides a comprehensive summary of the transaction. You can use this view to review payment details, print documentation, or share payment information with others.")]
    [Header("Available Actions")]
    [Guide("From the payment view, you can perform several actions using the buttons at the bottom of the screen:")]
    [Guide("• `Edit` - Modify the payment details, including payee, amounts, or line items")]
    [Guide("• `Print` - Generate a PDF version of the payment for your records or to provide to the payee")]
    [Guide("• `Email` - Send the payment details directly to the payee's email address")]
    [Guide("• `Copy to` - Duplicate this payment as a new transaction, which is useful for recurring payments or similar transactions")]
    [Header("Related Topics")]
    [LinkGuide("To learn how to create and edit payments, see:", typeof(PaymentForm))]
    internal sealed class PaymentView : TransactionView<ManagerServer.Model.Payment>
    {
        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForPayment>();
        }

        protected override string GetRecipient()
        {
            var business = ApplicationData.Businesses.Get(Business);
            return business.SingleOrDefault<Supplier>(business.SingleOrDefault<Payment>(Key)?.Supplier)?.Email;
        }

        protected override Type[] GetCopyToOptions()
        {
            return [ typeof(ManagerServer.Model.Payment), typeof(ManagerServer.Model.Receipt), typeof(ManagerServer.Model.RecurringPayment) ];
        }

        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new PaymentTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}
