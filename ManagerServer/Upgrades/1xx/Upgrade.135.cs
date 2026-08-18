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
        private static async Task<IEnumerable<Model.Object>> Upgrade135(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete22.BankAccount22>().Where(x => x.Obsolete_BankReconciliationDate2.HasValue).ToArray())
            {
                list.Add(new Model.Obsolete.Obsolete41.BankReconciliationStatement41() { Key = Guid.CreateVersion7(), Date = e.Obsolete_BankReconciliationDate2.Value, BankAccount = e.Key });
            }
            return list;
        }
    }
}
