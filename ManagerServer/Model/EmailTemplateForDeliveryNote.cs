using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("4768f2b1-4388-4cc3-ac50-c00284434dd6")]
    public sealed class EmailTemplateForDeliveryNote : Object, IEmailTemplate
    {
        [Guide("Default email subject line when emailing delivery notes.")]
        [ProtoMember(1), Long] public string Subject { get; set; }
        
        [Guide("Default email body text when emailing delivery notes. You can include merge fields to personalize the message.")]
        [ProtoMember(2), Long, Textarea] public string MessageBody { get; set; }

        public string GetBody() => MessageBody;
        public string GetSubject() => Subject;
    }
}
