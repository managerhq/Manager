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
        private static async Task<IEnumerable<Model.Object>> Upgrade182(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            var journalEntriesWithNotes = objects.OfType<ManagerServer.Model.JournalEntry>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray();
            if (journalEntriesWithNotes.Any())
            {
                var customField = new Guid("3cd7f782-9af1-4b5d-a4e4-b5ec1aad2304");
                list.Add(new ManagerServer.Model.CustomField() { Key = customField, Name = Strings.Notes, Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.JournalEntry)), DisplayOnView = true });
                foreach (var e in journalEntriesWithNotes)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField, e.Obsolete_Notes);
                    list.Add(e);
                }
            }

            var payslipsWithNotes = objects.OfType<ManagerServer.Model.Payslip>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray();
            var recurringPayslipsWithNotes = objects.OfType<ManagerServer.Model.RecurringPayslip>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray();
            if (payslipsWithNotes.Any() || recurringPayslipsWithNotes.Any())
            {
                var customField = new Guid("dd60ed00-9b2d-4345-980f-57afb09a0cd5");
                list.Add(new ManagerServer.Model.CustomField() { Key = customField, Name = Strings.Notes, Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Payslip)), DisplayOnView = true });
                foreach (var e in payslipsWithNotes)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField, e.Obsolete_Notes);
                    list.Add(e);
                }
                foreach (var e in recurringPayslipsWithNotes)
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
