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
        private static async Task<IEnumerable<Model.Object>> Upgrade298(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var emailKey = typeof(ManagerServer.Model.Obsolete.Obsolete70.Email).GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>().Value;

            using (var tx = objects.BeginTransaction())
            {
                tx.CreateTable<ManagerServer.ApplicationData.Change>();
                tx.Commit();
            }
            var keys = new HashSet<Guid>(objects.Query<Guid>("SELECT Key FROM Emails"));
            using (var tx = objects.BeginTransaction())
            {
                foreach (var e in objects.Table<ManagerServer.ApplicationData.Change>().Where(x => x.ContentTypeAfter == emailKey))
                {
                    if (keys.Contains(e.Key)) continue;

                    using (var ms = new System.IO.MemoryStream(e.ContentAfter))
                    {
                        var email = ProtoBuf.Serializer.Deserialize<ManagerServer.Model.Obsolete.Obsolete70.Email>(ms);

                        tx.InsertOrReplace(new ManagerServer.ApplicationData.Email()
                        {
                            Key = e.Key,
                            Body = email.Body,
                            Recipient = email.Recipient,
                            Sender = email.Sender,
                            Subject = email.Subject,
                            Timestamp = e.Timestamp,
                            Object = e.Object,
                            User = e.User,
                            Filename = email.Filename
                        });
                    }

                    tx.Delete<ManagerServer.ApplicationData.Change>(e.Key);
                }
                tx.Commit();
            }
            return null;
        }
    }
}
