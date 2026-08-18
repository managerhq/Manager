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
        private static async Task<IEnumerable<Model.Object>> Upgrade32(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var journalEntries = objects.OfType<Model.JournalEntry>().Where(x => x.Obsolete_IsReversing).ToArray();
            foreach (var e in journalEntries)
            {
                var o = new Model.JournalEntry();
                o.Date = (e.Date < DateTime.MaxValue ? e.Date.AddDays(1) : e.Date);
                o.Key = Guid.CreateVersion7();
                o.Narration = "Reversing: " + e.Narration;
                o.Reference = e.Reference;
                o.Obsolete_Notes = e.Obsolete_Notes;
                o.Obsolete_Lines = e.Obsolete_Lines.Select(x => new Model.Obsolete.Obsolete76.TransactionLine() { Account = x.Account, Amount = x.Amount * -1, Obsolete_BankAccount = x.Obsolete_BankAccount, Description = x.Description, Obsolete_PurchaseInvoice = x.Obsolete_PurchaseInvoice, Qty = x.Qty, Obsolete_SalesInvoice = x.Obsolete_SalesInvoice, Item = x.Item, TaxCode = x.TaxCode }).ToArray();
                list.Add(o);

                e.Obsolete_IsReversing = false;
                list.Add(e);
            }
            return list;
        }
    }
}
