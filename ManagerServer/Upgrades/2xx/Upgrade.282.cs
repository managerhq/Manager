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
        private static async Task<IEnumerable<Model.Object>> Upgrade282(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var customFields = objects.OfType<ManagerServer.Model.CustomField>().Where(x => x.Obsolete_FormType == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment))).ToArray();

            var map = new Dictionary<Guid, Guid>();

            foreach (var e in customFields)
            {
                e.Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Receipt));

                var customField = new ManagerServer.Model.CustomField()
                {
                    Key = Guid.CreateVersion7(),
                    Description = e.Description,
                    Obsolete_DisplayOnList = e.Obsolete_DisplayOnList,
                    DisplayOnView = e.DisplayOnView,
                    Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Payment)),
                    Inactive = e.Inactive,
                    Name = e.Name,
                    Position = e.Position,
                    Size = e.Size,
                    Type = e.Type,
                    OptionsForDropdownList = e.OptionsForDropdownList
                };

                list.Add(e);
                list.Add(customField);

                map.Add(e.Key, customField.Key);
            }

            /*
            if (customFields.Any())
            {
                foreach (var e in objects.OfType<Manager.Model.Payment>().ToArray())
                {
                    if (e.CustomFields != null) continue;
                    if (e.Obsolete_CustomFields == null) continue;
                    if (e.Obsolete_CustomFields.Count == 0) continue;

                    var paymentCustomFields = new Dictionary<Guid, string>();
                    foreach (var e2 in e.Obsolete_CustomFields)
                    {
                        if (map.ContainsKey(e2.Key))
                        {
                            paymentCustomFields.Add(map[e2.Key], e2.Value);
                        }
                    }
                    e.CustomFields = paymentCustomFields;

                    list.Add(e);
                }
            }
            */

            return list;
        }
    }
}
