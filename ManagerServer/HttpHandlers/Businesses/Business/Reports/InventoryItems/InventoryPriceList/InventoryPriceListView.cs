using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.InventoryPriceList;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryPriceList
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryPriceList))]
    [Guide("The Inventory Price List view displays sales prices for inventory items.")]
    [Guide("It shows configured pricing for selected inventory items based on filter criteria.")]
    [LinkGuide("For more information see:", typeof(InventoryPriceListForm))]
    internal sealed class InventoryPriceListView : DefaultView<GetInventoryPriceListView>
    {
        protected override Tuple<string, BusinessTemplate> GetFooterAction()
        {
            return new Tuple<string, BusinessTemplate>(Strings.NewSalesQuote, new SalesQuotes.SalesQuoteForm() { Business = Business, Source = Key });
        }        
    }
}