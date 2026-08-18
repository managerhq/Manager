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
        private static async Task<IEnumerable<Model.Object>> Upgrade101(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.Payslip>().Where(x => x.Obsolete_TrackingCode.HasValue).ToArray())
            {
                if (e.Earnings != null)
                {
                    foreach (var e2 in e.Earnings) e2.Division = e.Obsolete_TrackingCode;
                }
                if (e.Contributions != null)
                {
                    foreach (var e2 in e.Contributions) e2.Division = e.Obsolete_TrackingCode;
                }
                list.Add(e);
            }
            return list;
        }
    }
}
