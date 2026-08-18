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
        private static async Task<IEnumerable<Model.Object>> Upgrade55(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete33.Receipt33>().Where(x => x.Obsolete_India_TaxDeductedAtSource != 0m).ToArray())
            {
                var list2 = new List<Model.Obsolete.Obsolete76.TransactionLine>(e.Lines);
                list2.Add(new Model.Obsolete.Obsolete76.TransactionLine() { Account = new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71"), Amount = e.Obsolete_India_TaxDeductedAtSource * -1 });
                e.Obsolete_India_TaxDeductedAtSource = 0m;
                e.Lines = list2.ToArray();
                list.Add(e);
            }
            return list;
        }
    }
}
