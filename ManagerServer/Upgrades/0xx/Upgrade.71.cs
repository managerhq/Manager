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
        private static async Task<IEnumerable<Model.Object>> Upgrade71(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var taxCodes = objects.OfType<Model.TaxCode>().ToArray();
            foreach (var e in taxCodes)
            {
                if (e.Components == null || e.Components.Length == 0) continue;
                if (e.Components.Any(x => x.Obsolete_IsCompound))
                {
                    var baseRate = e.Components.Where(x => !x.Obsolete_IsCompound).Sum(x => x.ComponentRate);
                    foreach (var e2 in e.Components.Where(x => x.Obsolete_IsCompound))
                    {
                        var addon = baseRate / 100m * e2.ComponentRate;
                        e2.ComponentRate += addon;
                    }
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
