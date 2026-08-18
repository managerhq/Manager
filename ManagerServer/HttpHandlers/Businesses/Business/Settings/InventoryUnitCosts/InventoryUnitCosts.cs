using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryUnitCosts
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(InventoryItems))]
    [Guid("b6e2e087-56f4-4548-9768-fd1c6c91c8b6")]
    [Title(nameof(Strings.InventoryUnitCosts))]    
    [Guide("The **Inventory Unit Costs** screen enables you to manage unit costs for your inventory items at specific dates.")]
    [SettingsItemScreenshot("fa-scanner-keyboard", nameof(Strings.InventoryUnitCosts))]
    [Guide("When you sell, write off, or use an inventory item in a production order, Manager will find the unit cost from this screen to match with your inventory transaction.")]
    [Header("Manual Entry")]
    [Guide("To create a new inventory unit cost, click the **New Inventory Unit Cost** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.InventoryUnitCosts), name: nameof(Strings.NewInventoryUnitCost))]
    [Header("Automated Cost Management")]
    [Guide("However, instead of creating inventory unit costs manually, use the **Inventory Cost Correction** screen to automate this task.")]
    [Guide("The **Inventory Cost Correction** screen analyzes all your transactions and suggests which inventory unit costs should be created, updated, or deleted so that your *cost of sales* calculations are accurate.")]
    [Guide("To access the **Inventory Cost Correction** screen, click the **Inventory Cost Correction** button in the bottom-right corner.")]
    [SmallBottomButtonScreenshot(nameof(Strings.InventoryCostCorrection))]
    [LinkGuide("For more information, see:", typeof(InventoryCostCorrection))]
    internal sealed class InventoryUnitCosts : NakedObjectsWithAutomaticRows<InventoryUnitCost>
    {
        [Default]
        [WarnIfFutureDate]
        [Center, MinWidth]
        [WhitespaceNoWrap]
        [Guid("17e30cd9-fa29-4d95-b89e-1ea3f5888e06")]
        public DateTime[] GetDate(ManagerServer.Model.InventoryUnitCost[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("d411d4fa-2ecf-4169-9c31-827ecc636bf2")]
        public NamedObject[] GetInventoryItem(ManagerServer.Model.InventoryUnitCost[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<InventoryItem>(x.InventoryItem)).ToArray();
        }

        [Default]
        [Right, Bold]
        [Guid("98e5b17b-a6b2-4bab-ab09-e3647a1b1d96")]
        public Tuple<decimal, Currency>[] GetUnitCost(ManagerServer.Model.InventoryUnitCost[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => new Tuple<decimal, Currency>(x.UnitCost, baseCurrency)).ToArray();
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: new InventoryCostCorrection() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.InventoryCostCorrection);
            base.OnFooterEndSection(context);
        }
    }
}