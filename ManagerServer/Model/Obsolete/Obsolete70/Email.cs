using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete70
{
    [ProtoContract]
    [Guid("238ed44a-1d0f-490b-b17f-abce6ed9f25a")]
    public sealed class Email
    {
        [ProtoMember(1)] public string Sender;
        [ProtoMember(2)] public string Recipient;
        [ProtoMember(3)] public string Subject;
        [ProtoMember(4)] public string Body;
        [ProtoMember(5)] public string Filename;
        [ProtoMember(6)] public byte[] Blob;
        [ProtoMember(7)] public Attachment[] Attachments;

        [ProtoContract]
        public sealed class Attachment
        {
            [ProtoMember(1)] public Guid Key;
            [ProtoMember(2)] public string Filename;
        }
    }
}
