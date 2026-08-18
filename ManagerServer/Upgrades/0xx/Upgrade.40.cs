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
        private static async Task<IEnumerable<Model.Object>> Upgrade40(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var inPocketIncome = objects.OfType<Model.Obsolete.Obsolete04.InPocketIncome04>().ToArray();
            foreach (var e in inPocketIncome)
            {
                var o = new Model.Obsolete.Obsolete33.Receipt33();
                o.DebitAccount = e.To;
                o.Date = e.Date;
                o.Description = e.Notes;
                o.Key = e.Key;
                o.Lines = e.Lines;
                o.Payer = e.From;
                list.Add(o);
            }
            return list;
        }
    }
}
