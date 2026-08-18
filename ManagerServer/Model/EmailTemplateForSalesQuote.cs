using System;
using System.Collections.Generic;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("cc707d1f-5a8d-43f1-84a4-22732fb0ccd6")]
    public sealed class EmailTemplateForSalesQuote : Object, IEmailTemplate
    {
        [Guide("Default email subject line when emailing sales quotes.")]
        [ProtoMember(1), Long] public string Subject { get; set; }
        
        [Guide("Default email body text when emailing sales quotes. You can include merge fields to personalize the message.")]
        [ProtoMember(2), Long, Textarea] public string MessageBody { get; set; }

        public string GetBody() => MessageBody;
        public string GetSubject() => Subject;
    }
}
