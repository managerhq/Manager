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
        private static async Task<IEnumerable<Model.Object>> Upgrade232(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.ProfitAndLossStatementAccount>().Where(x => x.Obsolete_Code.HasValue).ToArray())
            {
                e.Position = e.Obsolete_Code.Value;
                e.Code = e.Obsolete_Code.Value.ToString();
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete62.ProfitAndLossStatementBuiltInAccount>().Where(x => x.Obsolete_Code.HasValue).ToArray())
            {
                e.Position = e.Obsolete_Code.Value;
                e.Code = e.Obsolete_Code.Value.ToString();
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.ProfitAndLossStatementGroup>().Where(x => x.Obsolete_Code.HasValue).ToArray())
            {
                e.Position = e.Obsolete_Code.Value;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.BalanceSheetAccount>().Where(x => x.Obsolete_Code.HasValue).ToArray())
            {
                e.Position = e.Obsolete_Code.Value;
                e.Code = e.Obsolete_Code.Value.ToString();
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete63.BalanceSheetBuiltInAccount>().Where(x => x.Obsolete_Code.HasValue).ToArray())
            {
                e.Position = e.Obsolete_Code.Value;
                e.Code = e.Obsolete_Code.Value.ToString();
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.BalanceSheetGroup>().Where(x => x.Obsolete_Code.HasValue).ToArray())
            {
                e.Position = e.Obsolete_Code.Value;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Subtotal>().Where(x => x.Obsolete_Code.HasValue).ToArray())
            {
                e.Position = e.Obsolete_Code.Value;
                list.Add(e);
            }
            return list;
        }
    }
}
