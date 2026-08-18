using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete37
{
    [ProtoContract]
    [Guid("d2400a96-6537-4b04-a349-4d4513e836b6")]
    internal sealed class SalesInvoiceDefaultDueDate37 : Object
    {
        [ProtoMember(21)]
        public DueDateType2 DueDateType;
        [ProtoMember(22)]
        public int? DueDateDays;
    }
}
