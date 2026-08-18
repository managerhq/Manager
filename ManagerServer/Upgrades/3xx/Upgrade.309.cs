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
        private static async Task<IEnumerable<Model.Object>> Upgrade309(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            if (objects.OfType<ManagerServer.Model.TaxCode>().Any())
            {
                var customFields = new Dictionary<Guid, Tuple<Guid, string>>();
                customFields.Add(new Guid("10d8f9dc-db1e-4c87-9480-a696f59aeddf"), new Tuple<Guid, string>(new Guid("6f95e5e5-50e8-4bbb-a750-2550be8cc47c"), "GST Free Import Export"));
                customFields.Add(new Guid("14f63584-be71-40ca-9028-1a60e2e2cc90"), new Tuple<Guid, string>(new Guid("6f95e5e5-50e8-4bbb-a750-2550be8cc47c"), "GST Free"));
                customFields.Add(new Guid("3e2cb830-e229-4525-8794-2e38761b3cfd"), new Tuple<Guid, string>(new Guid("6f95e5e5-50e8-4bbb-a750-2550be8cc47c"), "GST 10% (CAPEX)"));
                customFields.Add(new Guid("73952f89-6139-4fd0-a937-165b8ed66ba8"), new Tuple<Guid, string>(new Guid("6f95e5e5-50e8-4bbb-a750-2550be8cc47c"), "GST 10% (Deferred)"));
                customFields.Add(new Guid("8cf9d117-3142-4d9c-82ee-b57a0e22c809"), new Tuple<Guid, string>(new Guid("6f95e5e5-50e8-4bbb-a750-2550be8cc47c"), "GST 10%"));
                customFields.Add(new Guid("9fe15ead-2138-4f4b-b3f5-398857d08503"), new Tuple<Guid, string>(new Guid("6f95e5e5-50e8-4bbb-a750-2550be8cc47c"), "GST on Imports"));
                customFields.Add(new Guid("f218a321-f83d-4d06-8d02-b52f595cc4fe"), new Tuple<Guid, string>(new Guid("6f95e5e5-50e8-4bbb-a750-2550be8cc47c"), "Input Taxed"));
                customFields.Add(new Guid("1a88fd08-a595-4e12-97d3-85fc165eecdc"), new Tuple<Guid, string>(new Guid("3357a960-1488-490b-b6cb-378c8f9b4295"), "GST 0%"));
                customFields.Add(new Guid("705acb55-550e-458a-bf9a-c4c7021dc351"), new Tuple<Guid, string>(new Guid("3357a960-1488-490b-b6cb-378c8f9b4295"), "GST Exempt"));
                customFields.Add(new Guid("d865c9b1-17b9-488b-b29b-b15c0b3c3246"), new Tuple<Guid, string>(new Guid("3357a960-1488-490b-b6cb-378c8f9b4295"), "GST Adjustment"));
                customFields.Add(new Guid("ee8cacde-58da-48ec-8aa9-aa6acba9c32f"), new Tuple<Guid, string>(new Guid("3357a960-1488-490b-b6cb-378c8f9b4295"), "GST 15%"));
                customFields.Add(new Guid("1731afd8-40df-484c-8335-81a1451ab8f8"), new Tuple<Guid, string>(new Guid("988936d4-c5bb-41af-a5d8-c3c503b4a22d"), "VAT 15%"));
                customFields.Add(new Guid("45da7b65-f249-4ac3-a171-4a888276d237"), new Tuple<Guid, string>(new Guid("988936d4-c5bb-41af-a5d8-c3c503b4a22d"), "VAT Free Exports"));
                customFields.Add(new Guid("46712241-b20a-42e8-a29a-876f15e50f94"), new Tuple<Guid, string>(new Guid("988936d4-c5bb-41af-a5d8-c3c503b4a22d"), "VAT 0%"));
                customFields.Add(new Guid("574a0647-25c3-4aff-886b-c7d64b641a1c"), new Tuple<Guid, string>(new Guid("988936d4-c5bb-41af-a5d8-c3c503b4a22d"), "VAT 5%"));
                customFields.Add(new Guid("cc862e5b-a055-43f7-b585-4c70f041bc19"), new Tuple<Guid, string>(new Guid("988936d4-c5bb-41af-a5d8-c3c503b4a22d"), "VAT Exempt"));
                customFields.Add(new Guid("42a5002c-5c8f-4def-8672-4e6f3fc09654"), new Tuple<Guid, string>(new Guid("0c2354c3-9a05-42d3-b6df-f4c2ef7a519b"), "VAT Exempt"));
                customFields.Add(new Guid("56769971-405e-47bd-bd13-d64de0eae752"), new Tuple<Guid, string>(new Guid("0c2354c3-9a05-42d3-b6df-f4c2ef7a519b"), "VAT 5%"));
                customFields.Add(new Guid("6959fb01-3a48-486a-9bec-a0681a662f03"), new Tuple<Guid, string>(new Guid("0c2354c3-9a05-42d3-b6df-f4c2ef7a519b"), "VAT 0%"));
                customFields.Add(new Guid("70364d69-174a-4804-881e-852bdbff59e2"), new Tuple<Guid, string>(new Guid("0c2354c3-9a05-42d3-b6df-f4c2ef7a519b"), "VAT 0% (EU)"));
                customFields.Add(new Guid("b926c2d8-09e4-496c-9a2c-818c8aaa36ed"), new Tuple<Guid, string>(new Guid("0c2354c3-9a05-42d3-b6df-f4c2ef7a519b"), "VAT 20%"));
                customFields.Add(new Guid("115e658e-84f8-4c46-8d0a-e9b28f317d35"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 5% (Член 32-4)"));
                customFields.Add(new Guid("17b4df47-2148-40bd-a2c6-56d426d44ca0"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 18% (Член 32-а)"));
                customFields.Add(new Guid("31a9a48c-5733-40bf-b135-9422f3c0592e"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 18% (Увоз)"));
                customFields.Add(new Guid("5a254e38-7b2a-474d-9ab0-8777e37d7d56"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 5% (Увоз)"));
                customFields.Add(new Guid("8fad8a0f-f932-454e-b90b-7b48b6a02e1a"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 5%"));
                customFields.Add(new Guid("9201a831-7cf6-4963-a33d-fd404c6f02dc"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 18% (Член 32-4)"));
                customFields.Add(new Guid("938b13c6-227d-4945-8eb5-60a88e980dfe"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 0% (Без право на одбивка)"));
                customFields.Add(new Guid("a0af1cab-581e-41bc-b711-59390b68a767"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 0% (Со право на одбивка)"));
                customFields.Add(new Guid("ba184918-bbf2-4585-b388-6ad7a743ac46"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 10%"));
                customFields.Add(new Guid("d40ac9e4-f58a-4ca8-837f-266ff1ce504b"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 5% (Член 32-а)"));
                customFields.Add(new Guid("d7eba1a0-d27c-465c-94ca-38e7b480266a"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 18%"));
                customFields.Add(new Guid("e141987c-19f2-4982-84ee-a45b94e68c93"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 0% (Извоз)"));
                customFields.Add(new Guid("f7a7d590-3354-459b-a786-3730ea70de8a"), new Tuple<Guid, string>(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"), "ДДВ 0% (Немаат седиште во земјата)"));
                customFields.Add(new Guid("1f681bc9-93fd-4e09-815f-2f1c6bc5044b"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 6% EU"));
                customFields.Add(new Guid("31dce658-cff1-4212-870b-5fd04cb83b1c"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 9%"));
                customFields.Add(new Guid("33855cc4-964b-44d1-be27-cf268b0ad77d"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 21%"));
                customFields.Add(new Guid("3a57686a-7ce6-43e7-aa2a-dc5e98ad931b"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 21% non-EU"));
                customFields.Add(new Guid("4d895b0c-a40c-44ff-b441-9620c7619699"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 9% EU"));
                customFields.Add(new Guid("75eaae26-98a4-4e1a-9d75-91bf2b7e7b11"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 0% non-EU"));
                customFields.Add(new Guid("815852ae-5a5d-4688-aa9b-0b06f3982ef2"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 9% non-EU"));
                customFields.Add(new Guid("93cdbadc-4a15-41ff-bf68-4ed927915680"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 21% EU"));
                customFields.Add(new Guid("9f2d06f9-0e16-4192-afa3-bff747910088"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW privégebruik"));
                customFields.Add(new Guid("ad18e082-df57-44f9-8fd0-ab3c5275d230"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 0% verlegd"));
                customFields.Add(new Guid("ad40e782-1f1c-486a-91a4-266e7001c8b3"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 6%"));
                customFields.Add(new Guid("bddb4876-fd25-49bf-ac71-5365878268ef"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 6% non-EU"));
                customFields.Add(new Guid("c6b298fa-f994-47ac-b6c0-3299fbac8306"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 0% vrijgesteld"));
                customFields.Add(new Guid("ce2c6c96-4364-42a8-8cf3-8315d1a4b246"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 21% verlegd"));
                customFields.Add(new Guid("d0eca2ec-e9da-40de-9be6-e2e9fb34269a"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 0% EU"));
                customFields.Add(new Guid("dd28f171-7422-44a3-937a-432e8d2970ea"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 9% verlegd"));
                customFields.Add(new Guid("e5f20470-dffa-4453-a0a9-42a403c04e23"), new Tuple<Guid, string>(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"), "BTW 6% verlegd"));

                foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>())
                {
                    if (customFields.ContainsKey(e.Key))
                    {
                        if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                        e.CustomFields[customFields[e.Key].Item1] = customFields[e.Key].Item2;
                        list.Add(e);
                    }
                }
            }

            return list;
        }
    }
}
