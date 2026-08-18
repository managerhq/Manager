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
        private static async Task<IEnumerable<Model.Object>> Upgrade74(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var list2 = new List<DateTime>();
            if (objects.OfType<Model.SalesInvoice>().Any()) list2.Add(objects.OfType<Model.SalesInvoice>().Min(x => x.IssueDate));
            if (objects.OfType<Model.PurchaseInvoice>().Any()) list2.Add(objects.OfType<Model.PurchaseInvoice>().Min(x => x.IssueDate));
            if (objects.OfType<Model.JournalEntry>().Any()) list2.Add(objects.OfType<Model.JournalEntry>().Min(x => x.Date));
            if (objects.OfType<Model.Obsolete.Obsolete33.Payment33>().Any()) list2.Add(objects.OfType<Model.Obsolete.Obsolete33.Payment33>().Min(x => x.Date));
            if (objects.OfType<Model.Obsolete.Obsolete33.Receipt33>().Any()) list2.Add(objects.OfType<Model.Obsolete.Obsolete33.Receipt33>().Min(x => x.Date));
            if (objects.OfType<Model.ExpenseClaim>().Any()) list2.Add(objects.OfType<Model.ExpenseClaim>().Min(x => x.Date));
            var minDate = list2.Any() ? list2.Min() : DateTime.Today;

            list.Add(new Model.Obsolete.Obsolete72.StartDate() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete72.StartDate)), Date = minDate });
            var balance = objects.OfType<Model.Obsolete.Obsolete18.GeneralLedgerAccount18>().Where(x => x.Obsolete_HasOpeningBalance).Where(x => x.Category == ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income || x.Category == ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses).Sum(x => x.StartingBalance);
            var type = DebitCredit.Debit;
            if (balance < 0m)
            {
                type = DebitCredit.Credit;
                balance = balance * -1;
            }
            list.Add(new Model.Obsolete.Obsolete18.ControlAccount18() { Key = new Guid("01c00313-4790-451e-ae05-1ad6fc6fa476"), StartingBalance = balance, StartingBalanceType = type });
            return list;
        }
    }
}
