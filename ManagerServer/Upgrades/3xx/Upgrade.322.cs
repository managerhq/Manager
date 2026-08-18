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
        private static async Task<IEnumerable<Model.Object>> Upgrade322(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>().ToArray())
            {
                if (e.CustomFields == null) continue;

                if (e.CustomFields.TryGetValue(new Guid("6f95e5e5-50e8-4bbb-a750-2550be8cc47c"), out string value))
                {
                    if (value == "GST Free Import Export")
                    {
                        e.ReportingCategory = new Guid("2108b786-8999-473a-97b2-30b0214be867");
                    }
                    else if (value == "GST Free")
                    {
                        e.ReportingCategory = new Guid("529c8964-4cc6-46d3-a4f9-02af1eb4ea85");
                    }
                    else if (value == "GST 10% (CAPEX)")
                    {
                        e.ReportingCategory = new Guid("0b667cb2-173f-43f7-86da-7023622b51fc");
                        e.TaxAmountReportingCategory = new Guid("6b8ee065-60a5-4820-a896-1ceb681f53fe");
                    }
                    else if (value == "GST 10% (Deferred)")
                    {
                        e.ReportingCategory = new Guid("32d6a783-3c55-4a65-92dc-2041e546951a");
                        e.TaxAmountReportingCategory = new Guid("4e4b5344-49b5-494b-9a73-86c492d6d399");
                        e.TaxAmountReversedReportingCategory = new Guid("3671a124-a6bc-41c4-b0a4-59ed4b559daf");
                    }
                    else if (value == "GST 10%")
                    {
                        e.ReportingCategory = new Guid("32d6a783-3c55-4a65-92dc-2041e546951a");
                        e.TaxAmountReportingCategory = new Guid("4e4b5344-49b5-494b-9a73-86c492d6d399");
                    }
                    else if (value == "GST on Imports")
                    {
                        e.TaxAmountReportingCategory = new Guid("4e4b5344-49b5-494b-9a73-86c492d6d399");
                    }
                    else if (value == "Input Taxed")
                    {
                        e.ReportingCategory = new Guid("255e0796-b9cf-4425-b785-3f442993215d");
                    }

                    list.Add(e);
                }
            }
            return list;
        }
    }
}
