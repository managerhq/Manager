using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("21129fc9-26db-4cab-a70b-b42802f7017d")]
    public sealed class EmailTemplateForPurchaseInvoice : Object, IEmailTemplate
    {
        [Guide("Default email subject line when emailing purchase invoices.")]
        [ProtoMember(1), Long] public string Subject { get; set; }
        
        [Guide("Default email body text when emailing purchase invoices. You can include merge fields to personalize the message.")]
        [ProtoMember(2), Long, Textarea] public string MessageBody { get; set; }

        public string GetBody() => MessageBody;
        public string GetSubject() => Subject;
    }
}
