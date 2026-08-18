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
        private static async Task<IEnumerable<Model.Object>> Upgrade237(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            using (var tx = objects.BeginTransaction())
            {
                tx.CreateTable<ManagerServer.ApplicationData.Email>();
                foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete57.Email>())
                {
                    tx.InsertOrReplace(new ManagerServer.ApplicationData.Email() { Key = e.Key, Sender = e.From, Recipient = string.Join(";", e.To ?? new string[0]), Subject = e.Subject, Body = e.Body, Timestamp = e.Date.Ticks });
                }
                tx.Commit();
            }

            return null;
        }
    }
}
