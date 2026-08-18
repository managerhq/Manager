using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.SalesOrders
{
    [ProtoContract]
    internal sealed class GetSalesOrderView : GetTransactionView<Model.SalesOrder>
    {
        protected override TransactionView GetViewData(Model.SalesOrder o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.SalesOrder;
            if (o.HasSalesOrderCustomTitle && !string.IsNullOrEmpty(o.SalesOrderCustomTitle)) viewData.title = o.SalesOrderCustomTitle;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.IssueDate, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            var customer = Database.SingleOrDefault<Model.Customer>(o.Customer);

            if (customer != null)
            {
                var salesQuote = Database.SingleOrDefault<Model.SalesQuote>(o.SalesQuote);
                if (salesQuote != null)
                {
                    viewData.fields.Add(new TransactionView.Field { label = Strings.QuoteNumber, text = salesQuote.GetName() });
                }
            }

            viewData.table = BuildTable(o, showTaxAmountOnLineItems: o.ShowTaxAmountColumn, showLineNumbers: o.HasLineNumber, forceTotals: true, showItemImages: o.ShowItemImages);

            if (customer == null) return viewData;

            viewData.recipient.code = customer.Code;
            viewData.recipient.name = customer.Name;
            viewData.recipient.address = o.BillingAddress;
            if (string.IsNullOrWhiteSpace(viewData.recipient.address)) viewData.recipient.address = customer.BillingAddress;
            viewData.recipient.email = customer.Email;
            viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.Customer), customer.CustomFields));
            viewData.custom_fields.AddRange(GetCustomFields2(typeof(Model.Customer), customer.CustomFields2));
            return viewData;
        }
    }
}
