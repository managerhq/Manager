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
        private static async Task<IEnumerable<Model.Object>> Upgrade345(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var themes = new Dictionary<Guid, string>();
            themes.Add(new Guid("ff81dd4f-7961-408c-9c87-88fbe18b2140"), "Smooth Black");
            themes.Add(new Guid("8533b45b-f061-4d57-a234-84cfe644dd17"), "Smooth Navy");
            themes.Add(new Guid("f31748cd-9bb1-4dfb-9f2b-f372f7909212"), "Smooth Orange");
            themes.Add(new Guid("d336666e-eb43-4ef8-980a-4fe684362dfa"), "Smooth Gray");
            themes.Add(new Guid("d448fc80-cffd-48a5-bc0b-5b255dff89ea"), "Smooth Yellow");
            themes.Add(new Guid("3684bbf2-e0a7-400c-954d-61842530a53b"), "Smooth Blue");
            themes.Add(new Guid("f7e53b2d-9f20-4f50-af29-7399f081a825"), "Smooth Green");

            themes.Add(new Guid("e415e946-f3d6-4e76-babc-6c812a998894"), "Bold Black");
            themes.Add(new Guid("c8c5e179-a521-48b8-98e4-edb8a2234761"), "Bold Navy");
            themes.Add(new Guid("5335a9a3-cc7d-49a6-9ed5-3ebac96ddfaa"), "Bold Orange");
            themes.Add(new Guid("d1b905bd-f27d-4c1d-9d67-56d35fb39657"), "Bold Gray");
            themes.Add(new Guid("d795b8eb-cbab-463d-8b5f-ef37e069ee33"), "Bold Yellow");
            themes.Add(new Guid("d1b2e219-181c-4ce4-a47d-acc188522072"), "Bold Blue");
            themes.Add(new Guid("4d3d6b1d-8d7d-494d-b661-72f4e0ab873b"), "Bold Green");

            themes.Add(new Guid("2ef6944c-24d1-44d9-b401-c0f1ba11d598"), "Glossy Black");
            themes.Add(new Guid("e72866c8-df0f-4a9f-a37d-37cb04cf3d38"), "Glossy Navy");
            themes.Add(new Guid("e5ca3981-fffe-41e4-91b8-ddeffacd136b"), "Glossy Orange");
            themes.Add(new Guid("d4dbd29d-aa1f-461a-848b-4dff6cfce205"), "Glossy Gray");
            themes.Add(new Guid("b847cce5-f9a4-41e3-acf9-b6a03861dcf2"), "Glossy Yellow");
            themes.Add(new Guid("0a10a302-bbf2-4087-a428-a7d16e97381c"), "Glossy Blue");
            themes.Add(new Guid("41a126c5-592f-4af5-b9a8-57cd48426e8e"), "Glossy Green");

            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete77.BuiltInTheme>())
            {
                if (themes.ContainsKey(e.Key))
                {
                    var s = typeof(Upgrade).Assembly.GetManifestResourceStream("ManagerServer.Core.Model.Obsolete.Obsolete77." + e.Key.ToString() + ".html");
                    if (s != null)
                    {
                        var template = new StreamReader(s).ReadToEnd();
                        var theme = new CustomTheme() { Key = e.Key, Name = themes[e.Key], Template = template, Inactive = !e.Active };
                        list.Add(theme);
                    }
                }
            }

            return list;
        }
    }
}
