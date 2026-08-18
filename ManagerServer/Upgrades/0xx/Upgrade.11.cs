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
        private static async Task<IEnumerable<Model.Object>> Upgrade11(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var purchaseInvoices = objects.OfType<Model.Obsolete.Obsolete02.PurchaseInvoice02>().ToArray();
            foreach (var e in purchaseInvoices)
            {
                var o = new Model.PurchaseInvoice();
                o.Key = e.Key;
                o.DueDateDate = e.DueDate;
                o.IssueDate = e.IssueDate ?? DateTime.MaxValue;
                o.Obsolete_Notes = e.Notes;
                o.Reference = e.Reference;
                o.Supplier = e.From;
                o.Obsolete_Lines = e.Lines.Select(x => new Model.Obsolete.Obsolete76.TransactionLine() { Account = x.Account, Description = e.Notes, Amount = x.Obsolete_Amount ?? x.UnitPrice, TaxCode = x.Tax, Qty = x.Qty }).ToArray();
                list.Add(o);
            }
            return list.ToArray();
        }
    }
}
