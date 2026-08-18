using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete36
{
    [ProtoContract]
    [Guid("8e9ee26c-e685-4aaf-8a11-da5068e09a25")]
    internal sealed class SalesInvoiceDefaultNotes36 : Object
    {
        [ProtoMember(1)]
        public string Value;
    }
}
