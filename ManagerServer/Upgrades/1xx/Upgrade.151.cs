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
        private static async Task<IEnumerable<Model.Object>> Upgrade151(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var salesInvoices = objects.OfType<ManagerServer.Model.SalesInvoice>().ToDictionary(x => x.Key);
            var purchaseInvoices = objects.OfType<ManagerServer.Model.PurchaseInvoice>().ToDictionary(x => x.Key);

            var fix = new Func<ManagerServer.Model.Obsolete.Obsolete76.TransactionLine[], bool>(lines =>
            {
                var dirty = false;
                if (lines != null)
                {
                    foreach (var e2 in lines)
                    {
                        if (e2 == null) continue;
                        if (!e2.Account.HasValue) continue;

                        if (salesInvoices.ContainsKey(e2.Account.Value))
                        {
                            dirty = true;
                            e2.Invoice = e2.Account.Value;
                            e2.Account = salesInvoices[e2.Account.Value].Customer;
                        }
                        else if (purchaseInvoices.ContainsKey(e2.Account.Value))
                        {
                            dirty = true;
                            e2.Invoice = e2.Account.Value;
                            e2.Account = purchaseInvoices[e2.Account.Value].Supplier;
                        }
                    }
                }
                return dirty;
            });

            var list = new List<Model.Object>();

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().ToArray()) if (fix(e.Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().ToArray()) if (fix(e.Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.ExpenseClaim>().ToArray()) if (fix(e.Obsolete_Lines2)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete67.BankRule>().ToArray())
            {
                if (e.Obsolete_GeneralLedgerAccount == ManagerServer.Model.Master.AccountKeys.Obsolete_CustomerCredits)
                {
                    e.Obsolete_GeneralLedgerAccount = ManagerServer.Model.Master.AccountKeys.AccountsReceivable;
                    list.Add(e);
                }
                else if (e.Obsolete_GeneralLedgerAccount == ManagerServer.Model.Master.AccountKeys.Obsolete_SupplierCredits)
                {
                    e.Obsolete_GeneralLedgerAccount = ManagerServer.Model.Master.AccountKeys.AccountsPayable;
                    list.Add(e);
                }
            }

            return list;
        }
    }
}
