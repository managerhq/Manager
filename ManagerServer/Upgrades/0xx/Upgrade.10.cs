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
        private static async Task<IEnumerable<Model.Object>> Upgrade10(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var salesInvoices = objects.OfType<Model.Obsolete.Obsolete02.SalesInvoice02>().ToArray();
            foreach (var e in salesInvoices)
            {
                var o = new Model.SalesInvoice();
                o.Key = e.Key;
                o.AmountsIncludeTax = e.AmountsIncludeTax;
                o.BillingAddress = e.BillingAddress;
                o.DueDateDate = e.DueDate;
                o.IssueDate = e.IssueDate ?? DateTime.MaxValue;
                o.Obsolete_InternalNotes = e.Notes;
                o.Reference = e.Reference;
                o.Customer = e.To;
                o.Obsolete_Lines = e.Lines.Select(x => new Model.Obsolete.Obsolete76.TransactionLine() { Account = x.Account, Amount = x.Obsolete_Amount ?? x.UnitPrice, Description = x.Description, TaxCode = x.Tax, Qty = x.Qty }).ToArray();
                list.Add(o);
            }
            return list.ToArray();
        }
    }
}
