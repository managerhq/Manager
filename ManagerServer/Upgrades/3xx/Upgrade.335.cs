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
        private static async Task<IEnumerable<Model.Object>> Upgrade335(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.ProfitAndLossStatementAccount>().Where(x => x.DefaultTaxCode.HasValue))
            {
                e.HasDefaultTaxCode = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.ProfitAndLossStatementAccountBillableTimeInvoiced>().Where(x => x.DefaultTaxCode.HasValue))
            {
                e.HasDefaultTaxCode = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.ProfitAndLossStatementAccountInventorySales>().Where(x => x.DefaultTaxCode.HasValue))
            {
                e.HasDefaultTaxCode = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.BalanceSheetAccount>().Where(x => x.DefaultTaxCode.HasValue))
            {
                e.HasDefaultTaxCode = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.BalanceSheetBillableExpensesAccount>().Where(x => x.DefaultTaxCode.HasValue))
            {
                e.HasDefaultTaxCode = true;
                list.Add(e);
            }
            return list;
        }
    }
}
