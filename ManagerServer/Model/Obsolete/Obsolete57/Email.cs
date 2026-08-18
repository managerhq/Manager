using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete57
{
    [ProtoContract]
    [Guid("3b1da65d-85f0-4264-b128-b79c21348fee")]
    public sealed class Email : Object
    {
        [ProtoMember(1)]
        public string From;
        [ProtoMember(2)]
        public string[] To;
        [ProtoMember(3)]
        public string Subject;
        [ProtoMember(4)]
        public string Body;
        [ProtoMember(5)]
        public DateTime Date;

        [ProtoMember(6)]
        public DateTime? Obsolete_Viewed;
        [ProtoMember(8)]
        public string Obsolete_Attachment;
    }
}
