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
        private static async Task<IEnumerable<Model.Object>> Upgrade326(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>().ToArray())
            {
                if (e.CustomFields == null) continue;

                if (e.CustomFields.TryGetValue(new Guid("b7f51bfe-7d0d-476f-87ff-247598809dc3"), out string value))
                {
                    if (value == "1 - Standard Rate (15%)")
                    {
                        e.ReportingCategory = new Guid("29f76316-8627-41e4-a72f-a3217fc697de");
                        e.TaxAmountReportingCategory = new Guid("e261d697-172b-44f2-8a91-613f5f1f4e71");
                    }
                    else if (value == "2 - Standard Rate (15% Capital)")
                    {
                        e.ReportingCategory = new Guid("517f393f-6ebb-4c5b-87b5-3f1c21e57406");
                        e.TaxAmountReportingCategory = new Guid("2fdb1fe7-16e9-4b14-aaad-63bcc2fce5ba");
                    }
                    else if (value == "3 - Zero rate (local)")
                    {
                        e.ReportingCategory = new Guid("ede809fe-5e5d-43e8-ae39-907dc6f79c68");
                    }
                    else if (value == "4 - Zero Rate (export)")
                    {
                        e.ReportingCategory = new Guid("b6a161f7-a9c6-4624-bf37-2441b5b9d6ad");
                    }
                    else if (value == "5 - Accommodation (less than 28 days)")
                    {
                        e.ReportingCategory = new Guid("1b8ddebf-7f8c-464b-b7ee-96383471123d");
                        e.TaxAmountReportingCategory = new Guid("0a5a166e-c894-4ae5-a1b0-cf605f5bb6a0");
                        if (e.Components != null)
                        {
                            foreach (var e2 in e.Components)
                            {
                                e2.ComponentTaxAmountReportingCategory = new Guid("0a5a166e-c894-4ae5-a1b0-cf605f5bb6a0");
                            }
                        }
                    }
                    else if (value == "6 - Accommodation (28 days or more)")
                    {
                        e.ReportingCategory = new Guid("a2cb9db6-9f03-47e9-90e8-fd0bb613ce92");
                        e.TaxAmountReportingCategory = new Guid("c20b2738-2dfe-4800-8e6c-d3926aca8358");
                        if (e.Components != null)
                        {
                            foreach (var e2 in e.Components)
                            {
                                e2.ComponentTaxAmountReportingCategory = new Guid("c20b2738-2dfe-4800-8e6c-d3926aca8358");
                            }
                        }
                    }
                    else if (value == "7 - Other & imports")
                    {
                        e.TaxAmountReportingCategory = new Guid("219f8177-bf86-4e37-aec4-3df40ec749a2");
                    }
                    else if (value == "8 - Change in use")
                    {
                        e.TaxAmountReportingCategory = new Guid("4c33fe37-0838-4770-b173-0549db06fcd5");
                    }
                    else if (value == "9 - Exempt")
                    {
                        e.ReportingCategory = new Guid("f7685b72-1274-40ab-99f3-4919a4f73dc7");
                    }
                    else if (value == "Bad debts written off")
                    {
                        e.ReportingCategory = new Guid("8d0d3031-2910-4beb-b15d-9b962586d0d5");
                        e.TaxAmountReportingCategory = new Guid("7f2b3250-349e-44cf-90ba-5142e4aef17c");
                    }
                    else if (value == "Other")
                    {
                        e.ReportingCategory = new Guid("5453879b-d40a-4307-81a7-ea69705025d8");
                        e.TaxAmountReportingCategory = new Guid("5f9b7f15-4d68-42a0-835c-d09f5dfb6b39");
                    }
                    else if (value == "Imported capital goods")
                    {
                        e.TaxAmountReportingCategory = new Guid("3c15b97c-c273-433a-8121-f943658ac122");
                    }

                    list.Add(e);
                }
            }
            return list;
        }
    }
}
