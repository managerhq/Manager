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
        private static async Task<IEnumerable<Model.Object>> Upgrade221(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete51.IntangibleAssetAmortization51>().GroupBy(x => x.Date))
            {
                var amortizationEntry = new ManagerServer.Model.AmortizationEntry() { Date = e.Key, Key = Guid.CreateVersion7() };
                amortizationEntry.Lines = e.Select(x => new ManagerServer.Model.AmortizationEntry.Line() { IntangibleAsset = x.IntangibleAsset, Amount = x.Amount, Division = x.TrackingCode }).ToArray();
                list.Add(amortizationEntry);
            }
            return list;
        }
    }
}
