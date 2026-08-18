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
        private static async Task<IEnumerable<Model.Object>> Upgrade42(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var journalEntries = objects.OfType<Model.JournalEntry>().Where(x => x.Obsolete_Lines != null && x.Obsolete_Lines.Any(y => y.Amount != 0m)).ToArray();
            foreach (var e in journalEntries)
            {
                foreach (var line in e.Obsolete_Lines)
                {
                    if (line.Amount == 0) continue;
                    if (line.Amount > 0)
                    {
                        line.Debit = line.Amount;
                        line.Credit = null;
                        line.Amount = 0m;
                    }
                    if (line.Amount < 0)
                    {
                        line.Credit = line.Amount * -1;
                        line.Debit = null;
                        line.Amount = 0m;
                    }
                }

                list.Add(e);
            }
            return list;
        }
    }
}
