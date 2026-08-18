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
        private static async Task<IEnumerable<Model.Object>> Upgrade354(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            if (objects.OfType<ManagerServer.Model.SalesOrder>().Any())
            {
                var columns = new CustomColumns.CustomColumn[] {
                    new CustomColumns.CustomColumn() { Key = new Guid("0dbbd25c-0f41-47f1-a392-c1370ccc9672"), Enabled = true },
                    new CustomColumns.CustomColumn() { Key = new Guid("916db179-c8ef-47fe-9ab3-d08e94513cd5"), Enabled = true },
                    new CustomColumns.CustomColumn() { Key = new Guid("be67f579-9b1f-4bca-991b-4a5abcb820ea"), Enabled = true },
                    new CustomColumns.CustomColumn() { Key = new Guid("72f722b5-6f50-45d3-ad28-c9a523713333"), Enabled = true },
                    new CustomColumns.CustomColumn() { Key = new Guid("12104b52-ecce-45b7-8087-e8783d03d485"), Enabled = true }
                }.ToList();

                if (objects.OfType<ManagerServer.Model.SalesOrder>().Any(x => x.Obsolete_TrackQuantityToDeliver))
                {
                    columns.Add(new CustomColumns.CustomColumn() { Key = new Guid("9788c257-1485-4d93-bda2-29eed6290295"), Enabled = true });
                    columns.Add(new CustomColumns.CustomColumn() { Key = new Guid("ba9b603f-4720-4ea4-a803-574270a75a27"), Enabled = true });
                }

                columns.Add(new CustomColumns.CustomColumn() { Key = new Guid("10b75b0a-0c48-421d-aa0a-0355ee3a9947"), Enabled = true });
                columns.Add(new CustomColumns.CustomColumn() { Key = new Guid("8364f509-1f09-4077-b038-6889f508afcd"), Enabled = true });

                list.Add(new ManagerServer.Model.CustomColumns()
                {
                    Key = new Guid("d4912688-adef-40fd-8cec-d67aab0f967a"),
                    Columns = columns.ToArray()
                });
            }
            return list;
        }
    }
}
