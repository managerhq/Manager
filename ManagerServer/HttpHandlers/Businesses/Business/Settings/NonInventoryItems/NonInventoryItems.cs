using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.NonInventoryItems
{
    [ProtoContract]
    [NamespaceEntry]    
    [Guid("c4711963-0e6b-4ef6-9c10-c10b645b57fc")]
    [Title(nameof(Strings.NonInventoryItems))]
    [Guide("Non-inventory items are products or services that you buy and sell but don't need to track quantities for. They automatically populate line items on your invoices, orders, and quotes, saving you time on data entry.")]
    [Guide("Unlike *inventory items*, non-inventory items are not monitored for quantity on hand or inventory value. This makes them perfect for services, labor charges, or products where you don't need inventory control.")]
    [Guide("Common uses include professional services, consultation fees, shipping charges, or any frequently used line items that don't require quantity tracking.")]
    [Guide("To access non-inventory items, go to the **Settings** tab and click **Non-Inventory Items**.")]
    [SettingsItemScreenshot("fa-th", nameof(Strings.NonInventoryItems))]
    internal sealed class NonInventoryItems : NakedObjectsWithAutomaticRows<ManagerServer.Model.NonInventoryItem>
    {
        [Guid("bd6ff388-7817-482f-8e71-d63144084f81")]
        public string[] GetItemCode(ManagerServer.Model.NonInventoryItem[] rows)
        {
            return rows.Select(x => x.Code).ToArray();
        }

        [Default]
        [Guid("3b58485d-43e0-4973-824b-54ae9b54fd86")]
        public string[] GetItemName(ManagerServer.Model.NonInventoryItem[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Guid("e716afbe-f5e3-49d6-909e-c0f3db4b6874")]
        public string[] GetDescription(ManagerServer.Model.NonInventoryItem[] rows)
        {
            return rows.Select(x => x.HasDefaultLineDescription ? x.DefaultLineDescription : null).ToArray();
        }

        [Guid("52e38221-2e16-4554-abe7-de48c1affaef")]
        public string[] GetUnitName(ManagerServer.Model.NonInventoryItem[] rows)
        {
            return rows.Select(x => x.UnitName).ToArray();
        }

        [Default]
        [Right]
        [Guid("6a4c387c-28fa-4824-b4c1-18367df6c51b")]
        public Tuple<decimal, Currency>[] GetPurchasePrice(ManagerServer.Model.NonInventoryItem[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => x.HasDefaultPurchaseUnitPrice ? new Tuple<decimal, Currency>(x.DefaultPurchaseUnitPrice, baseCurrency) : null).ToArray();
        }

        [Default]
        [Right]
        [Guid("379329fc-8dfc-4bbe-b638-56dcd52c3994")]
        public Tuple<decimal, Currency>[] GetSalePrice(ManagerServer.Model.NonInventoryItem[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => x.HasDefaultSalesUnitPrice ? new Tuple<decimal, Currency>(x.DefaultSalesUnitPrice, baseCurrency) : null).ToArray();
        }
    }
}
