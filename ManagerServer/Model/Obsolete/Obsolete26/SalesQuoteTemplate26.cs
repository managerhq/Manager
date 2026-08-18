using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete26
{
    [ProtoContract]
    [Guid("2903bbf5-6c43-4fbf-9eef-9b239b784f87")]
    internal sealed class SalesQuoteTemplate26 : Object
    {
        [ProtoMember(1)]
        public string Title;
        [ProtoMember(4)]
        public string ReferenceNumberPrefix;

        [ProtoMember(2)]
        public string Obsolete_Notes;
        [ProtoMember(3)]
        public bool Obsolete_AmountsIncludeTax;
    }
}
