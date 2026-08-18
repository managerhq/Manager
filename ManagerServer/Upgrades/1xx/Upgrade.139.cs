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
        private static async Task<IEnumerable<Model.Object>> Upgrade139(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().Where(x => x.Obsolete_Lines == null && x.Lines != null).ToArray())
            {
                var dirty = false;
                var originalLines = new List<ManagerServer.Model.Obsolete.Obsolete76.TransactionLine>();
                foreach (var e2 in e.Lines)
                {
                    if (e2 == null) continue;
                    originalLines.Add(e2.Clone());
                    if (e2.Qty.HasValue)
                    {
                        dirty = true;
                        if (e2.Qty.Value == 0m) e2.Qty = null;
                        else e2.Amount = e2.Amount / e2.Qty.Value;
                    }
                }
                if (dirty)
                {
                    e.Obsolete_Lines = originalLines.ToArray();
                    list.Add(e);
                }
            }

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().Where(x => x.Obsolete_Lines == null && x.Lines != null).ToArray())
            {
                var dirty = false;
                var originalLines = new List<ManagerServer.Model.Obsolete.Obsolete76.TransactionLine>();
                foreach (var e2 in e.Lines)
                {
                    if (e2 == null) continue;
                    originalLines.Add(e2.Clone());
                    if (e2.Qty.HasValue)
                    {
                        dirty = true;
                        if (e2.Qty.Value == 0m) e2.Qty = null;
                        else e2.Amount = e2.Amount / e2.Qty.Value;
                    }
                }
                if (dirty)
                {
                    e.Obsolete_Lines = originalLines.ToArray();
                    list.Add(e);
                }
            }

            foreach (var e in objects.OfType<ManagerServer.Model.ExpenseClaim>().Where(x => x.Obsolete_Lines == null && x.Lines != null).ToArray())
            {
                var dirty = false;
                var originalLines = new List<ManagerServer.Model.Obsolete.Obsolete76.TransactionLine>();
                foreach (var e2 in e.Obsolete_Lines2)
                {
                    if (e2 == null) continue;
                    originalLines.Add(e2.Clone());
                    if (e2.Qty.HasValue)
                    {
                        dirty = true;
                        if (e2.Qty.Value == 0m) e2.Qty = null;
                        else e2.Amount = e2.Amount / e2.Qty.Value;
                    }
                }
                if (dirty)
                {
                    e.Obsolete_Lines = originalLines.ToArray();
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
