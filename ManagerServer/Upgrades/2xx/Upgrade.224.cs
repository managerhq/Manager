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
        private static async Task<IEnumerable<Model.Object>> Upgrade224(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var billableExpenses = objects.OfType<ManagerServer.Model.Obsolete.Obsolete52.BillableExpense>().ToDictionary(x => x.Key);

            var list = new List<Model.Object>();

            if (billableExpenses.Any(x => x.Value.Status == DisbursementStatus.WrittenOff))
            {
                list.Add(new Model.ProfitAndLossStatementAccount() { Key = new Guid("ec48e19a-51c0-4115-bfe4-96ea1254bfe5"), Name = Strings.Billable_expenses + " - " + Strings.WrittenOff });
            }

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment>().ToArray())
            {
                var dirty = false;
                if (e.Lines == null) continue;
                foreach (var e2 in e.Lines)
                {
                    if (e2.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && e2.BillableExpenseCustomer.HasValue && e2.Obsolete_BillableExpense.HasValue && billableExpenses.ContainsKey(e2.Obsolete_BillableExpense.Value))
                    {
                        if (billableExpenses[e2.Obsolete_BillableExpense.Value].Status == DisbursementStatus.Invoiced)
                        {
                            e2.BillableExpenseSalesInvoice = billableExpenses[e2.Obsolete_BillableExpense.Value].SalesInvoice;
                            dirty = true;
                        }
                        if (billableExpenses[e2.Obsolete_BillableExpense.Value].Status == DisbursementStatus.WrittenOff)
                        {
                            e2.Account = new Guid("ec48e19a-51c0-4115-bfe4-96ea1254bfe5");
                            dirty = true;
                        }
                    }
                }
                if (dirty)
                {
                    list.Add(e);
                }
            }

            foreach (var e in objects.OfType<ManagerServer.Model.ExpenseClaim>().ToArray())
            {
                var dirty = false;
                if (e.Lines == null) continue;
                foreach (var e2 in e.Obsolete_Lines2)
                {
                    if (e2.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && e2.BillableExpenseCustomer.HasValue && e2.Obsolete_BillableExpense.HasValue && billableExpenses.ContainsKey(e2.Obsolete_BillableExpense.Value))
                    {
                        if (billableExpenses[e2.Obsolete_BillableExpense.Value].Status == DisbursementStatus.Invoiced)
                        {
                            e2.BillableExpenseSalesInvoice = billableExpenses[e2.Obsolete_BillableExpense.Value].SalesInvoice;
                            dirty = true;
                        }
                        if (billableExpenses[e2.Obsolete_BillableExpense.Value].Status == DisbursementStatus.WrittenOff)
                        {
                            e2.Account = new Guid("ec48e19a-51c0-4115-bfe4-96ea1254bfe5");
                            dirty = true;
                        }
                    }
                }
                if (dirty)
                {
                    list.Add(e);
                }
            }

            foreach (var e in objects.OfType<ManagerServer.Model.PurchaseInvoice>().ToArray())
            {
                var dirty = false;
                if (e.Lines == null) continue;
                foreach (var e2 in e.Obsolete_Lines)
                {
                    if (e2.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && e2.BillableExpenseCustomer.HasValue && e2.Obsolete_BillableExpense.HasValue && billableExpenses.ContainsKey(e2.Obsolete_BillableExpense.Value))
                    {
                        if (billableExpenses[e2.Obsolete_BillableExpense.Value].Status == DisbursementStatus.Invoiced)
                        {
                            e2.BillableExpenseSalesInvoice = billableExpenses[e2.Obsolete_BillableExpense.Value].SalesInvoice;
                            dirty = true;
                        }
                        if (billableExpenses[e2.Obsolete_BillableExpense.Value].Status == DisbursementStatus.WrittenOff)
                        {
                            e2.Account = new Guid("ec48e19a-51c0-4115-bfe4-96ea1254bfe5");
                            dirty = true;
                        }
                    }
                }
                if (dirty)
                {
                    list.Add(e);
                }
            }

            foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>().ToArray())
            {
                var dirty = false;
                if (e.Lines == null) continue;
                foreach (var e2 in e.Obsolete_Lines)
                {
                    if (e2.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && e2.BillableExpenseCustomer.HasValue && e2.Obsolete_BillableExpense.HasValue && billableExpenses.ContainsKey(e2.Obsolete_BillableExpense.Value))
                    {
                        if (billableExpenses[e2.Obsolete_BillableExpense.Value].Status == DisbursementStatus.Invoiced)
                        {
                            e2.BillableExpenseSalesInvoice = billableExpenses[e2.Obsolete_BillableExpense.Value].SalesInvoice;
                            dirty = true;
                        }
                        if (billableExpenses[e2.Obsolete_BillableExpense.Value].Status == DisbursementStatus.WrittenOff)
                        {
                            e2.Account = new Guid("ec48e19a-51c0-4115-bfe4-96ea1254bfe5");
                            dirty = true;
                        }
                    }
                }
                if (dirty)
                {
                    list.Add(e);
                }
            }

            return list;
        }
    }
}
