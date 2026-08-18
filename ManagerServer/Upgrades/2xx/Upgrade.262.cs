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
        private static async Task<IEnumerable<Model.Object>> Upgrade262(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.DeliveryNote>())
            {
                if (e.Lines == null && e.Obsolete_Lines != null)
                {
                    e.Lines = e.Obsolete_Lines.Where(x => x != null).Select(x => new DeliveryNote.Line()
                    {
                        Item = x.Item,
                        LineDescription = x.Description,
                        CustomFields = x.CustomFields,
                        Qty = x.Qty
                    }).ToArray();
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
