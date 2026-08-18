using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.WithholdingTaxReceipts
{
    [ProtoContract]
    internal sealed class GetWithholdingTaxReceiptView : GetTransactionView<Model.WithholdingTaxReceipt>
    {
        protected override TransactionView GetViewData(Model.WithholdingTaxReceipt o)
        {
            var customer = Database.SingleOrDefault<Model.Customer>(o.Customer);
            var currencies = Query.Currencies.GetCurrencyProvider(Business);

            var viewData = new TransactionView();
            viewData.title = Strings.WithholdingTaxReceipt;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });

            if (customer != null)
            {
                viewData.table.columns.Add(new TransactionView.Column { label = Strings.Description });
                viewData.table.columns.Add(new TransactionView.Column { label = Strings.Amount, align = "right", nowrap = true });

                var row = new TransactionView.Row();
                row.cells.Add(new TransactionView.Cell { text = o.Description });
                row.cells.Add(new TransactionView.Cell { value = o.Amount, text = o.Amount.ToCurrencyString(currencies.Get(customer.Currency), CurrencySymbol.None) });
                viewData.table.rows.Add(row);

                viewData.recipient.code = customer.Code;
                viewData.recipient.name = customer.Name;
                viewData.recipient.address = customer.BillingAddress;
                viewData.recipient.email = customer.Email;

                viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.Customer), customer.CustomFields));
                viewData.custom_fields.AddRange(GetCustomFields2(typeof(Model.Customer), customer.CustomFields2));
            }
            return viewData;
        }
    }
}
