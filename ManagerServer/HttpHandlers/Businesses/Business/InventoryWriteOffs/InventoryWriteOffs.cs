using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryWriteOffs
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.InventoryWriteOffs))]
    [Guid("1c5f6985-d0ff-46ed-908f-a94feadd79c4")]
    [Guide("The **Inventory Write-offs** tab helps you record and track inventory losses. Use this feature when inventory items are damaged, lost, stolen, or otherwise removed from stock outside of normal sales transactions.")]
    [Guide("Inventory write-offs maintain accurate *inventory records* by properly accounting for items that can no longer be sold or used.")]
    [TabScreenshot("fa-eraser", nameof(Strings.InventoryWriteOffs))]
    [Header("Creating Write-offs")]
    [Guide("To create a new inventory write-off, click the **New Inventory Write-off** button.")]
    [HeroButtonScreenshot(nameof(Strings.InventoryWriteOffs), nameof(Strings.NewInventoryWriteOff))]
    [Header("Understanding the List")]
    [Guide("The **Inventory Write-offs** tab displays all recorded write-offs with the following information:")]
    [Columns]
    internal sealed class InventoryWriteOffs : NakedObjectsWithAutomaticRows<ManagerServer.Model.InventoryWriteOff>
    {
        [Default]
        [WarnIfFutureDate]
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("aeb956fc-259b-4b25-b010-1ca2716672be")]
        [Guide("The date when the inventory write-off occurred or was recorded in the system.")]
        public DateTime[] GetDate(InventoryWriteOff[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [PaddedSorting]
        [WarnIfNotUnique]
        [Guid("0cbefd4c-650a-4c53-98b4-7bc0715b93c0")]
        [Guide("A unique reference number that identifies this specific inventory write-off transaction.")]
        public string[] GetReference(InventoryWriteOff[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Guid("ae2c99bf-47f7-4464-a7d0-e889fc8e2f1d")]
        [Guide("The *inventory location* where the written-off items were stored. This helps track losses by location.")]
        public string[] GetInventoryLocation(InventoryWriteOff[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<CustomInventoryLocation>(x.InventoryLocation)?.Name).ToArray();
        }

        [Default]
        [Guid("15680314-2e2a-4bc7-86b7-9ed64358e7fc")]
        [Guide("A brief description explaining the reason for the inventory write-off, such as damage details or circumstances of loss.")]
        public string[] GetDescription(InventoryWriteOff[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Default]
        [Right, Sum, Bold]
        [Guid("1c8db852-82e6-4a68-8131-f54a449d4e3b")]
        [Guide("The total *cost value* of all inventory items included in this write-off. This amount represents the financial loss to your business.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetTotalCost(InventoryWriteOff[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => x.CostOfSales(database).HasValue ? new Tuple<decimal, Currency, BusinessTemplate>(x.CostOfSales(database).Value, baseCurrency, new InventoryWriteOffCosts() { Business = Business, Transaction = x.Key, ReverseSign = true, Referrer = referrer }) : null).ToArray();
        }
    }
}
