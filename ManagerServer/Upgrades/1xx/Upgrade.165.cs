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
        private static async Task<IEnumerable<Model.Object>> Upgrade165(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var tabs = objects.OfType<ManagerServer.Model.Tabs>().SingleOrDefault(x => x.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Tabs)));
            if (tabs == null) return null;
            if (!objects.OfType<ManagerServer.Model.DeliveryNote>().Any()) tabs.DeliveryNotes = false;
            if (!objects.OfType<ManagerServer.Model.GoodsReceipt>().Any()) tabs.GoodsReceipts = false;
            return new[] { tabs };
        }
    }
}
