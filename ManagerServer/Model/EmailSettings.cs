using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("a4ddb0e3-b207-4fee-aa01-f104b6c09932")]
    public sealed class EmailSettings : Object
    {
        [Guide("Manager.io supports two protocols: HTTP and SMTP.")]
        [ProtoMember(24)] public Protocol Protocol { get; set; }
        [Guide("If you have chosen HTTP in `Protocol` field, enter URL of HTTP server. Manager.io runs free public email service at **email.manager.io** so you can enter this into `HttpServer` field.")]
        [ProtoMember(25), IfEnum(nameof(Protocol), 0), Prepend("https://")] public string HttpServer { get; set; }
        [Guide("If you have chosen HTTP in `Protocol` field, you also need to specify which email address where replies to your emails should be delivered to. This is typically email address of your business.")]
        [ProtoMember(26), IfEnum(nameof(Protocol), 0), Prepend(nameof(Strings.EmailAddress)), Label(nameof(Strings.ReplyTo))] public string HttpReplyTo { get; set; }
        [Header("SMTP Configuration")]
        [Guide("If you have chosen SMTP in the `Protocol` field, enter the hostname of your SMTP server.")]
        [Guide("The hostname is the server name provided by your email service (examples: smtp.gmail.com, smtp.mail.yahoo.com, smtp.office365.com).")]
        [ProtoMember(3), IfEnum(nameof(Protocol), 1), Prepend(nameof(Strings.Hostname))] public string SmtpServer { get; set; }
        [Guide("The `Port` number can be 465, 587, or 25.")]
        [Guide("It's recommended to choose either 465 or 587 because these ports are securely encrypted, unlike port 25.")]
        [ProtoMember(4), IfEnum(nameof(Protocol), 1), Prepend(nameof(Strings.Port)), NoLabel] public SmtpPort Port { get; set; }
        [Header("SMTP Authentication")]
        [Guide("`Username` is the name you use to log in with your email provider.")]
        [Guide("This is often the email address associated with the account, but some providers may require a different username.")]
        [ProtoMember(7), IfEnum(nameof(Protocol), 1), Prepend(nameof(Strings.Username))] public string SmtpCredentials { get; set; }
        [Guide("If your username doesn't look like an email address, an additional `EmailAddress` field will appear.")]
        [Guide("Enter the email address linked to the sending account in this field.")]
        [ProtoMember(16), IfEnum(nameof(Protocol), 1), Prepend(nameof(Strings.EmailAddress)), NoLabel, IfDoesNotContain("@", nameof(SmtpCredentials))] public string EmailAddress { get; set; }
        [Guide("Enter the password associated with your username.")]
        [Guide("Click the `ShowPassword` button if you wish to view your password as you type it.")]
        [ProtoMember(8), IfEnum(nameof(Protocol), 1), Prepend(nameof(Strings.Password)), NoLabel, Placeholder("********")] public string Password { get; set; }
        [Header("Additional Options")]
        [Guide("Select the `SendCopy` option to send duplicates of your outgoing emails to an additional email address.")]
        [Guide("This is helpful for archiving emails sent from the program.")]
        [ProtoMember(10), IfEnum(nameof(Protocol), 1)] public bool SendCopy { get; set; }
        [Guide("Enter the email address where copies of sent emails should be delivered.")]
        [ProtoMember(1), NoLabel, IfTrue(nameof(SendCopy)), Prepend(nameof(Strings.EmailAddress))] public string SendCopyEmail { get; set; }
        [Guide("Select `ReceiveRepliesAtADifferentAddressThanYouSendFrom` if you want replies sent to a different email address.")]
        [Guide("When selected, a field will appear where you can enter the reply-to email address.")]
        [ProtoMember(20), IfEnum(nameof(Protocol), 1)] public bool ReceiveRepliesAtADifferentAddressThanYouSendFrom { get; set; }
        [Guide("Enter the email address where replies should be sent.")]
        [ProtoMember(21), NoLabel, IfTrue(nameof(ReceiveRepliesAtADifferentAddressThanYouSendFrom)), Prepend(nameof(Strings.EmailAddress))] public string ReplyTo { get; set; }
        [Header("Security Settings")]
        [Guide("The `DoNotVerifyTLSCertificate` checkbox allows you to skip the validation of self-signed certificates.")]
        [Guide("Only use this option if you are emailing from your own server that uses self-signed certificates.")]
        [Guide("For security purposes, when using established email providers like Gmail, Yahoo Mail, or Microsoft Office 365, always leave this checkbox unchecked.")]
        [ProtoMember(22), IfEnum(nameof(Protocol), 1)] public bool DoNotVerifyTLSCertificate { get; set; }

        [ProtoMember(12)] public PageSize Obsolete_PageSize { get; set; }
        [ProtoMember(19)] public EmailFormat Obsolete_Format2 { get; set; }
        [ProtoMember(14)] public string Obsolete_NewPassword { get; set; }
        [ProtoMember(15)] public int Obsolete_PdfStandard { get; set; }
        [ProtoMember(11)] public string Obsolete_Name { get; set; }
        [ProtoMember(9)] public bool Obsolete_TrackingEnabled { get; set; }
        [ProtoMember(2)] public bool Obsolete_UseCustomSmtpServer { get; set; }
        [ProtoMember(6)] public bool Obsolete_RequiresAuthentication { get; set; }
        [ProtoMember(5)] public bool Obsolete_SSL { get; set; }
        [ProtoMember(13)] public int Obsolete_EmailSendingFormat { get; set; }
        [ProtoMember(17)] public bool Obsolete_EmailTracking { get; set; }
        [ProtoMember(18)] public int Obsolete_Format { get; set; }
        [ProtoMember(23)] public bool Obsolete_DoNotUseInternationalDeliveryFormat { get; set; }

        public override bool IsInactive()
        {
            if (Protocol == Protocol.HTTP)
            {
                if (string.IsNullOrWhiteSpace(HttpServer)) return true;
                if (string.IsNullOrWhiteSpace(HttpReplyTo)) return true;
            }
            if (Protocol == Protocol.SMTP)
            {
                if (string.IsNullOrWhiteSpace(SmtpServer)) return true;
            }
            return false;
        }
    }
}