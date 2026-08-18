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
        private static async Task<IEnumerable<Model.Object>> Upgrade407(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            /*
            if (ApplicationData.Instance.S3Blobs != null)
            {
                var pageCount = objects.ExecuteScalar<long>("PRAGMA page_count");
                var pageFreeList = objects.ExecuteScalar<long>("PRAGMA freelist_count");
                var treshhold = pageCount / 2;

                if (pageFreeList > treshhold)
                {
                    objects.Pragma("auto_vacuum = INCREMENTAL");
                    objects.Vacuum();
                }
            }
            */
            return null;
        }
    }
}
