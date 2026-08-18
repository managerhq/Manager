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
        private static async Task<IEnumerable<Model.Object>> Upgrade83(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var quotesWithInternalNotes = objects.OfType<Model.SalesQuote>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_InternalNotes)).ToArray();
            if (quotesWithInternalNotes.Any())
            {
                var key = Guid.CreateVersion7();
                list.Add(new Model.CustomField() { Key = key, Name = "Internal information", Obsolete_FormType = Model.Object.GetGuidByType(typeof(Model.SalesQuote)) });
                foreach (var e in quotesWithInternalNotes)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    e.CustomFields.Add(key, e.Obsolete_InternalNotes);
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
