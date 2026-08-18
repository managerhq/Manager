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
        private static async Task<IEnumerable<Model.Object>> Upgrade328(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>().ToArray())
            {
                if (e.CustomFields == null) continue;

                if (e.CustomFields.TryGetValue(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), out string value))
                {
                    if (value == "BTW 6% EU")
                    {
                        e.ReportingCategory = new Guid("b23ed8bd-a5d8-48a5-ac76-3d3052670c14");
                    }
                    else if (value == "BTW 9%")
                    {
                        e.ReportingCategory = new Guid("7e759940-4d2f-40e1-88f7-a19d04e8da15");
                        e.TaxAmountReportingCategory = new Guid("fbd9872a-88ff-42e3-9989-128fa1686764");
                    }
                    else if (value == "BTW 21%")
                    {
                        e.ReportingCategory = new Guid("69577f89-05e7-4855-8b9a-d841636fa15d");
                        e.TaxAmountReportingCategory = new Guid("fc81db2c-54e6-4cc1-a34e-38cd374e104b");
                    }
                    else if (value == "BTW 21% non-EU")
                    {
                        e.ReportingCategory = new Guid("fa6074ad-56fe-4a45-8d2a-592e259f9d24");
                    }
                    else if (value == "BTW 9% EU")
                    {
                        e.ReportingCategory = new Guid("d9e35291-8956-4891-9e7a-ca35bacb502b");
                    }
                    else if (value == "BTW 0% non-EU")
                    {
                        e.ReportingCategory = new Guid("4518b12f-2cdf-44cf-a6ad-ab185f7d2030");
                    }
                    else if (value == "BTW 9% non-EU")
                    {
                        e.ReportingCategory = new Guid("0865745d-8902-44fb-91c9-94646e2cf634");
                    }
                    else if (value == "BTW 21% EU")
                    {
                        e.ReportingCategory = new Guid("9cc68957-2300-40e6-a219-7c2ce8f3769e");
                    }
                    else if (value == "BTW privégebruik")
                    {
                        e.ReportingCategory = new Guid("a90c6fdf-a034-42ca-a2bf-e3e8807e85b1");
                        e.TaxAmountReportingCategory = new Guid("13d9c26e-f9c6-41cb-b653-91f21bd27628");
                    }
                    else if (value == "BTW 0% verlegd")
                    {
                        e.ReportingCategory = new Guid("3a97d8bd-ea09-4674-87d6-e61498e3292d");
                    }
                    else if (value == "BTW 6%")
                    {
                        e.ReportingCategory = new Guid("cfd53d9e-46f4-41fd-83a9-c9a5ed12252b");
                        e.TaxAmountReportingCategory = new Guid("57f036ff-97a9-4b61-a998-dc72407ce75d");
                    }
                    else if (value == "BTW 6% non-EU")
                    {
                        e.ReportingCategory = new Guid("d5b050a4-fe2f-4346-9b8c-1c97d47e842e");
                    }
                    else if (value == "BTW 0% vrijgesteld")
                    {
                        e.ReportingCategory = new Guid("a0ac84a7-b379-4351-a0b4-5b836170e2d3");
                    }
                    else if (value == "BTW 21% verlegd")
                    {
                        e.ReportingCategory = new Guid("70238b59-21c6-4c7f-9834-0db8566ca656");
                    }
                    else if (value == "BTW 0% EU")
                    {
                        e.ReportingCategory = new Guid("21f63491-4058-4ce7-8f80-9dc3e5e70e27");
                    }
                    else if (value == "BTW 9% verlegd")
                    {
                        e.ReportingCategory = new Guid("ee1d5b46-44ed-4e45-9a41-d7f6e17d52fb");
                    }
                    else if (value == "BTW 6% verlegd")
                    {
                        e.ReportingCategory = new Guid("6daef94f-22bf-4390-896f-7bb3a1432d9d");
                    }

                    list.Add(e);
                }
            }
            return list;
        }
    }
}
