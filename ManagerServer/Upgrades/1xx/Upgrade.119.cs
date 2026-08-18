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
        private static async Task<IEnumerable<Model.Object>> Upgrade119(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().Where(x => x.Obsolete_JournalEntry != null && string.IsNullOrWhiteSpace(x.Notes) && !string.IsNullOrWhiteSpace(x.Obsolete_JournalEntry.Obsolete_Notes)).ToArray())
            {
                e.Notes = e.Obsolete_JournalEntry.Obsolete_Notes;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().Where(x => x.Obsolete_JournalEntry != null && string.IsNullOrWhiteSpace(x.Notes) && !string.IsNullOrWhiteSpace(x.Obsolete_JournalEntry.Obsolete_Notes)).ToArray())
            {
                e.Notes = e.Obsolete_JournalEntry.Obsolete_Notes;
                list.Add(e);
            }
            return list;
        }
    }
}
