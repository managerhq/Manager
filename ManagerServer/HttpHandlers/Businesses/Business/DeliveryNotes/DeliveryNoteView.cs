using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.DeliveryNotes
{
    [ProtoContract]
    [Title(nameof(Strings.DeliveryNote))]
    [Guide("The *delivery note* view displays comprehensive details about a specific delivery note, including recipient information, delivery date, and itemized products or services being delivered.")]
    [Guide("A delivery note serves as proof of delivery and typically accompanies goods when they are shipped to a customer. It helps track what was sent and when, without including pricing information.")]
    [Header("Available Actions")]
    [Guide("From this view, you can perform several actions:")]
    [Guide("• **Edit** - Modify the delivery note details by clicking the **Edit** button")]
    [Guide("• **Print** - Generate a printed copy of the delivery note for physical records or to include with shipments")]
    [Guide("• **Email** - Send the delivery note directly to your customer via email")]
    [Guide("• **Copy to** - Convert this delivery note into other transaction types such as a *sales invoice*")]
    [Header("Key Information Displayed")]
    [Guide("The delivery note view shows essential information including the *delivery date*, *reference number*, associated *order number* and *invoice number* (if applicable), and the *inventory location* from which items are being delivered.")]
    [Guide("Customer details such as name, delivery address, and contact information are prominently displayed, along with any *custom fields* you have configured for customers.")]
    [LinkGuide("To learn how to create or edit delivery notes, see:", typeof(DeliveryNoteForm))]
    internal sealed class DeliveryNoteView : TransactionView<ManagerServer.Model.DeliveryNote>
    {
        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Transaction)];
        }

        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForDeliveryNote>();
        }
    }
}