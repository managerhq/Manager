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
        private static async Task<IEnumerable<Model.Object>> Upgrade160(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var guids = new HashSet<Guid>();

            var fix = new Func<ManagerServer.Model.Obsolete.Obsolete76.TransactionLine[], bool>(lines =>
            {
                var dirty = false;
                if (lines != null)
                {
                    foreach (var e2 in lines)
                    {
                        if (e2 == null) continue;
                        if (!e2.Account.HasValue) continue;
                        if (e2.Account.Value != ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount) continue;
                        if (!e2.Obsolete_BillableExpense.HasValue) continue;

                        if (guids.Contains(e2.Obsolete_BillableExpense.Value))
                        {
                            dirty = true;
                            e2.Obsolete_Disbursement = e2.Obsolete_BillableExpense;
                            e2.Obsolete_BillableExpense = Guid.CreateVersion7();
                        }
                        else
                        {
                            guids.Add(e2.Obsolete_BillableExpense.Value);
                        }
                    }
                }
                return dirty;
            });

            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().OrderBy(x => x.Date).ToArray()) if (fix(e.Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().OrderBy(x => x.Date).ToArray()) if (fix(e.Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.ExpenseClaim>().OrderBy(x => x.Date).ToArray()) if (fix(e.Obsolete_Lines2)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.PurchaseInvoice>().OrderBy(x => x.IssueDate).ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>().OrderBy(x => x.Date).ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);
            return list;
        }
    }
}
