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
        private static async Task<IEnumerable<Model.Object>> Upgrade96(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var expenseClaimPayers = new HashSet<Guid>(objects.OfType<Model.ExpenseClaimsPayer>().Select(x => x.Key));

            foreach (var e in objects.OfType<Model.JournalEntry>().Where(x => x.Lines != null && x.Lines.Any(y => y.Account.HasValue && expenseClaimPayers.Contains(y.Account.Value))).ToArray())
            {
                foreach (var e2 in e.Obsolete_Lines.Where(x => x.Account.HasValue && expenseClaimPayers.Contains(x.Account.Value)))
                {
                    e2.Obsolete_ExpenseClaimPayer = e2.Account;
                    e2.Account = Model.Master.AccountKeys.ExpenseClaims;
                }
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete33.Receipt33>().Where(x => x.Lines != null && x.Lines.Any(y => y.Account.HasValue && expenseClaimPayers.Contains(y.Account.Value))).ToArray())
            {
                foreach (var e2 in e.Lines.Where(x => x.Account.HasValue && expenseClaimPayers.Contains(x.Account.Value)))
                {
                    e2.Obsolete_ExpenseClaimPayer = e2.Account;
                    e2.Account = Model.Master.AccountKeys.ExpenseClaims;
                }
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete33.Payment33>().Where(x => x.Lines != null && x.Lines.Any(y => y.Account.HasValue && expenseClaimPayers.Contains(y.Account.Value))).ToArray())
            {
                foreach (var e2 in e.Lines.Where(x => x.Account.HasValue && expenseClaimPayers.Contains(x.Account.Value)))
                {
                    e2.Obsolete_ExpenseClaimPayer = e2.Account;
                    e2.Account = Model.Master.AccountKeys.ExpenseClaims;
                }
                list.Add(e);
            }
            return list;
        }
    }
}
