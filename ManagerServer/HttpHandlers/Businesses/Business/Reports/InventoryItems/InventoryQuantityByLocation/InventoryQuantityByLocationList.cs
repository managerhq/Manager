using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryQuantityByLocation
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryQuantityByLocation))]
    [Guide("`InventoryQuantityByLocation` provides a detailed overview of your inventory levels across multiple inventory locations, enabling efficient tracking and management of stock distribution.")]
    [Guide("To create a new `InventoryQuantityByLocation`, go to `Reports` tab, click `InventoryQuantityByLocation`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.InventoryQuantityByLocation), name: nameof(Strings.NewReport))]
    internal sealed class InventoryQuantityByLocationList : NakedObjectsWithAutomaticRows<ManagerServer.Model.InventoryQuantityByLocation>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewReport);
        }

        [Default, MinWidth, Center, WhitespaceNoWrap]
        [Guid("07c5f2cf-6419-44ce-81a7-ffed60bce6de")]
        public DateTime[] GetDate(ManagerServer.Model.InventoryQuantityByLocation[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("f3ebf927-824d-4229-87ff-fada78e63b40")]
        public string[] GetDescription(ManagerServer.Model.InventoryQuantityByLocation[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }
    }
}