using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("2e541a82-94d7-42fc-a388-26bdc0803455")]
    public sealed class Attachment : Object, IComparable<Attachment>
    {
        [Guide("The date when this attachment was uploaded or created.")]
        [Guide("This date helps track when documents were added to the system for audit purposes.")]
        [ProtoMember(1)] public DateTime Date { get; set; }
        [Guide("The filename of the attached document. This helps identify the attachment content.")]
        [Guide("Attachments can include supporting documents like receipts, contracts, or correspondence.")]
        [ProtoMember(2)] public string Name { get; set; }
        [Guide("The file size in bytes of the attachment.")]
        [Guide("Large attachments may affect system performance and backup sizes.")]
        [ProtoMember(4), Hidden] public int Size { get; set; }
        [Guide("The record this attachment is linked to.")]
        [Guide("Attachments are associated with transactions or records to provide supporting documentation.")]
        [ProtoMember(6), Hidden] public Guid? Object { get; set; }
        [ProtoMember(12), Hidden] public byte[] Sha256 { get; set; }

        [ProtoMember(3)] public string Obsolete_ContentType { get; set; }
        [ProtoMember(5)] public string Obsolete_Url { get; set; }
        [ProtoMember(7)] public byte[] Obsolete_AesKey { get; set; }
        [ProtoMember(8)] public byte[] Obsolete_AesIV { get; set; }
        [ProtoMember(9)] public Guid Obsolete_Account { get; set; }
        [ProtoMember(10)] public Guid Obsolete_File { get; set; }
        [ProtoMember(11)] public bool Obsolete_IsLocal { get; set; }

        int IComparable<Attachment>.CompareTo(Attachment other)
        {
            return (Object ?? Guid.Empty).CompareTo(other.Object ?? Guid.Empty);
        }
    }
}