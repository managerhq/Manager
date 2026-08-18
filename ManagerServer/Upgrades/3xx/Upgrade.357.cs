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
        private static async Task<IEnumerable<Model.Object>> Upgrade357(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            if (objects.OfType<ManagerServer.Model.InventoryItem>().Any(x => !string.IsNullOrWhiteSpace(x.ItemCode)))
            {
                list.Add(new ManagerServer.Model.CustomColumns()
                {
                    Key = new Guid("80bdd6ea-c139-43a1-9de4-4f281c38970f"),
                    Columns = new CustomColumns.CustomColumn[] {
                        new CustomColumns.CustomColumn() { Key = new Guid("72c52313-6054-4682-ad12-cc4d5676e5b8"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("63d7d695-75d2-4f7a-ab63-f38696dca522"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("762ceb3b-9288-4392-b5f3-fa13d1a42b76"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("de062f5e-3691-4fe3-9361-5a9fa4dade1d"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("4003bafc-5587-4a86-a9fd-0b3b679fac09"), Enabled = true }
                    }
                });
            }
            return list;
        }
    }
}
