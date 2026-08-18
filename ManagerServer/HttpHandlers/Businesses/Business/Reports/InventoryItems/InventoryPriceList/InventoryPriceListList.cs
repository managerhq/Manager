using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryPriceList
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryPriceList))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`InventoryPriceList` provides a comprehensive overview of the current prices for all items in your inventory, helping you manage and update pricing efficiently.")]
    [Guide("To create a new `InventoryPriceList`, go to `Reports` tab, click `InventoryPriceList`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.InventoryPriceList), name: nameof(Strings.NewReport))]
    internal sealed class InventoryPriceListList : PersistentObjectTable<ManagerServer.Model.InventoryPriceList>
    {
        [Guid("729b5397-6e9d-4872-a765-6907680e1315")]
        public string GetName(ManagerServer.Model.InventoryPriceList o) => o.Name;

        [Guid("0e3281af-8dfa-4ecf-915f-dc13a41a0c78")]
        public string GetFilter(ManagerServer.Model.InventoryPriceList o) => (o.FilterByCustomField && o.CustomField.HasValue ? o.Filter : null);
    }
}