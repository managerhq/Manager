using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.DeliveryNote
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(DeliveryNotes))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.DeliveryNote))]
    [Guide("Configure the email template for sending delivery notes to customers.")]
    [Guide("Customize subject line and message body with placeholders for dynamic content.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForDeliveryNote))]
    internal sealed class EmailTemplateForDeliveryNoteForm : NakedVueForm<ManagerServer.Model.EmailTemplateForDeliveryNote>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForDeliveryNote>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}