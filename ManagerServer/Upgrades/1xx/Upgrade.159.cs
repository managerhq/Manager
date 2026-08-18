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
        private static async Task<IEnumerable<Model.Object>> Upgrade159(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.WithholdingTaxType == WithholdingTaxType.Amount && x.WithholdingTaxAmount != 0m).ToArray())
            {
                e.WithholdingTax = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.WithholdingTaxType == WithholdingTaxType.Rate && x.WithholdingTaxPercentage != 0m).ToArray())
            {
                e.WithholdingTax = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.CreditNote>().Where(x => x.WithholdingTaxType == WithholdingTaxType.Amount && x.WithholdingTaxAmount != 0m).ToArray())
            {
                e.WithholdingTax = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.CreditNote>().Where(x => x.WithholdingTaxType == WithholdingTaxType.Rate && x.WithholdingTaxRate != 0m).ToArray())
            {
                e.WithholdingTax = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.RoundingMethod != RoundingMethod.None).ToArray())
            {
                e.Rounding = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.SalesQuote>().Where(x => x.RoundingMethod != RoundingMethod.None).ToArray())
            {
                e.Rounding = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.RecurringSalesInvoice>().Where(x => x.RoundingMethod != RoundingMethod.None).ToArray())
            {
                e.Rounding = true;
                list.Add(e);
            }
            return list;
        }
    }
}
