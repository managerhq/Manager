using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete34
{
    [ProtoContract]
    [Guid("a0ef856b-8e18-43a3-91af-9b085abe5574")]
    internal sealed class PurchaseInvoiceOverpaymentFix34 : Object
    {
        [ProtoMember(1)]
        public bool Fixed;
    }
}
