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
        private static async Task<IEnumerable<Model.Object>> Upgrade57(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var plugins = objects.OfType<Model.Obsolete.Obsolete08.Plugin08>().ToDictionary(x => x.Key);
            var list2 = new List<Guid>();
            if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_AustraliaGst))
            {
                list2.Add(new Guid("8cf9d117-3142-4d9c-82ee-b57a0e22c809"));
                list2.Add(new Guid("14f63584-be71-40ca-9028-1a60e2e2cc90"));
                list2.Add(new Guid("10d8f9dc-db1e-4c87-9480-a696f59aeddf"));
                list2.Add(new Guid("f218a321-f83d-4d06-8d02-b52f595cc4fe"));
            }
            if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_BelgiumVat))
            {
                list2.Add(new Guid("a882ae8c-c278-4f7a-b6b5-c9ec4d76d69d"));
                list2.Add(new Guid("5f1191d4-149a-4ec8-bd18-88190c48669c"));
                list2.Add(new Guid("104ed048-88f0-4f9b-9b06-df4f8d5735d1"));
            }
            if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_IndiaCentralStateTax))
            {
                list2.Add(new Guid("cdca06f2-9732-45a2-88a2-3dfeb2da3fc0"));
                list2.Add(new Guid("20888f96-8ef3-4023-9c54-3f6e41cf14f0"));
            }
            if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_IndiaServiceTax))
            {
                list2.Add(new Guid("71813c71-7833-4911-9cbe-3d961486344d"));
            }
            if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_NewZealandGst))
            {
                list2.Add(new Guid("ee8cacde-58da-48ec-8aa9-aa6acba9c32f"));
            }
            if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_NorwayVat))
            {
                list2.Add(new Guid("454d3e9e-805e-4c42-a79e-17ca98bc397c"));
                list2.Add(new Guid("023bf9ff-ad08-48d2-9913-db2f9f4dec4b"));
                list2.Add(new Guid("5a71058a-c8d0-46a5-81a4-3cee4320aae0"));
                list2.Add(new Guid("01324570-474b-48bc-b893-d8fdc08a0a0f"));
            }
            if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_PhilippinesVat))
            {
                list2.Add(new Guid("835b3f27-3a6d-4d43-a79c-2016321ca388"));
                list2.Add(new Guid("e111d796-d67c-454b-9b4b-aa12d5f788ef"));
                list2.Add(new Guid("a2ed1aaa-be8f-403f-b113-0e94e31f815f"));
                list2.Add(new Guid("942779a5-e3b5-4b19-8f84-1aa5e8056744"));
            }
            if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_SouthAfricaVat))
            {
                list2.Add(new Guid("08d40919-b4cb-4668-8969-b51a63da203e"));
            }
            if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_UnitedKingdomVat))
            {
                list2.Add(new Guid("6959fb01-3a48-486a-9bec-a0681a662f03"));
                list2.Add(new Guid("56769971-405e-47bd-bd13-d64de0eae752"));
                list2.Add(new Guid("b926c2d8-09e4-496c-9a2c-818c8aaa36ed"));
                list2.Add(new Guid("42a5002c-5c8f-4def-8672-4e6f3fc09654"));
            }
            var inBuiltTaxCodes = objects.OfType<Model.Obsolete.Obsolete47.InBuiltTaxCode47>().ToDictionary(x => x.Key);
            list2 = list2.Where(x => !inBuiltTaxCodes.ContainsKey(x)).ToList();
            foreach (var e in list2.Where(x => !inBuiltTaxCodes.ContainsKey(x)).ToList())
            {
                if (!plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.TaxCodes))
                {
                    list.Add(new Model.Obsolete.Obsolete08.Plugin08() { Key = ManagerServer.Model.Obsolete.Obsolete08.Plugins08.TaxCodes });
                }
                else
                {
                    list.Add(new Model.Obsolete.Obsolete47.InBuiltTaxCode47() { Key = e });
                }
            }
            return list;
        }
    }
}
