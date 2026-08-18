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
        private static async Task<IEnumerable<Model.Object>> Upgrade271(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var billableExpensesAccount = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetBillableExpensesAccount));
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>())
            {
                if (e.Lines == null) continue;
                foreach (var e2 in e.Lines)
                {
                    if (e2 == null) continue;
                    if (e2.Account == billableExpensesAccount)
                    {
                        if (!e2.BillableExpenseCustomer.HasValue) e2.BillableExpenseCustomer = e2.AccountsReceivableCustomer;
                        if (!e2.BillableExpenseSalesInvoice.HasValue) e2.BillableExpenseSalesInvoice = e2.AccountsReceivableSalesInvoice;
                        list.Add(e);
                    }
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.RecurringJournalEntry>())
            {
                if (e.Lines == null) continue;
                foreach (var e2 in e.Lines)
                {
                    if (e2 == null) continue;
                    if (e2.Account == billableExpensesAccount)
                    {
                        if (!e2.BillableExpenseCustomer.HasValue) e2.BillableExpenseCustomer = e2.AccountsReceivableCustomer;
                        if (!e2.BillableExpenseSalesInvoice.HasValue) e2.BillableExpenseSalesInvoice = e2.AccountsReceivableSalesInvoice;
                        list.Add(e);
                    }
                }
            }
            return list.Distinct();
        }
    }
}
