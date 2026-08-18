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
        private static async Task<IEnumerable<Model.Object>> Upgrade327(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>().ToArray())
            {
                if (e.CustomFields == null) continue;

                if (e.CustomFields.TryGetValue(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), out string value))
                {
                    if (value == "ДДВ 5% (Член 32-4)")
                    {
                        e.ReportingCategory = new Guid("875ecd14-2415-4e81-b756-afc04c031faf");
                    }
                    else if (value == "ДДВ 18% (Член 32-а)")
                    {
                        e.ReportingCategory = new Guid("85da12a0-a309-4ef1-9802-1107d9acf54e");
                    }
                    else if (value == "ДДВ 18% (Увоз)")
                    {
                        e.ReportingCategory = new Guid("65ecd57a-6559-47d9-8809-ed7dc60cf4e8");
                        e.TaxAmountReportingCategory = new Guid("9685ad5f-1427-43b6-8d3a-602b1800c031");
                    }
                    else if (value == "ДДВ 5% (Увоз)")
                    {
                        e.ReportingCategory = new Guid("ea4dab9a-4715-4806-a380-8a748f355a97");
                        e.TaxAmountReportingCategory = new Guid("85749a73-8e42-49ce-b844-d1656cabeb60");
                    }
                    else if (value == "ДДВ 5%")
                    {
                        e.ReportingCategory = new Guid("e79b39ae-97fb-486b-8754-618f1b6b3dda");
                        e.TaxAmountReportingCategory = new Guid("e24479bb-ed46-4b2b-91ce-a59a684ca3c8");
                    }
                    else if (value == "ДДВ 18% (Член 32-4)")
                    {
                        e.ReportingCategory = new Guid("7b434cec-3cac-4b76-894a-8000da50846b");
                    }
                    else if (value == "ДДВ 0% (Без право на одбивка)")
                    {
                        e.ReportingCategory = new Guid("c5563d0c-8eb8-43c6-a66b-52113d907072");
                    }
                    else if (value == "ДДВ 0% (Со право на одбивка)")
                    {
                        e.ReportingCategory = new Guid("58b41ba4-48ff-4cb9-9d88-28290c2c19d4");
                    }
                    else if (value == "ДДВ 10%")
                    {
                        e.ReportingCategory = new Guid("1da12922-9c81-4f35-9b97-f48531430dcc");
                        e.TaxAmountReportingCategory = new Guid("5445b882-092d-44f7-95af-beafc3c5a42e");
                    }
                    else if (value == "ДДВ 5% (Член 32-а)")
                    {
                        e.ReportingCategory = new Guid("3d98526f-b566-4db1-bc62-eb2e91ffeb69");
                    }
                    else if (value == "ДДВ 18%")
                    {
                        e.ReportingCategory = new Guid("5c186267-5fd3-4a89-a227-57ff33cc24fb");
                        e.TaxAmountReportingCategory = new Guid("6fb33e28-2861-4052-bab9-c61dfa17452b");
                    }
                    else if (value == "ДДВ 0% (Извоз)")
                    {
                        e.ReportingCategory = new Guid("0ecb9879-01a2-43ce-8a7c-47839ffa59f5");
                    }
                    else if (value == "ДДВ 0% (Немаат седиште во земјата)")
                    {
                        e.ReportingCategory = new Guid("2b6cea37-221f-4c1f-a3e1-4ed623fce916");
                    }

                    list.Add(e);
                }
            }
            return list;
        }
    }
}
