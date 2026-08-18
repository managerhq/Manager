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
        private static async Task<IEnumerable<Model.Object>> Upgrade95(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.SalesInvoice>().Where(x => x.Lines != null && x.Lines.Any(y => y.Account == Model.Master.AccountKeys.BillableTimeUnbilled)).ToArray())
            {
                foreach (var e2 in e.Lines.Where(x => x.Account == Model.Master.AccountKeys.BillableTimeUnbilled)) e2.Account = Model.Master.AccountKeys.BillableTimeInvoiced;
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.CreditNote>().Where(x => x.Obsolete_Lines != null && x.Obsolete_Lines.Any(y => y.Account == Model.Master.AccountKeys.BillableTimeUnbilled)).ToArray())
            {
                foreach (var e2 in e.Obsolete_Lines.Where(x => x.Account == Model.Master.AccountKeys.BillableTimeUnbilled)) e2.Account = Model.Master.AccountKeys.BillableTimeInvoiced;
                list.Add(e);
            }
            return list;
        }
    }
}
