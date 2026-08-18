using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.PurchaseInvoices
{
    [ProtoContract]
    internal sealed class GetPurchaseInvoiceView : GetTransactionView<Model.PurchaseInvoice>
    {
        protected override TransactionView GetViewData(Model.PurchaseInvoice o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.PurchaseInvoice;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.InvoiceDate, text = o.IssueDate.ToLocalShortDisplayString() });
            viewData.fields.Add(new TransactionView.Field { label = Strings.DueDate, text = o.GetDueDate().ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.InvoiceNumber, text = o.Reference });

            var purchaseOrder = Database.SingleOrDefault<Model.PurchaseOrder>(o.PurchaseOrder);

            if (!string.IsNullOrWhiteSpace(purchaseOrder?.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.OrderNumber, text = purchaseOrder.Reference });
            else if (!string.IsNullOrWhiteSpace(o.OrderNumber)) viewData.fields.Add(new TransactionView.Field { label = Strings.OrderNumber, text = o.OrderNumber });

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
                }
            }

            viewData.table = BuildTable(o, showTaxAmountOnLineItems: o.ShowTaxAmountColumn, showLineNumbers: o.HasLineNumber);

            if (!o.HideBalanceDue)
            {
                var purchaseInvoiceTransactions = new Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchPurchaseInvoices(o.Supplier.HasValue ? new[] { o.Supplier.Value } : null).Where(x => x.GeneralLedgerAccount.IsAccountsPayable && x.Supplier?.Key == o.Supplier && x.PurchaseInvoice?.Key == Key).ToArray();
                foreach (var e in purchaseInvoiceTransactions.OrderBy(x => x.Date))
                {
                    if (e.PurchaseInvoiceAsTransaction != null) continue;
                    var transactionAmount = string.Empty;
                    if (e.TransactionCurrency != e.AccountCurrency) transactionAmount = (e.TransactionAmount < 0m ? e.TransactionAmount * -1m : e.TransactionAmount).ToCurrencyString(e.TransactionCurrency, CurrencySymbol.Short);
                    var label = string.Join(" — ", new[] { e.Transaction.GetTransactionName(), (e.OriginalDate ?? e.Date).ToLocalShortDisplayString(), transactionAmount }.Where(x => !string.IsNullOrWhiteSpace(x)));
                    viewData.table.totals.Add(new TransactionView.Total { label = label, text = (e.AccountAmount * -1m).ToCurrencyString(e.AccountCurrency, CurrencySymbol.Short) });
                }
                if (purchaseInvoiceTransactions.Any(x => x.PurchaseInvoiceAsTransaction == null))
                {
                    var purchaseInvoiceAmountDue = purchaseInvoiceTransactions.Sum(x => x.AccountAmount) * -1m;
                    viewData.table.totals.Add(new TransactionView.Total { label = Strings.BalanceDue, emphasis = true, text = purchaseInvoiceAmountDue.ToCurrencyString(purchaseInvoiceTransactions.First().AccountCurrency, CurrencySymbol.Short) });
                }
            }

            return viewData;
        }
    }
}
