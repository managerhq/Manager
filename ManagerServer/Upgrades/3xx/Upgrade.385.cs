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
        private static async Task<IEnumerable<Model.Object>> Upgrade385(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            var controlAccounts = new HashSet<Guid>(objects.OfType<ControlAccountForInventoryItems>().Select(x => x.Key));
            controlAccounts.Add(objects.Single<BalanceSheetInventoryOnHandAccount>().Key);

            foreach (var e in objects.OfType<JournalEntry>())
            {
                if (e.Lines == null) continue;
                if (!e.Lines.Any(x => x.Obsolete_InventoryItem.HasValue)) continue;

                foreach (var e2 in e.Lines)
                {
                    if (!e2.Account.HasValue) continue;
                    if (!e2.Obsolete_InventoryItem.HasValue) continue;
                    if (controlAccounts.Contains(e2.Account.Value))
                    {
                        e2.Item = e2.Obsolete_InventoryItem.Value;
                    }
                }

                e.ItemColumn = true;
                list.Add(e);
            }

            return list;
        }
    }
}
