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
        private static async Task<IEnumerable<Model.Object>> Upgrade49(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete07.CashReceipt07>().ToArray())
            {
                var o = new Model.Obsolete.Obsolete33.Receipt33();
                o.Date = e.Date;
                o.DebitAccount = e.CashAccount;
                o.Description = e.Description;
                o.Obsolete_India_TaxDeductedAtSource = e.India_TaxDeductedAtSource;
                o.Key = e.Key;
                o.Lines = e.Lines;
                o.Payer = e.Payer;
                o.Reference = e.Reference;
                list.Add(o);
            }
            return list;
        }
    }
}
