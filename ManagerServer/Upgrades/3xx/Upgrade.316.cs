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
        private static async Task<IEnumerable<Model.Object>> Upgrade316(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Customer>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_BusinessIdentifier)))
            {
                if (!string.IsNullOrWhiteSpace(e.BillingAddress)) e.BillingAddress = e.BillingAddress.TrimEnd() + '\n';
                e.BillingAddress += e.Obsolete_BusinessIdentifier;
                list.Add(e);
            }
            return list;
        }
    }
}
