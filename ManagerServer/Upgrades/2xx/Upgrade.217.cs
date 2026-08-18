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
        private static async Task<IEnumerable<Model.Object>> Upgrade217(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.PayslipTotalsPerItemAndEmployee>().ToArray())
            {
                e.Periods = new Model.PayslipTotalsPerItemAndEmployee.Period[] { new Model.PayslipTotalsPerItemAndEmployee.Period() { FromDate = e.Obsolete_From, ToDate = e.Obsolete_To ?? DateTime.Today } };
                list.Add(e);
            }
            return list;
        }
    }
}
