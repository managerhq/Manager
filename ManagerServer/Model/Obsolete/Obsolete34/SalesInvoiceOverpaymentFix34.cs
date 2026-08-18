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
    [Guid("a94200fd-d1f6-4a09-ab71-707f362d4a15")]
    internal sealed class SalesInvoiceOverpaymentFix34 : Object
    {
        [ProtoMember(1)]
        public bool Fixed;
    }
}
