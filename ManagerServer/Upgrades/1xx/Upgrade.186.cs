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
        private static async Task<IEnumerable<Model.Object>> Upgrade186(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.InventoryItem>().Where(x => x.CustomFields != null))
            {
                foreach (var e2 in e.CustomFields.Keys.ToArray())
                {
                    var text = e.CustomFields[e2];
                    if (string.IsNullOrWhiteSpace(text)) continue;
                    if (text.EndsWith("\r\n"))
                    {
                        e.CustomFields[e2] = text.Substring(0, text.Length - 2);
                        list.Add(e);
                    }
                    else if (text.EndsWith("\r"))
                    {
                        e.CustomFields[e2] = text.Substring(0, text.Length - 1);
                        list.Add(e);
                    }
                    else if (text.EndsWith("\n"))
                    {
                        e.CustomFields[e2] = text.Substring(0, text.Length - 1);
                        list.Add(e);
                    }
                }
            }
            return list;
        }
    }
}
