using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.DebitNote
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(DebitNotes))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.DebitNote))]
    [Guide("Configure the email template for sending debit notes to suppliers.")]
    [Guide("Customize subject line and message body with placeholders for dynamic content.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForDebitNote))]
    internal sealed class EmailTemplateForDebitNoteForm : NakedVueForm<ManagerServer.Model.EmailTemplateForDebitNote>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForDebitNote>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}