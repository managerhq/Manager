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
        private static async Task<IEnumerable<Model.Object>> Upgrade121(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Lines)
                {
                    if (e2.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesInvoiced)
                    {
                        dirty = true;
                        e2.Account = ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount;
                    }
                }
                if (dirty)
                {
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
