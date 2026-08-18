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
        private static async Task<IEnumerable<Model.Object>> Upgrade43(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var outOfPocketExpensesPayors = objects.OfType<Model.Obsolete.Obsolete40.OutOfPocketExpensePayor40>().ToDictionary(x => x.Key);
            var outOfPocketExpenses = objects.OfType<Model.Obsolete.Obsolete33.Payment33>().Where(x => x.CreditAccount.HasValue && outOfPocketExpensesPayors.ContainsKey(x.CreditAccount.Value)).ToArray();
            foreach (var e in outOfPocketExpenses)
            {
                var o = new Model.ExpenseClaim();
                o.Date = e.Date;
                o.Description = e.Description;
                o.Key = e.Key;
                o.Obsolete_Lines2 = e.Lines;
                o.Payee = e.Payee;
                o.Reference = e.Reference;
                if (e.CreditAccount.HasValue && outOfPocketExpensesPayors.ContainsKey(e.CreditAccount.Value))
                {
                    o.Obsolete_Payor = outOfPocketExpensesPayors[e.CreditAccount.Value].Name;
                    o.Obsolete_CreditAccount = outOfPocketExpensesPayors[e.CreditAccount.Value].Account;
                }
                list.Add(o);
            }
            return list;
        }
    }
}
