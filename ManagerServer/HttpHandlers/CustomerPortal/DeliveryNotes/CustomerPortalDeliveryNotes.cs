using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.CustomerPortal.DeliveryNotes
{
    [ProtoContract]
    class CustomerPortalDeliveryNotes : Table<CustomerPortalDeliveryNotes.DeliveryNote>
    {
        protected override string GetTitle()
        {
            return Strings.DeliveryNotes;
        }

        protected override IEnumerable<DeliveryNote> GetItems()
        {
            var database = ApplicationData.Businesses.Get(Business);
            var customerPortal = database.SingleOrDefault<ManagerServer.Model.CustomerPortal>(CustomerPortal);

            var deliveryNoteKey = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.DeliveryNote));

            return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.DeliveryNote>().Where(x => x.Key != deliveryNoteKey && x.Customer == customerPortal.Customer.Value).OrderByDescending(x => x.DeliveryDate).Select(x => new DeliveryNote()
            {
                View = new CustomerPortalDeliveryNote() { Business = Business, CustomerPortal = CustomerPortal, Key = x.Key },
                Date = x.DeliveryDate,
                Reference = x.Reference,
                Description = x.Description
            });
        }

        public sealed class DeliveryNote : Item
        {
            public DateTime Date;
            public string Reference;
            [Long] public string Description;
        }
    }
}
