using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using System.Text;
using System.IO;
using ManagerServer.Model;
using System.Reflection;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade180(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            var salesQuotesWithNotes = objects.OfType<ManagerServer.Model.SalesQuote>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray();
            if (salesQuotesWithNotes.Any())
            {
                var customField = new Guid("0594cf61-1b01-4a81-a26f-7059f688930b");
                var defaultNotes = objects.OfType<ManagerServer.Model.Obsolete.Obsolete36.SalesQuoteDefaultNotes36>().FirstOrDefault(x => x.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete36.SalesQuoteDefaultNotes36))) ?? new ManagerServer.Model.Obsolete.Obsolete36.SalesQuoteDefaultNotes36();
                list.Add(new ManagerServer.Model.CustomField() { Key = customField, Name = "Notes", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SalesQuote)), Obsolete_DefaultValue = defaultNotes.Value, DisplayOnView = true });
                foreach (var e in salesQuotesWithNotes)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField, e.Obsolete_Notes);
                    list.Add(e);
                }
            }

            var salesInvoicesWithNotes = objects.OfType<ManagerServer.Model.SalesInvoice>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray();
            var recurringSalesInvoicesWithNotes = objects.OfType<ManagerServer.Model.RecurringSalesInvoice>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray();
            if (salesInvoicesWithNotes.Any() || recurringSalesInvoicesWithNotes.Any())
            {
                var customField = new Guid("3e57ff28-c959-417d-bc28-ae4b4a913d8f");
                var defaultNotes = objects.OfType<ManagerServer.Model.Obsolete.Obsolete36.SalesInvoiceDefaultNotes36>().FirstOrDefault(x => x.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete36.SalesInvoiceDefaultNotes36))) ?? new ManagerServer.Model.Obsolete.Obsolete36.SalesInvoiceDefaultNotes36();
                list.Add(new ManagerServer.Model.CustomField() { Key = customField, Name = "Notes", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SalesInvoice)), Obsolete_DefaultValue = defaultNotes.Value, DisplayOnView = true });
                foreach (var e in salesInvoicesWithNotes)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField, e.Obsolete_Notes);
                    list.Add(e);
                }
                foreach (var e in recurringSalesInvoicesWithNotes)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField, e.Obsolete_Notes);
                    list.Add(e);
                }
            }

            var deliveryNotesWithNotes = objects.OfType<ManagerServer.Model.DeliveryNote>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray();
            if (deliveryNotesWithNotes.Any())
            {
                var customField = new Guid("d86a9646-4007-4cd5-a523-ee3d7556516f");
                var defaultNotes = objects.OfType<ManagerServer.Model.Obsolete.Obsolete36.DeliveryNotesDefaultNotes36>().FirstOrDefault(x => x.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete36.DeliveryNotesDefaultNotes36))) ?? new ManagerServer.Model.Obsolete.Obsolete36.DeliveryNotesDefaultNotes36();
                list.Add(new ManagerServer.Model.CustomField() { Key = customField, Name = "Notes", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.DeliveryNote)), Obsolete_DefaultValue = defaultNotes.Value, DisplayOnView = true });
                foreach (var e in deliveryNotesWithNotes)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField, e.Obsolete_Notes);
                    list.Add(e);
                }
            }

            var purchaseInvoicesWithNotes = objects.OfType<ManagerServer.Model.PurchaseInvoice>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray();
            var recurringPurchaseInvoicesWithNotes = objects.OfType<ManagerServer.Model.RecurringPurchaseInvoice>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray();
            if (purchaseInvoicesWithNotes.Any() || recurringPurchaseInvoicesWithNotes.Any())
            {
                var customField = new Guid("aaf1c9cc-341a-4a26-9baf-b6893817d450");
                list.Add(new ManagerServer.Model.CustomField() { Key = customField, Name = "Notes", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.PurchaseInvoice)), DisplayOnView = true });
                foreach (var e in purchaseInvoicesWithNotes)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField, e.Obsolete_Notes);
                    list.Add(e);
                }
                foreach (var e in recurringPurchaseInvoicesWithNotes)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField, e.Obsolete_Notes);
                    list.Add(e);
                }
            }

            return list;
        }
    }
}
