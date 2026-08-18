using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete47
{
    [ProtoContract]
    [Guid("d7ea8299-1479-43ac-a237-a3a513a3986b")]
    internal sealed class InBuiltTaxCode47 : Object
    {
        [ProtoMember(1)]
        public Guid? Account;
        [ProtoMember(2)]
        public bool Archived;
    }
}
