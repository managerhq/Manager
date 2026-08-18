using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("61648ae7-4613-459a-aca6-9913928f776f")]
    public sealed class EmailTemplateForPayslip : Object, IEmailTemplate
    {
        [Guide("Default email subject line when emailing payslips.")]
        [ProtoMember(1), Long] public string Subject { get; set; }
        
        [Guide("Default email body text when emailing payslips. You can include merge fields to personalize the message.")]
        [ProtoMember(2), Long, Textarea] public string MessageBody { get; set; }

        public string GetBody() => MessageBody;
        public string GetSubject() => Subject;
    }
}
