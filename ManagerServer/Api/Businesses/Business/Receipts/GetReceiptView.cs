using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.Receipts
{
    [ProtoContract]
    internal sealed class GetReceiptView : GetTransactionView<Model.Receipt>
    {
        protected override TransactionView GetViewData(Model.Receipt o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.Receipt;
            if (o.HasReceiptCustomTitle && !string.IsNullOrWhiteSpace(o.ReceiptCustomTitle)) viewData.title = o.ReceiptCustomTitle;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            if (o.PaidBy == Model.Enums.PayerPayeeType.Customer)
            {
                var customer = Database.SingleOrDefault<Model.Customer>(o.Customer);
                if (customer != null)
                {
                    viewData.recipient.code = customer.Code;
                    viewData.recipient.name = customer.Name;
                    viewData.recipient.address = customer.BillingAddress;
                    viewData.recipient.email = customer.Email;
                    viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.Customer), customer.CustomFields));
                    viewData.custom_fields.AddRange(GetCustomFields2(typeof(Model.Customer), customer.CustomFields2));
                }
            }
            if (o.PaidBy == Model.Enums.PayerPayeeType.Supplier)
            {
                var supplier = Database.SingleOrDefault<Model.Supplier>(o.Supplier);
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
            if (o.PaidBy == Model.Enums.PayerPayeeType.Other)
            {
                viewData.recipient.name = o.Contact;
            }

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            viewData.table = BuildTable(o, showTaxAmountOnLineItems: o.ShowTaxAmountColumn, showLineNumbers: o.HasLineNumber);

            if (o.ReceivedIn.HasValue)
            {
                var bankAccount = Database.SingleOrDefault<Model.BankOrCashAccount>(o.ReceivedIn.Value);
                if (bankAccount != null) viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.BankOrCashAccount), bankAccount.CustomFields));
            }

            return viewData;
        }
    }
}
