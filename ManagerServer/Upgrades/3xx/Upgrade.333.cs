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
        private static async Task<IEnumerable<Model.Object>> Upgrade333(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            var reportingCategories = new HashSet<Guid>();
            reportingCategories.Add(new Guid("9cc68957-2300-40e6-a219-7c2ce8f3769e"));
            reportingCategories.Add(new Guid("b23ed8bd-a5d8-48a5-ac76-3d3052670c14"));
            reportingCategories.Add(new Guid("d9e35291-8956-4891-9e7a-ca35bacb502b"));
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>().Where(x => x.ReportingCategory.HasValue && reportingCategories.Contains(x.ReportingCategory.Value)).ToArray())
            {
                e.TaxAmountReportingCategory = new Guid("7d4b9447-8bba-48b8-ae2e-3f58bb921cbd");
                e.TaxAmountReversedReportingCategory = new Guid("81c7b060-bc5d-46d0-8000-999069d7b2cc");
                list.Add(e);
            }

            var reportingCategories2 = new HashSet<Guid>();
            reportingCategories2.Add(new Guid("fa6074ad-56fe-4a45-8d2a-592e259f9d24"));
            reportingCategories2.Add(new Guid("d5b050a4-fe2f-4346-9b8c-1c97d47e842e"));
            reportingCategories2.Add(new Guid("0865745d-8902-44fb-91c9-94646e2cf634"));
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>().Where(x => x.ReportingCategory.HasValue && reportingCategories2.Contains(x.ReportingCategory.Value)).ToArray())
            {
                e.TaxAmountReportingCategory = new Guid("ae8911a5-12a2-4e36-a9f7-1baf382ea13c");
                e.TaxAmountReversedReportingCategory = new Guid("108043fc-1c37-477d-b32c-63b7b0690f75");
                list.Add(e);
            }
            return list;
        }
    }
}
