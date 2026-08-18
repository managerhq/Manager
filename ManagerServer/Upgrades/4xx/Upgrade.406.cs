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
        private static async Task<IEnumerable<Model.Object>> Upgrade406(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            /*
            if (ApplicationData.Instance.S3Blobs != null)
            {
                var attachments = objects.OfType<ManagerServer.Model.Attachment>().Where(x => x.Sha256 == null).ToArray();
                if (attachments.Length > 0)
                {
                    for (int i = 0; i < attachments.Length; i++)
                    {
                        progress.Report(new Tuple<int, int>(i, attachments.Length));

                        var attachment = attachments[i];

                        var blob = objects.Find<ManagerServer.ApplicationData.Blob>(attachment.Key)?.Content;
                        if (blob == null) continue;

                        using (var ms = new MemoryStream(blob))
                        {
                            attachment.Sha256 = await ApplicationData.Instance.Storage.WriteAsync(ms.ToArray());
                            await ApplicationData.Instance.S3Blobs.WriteAsync(ms);
                        }

                        using (var tx = objects.BeginTransaction())
                        {
                            tx.InsertOrReplace2(attachment);
                            tx.Delete<ManagerServer.ApplicationData.Blob>(attachment.Key);
                            tx.Commit();
                        }
                    }
                }
            }
            */
            return null;
        }
    }
}
