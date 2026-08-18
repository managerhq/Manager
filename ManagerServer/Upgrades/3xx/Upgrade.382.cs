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
        private static async Task<IEnumerable<Model.Object>> Upgrade382(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            /*
            var list = new List<Manager.Model.Object>();

            var trackQuantityToDeliver = new HashSet<Guid>(objects.OfType<InventoryItem>().Where(x => x.Obsolete_TrackQuantityToDeliver).Select(x => x.Key));
            foreach (var e in objects.OfType<SalesInvoice>())
            {
                if (e.Lines == null) continue;
                if (e.Lines.Any(x => x.Item.HasValue && trackQuantityToDeliver.Contains(x.Item.Value)))
                {
                    foreach (var e2 in e.Lines.Where(x => x.Item.HasValue))
                    {
                        if (!trackQuantityToDeliver.Contains(e2.Item.Value)) e2.QtyDelivered = e2.Qty ?? 0m;
                    }

                    e.HasQtyDelivered = true;
                    e.CustomFields2 ??= new CustomFields();
                    e.CustomFields2.Booleans ??= new Dictionary<Guid, bool>();
                    e.CustomFields2.Booleans.Add(new Guid("706b558e-0086-415a-89c4-b08498364002"), true);
                    list.Add(e);
                }
            }

            var trackQuantityToReceive = new HashSet<Guid>(objects.OfType<InventoryItem>().Where(x => x.Obsolete_TrackQuantityToReceive).Select(x => x.Key));
            foreach (var e in objects.OfType<PurchaseInvoice>())
            {
                if (e.Lines == null) continue;
                if (e.Lines.Any(x => x.Item.HasValue && trackQuantityToReceive.Contains(x.Item.Value)))
                {
                    foreach (var e2 in e.Lines.Where(x => x.Item.HasValue))
                    {
                        if (!trackQuantityToReceive.Contains(e2.Item.Value)) e2.QtyReceived = e2.Qty ?? 0m;
                    }

                    e.HasQtyReceived = true;
                    e.CustomFields2 ??= new CustomFields();
                    e.CustomFields2.Booleans ??= new Dictionary<Guid, bool>();
                    e.CustomFields2.Booleans.Add(new Guid("706b558e-0086-415a-89c4-b08498364002"), true);
                    list.Add(e);
                }
            }

            return list;
            */
            return null;
        }
    }
}
