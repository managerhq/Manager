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
        private static async Task<IEnumerable<Model.Object>> Upgrade353(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            if (objects.OfType<ManagerServer.Model.BankOrCashAccount>().Any(x => x.CanHavePendingTransactions))
            {
                list.Add(new ManagerServer.Model.CustomColumns()
                {
                    Key = new Guid("efe0e273-fa0b-4824-a9d2-7225383d49de"),
                    Columns = new CustomColumns.CustomColumn[]
                    {
                        new CustomColumns.CustomColumn() { Key = new Guid("1276fd60-908a-489b-a7bd-c026987db9eb"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("5bc4536a-0504-47f6-8015-afde8a5456d3"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("677f58de-7ec6-4ef2-ad43-420cf63fbb1f"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("542f2b2c-df24-43b8-86eb-5d4df087587f"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("624e2a46-7a7c-48a9-b947-fcf4087aaf80"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("4c8470c9-91db-4170-acd4-3177bb21d590"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("577ebfc7-f35a-4b02-8377-f94ba3dfe13a"), Enabled = true }
                    }
                });
            }
            return list;
        }
    }
}
