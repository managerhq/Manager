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
        private static async Task<IEnumerable<Model.Object>> Upgrade317(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            if (objects.OfType<ManagerServer.Model.TaxCode>().Any())
            {
                foreach (var e in objects.OfType<ManagerServer.Model.Receipt>().Where(x => !x.Obsolete_AmountsIncludeTax))
                {
                    e.AmountsAreTaxExclusive = true;
                    list.Add(e);
                }
                foreach (var e in objects.OfType<ManagerServer.Model.Payment>().Where(x => !x.Obsolete_AmountsIncludeTax))
                {
                    e.AmountsAreTaxExclusive = true;
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
