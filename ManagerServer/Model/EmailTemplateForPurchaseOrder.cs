using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("a45a2ffe-2c07-4564-8a98-431032a3387f")]
    public sealed class EmailTemplateForPurchaseOrder : Object, IEmailTemplate
    {
        [Guide("Default email subject line when emailing purchase orders.")]
        [ProtoMember(1), Long] public string Subject { get; set; }
        
        [Guide("Default email body text when emailing purchase orders. You can include merge fields to personalize the message.")]
        [ProtoMember(2), Long, Textarea] public string MessageBody { get; set; }

        public string GetBody() => MessageBody;
        public string GetSubject() => Subject;
    }
}
