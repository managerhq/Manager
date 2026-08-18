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
        private static async Task<IEnumerable<Model.Object>> Upgrade120(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().Where(x => x.Lines != null).SelectMany(x => x.Lines).Where(x => x.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && x.Obsolete_BillableExpense.HasValue && x.Obsolete_DisbursementStatus != DisbursementStatus.Uninvoiced).ToArray()) list.Add(new ManagerServer.Model.Obsolete.Obsolete52.BillableExpense() { Key = e.Obsolete_BillableExpense.Value, Status = e.Obsolete_DisbursementStatus, SalesInvoice = e.Obsolete_DisbursementSalesInvoice, WriteOffDate = e.Obsolete_DisbursementWriteOffDate });
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete16.UnclearedPayment16>().Where(x => x.Lines != null).SelectMany(x => x.Lines).Where(x => x.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && x.Obsolete_BillableExpense.HasValue && x.Obsolete_DisbursementStatus != DisbursementStatus.Uninvoiced).ToArray()) list.Add(new ManagerServer.Model.Obsolete.Obsolete52.BillableExpense() { Key = e.Obsolete_BillableExpense.Value, Status = e.Obsolete_DisbursementStatus, SalesInvoice = e.Obsolete_DisbursementSalesInvoice, WriteOffDate = e.Obsolete_DisbursementWriteOffDate });
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().Where(x => x.Lines != null).SelectMany(x => x.Lines).Where(x => x.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && x.Obsolete_BillableExpense.HasValue && x.Obsolete_DisbursementStatus != DisbursementStatus.Uninvoiced).ToArray()) list.Add(new ManagerServer.Model.Obsolete.Obsolete52.BillableExpense() { Key = e.Obsolete_BillableExpense.Value, Status = e.Obsolete_DisbursementStatus, SalesInvoice = e.Obsolete_DisbursementSalesInvoice, WriteOffDate = e.Obsolete_DisbursementWriteOffDate });
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete16.UnclearedReceipt16>().Where(x => x.Lines != null).SelectMany(x => x.Lines).Where(x => x.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && x.Obsolete_BillableExpense.HasValue && x.Obsolete_DisbursementStatus != DisbursementStatus.Uninvoiced).ToArray()) list.Add(new ManagerServer.Model.Obsolete.Obsolete52.BillableExpense() { Key = e.Obsolete_BillableExpense.Value, Status = e.Obsolete_DisbursementStatus, SalesInvoice = e.Obsolete_DisbursementSalesInvoice, WriteOffDate = e.Obsolete_DisbursementWriteOffDate });
            foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>().Where(x => x.Obsolete_Lines != null).SelectMany(x => x.Obsolete_Lines).Where(x => x.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && x.Obsolete_BillableExpense.HasValue && x.Obsolete_DisbursementStatus != DisbursementStatus.Uninvoiced).ToArray()) list.Add(new ManagerServer.Model.Obsolete.Obsolete52.BillableExpense() { Key = e.Obsolete_BillableExpense.Value, Status = e.Obsolete_DisbursementStatus, SalesInvoice = e.Obsolete_DisbursementSalesInvoice, WriteOffDate = e.Obsolete_DisbursementWriteOffDate });
            foreach (var e in objects.OfType<ManagerServer.Model.ExpenseClaim>().Where(x => x.Lines != null).SelectMany(x => x.Obsolete_Lines2).Where(x => x.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && x.Obsolete_BillableExpense.HasValue && x.Obsolete_DisbursementStatus != DisbursementStatus.Uninvoiced).ToArray()) list.Add(new ManagerServer.Model.Obsolete.Obsolete52.BillableExpense() { Key = e.Obsolete_BillableExpense.Value, Status = e.Obsolete_DisbursementStatus, SalesInvoice = e.Obsolete_DisbursementSalesInvoice, WriteOffDate = e.Obsolete_DisbursementWriteOffDate });
            foreach (var e in objects.OfType<ManagerServer.Model.PurchaseInvoice>().Where(x => x.Obsolete_Lines != null).SelectMany(x => x.Obsolete_Lines).Where(x => x.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && x.Obsolete_BillableExpense.HasValue && x.Obsolete_DisbursementStatus != DisbursementStatus.Uninvoiced).ToArray()) list.Add(new ManagerServer.Model.Obsolete.Obsolete52.BillableExpense() { Key = e.Obsolete_BillableExpense.Value, Status = e.Obsolete_DisbursementStatus, SalesInvoice = e.Obsolete_DisbursementSalesInvoice, WriteOffDate = e.Obsolete_DisbursementWriteOffDate });
            return list;
        }
    }
}
