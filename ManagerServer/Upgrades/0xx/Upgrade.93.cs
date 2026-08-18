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
        private static async Task<IEnumerable<Model.Object>> Upgrade93(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var payers = new Dictionary<string, Guid>();
            foreach (var e in objects.OfType<Model.ExpenseClaimsPayer>().ToArray())
            {
                if (string.IsNullOrWhiteSpace(e.Name)) continue;
                if (payers.ContainsKey(e.Name)) continue;
                payers.Add(e.Name, e.Key);
            }

            foreach (var e in objects.OfType<Model.ExpenseClaim>().ToArray())
            {
                if (!string.IsNullOrWhiteSpace(e.Obsolete_Payor))
                {
                    if (!payers.ContainsKey(e.Obsolete_Payor))
                    {
                        var key = Guid.CreateVersion7();
                        list.Add(new Model.ExpenseClaimsPayer() { Key = key, Name = e.Obsolete_Payor });
                        payers.Add(e.Obsolete_Payor, key);
                    }
                    e.PaidBy = payers[e.Obsolete_Payor];
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
