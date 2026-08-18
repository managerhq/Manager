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
        private static async Task<IEnumerable<Model.Object>> Upgrade155(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>().ToArray())
            {
                e.TaxRate = TaxRate.CustomRate;
                e.Type = TaxRateType.MultipleRates;
                list.Add(e);
            }

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete29.CountryUnitedKingdomVatFlatRateTaxCode29>().ToArray())
            {
                list.Add(new ManagerServer.Model.TaxCode() { Key = e.Key, Name = e.Name, TaxRate = TaxRate.CustomRate, Type = TaxRateType.SingleRate, Obsolete_FlatRate = true, Rate = e.VatRate, Obsolete_FlatRateRate = e.FlatRate });
            }
            return list;
        }
    }
}
