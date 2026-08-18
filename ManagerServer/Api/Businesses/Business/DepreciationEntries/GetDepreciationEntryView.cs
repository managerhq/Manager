using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.DepreciationEntries
{
    [ProtoContract]
    internal sealed class GetDepreciationEntryView : GetTransactionView<Model.DepreciationEntry>
    {
        protected override TransactionView GetViewData(Model.DepreciationEntry o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.DepreciationEntry;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            viewData.table = BuildTable(o);

            return viewData;
        }
    }
}
