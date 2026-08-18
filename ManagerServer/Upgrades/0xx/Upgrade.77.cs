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
        private static async Task<IEnumerable<Model.Object>> Upgrade77(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.JournalEntry>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Obsolete_Lines)
                {
                    if (!string.IsNullOrWhiteSpace(e2.Obsolete_EquityReason))
                    {
                        e2.Description = e2.Obsolete_EquityReason;
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            return list;
        }
    }
}
