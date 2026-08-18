using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("18048748-7c70-49e6-bed4-b9d310736956")]
    public sealed class CustomerPortal : Object
    {
        [Guide("Select the customer who will have access to the portal.")]
        [ProtoMember(1), Autocomplete(typeof(Customer)), TableColumn] public Guid? Customer { get; set; }
        [Guide("Check this box to allow the customer to view their sales quotes.")]
        [ProtoMember(2)] public bool SalesQuotes { get; set; }
        [Guide("Check this box to allow the customer to view their sales orders.")]
        [ProtoMember(3)] public bool SalesOrders { get; set; }
        [Guide("Check this box to allow the customer to view their sales invoices.")]
        [ProtoMember(4)] public bool SalesInvoices { get; set; }
        [Guide("Check this box to allow the customer to view their credit notes.")]
        [ProtoMember(5)] public bool CreditNotes { get; set; }
        [Guide("Check this box to allow the customer to view their delivery notes.")]
        [ProtoMember(6)] public bool DeliveryNotes { get; set; }
    }
}
