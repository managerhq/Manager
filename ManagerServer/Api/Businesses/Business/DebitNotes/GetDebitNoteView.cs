using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.DebitNotes
{
    [ProtoContract]
    internal sealed class GetDebitNoteView : GetTransactionView<Model.DebitNote>
    {
        protected override TransactionView GetViewData(Model.DebitNote o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.DebitNote;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.IssueDate, text = o.IssueDate.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            var purchaseInvoice = Database.SingleOrDefault<Model.PurchaseInvoice>(o.PurchaseInvoice);
            if (purchaseInvoice != null)
            {
                viewData.fields.Add(new TransactionView.Field { label = Strings.Invoice, text = purchaseInvoice.Reference });
            }

            if (o.Supplier.HasValue)
            {
                var supplier = Database.SingleOrDefault<Model.Supplier>(o.Supplier.Value);
                if (supplier != null)
                {
                    viewData.recipient.code = supplier.Code;
                    viewData.recipient.name = supplier.Name;
                    viewData.recipient.address = supplier.Address;
                    viewData.recipient.email = supplier.Email;

                    viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.Supplier), supplier.CustomFields));
                    viewData.custom_fields.AddRange(GetCustomFields2(typeof(Model.Supplier), supplier.CustomFields2));
                }
            }

            viewData.table = BuildTable(o, showTaxAmountOnLineItems: o.ShowTaxAmountColumn, showLineNumbers: o.HasLineNumber);

            return viewData;
        }
    }
}
