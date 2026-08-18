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
        private static async Task<IEnumerable<Model.Object>> Upgrade324(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>().ToArray())
            {
                if (e.CustomFields == null) continue;

                if (e.CustomFields.TryGetValue(new Guid("0c2354c3-9a05-42d3-b6df-f4c2ef7a519b"), out string value))
                {
                    if (value == "VAT Exempt")
                    {
                        e.ReportingCategory = new Guid("20feecda-2c92-4553-bd36-9d1a9dba4c64");
                    }
                    else if (value == "VAT 5%")
                    {
                        e.ReportingCategory = new Guid("e111c2e4-6699-4fe7-8663-eef32894a550");
                        e.TaxAmountReportingCategory = new Guid("078cc06c-77e9-47b1-84c1-3a2551e43621");
                    }
                    else if (value == "VAT 0%")
                    {
                        e.ReportingCategory = new Guid("390d9a54-7f3b-4b7c-956b-25fccde4cfc1");
                    }
                    else if (value == "VAT 0% (EU)")
                    {
                        e.ReportingCategory = new Guid("b8a3b07c-caa5-4091-85b7-12308f5b0299");
                    }
                    else if (value == "VAT 20%")
                    {
                        e.ReportingCategory = new Guid("275ffc71-b52b-4299-87eb-ac667873e0ef");
                        e.TaxAmountReportingCategory = new Guid("9e805e14-7433-4096-80b9-01b03291a1bb");
                    }

                    list.Add(e);
                }
            }
            return list;
        }
    }
}
