using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("ff22d093-a191-47eb-acad-4ed0b439dc43")]
    public sealed class EmailTemplateForPurchaseQuote : Object, IEmailTemplate
    {
        [Guide("Default email subject line when emailing purchase quotes.")]
        [ProtoMember(1), Long] public string Subject { get; set; }
        
        [Guide("Default email body text when emailing purchase quotes. You can include merge fields to personalize the message.")]
        [ProtoMember(2), Long, Textarea] public string MessageBody { get; set; }

        public string GetBody() => MessageBody;
        public string GetSubject() => Subject;
    }
}
