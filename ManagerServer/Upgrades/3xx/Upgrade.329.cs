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
        private static async Task<IEnumerable<Model.Object>> Upgrade329(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>().ToArray())
            {
                if (e.CustomFields == null) continue;

                if (e.CustomFields.TryGetValue(new Guid("988936d4-c5bb-41af-a5d8-c3c503b4a22d"), out string value))
                {
                    if (value == "VAT 15%")
                    {
                        e.ReportingCategory = new Guid("d4c5c416-894c-48d5-ab2c-0a566744c92d");
                        e.TaxAmountReportingCategory = new Guid("6521e584-ae2c-4656-ad16-14b139f903e8");
                    }
                    else if (value == "VAT Free Exports")
                    {
                        e.ReportingCategory = new Guid("07189131-6316-402f-8fe1-2381ecd3ec0d");
                    }
                    else if (value == "VAT 0%")
                    {
                        e.ReportingCategory = new Guid("8b660d88-87ce-4d01-818e-988d558b6139");
                    }
                    else if (value == "VAT 5%")
                    {
                        e.ReportingCategory = new Guid("52a108ef-c4c2-4651-bea6-ce26d58cb3b5");
                        e.TaxAmountReportingCategory = new Guid("f88052e4-0ebe-466e-bf13-35a7193d3fe4");
                    }
                    else if (value == "VAT Exempt")
                    {
                        e.ReportingCategory = new Guid("8673675a-26ad-4afd-895c-9e0041f1d634");
                    }

                    list.Add(e);
                }
            }
            return list;
        }
    }
}
