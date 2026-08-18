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
        private static async Task<IEnumerable<Model.Object>> Upgrade417(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            using (var tx = objects.BeginTransaction())
            {
                var o1 = objects.Single<CustomerStatementsTransactions>();
                o1.CustomTheme = o1.CustomThemeId.HasValue;
                tx.InsertOrReplace2(o1);

                var o2 = objects.Single<CustomerStatementsUnpaidInvoices>();
                o2.CustomTheme = o2.CustomThemeId.HasValue;
                tx.InsertOrReplace2(o2);

                var o3 = objects.Single<SupplierStatementsTransactions>();
                o3.CustomTheme = o3.CustomThemeId.HasValue;
                tx.InsertOrReplace2(o3);

                var o4 = objects.Single<SupplierStatementsUnpaidInvoices>();
                o4.CustomTheme = o4.CustomThemeId.HasValue;
                tx.InsertOrReplace2(o4);

                foreach (var e in objects.OfType<CustomButton>())
                {
                    if (e.Endpoint != null && e.Endpoint.Contains("luboshasko.com"))
                    {
                        tx.Delete2(e.Key);
                    }
                }

                foreach (var e in objects.OfType<CustomButton>())
                {
                    if (e.Endpoint != null && e.Endpoint.Contains("aussiebankfeeds.com"))
                    {
                        if (e.Key != new Guid("c4ea5457-4934-4218-a3b8-f7b7d1ab1f2b"))
                        {
                            tx.Delete2(e.Key);
                        }
                    }
                }

                tx.Commit();
            }

            return null;
        }
    }
}