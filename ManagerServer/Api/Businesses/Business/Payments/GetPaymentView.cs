using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.Payments
{
    [ProtoContract]
    internal sealed class GetPaymentView : GetTransactionView<Model.Payment>
    {
        protected override TransactionView GetViewData(Model.Payment o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.Payment;
            if (o.HasPaymentCustomTitle && !string.IsNullOrWhiteSpace(o.PaymentCustomTitle)) viewData.title = o.PaymentCustomTitle;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            if (o.Payee == Model.Enums.PayerPayeeType.Customer)
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
            if (o.Payee == Model.Enums.PayerPayeeType.Supplier)
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
            if (o.Payee == Model.Enums.PayerPayeeType.Other)
            {
                viewData.recipient.name = o.Contact;
            }

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            viewData.table = BuildTable(o, showTaxAmountOnLineItems: o.ShowTaxAmountColumn, showLineNumbers: o.HasLineNumber);

            if (o.PaidFrom.HasValue)
            {
                var bankAccount = Database.SingleOrDefault<Model.BankOrCashAccount>(o.PaidFrom.Value);
                if (bankAccount != null) viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.BankOrCashAccount), bankAccount.CustomFields));
            }

            return viewData;
        }
    }
}
