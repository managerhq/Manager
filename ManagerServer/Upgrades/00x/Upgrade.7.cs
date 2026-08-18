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
        private static async Task<IEnumerable<Model.Object>> Upgrade7(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var contacts = objects.OfType<ManagerServer.Model.Obsolete.Obsolete01.Contact01>().ToDictionary(x => x.Key);
            var receipts = objects.OfType<ManagerServer.Model.Obsolete.Obsolete02.Receipt02>().Where(x => x.Obsolete_From.HasValue && contacts.ContainsKey(x.Obsolete_From.Value)).ToArray();
            var payments = objects.OfType<ManagerServer.Model.Obsolete.Obsolete02.Payment02>().Where(x => x.Obsolete_To.HasValue && contacts.ContainsKey(x.Obsolete_To.Value)).ToArray();
            foreach (var e in receipts)
            {
                e.From = contacts[e.Obsolete_From.Value].Name;
                e.Obsolete_From = null;
                list.Add(e);
            }
            foreach (var e in payments)
            {
                e.To = contacts[e.Obsolete_To.Value].Name;
                e.Obsolete_To = null;
                list.Add(e);
            }
            return list.ToArray();
        }
    }
}
