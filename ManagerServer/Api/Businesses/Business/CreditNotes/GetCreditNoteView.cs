using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.CreditNotes
{
    [ProtoContract]
    internal sealed class GetCreditNoteView : GetTransactionView<Model.CreditNote>
    {
        protected override TransactionView GetViewData(Model.CreditNote o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.CreditNote;
            if (o.HasCreditNoteCustomTitle && !string.IsNullOrWhiteSpace(o.CreditNoteCustomTitle)) viewData.title = o.CreditNoteCustomTitle;
            viewData.reference = o.Reference;
            if (o.Type == Model.Enums.CreditNoteType.EarlyPaymentDiscount) viewData.description = Strings.EarlyPaymentDiscount;
            else viewData.description = o.Description;

            var customCreditNoteTitleTaxCodes = Database.OfType<Model.TaxCode>().Where(x => x.CustomCreditNoteTitle && !string.IsNullOrWhiteSpace(x.CreditNoteTitle)).ToArray();
            var customCreditNoteTitleTaxCodeKeys = new HashSet<Guid>(customCreditNoteTitleTaxCodes.Select(x => x.Key));
            if (o.Lines != null)
            {
                foreach (var e in o.Lines.Where(x => x.TaxCode.HasValue && customCreditNoteTitleTaxCodeKeys.Contains(x.TaxCode.Value)))
                {
                    viewData.title = customCreditNoteTitleTaxCodes.First(x => x.Key == e.TaxCode.Value).CreditNoteTitle;
                    break;
                }
            }

            viewData.fields.Add(new TransactionView.Field { label = Strings.IssueDate, text = o.IssueDate.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });
            if (o.SalesInvoice.HasValue)
            {
                var invoice = Database.SingleOrDefault<Model.SalesInvoice>(o.SalesInvoice.Value);
                if (invoice != null) viewData.fields.Add(new TransactionView.Field { label = Strings.Invoice, text = invoice.Reference });
            }

            if (o.Customer.HasValue)
            {
                var customer = Database.SingleOrDefault<Model.Customer>(o.Customer.Value);
                if (customer != null)
                {
                    viewData.recipient.code = customer.Code;
                    viewData.recipient.name = customer.Name;
                    viewData.recipient.address = o.BillingAddress;
                    if (string.IsNullOrWhiteSpace(viewData.recipient.address)) viewData.recipient.address = customer.BillingAddress;
                    viewData.recipient.email = customer.Email;

                    viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.Customer), customer.CustomFields));
                    viewData.custom_fields.AddRange(GetCustomFields2(typeof(Model.Customer), customer.CustomFields2));
                }
            }

            viewData.table = BuildTable(o, showTaxAmountOnLineItems: o.ShowTaxAmountColumn, showLineNumbers: o.HasLineNumber);

            return viewData;
        }
    }
}
