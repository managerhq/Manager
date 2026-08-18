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
        private static async Task<IEnumerable<Model.Object>> Upgrade67(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.SalesInvoice>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Obsolete_Lines)
                {
                    if (e2.Account.HasValue && e2.Item.HasValue && e2.Account == Model.Master.AccountKeys.InventoryOnHand)
                    {
                        e2.Obsolete_InventoryItem = e2.Item;
                        e2.Item = null;
                        e2.Account = Model.Master.AccountKeys.InventorySales;
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            foreach (var e in objects.OfType<Model.SalesQuote>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Obsolete_Lines)
                {
                    if (e2.Account.HasValue && e2.Item.HasValue && e2.Account == Model.Master.AccountKeys.InventoryOnHand)
                    {
                        e2.Obsolete_InventoryItem = e2.Item;
                        e2.Item = null;
                        e2.Account = Model.Master.AccountKeys.InventorySales;
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            foreach (var e in objects.OfType<Model.CreditNote>().Where(x => x.Obsolete_Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Obsolete_Lines)
                {
                    if (e2.Account.HasValue && e2.Item.HasValue && e2.Account == Model.Master.AccountKeys.InventoryOnHand)
                    {
                        e2.Obsolete_InventoryItem = e2.Item;
                        e2.Item = null;
                        e2.Account = Model.Master.AccountKeys.InventorySales;
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            foreach (var e in objects.OfType<Model.PurchaseInvoice>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Obsolete_Lines)
                {
                    if (e2.Account.HasValue && e2.Item.HasValue && e2.Account == Model.Master.AccountKeys.InventoryOnHand)
                    {
                        e2.Obsolete_InventoryItem = e2.Item;
                        e2.Item = null;
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            foreach (var e in objects.OfType<Model.PurchaseOrder>().Where(x => x.Obsolete_Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Obsolete_Lines)
                {
                    if (e2.Account.HasValue && e2.Item.HasValue && e2.Account == Model.Master.AccountKeys.InventoryOnHand)
                    {
                        e2.Obsolete_InventoryItem = e2.Item;
                        e2.Item = null;
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            return list;
        }
    }
}
