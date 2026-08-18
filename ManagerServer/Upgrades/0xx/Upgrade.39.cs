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
        private static async Task<IEnumerable<Model.Object>> Upgrade39(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var outOfPocketExpensesPayors = objects.OfType<Model.Obsolete.Obsolete40.OutOfPocketExpensePayor40>().ToDictionary(x => x.Key);
            var outOfPocketExpenses = objects.OfType<Model.Obsolete.Obsolete04.OutOfPocketExpense04>().ToArray();
            foreach (var e in outOfPocketExpenses)
            {
                var o = new Model.ExpenseClaim();
                o.Date = e.Date;
                o.Description = e.Notes;
                o.Key = e.Key;
                o.Obsolete_Lines2 = e.Lines;
                o.Payee = e.To;
                if (e.From.HasValue && outOfPocketExpensesPayors.ContainsKey(e.From.Value))
                {
                    o.Obsolete_Payor = outOfPocketExpensesPayors[e.From.Value].Name;
                    o.Obsolete_CreditAccount = outOfPocketExpensesPayors[e.From.Value].Account;
                }
                list.Add(o);
            }
            return list;
        }
    }
}
