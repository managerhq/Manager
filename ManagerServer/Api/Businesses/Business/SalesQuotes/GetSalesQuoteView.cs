using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Collections.Generic;

namespace ManagerServer.Api.Businesses.Business.SalesQuotes
{
    [ProtoContract]
    internal sealed class GetSalesQuoteView : GetTransactionView<Model.SalesQuote>
    {
        protected override TransactionView GetViewData(Model.SalesQuote o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.Quote;
            if (o.HasSalesQuoteCustomTitle && !string.IsNullOrEmpty(o.SalesQuoteCustomTitle)) viewData.title = o.SalesQuoteCustomTitle;

            viewData.reference = o.Reference;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.IssueDate, text = o.IssueDate.ToLocalShortDisplayString() });
            if (o.GetExpiryDate().HasValue) viewData.fields.Add(new TransactionView.Field { label = Strings.ExpiryDate, text = o.GetExpiryDate().Value.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            var customer = Database.SingleOrDefault<Model.Customer>(o.Customer);

            var showTaxCodeOnLineItems = true;
            if (!o.AmountsIncludeTax && !o.ShowTaxAmountColumn && o.HideTotalAmount) showTaxCodeOnLineItems = false;

            viewData.table = BuildTable(o, showTaxAmountOnLineItems: o.ShowTaxAmountColumn, showTaxCodeOnLineItems: showTaxCodeOnLineItems, showLineNumbers: o.HasLineNumber, forceTotals: true, showItemImages: o.ShowItemImages);
            if (o.HideTotalAmount)
            {
                viewData.table.totals = new List<TransactionView.Total>();
                foreach (var e in viewData.table.columns) e.total = false;
            }

            viewData.recipient.address = o.BillingAddress;

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
