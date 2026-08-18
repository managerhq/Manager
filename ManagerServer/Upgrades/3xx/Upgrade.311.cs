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
        private static async Task<IEnumerable<Model.Object>> Upgrade311(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var keys = new HashSet<Guid>();
            keys.Add(new Guid("050a3828-8ba0-4db9-b560-f24a3c5e413b"));
            keys.Add(new Guid("0713b751-ee07-4d22-8b17-ad0131029ca0"));
            keys.Add(new Guid("07fb0059-a29a-4d7a-8a7c-64f63311c05c"));
            keys.Add(new Guid("0987c8fd-1720-4162-961e-64a5519b9c2c"));
            keys.Add(new Guid("0fba87ee-0386-401a-8d1f-302313b663f4"));
            keys.Add(new Guid("11acbfb3-9557-487a-8b8b-2528a2c43c53"));
            keys.Add(new Guid("1f6483e5-5ad9-4181-a9db-117827b67a9d"));
            keys.Add(new Guid("343a8633-5d10-46ca-9d20-0beed32ebab8"));
            keys.Add(new Guid("57a258c7-3296-4b4a-bc6c-82ba5755c222"));
            keys.Add(new Guid("6b837666-1039-4cfc-948b-5c36b30682c2"));
            keys.Add(new Guid("6f95e5e5-50e8-4bbb-a750-2550be8cc47c"));
            keys.Add(new Guid("8067ce2d-6a48-45f1-a5a9-5e2d923e9cc7"));
            keys.Add(new Guid("8198caf2-1125-4f8e-9469-527583e3ea5e"));
            keys.Add(new Guid("8ce55e79-ec7a-4e5d-a41f-84c7b7b7189e"));
            keys.Add(new Guid("91a2b722-99cd-41e1-b638-532793aae782"));
            keys.Add(new Guid("9483841a-8d21-4c77-8031-614f721878fa"));
            keys.Add(new Guid("b8c661cd-ff56-4862-853b-75d0f2920776"));
            keys.Add(new Guid("ce735064-e907-478f-83b0-f319b2a9a7fb"));
            keys.Add(new Guid("dc15193e-9883-4e89-a78b-7e6751db5240"));
            keys.Add(new Guid("e253ec72-1200-414a-941a-a93f4039a045"));
            keys.Add(new Guid("e3f5b9dd-ee9a-4fd0-9ca9-750c9aad9b1f"));
            keys.Add(new Guid("f6326e38-61c9-41c6-8bda-7a9f2d594150"));
            keys.Add(new Guid("f6859bbb-1736-4e60-81f2-2703a9ea4686"));
            keys.Add(new Guid("72766a67-2dab-4cb3-95ce-4faeba933b25"));
            keys.Add(new Guid("afb2997f-2213-4db4-b871-7192d4ef4564"));
            keys.Add(new Guid("da06cb58-591d-41df-92fe-a5be3f16c93a"));
            keys.Add(new Guid("de9ae1f1-df47-4298-a702-dc509f75eb06"));
            keys.Add(new Guid("07332ba3-3e82-4dc1-9451-1350f5d84e24"));
            keys.Add(new Guid("11acbfe1-0d24-4161-b366-fe905f2bcfd9"));
            keys.Add(new Guid("92b38154-38fc-479a-a296-2019f656d1e2"));
            keys.Add(new Guid("c4a0ccf7-9171-4e8e-b390-97f7052b1479"));
            keys.Add(new Guid("0c782b45-f50c-4b69-bbf3-75f42d7670a7"));
            keys.Add(new Guid("3357a960-1488-490b-b6cb-378c8f9b4295"));
            keys.Add(new Guid("f7fb2f55-8930-4ae0-9e91-9fb1ae0b51e1"));
            keys.Add(new Guid("994cef79-6da3-4fa1-9998-ad029a4358f0"));
            keys.Add(new Guid("988936d4-c5bb-41af-a5d8-c3c503b4a22d"));
            keys.Add(new Guid("d96d97e8-c857-42c6-8360-443c06a13de9"));
            keys.Add(new Guid("40dc02e6-b9e2-456e-a3b0-f3bb45aae9b4"));
            keys.Add(new Guid("c580f852-601c-4020-8aee-73ffbbf1181c"));
            keys.Add(new Guid("734f9a89-b048-46c5-b792-e652057c381f"));
            keys.Add(new Guid("0c2354c3-9a05-42d3-b6df-f4c2ef7a519b"));
            keys.Add(new Guid("732d52fa-05f0-439c-a601-cc22a46ea795"));
            keys.Add(new Guid("12e5e9fb-d8e8-4fce-aa33-8ba564117550"));
            keys.Add(new Guid("2d15fd3a-2063-489d-be8a-d620f44bc21c"));
            keys.Add(new Guid("67a99165-f78b-4e57-a076-0868f04e0ed1"));
            keys.Add(new Guid("8aea30c2-7bbe-4f2a-882b-4af8fdae2bc7"));
            keys.Add(new Guid("9c780537-d36d-4b72-a0cc-36219801f111"));
            keys.Add(new Guid("3c7e1105-c3ef-4aa0-9c9e-282c548dd29e"));
            keys.Add(new Guid("436f269c-126f-4055-847b-b8d146b7e1e8"));
            keys.Add(new Guid("9422cf3b-d783-42e2-ae4b-f80e4dd3f024"));
            keys.Add(new Guid("3df91926-ee38-4c0a-91ef-41f82bbf6e32"));
            keys.Add(new Guid("b755a3ef-32aa-4eab-8936-0e48b057f627"));

            var list = new List<ManagerServer.Model.Object>();

            list.AddRange(objects.OfType<ManagerServer.Model.CustomField>().Where(x => keys.Contains(x.Key)).Select(x => new ManagerServer.Model.Obsolete.ObsoleteSingleton() { Key = x.Key }));
            list.AddRange(objects.OfType<ManagerServer.Model.Obsolete.Obsolete74.ReportTransformation>().Where(x => keys.Contains(x.Key)).Select(x => new ManagerServer.Model.Obsolete.ObsoleteSingleton() { Key = x.Key }));
            list.AddRange(objects.OfType<ManagerServer.Model.ScriptExtension>().Where(x => keys.Contains(x.Key)).Select(x => new ManagerServer.Model.Obsolete.ObsoleteSingleton() { Key = x.Key }));

            var country = string.Empty;
            if (list.Any(x => x.Key == new Guid("11acbfe1-0d24-4161-b366-fe905f2bcfd9"))) country = "English|Australia";
            if (list.Any(x => x.Key == new Guid("92b38154-38fc-479a-a296-2019f656d1e2"))) country = "English|Australia";
            if (list.Any(x => x.Key == new Guid("07332ba3-3e82-4dc1-9451-1350f5d84e24"))) country = "English|Australia";
            if (list.Any(x => x.Key == new Guid("c4a0ccf7-9171-4e8e-b390-97f7052b1479"))) country = "English|Australia";

            if (list.Any(x => x.Key == new Guid("b755a3ef-32aa-4eab-8936-0e48b057f627"))) country = "Nederlands|Nederland";
            if (list.Any(x => x.Key == new Guid("994cef79-6da3-4fa1-9998-ad029a4358f0"))) country = "English|New Zealand";
            if (list.Any(x => x.Key == new Guid("734f9a89-b048-46c5-b792-e652057c381f"))) country = "English|Saudi Arabia";
            if (list.Any(x => x.Key == new Guid("12e5e9fb-d8e8-4fce-aa33-8ba564117550"))) country = "English|United Kingdom";
            if (list.Any(x => x.Key == new Guid("3c7e1105-c3ef-4aa0-9c9e-282c548dd29e"))) country = "Македонски|Македонија";

            if (!string.IsNullOrWhiteSpace(country))
            {
                var businessDetailsKey = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BusinessDetails));
                var businessDetails = objects.SingleOrDefault<ManagerServer.Model.BusinessDetails>(businessDetailsKey) ?? new ManagerServer.Model.BusinessDetails() { Key = businessDetailsKey };
                businessDetails.Obsolete_Country = country;
                list.Add(businessDetails);
            }

            return list;
        }
    }
}
