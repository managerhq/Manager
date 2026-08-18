using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.AmortizationEntries
{
    [ProtoContract]
    internal sealed class GetAmortizationEntryView : GetTransactionView<Model.AmortizationEntry>
    {
        protected override TransactionView GetViewData(Model.AmortizationEntry o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.AmortizationEntry;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            viewData.table = BuildTable(o);

            return viewData;
        }
    }
}
