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
        private static async Task<IEnumerable<Model.Object>> Upgrade134(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            if (!objects.OfType<Model.Obsolete.Obsolete46.CustomTaxReport46>().Any())
            {
                foreach (var e in objects.OfType<Model.Obsolete.Obsolete21.CountryAustraliaGstCalculationWorksheet21>().ToArray())
                {
                    list.Add(new Model.Obsolete.Obsolete46.CustomTaxReport46() { Key = e.Key, AccountingBasis = e.AccountingBasis, Description = e.Description, From = e.From, To = e.To ?? DateTime.Today, Type = new Guid("994a34c3-3692-4724-98bd-98b34615b317") });
                }
                foreach (var e in objects.OfType<Model.Obsolete.Obsolete21.CountryUnitedKingdomVatReturn21>().ToArray())
                {
                    list.Add(new Model.Obsolete.Obsolete46.CustomTaxReport46() { Key = e.Key, AccountingBasis = e.AccountingBasis, Description = e.Description, From = e.From, To = e.To ?? DateTime.Today, Type = new Guid("153a8219-2b74-4e48-87fa-7fbd23d50534") });
                }
                foreach (var e in objects.OfType<Model.Obsolete.Obsolete21.CountryNetherlandsVatReturn21>().ToArray())
                {
                    list.Add(new Model.Obsolete.Obsolete46.CustomTaxReport46() { Key = e.Key, AccountingBasis = e.AccountingBasis, Description = e.Description, From = e.From, To = e.To ?? DateTime.Today, Type = new Guid("e4d74007-5649-45b3-a7f5-01c9ad0ff5ba") });
                }
                return list;
            }
            return list;
        }
    }
}
