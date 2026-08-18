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
        private static async Task<IEnumerable<Model.Object>> Upgrade132(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            if (objects.OfType<ManagerServer.Model.InventoryItem>().Any())
            {
                var list2 = new List<Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>>();
                list2.AddRange(objects.OfType<Model.JournalEntry>().Where(x => x.Lines != null).Select(x => new Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>(x, x.Obsolete_Lines)).ToArray());
                list2.AddRange(objects.OfType<Model.Obsolete.Obsolete33.Payment33>().Where(x => x.Lines != null).Select(x => new Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>(x, x.Lines)).ToArray());
                list2.AddRange(objects.OfType<Model.Obsolete.Obsolete33.Receipt33>().Where(x => x.Lines != null).Select(x => new Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>(x, x.Lines)).ToArray());
                list2.AddRange(objects.OfType<Model.SalesInvoice>().Where(x => x.Obsolete_Lines != null).Select(x => new Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>(x, x.Obsolete_Lines)).ToArray());
                list2.AddRange(objects.OfType<Model.CreditNote>().Where(x => x.Obsolete_Lines != null).Select(x => new Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>(x, x.Obsolete_Lines)).ToArray());
                list2.AddRange(objects.OfType<Model.PurchaseInvoice>().Where(x => x.Obsolete_Lines != null).Select(x => new Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>(x, x.Obsolete_Lines)).ToArray());
                list2.AddRange(objects.OfType<Model.DebitNote>().Where(x => x.Lines != null).Select(x => new Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>(x, x.Obsolete_Lines)).ToArray());
                list2.AddRange(objects.OfType<Model.ExpenseClaim>().Where(x => x.Lines != null).Select(x => new Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>(x, x.Obsolete_Lines2)).ToArray());
                list2.AddRange(objects.OfType<Model.SalesQuote>().Where(x => x.Lines != null).Select(x => new Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>(x, x.Obsolete_Lines)).ToArray());
                list2.AddRange(objects.OfType<Model.PurchaseOrder>().Where(x => x.Lines != null).Select(x => new Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>(x, x.Obsolete_Lines)).ToArray());
                list2.AddRange(objects.OfType<Model.SalesOrder>().Where(x => x.Lines != null).Select(x => new Tuple<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>(x, x.Obsolete_Lines)).ToArray());

                foreach (var e in list2.ToArray())
                {
                    var dirty = false;
                    foreach (var e2 in e.Item2)
                    {
                        if (e2.Account == ManagerServer.Model.Master.AccountKeys.InventoryOnHand || e2.Account == ManagerServer.Model.Master.AccountKeys.InventorySales)
                        {
                            if (!e2.Qty.HasValue)
                            {
                                e2.Qty = 1m;
                                dirty = true;
                            }
                        }
                    }
                    if (dirty) list.Add(e.Item1);
                }
            }
            return list;
        }
    }
}
