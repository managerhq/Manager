using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using System.Linq;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("48279506-e1bc-4011-bb47-97c0336401a0")]
    public sealed class EmailTemplateForSalesInvoice : Object, IEmailTemplate
    {
        [Guide("Default email subject line when emailing sales invoices.")]
        [Guide("You can use merge fields like {BusinessName}, {InvoiceNumber}, and {CustomerName} to personalize the subject.")]
        [ProtoMember(1), Long, NoLabel] public string Subject { get; set; }
        
        [Guide("Default email body text when emailing sales invoices. You can include merge fields to personalize the message.")]
        [Guide("Common merge fields include {CustomerName}, {InvoiceNumber}, {TotalAmount}, and {DueDate}. The invoice PDF will be automatically attached.")]
        [ProtoMember(2), Long, Textarea, NoLabel] public string MessageBody { get; set; }

        public string GetBody() => MessageBody;
        public string GetSubject() => Subject;
    }
}
