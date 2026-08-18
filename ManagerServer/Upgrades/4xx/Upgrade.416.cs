using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Model;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade416(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var any = false;
            using (var tx = objects.BeginTransaction())
            {
                foreach (var e in objects.OfType<Model.Obsolete.Obsolete90.BankFeedProvider>().ToArray())
                {
                    if (e.Endpoint != null && e.Endpoint.Contains("basiq.manager.io"))
                    {
                        any = true;
                        tx.InsertOrReplace2(new CustomButton()
                        {
                            Key = e.Key,
                            Name = "Aussie Bank Feeds",
                            Endpoint = "aussiebankfeeds.com",
                            Placement = "bank-and-cash-accounts"
                        });
                    }
                }

                if (any)
                {
                    foreach (var e in objects.OfType<Model.Payment>().Where(x => !string.IsNullOrWhiteSpace(x.FdxTransactionId)).ToArray())
                    {
                        e.CustomFields2 ??= new CustomFields();
                        e.CustomFields2.Strings ??= new Dictionary<Guid, string>();
                        e.CustomFields2.Strings[new Guid("7b3e9d2c-4a8f-4b1e-9c5a-1f6e8d4a2b9c")] = e.FdxTransactionId;
                        tx.InsertOrReplace2(e);
                    }
                    foreach (var e in objects.OfType<Model.Receipt>().Where(x => !string.IsNullOrWhiteSpace(x.FdxTransactionId)).ToArray())
                    {
                        e.CustomFields2 ??= new CustomFields();
                        e.CustomFields2.Strings ??= new Dictionary<Guid, string>();
                        e.CustomFields2.Strings[new Guid("7b3e9d2c-4a8f-4b1e-9c5a-1f6e8d4a2b9c")] = e.FdxTransactionId;
                        tx.InsertOrReplace2(e);
                    }
                    foreach (var e in objects.OfType<Model.BankOrCashAccount>().Where(x => x.Obsolete_BankFeedProviderConfiguration != null).ToArray())
                    {
                        using (var ms = new MemoryStream(e.Obsolete_BankFeedProviderConfiguration))
                        {
                            var o = ProtoBuf.Serializer.Deserialize<BankFeedProviderConfig>(ms);
                            e.CustomFields2 ??= new CustomFields();
                            e.CustomFields2.Strings ??= new Dictionary<Guid, string>();
                            e.CustomFields2.Strings[new Guid("7b3e9d2c-4a8f-4b1e-9c5a-1f6e8d4a2b9c")] = o.AccountId;
                            e.CustomFields2.Dates ??= new Dictionary<Guid, DateTime?>();
                            e.CustomFields2.Dates[new Guid("2f8a6c1b-9d3e-4f7a-8b2c-5e1d4a9f6b3e")] = DateTime.UtcNow;
                            tx.InsertOrReplace2(e);
                        }
                    }
                }

                tx.Commit();
            }

            return null;
        }

        [ProtoContract]
        public sealed class BankFeedProviderConfig
        {
            [ProtoMember(4)] public string AccountId;
        }
    }
}