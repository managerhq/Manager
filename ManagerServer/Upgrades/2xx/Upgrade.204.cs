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
        private static async Task<IEnumerable<Model.Object>> Upgrade204(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.CustomField>().Where(x => x.Obsolete_FormType == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment))).Where(x => !string.IsNullOrWhiteSpace(x.Name)).GroupBy(x => x.Name.ToLowerInvariant().Trim()).ToDictionary(x => x.Key, x => x.ToArray()))
            {
                if (e.Value.Length == 1) continue;

                var newKey = Guid.CreateVersion7();
                var oldKeys = new HashSet<Guid>(e.Value.Select(x => x.Key).ToArray());

                foreach (var e2 in e.Value)
                {
                    list.Add(new ManagerServer.Model.Obsolete.Obsolete45.CustomField45() { Key = e2.Key, DisplayOnList = e2.Obsolete_DisplayOnList, DisplayOnView = e2.DisplayOnView, DropdownValues = e2.OptionsForDropdownList, FieldSize = e2.Size, FieldType = e2.Type, Inactive = e2.Inactive, Name = e2.Name, Position = e2.Position, Type = e2.Obsolete_FormType.Value });
                }
                list.Add(new ManagerServer.Model.CustomField() { Key = newKey, Obsolete_DisplayOnList = e.Value[0].Obsolete_DisplayOnList, DisplayOnView = e.Value[0].DisplayOnView, OptionsForDropdownList = e.Value[0].OptionsForDropdownList, Size = e.Value[0].Size, Type = e.Value[0].Type, Inactive = e.Value[0].Inactive, Name = e.Value[0].Name, Position = e.Value[0].Position, Obsolete_FormType = e.Value[0].Obsolete_FormType });

                foreach (var e2 in objects.OfType<ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment>().Where(x => x.CustomFields != null))
                {
                    var values = new List<string>();
                    foreach (var e3 in oldKeys)
                    {
                        if (!e2.CustomFields.ContainsKey(e3)) continue;
                        var value = e2.CustomFields[e3];
                        if (string.IsNullOrWhiteSpace(value)) continue;
                        values.Add(value);
                    }

                    if (values.Count > 0)
                    {
                        e2.CustomFields[newKey] = string.Join(" ", values);
                        list.Add(e2);
                    }
                }
            }
            return list;
        }
    }
}
