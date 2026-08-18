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
        private static async Task<IEnumerable<Model.Object>> Upgrade356(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            if (objects.OfType<ManagerServer.Model.BillableTime>().Any() || objects.OfType<ManagerServer.Model.BillableExpenses>().Any(x => x.Enabled))
            {
                list.Add(new ManagerServer.Model.CustomColumns()
                {
                    Key = new Guid("356ecb8d-36c6-4be5-b7a5-862050af55fb"),
                    Columns = new CustomColumns.CustomColumn[] {
                        new CustomColumns.CustomColumn() { Key = new Guid("8781dba6-3fb4-4158-8410-da6a1fffb5aa"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("24bcc2c0-c010-40d7-9970-08b2d26b0a50"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("5bb46491-8e3c-47be-b0e7-b08154aefd25"), Enabled = true },
                        new CustomColumns.CustomColumn() { Key = new Guid("f5a98b00-0fca-4463-9013-c0a5fd4c220a"), Enabled = true }
                    }
                });
            }
            return list;
        }
    }
}
