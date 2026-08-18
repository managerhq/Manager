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
        private static async Task<IEnumerable<Model.Object>> Upgrade226(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var account1 = objects.OfType<ManagerServer.Model.Obsolete.Obsolete62.ProfitAndLossStatementBuiltInAccount>().SingleOrDefault(x => x.Key == ManagerServer.Model.Master.AccountKeys.BillableExpensesInvoiced);
            if (account1 != null && !string.IsNullOrWhiteSpace(account1.Name))
            {
                account1.Name = null;
                list.Add(account1);
            }
            var account2 = objects.OfType<ManagerServer.Model.Obsolete.Obsolete62.ProfitAndLossStatementBuiltInAccount>().SingleOrDefault(x => x.Key == ManagerServer.Model.Master.AccountKeys.BillableExpensesCost);
            if (account2 != null && !string.IsNullOrWhiteSpace(account2.Name))
            {
                account2.Name = null;
                list.Add(account2);
            }
            return list;
        }
    }
}
