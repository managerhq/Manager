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
        private static async Task<IEnumerable<Model.Object>> Upgrade148(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();

            foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Obsolete_Lines)
                {
                    if (e2 == null) continue;
                    if (e2.Item.HasValue)
                    {
                        dirty = true;
                        e2.Obsolete_Item = e2.Item.Value;
                        e2.Item = null;
                    }
                }
                if (dirty) list.Add(e);
            }

            return list;
        }
    }
}
