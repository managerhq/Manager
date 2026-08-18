using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.CustomerPortal.Orders
{
    [ProtoContract]
    class CustomerPortalOrders : Table<CustomerPortalOrders.Order>
    {
        protected override string GetTitle()
        {
            return Strings.Orders;
        }

        protected override IEnumerable<Order> GetItems()
        {
            var database = ApplicationData.Businesses.Get(Business);
            var customerPortal = database.SingleOrDefault<ManagerServer.Model.CustomerPortal>(CustomerPortal);

            var salesOrderKey = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SalesOrder));

            return database.OfType<ManagerServer.Model.SalesOrder>().Where(x => x.Key != salesOrderKey && x.Customer == customerPortal.Customer.Value).OrderByDescending(x => x.Date).Select(x => new Order()
            {
                View = new CustomerPortalOrder() { Business = Business, CustomerPortal = CustomerPortal, Key = x.Key },
                Date = x.Date,
                Reference = x.Reference,
                Description = x.Description,
                Total = x.GetGeneralLedgerTransactions(database).Single(x => x.IsBalancing).AccountAmount,
            });
        }

        public sealed class Order : Item
        {
            public DateTime Date;
            public string Reference;
            [Long] public string Description;
            public decimal Total;
        }
    }
}
