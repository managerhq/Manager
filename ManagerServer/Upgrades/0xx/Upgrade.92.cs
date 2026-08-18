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
        private static async Task<IEnumerable<Model.Object>> Upgrade92(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete26.SalesInvoiceTemplate26>().ToArray())
            {
                if (!string.IsNullOrWhiteSpace(e.TermsAndPaymentAdvice)) e.TermsAndPaymentAdvice = e.TermsAndPaymentAdvice.Replace("\r\n\r\n", "\r\n");
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete36.SalesInvoiceDefaultNotes36>().ToArray())
            {
                if (!string.IsNullOrWhiteSpace(e.Value)) e.Value = e.Value.Replace("\r\n\r\n", "\r\n");
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.SalesInvoice>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes)).ToArray())
            {
                e.Obsolete_Notes = e.Obsolete_Notes.Replace("\r\n\r\n", "\r\n");
                list.Add(e);
            }
            return list;
        }
    }
}
