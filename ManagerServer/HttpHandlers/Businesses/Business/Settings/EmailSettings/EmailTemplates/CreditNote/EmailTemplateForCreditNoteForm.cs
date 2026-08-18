using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.CreditNote
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(CreditNotes))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.CreditNote))]
    [Guide("Configure the email template for sending credit notes to customers.")]
    [Guide("Customize subject line and message body with placeholders for dynamic content.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForCreditNote))]
    internal sealed class EmailTemplateForCreditNoteForm : NakedVueForm<ManagerServer.Model.EmailTemplateForCreditNote>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForCreditNote>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}