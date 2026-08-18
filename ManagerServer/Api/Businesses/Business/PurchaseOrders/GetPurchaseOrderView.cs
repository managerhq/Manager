using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.PurchaseOrders
{
    [ProtoContract]
    internal sealed class GetPurchaseOrderView : GetTransactionView<Model.PurchaseOrder>
    {
        protected override TransactionView GetViewData(Model.PurchaseOrder o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.PurchaseOrder;
            if (o.HasPurchaseOrderCustomTitle && !string.IsNullOrEmpty(o.PurchaseOrderCustomTitle)) viewData.title = o.PurchaseOrderCustomTitle;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.IssueDate, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            var supplier = Database.SingleOrDefault<Model.Supplier>(o.Supplier);

            viewData.table = BuildTable(o, showTaxAmountOnLineItems: o.ShowTaxAmountColumn, showLineNumbers: o.HasLineNumber);

            if (supplier == null) return viewData;

            viewData.recipient.code = supplier.Code;
            viewData.recipient.name = supplier.Name;
            viewData.recipient.address = supplier.Address;
            viewData.recipient.email = supplier.Email;

            viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.Supplier), supplier.CustomFields));
            viewData.custom_fields.AddRange(GetCustomFields2(typeof(Model.Supplier), supplier.CustomFields2));
            return viewData;
        }
    }
}
