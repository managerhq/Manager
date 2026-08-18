using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.ProductionOrders
{
    [ProtoContract]
    [Title(nameof(Strings.ProductionOrder))]
    [Guide("The *Production Order* view displays detailed information about a specific production order, including the date, reference number, and description.")]
    [Guide("This view shows a breakdown of all raw materials consumed during the production process, with quantities and costs for each item.")]
    [Guide("The total cost calculation includes both inventory items consumed and any additional production costs, such as labor or overhead expenses.")]
    [Guide("At the bottom of the view, you can see the quantity and details of the finished goods produced from this production order.")]
    [LinkGuide("To learn how to create and edit production orders, see:", typeof(ProductionOrderForm))]
    internal sealed class ProductionOrderView : TransactionView<ManagerServer.Model.ProductionOrder>
    {
        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new ProductionOrderTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}