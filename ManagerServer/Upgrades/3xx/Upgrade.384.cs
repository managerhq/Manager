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
        private static async Task<IEnumerable<Model.Object>> Upgrade384(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var landingCostItem = new Guid("3458c24f-2a5f-4dcf-9de7-7340b1463d9c");

            var list = new List<ManagerServer.Model.Object>();

            foreach (var e in objects.OfType<PurchaseInvoice>())
            {
                if (e.Lines == null) continue;
                if (!e.Lines.Any(x => x.Item == landingCostItem)) continue;

                var landingCostLines = new List<PurchaseInvoice.LandedCostLine>();
                var lines = new List<PurchaseInvoice.Line>();

                foreach (var e2 in e.Lines.ToArray())
                {
                    if (e2.Item == landingCostItem)
                    {
                        var lineTotal = e2.GetLineTotal(e);

                        if (lineTotal != 0m && e2.GetDiscountPercentage(e).HasValue && e2.GetDiscountPercentage(e).Value != 0m)
                        {
                            lineTotal = lineTotal / 100m * (100m - e2.GetDiscountPercentage(e).Value);
                        }

                        if (e2.GetDiscountAmount(e).HasValue)
                        {
                            lineTotal = lineTotal - e2.GetDiscountAmount(e).Value;
                        }

                        landingCostLines.Add(new PurchaseInvoice.LandedCostLine()
                        {
                            LandedCostAmount = lineTotal,
                            LandedCostDescription = e2.LineDescription,
                            LandedCostTaxCode = e2.TaxCode,
                            Obsolete_Line = e2
                        });
                    }
                    else
                    {
                        lines.Add(e2);
                    }
                }

                e.Lines = lines.ToArray();
                e.FreightIn = true;
                e.LandedCostLines = landingCostLines.ToArray();
                list.Add(e);
            }

            return list;
        }
    }
}
