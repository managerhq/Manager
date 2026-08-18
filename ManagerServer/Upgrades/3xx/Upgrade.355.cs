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
        private static async Task<IEnumerable<Model.Object>> Upgrade355(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            if (objects.OfType<ManagerServer.Model.PurchaseOrder>().Any())
            {
                var columns = new CustomColumns.CustomColumn[] {
                    new CustomColumns.CustomColumn() { Key = new Guid("1e354a36-3dc6-4fca-9ba4-53ec4c71c846"), Enabled = true },
                    new CustomColumns.CustomColumn() { Key = new Guid("4982128c-5ba1-4f59-a29e-1d08b157dadf"), Enabled = true },
                    new CustomColumns.CustomColumn() { Key = new Guid("895a4366-1c0e-4b1c-90d4-b1971c8dce0d"), Enabled = true },
                    new CustomColumns.CustomColumn() { Key = new Guid("77624b46-6bca-454d-aa03-87a271645787"), Enabled = true },
                    new CustomColumns.CustomColumn() { Key = new Guid("b8c688bb-8bd7-4e3c-b24b-91ec5a004b74"), Enabled = true }
                }.ToList();

                if (objects.OfType<ManagerServer.Model.PurchaseOrder>().Any(x => x.Obsolete_TrackQuantityToReceive))
                {
                    columns.Add(new CustomColumns.CustomColumn() { Key = new Guid("7f127b7b-21a0-42a0-aef2-e1f809d94f34"), Enabled = true });
                    columns.Add(new CustomColumns.CustomColumn() { Key = new Guid("28706539-819e-4c48-86c5-c4f4171ca358"), Enabled = true });
                }

                columns.Add(new CustomColumns.CustomColumn() { Key = new Guid("35d42d77-d8de-4c67-9359-a9068d2f8bcb"), Enabled = true });
                columns.Add(new CustomColumns.CustomColumn() { Key = new Guid("944d34bf-f4d6-4534-a7d2-3928223268a1"), Enabled = true });

                list.Add(new ManagerServer.Model.CustomColumns()
                {
                    Key = new Guid("446ddae4-2f0a-4a55-aa28-0502b393d360"),
                    Columns = columns.ToArray()
                });
            }
            return list;
        }
    }
}
