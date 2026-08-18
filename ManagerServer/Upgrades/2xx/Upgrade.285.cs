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
        private static async Task<IEnumerable<Model.Object>> Upgrade285(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var deliveryNotes = new HashSet<Guid>(objects.OfType<ManagerServer.Model.DeliveryNote>().Where(x => x.Lines != null).SelectMany(x => x.Lines).Where(x => x.Item.HasValue).Select(x => x.Item.Value).Distinct());
            var goodsReceipts = new HashSet<Guid>(objects.OfType<ManagerServer.Model.GoodsReceipt>().Where(x => x.Lines != null).SelectMany(x => x.Lines).Where(x => x.Item.HasValue).Select(x => x.Item.Value).Distinct());
            foreach (var e in objects.OfType<ManagerServer.Model.InventoryItem>())
            {
                if (deliveryNotes.Contains(e.Key) || goodsReceipts.Contains(e.Key))
                {
                    if (deliveryNotes.Contains(e.Key)) e.Obsolete_TrackQuantityToDeliver = true;
                    if (goodsReceipts.Contains(e.Key)) e.Obsolete_TrackQuantityToReceive = true;
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
