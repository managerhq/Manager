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
        private static async Task<IEnumerable<Model.Object>> Upgrade320(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.PayslipEarningsItem>().Where(x => x.CustomFields != null))
            {
                if (e.CustomFields.TryGetValue(new Guid("8198caf2-1125-4f8e-9469-527583e3ea5e"), out string value))
                {
                    switch (value)
                    {
                        case "Gross payments": e.ReportingCategory = new Guid("f25bdb57-d366-45cc-93db-2e401fb7001f"); break;
                        case "CDEP payments": e.ReportingCategory = new Guid("35b90096-c604-4f02-b0f1-edce5bed95c7"); break;
                        case "Allowance - Car": e.ReportingCategory = new Guid("f6e11c1e-47a9-477f-805f-e60b43e7bf72"); break;
                        case "Allowance - Transport": e.ReportingCategory = new Guid("cf03ab85-80a9-4cdb-afca-8b9db1e300d7"); break;
                        case "Allowance - Travel": e.ReportingCategory = new Guid("9d376dc3-c56c-48fa-95cd-976223d57a66"); break;
                        case "Allowance - Meals": e.ReportingCategory = new Guid("0477def9-f904-497f-8b37-6b174227d544"); break;
                        case "Allowance - Laundry": e.ReportingCategory = new Guid("9cd51c1d-fdf9-41cb-89fd-1f65e72d13b2"); break;
                        case "Allowance - Other": e.ReportingCategory = new Guid("ae443a9c-fb4e-4518-a829-f3f4bbfbfb10"); break;
                        case "Lump sum A": e.ReportingCategory = new Guid("2cc195f2-6f31-4035-a969-9c98c80ce132"); break;
                        case "Lump sum B": e.ReportingCategory = new Guid("3540ac43-bcf2-4499-855e-d13612ab9829"); break;
                        case "Lump sum D": e.ReportingCategory = new Guid("c8adcd5b-af39-4adf-8c67-d5038aa6c008"); break;
                        case "Lump sum E": e.ReportingCategory = new Guid("1c8995a9-b06c-4f93-add4-66db7fb577b7"); break;
                        case "Exempt foreign employment income": e.ReportingCategory = new Guid("f9e00d0c-396d-4ba4-9ab0-e81114c8b1f5"); break;
                    }

                    list.Add(e);
                }
                if (e.CustomFields.TryGetValue(new Guid("07fb0059-a29a-4d7a-8a7c-64f63311c05c"), out string value2))
                {
                    if (value2 == "Yes") e.ReportingCategory = new Guid("1eeec512-af7f-4cc5-bb83-1231c27c2e91");
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.PayslipDeductionItem>().Where(x => x.CustomFields != null))
            {
                if (e.CustomFields.TryGetValue(new Guid("e253ec72-1200-414a-941a-a93f4039a045"), out string value))
                {
                    switch (value)
                    {
                        case "PAYG": e.ReportingCategory = new Guid("43dc8de9-5e59-471e-b220-111c97ada19e"); break;
                        case "Workplace giving": e.ReportingCategory = new Guid("f1bc8fba-622b-44bf-893c-ad33834c7c88"); break;
                        case "Union / association fees": e.ReportingCategory = new Guid("0a084bbf-9b39-460a-aa36-5085ed19c99c"); break;
                    }

                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.PayslipContributionItem>().Where(x => x.CustomFields != null))
            {
                if (e.CustomFields.TryGetValue(new Guid("8ce55e79-ec7a-4e5d-a41f-84c7b7b7189e"), out string value))
                {
                    switch (value)
                    {
                        case "Superannuation guarantee": e.ReportingCategory = new Guid("d4bc3a93-b10a-4a88-ab40-d4539bff054e"); break;
                        case "Reportable employer super contribution": e.ReportingCategory = new Guid("e42078eb-62da-4992-845b-084a976e404d"); break;
                    }

                    list.Add(e);
                }
            }
            return list;
        }
    }
}
