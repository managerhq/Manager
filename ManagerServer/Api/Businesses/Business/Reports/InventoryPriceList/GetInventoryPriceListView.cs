using ManagerServer.Globalization;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryPriceList;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryPriceList
{
    [ProtoContract]
    internal sealed class GetInventoryPriceListView : GetReportView<Model.InventoryPriceList>
    {
        protected override string DefaultTitle => Strings.InventoryPriceList;

        protected override ReportModel Build(Database business, Model.InventoryPriceList report)
        {
            var model = new ReportModel();
            model.Subtitle2 = report.Name;
            model.Columns.Add(new Column { Name = Strings.SalePrice, IsBold = true });

            foreach (var e in GetInventoryItems(Business, report))
            {
                model.Rows.Items.Add(new Row
                {
                    Name = e.NameWithCode,
                    Cells = [ReportNumberFormat.Cell(e.DefaultSalesUnitPrice, NumberStyle.Currency, model.WholeNumbers)],
                });
            }

            return model;
        }

        public static ManagerServer.Model.InventoryItem[] GetInventoryItems(string fileId, ManagerServer.Model.InventoryPriceList report)
        {
            var list = new List<ManagerServer.Model.InventoryItem>();

            var inventoryItems = ApplicationData.Instance.Businesses.Get(fileId).OfType<ManagerServer.Model.InventoryItem>().ToArray();
            foreach (var e in inventoryItems.OrderBy(x => x.NameWithCode))
            {
                if (e.DefaultSalesUnitPrice == 0m) continue;
                if (report.FilterByCustomField)
                {
                    if (!report.CustomField.HasValue) continue;
                    if (e.CustomFields == null) continue;
                    if (!e.CustomFields.ContainsKey(report.CustomField.Value)) continue;
                    if (e.CustomFields[report.CustomField.Value] != report.Filter) continue;
                }
                list.Add(e);
            }

            return list.ToArray();
        }
    }
}
