using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business.Receipts
{
    [ProtoContract]
    [Title(nameof(Strings.Receipt))]
    [Guide("The Receipt view displays the details of a receipt transaction that has been recorded in the system.")]
    [Guide("A receipt represents money received from customers, suppliers, or other parties. This view shows all the important details including the payer, amount, date, and line items.")]
    [Guide("From this view, you can perform several actions:")]
    [Guide("• `Print` - Generate a PDF version of the receipt for printing or saving")]
    [Guide("• `Email` - Send the receipt directly to the payer via email")]
    [Guide("• `Edit` - Make changes to the receipt details")]
    [Guide("• `Copy to` - Create a new transaction based on this receipt")]
    [LinkGuide("To learn about creating and editing receipts, see:", typeof(ReceiptForm))]
    internal sealed class ReceiptView : TransactionView<ManagerServer.Model.Receipt>
    {
        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForReceipt>();
        }

        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Receipt), typeof(ManagerServer.Model.Payment), typeof(ManagerServer.Model.RecurringReceipt)];
        }

        protected override string GetRecipient()
        {
            var business = ApplicationData.Businesses.Get(Business);
            return business.SingleOrDefault<Customer>(business.SingleOrDefault<Receipt>(Key)?.Customer)?.Email;
        }

        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new ReceiptTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}
