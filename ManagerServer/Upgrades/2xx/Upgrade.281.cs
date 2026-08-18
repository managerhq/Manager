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
        private static async Task<IEnumerable<Model.Object>> Upgrade281(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.UserPermissions>().ToArray())
            {
                if (e.Obsolete_Tabs2 != null && e.Obsolete_Tabs2.ContainsKey("ReceiptsAndPayments"))
                {
                    if (!e.Obsolete_Tabs2.ContainsKey("Receipts")) e.Obsolete_Tabs2.Add("Receipts", e.Obsolete_Tabs2["ReceiptsAndPayments"]);
                    if (!e.Obsolete_Tabs2.ContainsKey("Payments")) e.Obsolete_Tabs2.Add("Payments", e.Obsolete_Tabs2["ReceiptsAndPayments"]);
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
