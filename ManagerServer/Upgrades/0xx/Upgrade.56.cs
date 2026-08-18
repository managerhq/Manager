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
        private static async Task<IEnumerable<Model.Object>> Upgrade56(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.Customer>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Philippines_TIN_Number) || !string.IsNullOrWhiteSpace(x.Obsolete_SouthAfrica_VAT_Number)).ToArray())
            {
                if (!string.IsNullOrWhiteSpace(e.Obsolete_Philippines_TIN_Number))
                {
                    e.Obsolete_BusinessIdentifier = "TIN " + e.Obsolete_Philippines_TIN_Number;
                    e.Obsolete_Philippines_TIN_Number = null;
                }
                if (!string.IsNullOrWhiteSpace(e.Obsolete_SouthAfrica_VAT_Number))
                {
                    e.Obsolete_BusinessIdentifier = "VAT #" + e.Obsolete_SouthAfrica_VAT_Number;
                    e.Obsolete_SouthAfrica_VAT_Number = null;
                }
                list.Add(e);
            }
            return list;
        }
    }
}
