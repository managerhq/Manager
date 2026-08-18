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
        private static async Task<IEnumerable<Model.Object>> Upgrade379(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var key = new Guid("38cf4712-6e95-4ce1-b53a-bff03edad273");

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete81.BusinessLogo>())
            {
                if (e.Content == null) continue;
                if (e.Content.Length == 0) continue;

                using (var tx = objects.BeginTransaction())
                {
                    tx.CreateTable<ManagerServer.ApplicationData.Image>();
                    tx.InsertOrReplace(new ManagerServer.ApplicationData.Image()
                    {
                        Key = key,
                        Content = e.Content,
                        ContentType = e.ContentType,
                        Timestamp = e.Timestamp
                    });
                    tx.Commit();
                }
            }

            return null;
        }
    }
}
