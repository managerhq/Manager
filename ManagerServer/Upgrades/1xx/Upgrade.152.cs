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
        private static async Task<IEnumerable<Model.Object>> Upgrade152(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();

            var bilableTime = objects.OfType<ManagerServer.Model.BillableTime>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_StaffMember) || !string.IsNullOrWhiteSpace(x.Obsolete_Category)).ToArray();

            if (bilableTime.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_StaffMember)))
            {
                var values = string.Join("\n", bilableTime.Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_StaffMember)).Select(x => x.Obsolete_StaffMember).Distinct().OrderBy(x => x).ToArray());
                var customField = new ManagerServer.Model.CustomField() { Key = Guid.CreateVersion7(), Obsolete_DisplayOnList = true, Name = "Staff member", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.DropdownList, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BillableTime)), OptionsForDropdownList = values };
                list.Add(customField);
                foreach (var e in bilableTime.Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_StaffMember)).ToArray())
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField.Key, e.Obsolete_StaffMember);
                    list.Add(e);
                }
            }

            if (bilableTime.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Category)))
            {
                var values = string.Join("\n", bilableTime.Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Category)).Select(x => x.Obsolete_Category).Distinct().OrderBy(x => x).ToArray());
                var customField = new ManagerServer.Model.CustomField() { Key = Guid.CreateVersion7(), Obsolete_DisplayOnList = true, Name = "Category", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.DropdownList, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BillableTime)), OptionsForDropdownList = values };
                list.Add(customField);
                foreach (var e in bilableTime.Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Category)).ToArray())
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(customField.Key, e.Obsolete_Category);
                    list.Add(e);
                }
            }

            return list;
        }
    }
}
