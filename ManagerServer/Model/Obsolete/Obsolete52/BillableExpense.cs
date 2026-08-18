using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete52
{
    [ProtoContract]
    [Guid("5859f372-7469-4cff-b7b1-3155fdf1ca2e")]
    public sealed class BillableExpense : Object
    {
        [ProtoMember(1)]
        public DisbursementStatus Status;
        [ProtoMember(2)]
        public Guid? SalesInvoice;
        [ProtoMember(3)]
        public DateTime? WriteOffDate;
    }
}
