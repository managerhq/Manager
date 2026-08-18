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
        private static async Task<IEnumerable<Model.Object>> Upgrade323(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>().ToArray())
            {
                if (e.CustomFields == null) continue;

                if (e.CustomFields.TryGetValue(new Guid("3357a960-1488-490b-b6cb-378c8f9b4295"), out string value))
                {
                    if (value == "GST 0%")
                    {
                        e.ReportingCategory = new Guid("c24a8d13-6c4d-4a09-9fe1-f9ad79854def");
                    }
                    else if (value == "GST Exempt")
                    {
                        e.ReportingCategory = new Guid("c24a8d13-6c4d-4a09-9fe1-f9ad79854def");
                    }
                    else if (value == "GST Adjustment")
                    {
                        e.TaxAmountReportingCategory = new Guid("576ea6d8-b34c-4e1c-b4b6-a7d7d8d85607");
                    }
                    else if (value == "GST 15%")
                    {
                        e.ReportingCategory = new Guid("c5d2b499-a491-4dfb-adf5-79eea3a1cc6b");
                        e.TaxAmountReportingCategory = new Guid("75556a60-da93-489f-aa35-edc5f6ccebd8");
                    }

                    list.Add(e);
                }
            }
            return list;
        }
    }
}
