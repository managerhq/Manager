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
        private static async Task<IEnumerable<Model.Object>> Upgrade68(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var creditNotes = objects.OfType<Model.CreditNote>().ToDictionary(x => x.Key, x => x.Customer);
            foreach (var e in objects.OfType<Model.JournalEntry>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Obsolete_Lines)
                {
                    if (e2.Account.HasValue && e2.Obsolete_SalesInvoice.HasValue && e2.Account == Model.Master.AccountKeys.AccountsReceivable && creditNotes.ContainsKey(e2.Obsolete_SalesInvoice.Value))
                    {
                        e2.Account = Model.Master.AccountKeys.Obsolete_CustomerCredits;
                        e2.Obsolete_Customer = creditNotes[e2.Obsolete_SalesInvoice.Value];
                        e2.Obsolete_SalesInvoice = null;
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete33.Payment33>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Lines)
                {
                    if (e2.Account.HasValue && e2.Obsolete_SalesInvoice.HasValue && e2.Account == Model.Master.AccountKeys.AccountsReceivable && creditNotes.ContainsKey(e2.Obsolete_SalesInvoice.Value))
                    {
                        e2.Account = Model.Master.AccountKeys.Obsolete_CustomerCredits;
                        e2.Obsolete_Customer = creditNotes[e2.Obsolete_SalesInvoice.Value];
                        e2.Obsolete_SalesInvoice = null;
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete33.Receipt33>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Lines)
                {
                    if (e2.Account.HasValue && e2.Obsolete_SalesInvoice.HasValue && e2.Account == Model.Master.AccountKeys.AccountsReceivable && creditNotes.ContainsKey(e2.Obsolete_SalesInvoice.Value))
                    {
                        e2.Account = Model.Master.AccountKeys.Obsolete_CustomerCredits;
                        e2.Obsolete_Customer = creditNotes[e2.Obsolete_SalesInvoice.Value];
                        e2.Obsolete_SalesInvoice = null;
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            foreach (var e in objects.OfType<Model.ExpenseClaim>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Obsolete_Lines2)
                {
                    if (e2.Account.HasValue && e2.Obsolete_SalesInvoice.HasValue && e2.Account == Model.Master.AccountKeys.AccountsReceivable && creditNotes.ContainsKey(e2.Obsolete_SalesInvoice.Value))
                    {
                        e2.Account = Model.Master.AccountKeys.Obsolete_CustomerCredits;
                        e2.Obsolete_Customer = creditNotes[e2.Obsolete_SalesInvoice.Value];
                        e2.Obsolete_SalesInvoice = null;
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            return list;
        }
    }
}
