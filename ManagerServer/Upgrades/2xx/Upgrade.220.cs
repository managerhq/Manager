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
        private static async Task<IEnumerable<Model.Object>> Upgrade220(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete50.FixedAssetDepreciation50>().GroupBy(x => x.Date))
            {
                var depreciationEntry = new ManagerServer.Model.DepreciationEntry() { Date = e.Key, Key = Guid.CreateVersion7() };
                depreciationEntry.Lines = e.Select(x => new ManagerServer.Model.DepreciationEntry.Line() { FixedAsset = x.FixedAsset, Amount = x.Amount, Division = x.TrackingCode }).ToArray();
                list.Add(depreciationEntry);
            }
            return list;
        }
    }
}
