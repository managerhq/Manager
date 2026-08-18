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
        private static async Task<IEnumerable<Model.Object>> Upgrade178(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var notes = objects.OfType<ManagerServer.Model.Obsolete.Obsolete42.BankReceipt42>().Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)) || objects.OfType<ManagerServer.Model.Obsolete.Obsolete42.BankPayment42>().Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes));
            if (notes)
            {
                var customField = Guid.CreateVersion7();
                list.Add(new ManagerServer.Model.CustomField() { Key = customField, Name = Strings.Notes, Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment)), DisplayOnView = true });
                foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete42.BankReceipt42>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray())
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField, e.Obsolete_Notes);
                    list.Add(e);
                }
                foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete42.BankPayment42>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray())
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField, e.Obsolete_Notes);
                    list.Add(e);
                }
            }

            var notes2 = objects.OfType<ManagerServer.Model.Obsolete.Obsolete43.CashReceipt43>().Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)) || objects.OfType<ManagerServer.Model.Obsolete.Obsolete43.CashPayment43>().Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes));
            if (notes2)
            {
                var customField = Guid.CreateVersion7();
                list.Add(new ManagerServer.Model.CustomField() { Key = customField, Name = Strings.Notes, Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete44.CashTransaction44)), DisplayOnView = true });
                foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete43.CashReceipt43>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray())
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField, e.Obsolete_Notes);
                    list.Add(e);
                }
                foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete43.CashPayment43>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray())
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
