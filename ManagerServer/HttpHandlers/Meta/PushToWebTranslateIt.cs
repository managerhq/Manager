#if DEBUG
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Meta
{
    [ProtoContract]
    internal sealed class PushToWebTranslateIt : HttpHandler
    {
        public override async Task Get()
        {
            var apiKey = "xxxxxxxxxxxxxxxxxxx";
            var localStrings = GetManagerStrings();
            var remoteStrings = await GetWebTranslateItStrings(apiKey);

            var inserts = new List<string>();
            var renames = new List<(string from, string to)>();
            var updates = new List<string>();
            var deletes = new List<string>();

            foreach (var e in localStrings)
            {
                var remoteString = remoteStrings.SingleOrDefault(x => x.key == e.key);
                if (remoteString == null)
                {
                    if (!remoteStrings.Any(x => x.translations[0].text == e.translations[0].text))
                    {
                        inserts.Add(e.key);
                    }
                    else
                    {
                        var original = remoteStrings.Single(x => x.translations[0].text == e.translations[0].text);
                        renames.Add((original.key, e.key));
                    }
                }
                else if (remoteString.translations[0].text != e.translations[0].text)
                {
                    updates.Add(e.key);
                }
            }

            foreach (var e in remoteStrings)
            {
                if (!localStrings.Any(x => x.key == e.key))
                {
                    deletes.Add(e.key);
                }
            }

            using (Html())
            {
                using (Head()) { }
                using (Body())
                {
                    using (H1()) Write("Insert");
                    using (Ul())
                    {
                        foreach (var key in inserts)
                            using (Li()) Write(key);
                    }

                    using (H1()) Write("Rename");
                    using (Ul())
                    {
                        foreach (var (from, to) in renames)
                            using (Li()) Write($"{from} -> {to}");
                    }

                    using (H1()) Write("Update");
                    using (Ul())
                    {
                        foreach (var key in updates)
                            using (Li()) Write(key);
                    }

                    using (H1()) Write("Delete");
                    using (Ul())
                    {
                        foreach (var key in deletes)
                            using (Li()) Write(key);
                    }

                    using (Form(method: "POST"))
                    {
                        InputSubmit(value: "Push");
                    }
                }
            }
        }

        public override async Task Post()
        {
            var apiKey = "xxxxxxxxxxxxxxxxxxxxxxxx";
            var client = new HttpClient();
            var localStrings = GetManagerStrings();
            var remoteStrings = await GetWebTranslateItStrings(apiKey);

            foreach (var e in localStrings)
            {
                var remoteString = remoteStrings.SingleOrDefault(x => x.key == e.key);
                if (remoteString == null)
                {
                    if (!remoteStrings.Any(x => x.translations[0].text == e.translations[0].text))
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                        {
                            key = e.key,
                            file = new { file_name = "Strings.json" },
                            translations = new[] { new { locale = "en", text = e.translations[0].text } }
                        });
                        await client.PostAsync("https://webtranslateit.com/api/projects/" + apiKey + "/strings", new StringContent(json, Encoding.UTF8, "application/json"));
                    }
                    else
                    {
                        var original = remoteStrings.Single(x => x.translations[0].text == e.translations[0].text);
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { key = e.key });
                        await client.PutAsync("https://webtranslateit.com/api/projects/" + apiKey + "/strings/" + original.id, new StringContent(json, Encoding.UTF8, "application/json"));
                        original.key = e.key;
                    }
                }
                else if (remoteString.translations[0].text != e.translations[0].text)
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { text = e.translations[0].text });
                    await client.PostAsync("https://webtranslateit.com/api/projects/" + apiKey + "/strings/" + remoteString.id + "/locales/en/translations", new StringContent(json, Encoding.UTF8, "application/json"));
                }
            }

            foreach (var e in remoteStrings)
            {
                if (!localStrings.Any(x => x.key == e.key))
                {
                    await client.DeleteAsync("https://webtranslateit.com/api/projects/" + apiKey + "/strings/" + e.id);
                }
            }

            Response.Redirect(this.ToUrl());
        }

        private async Task<List<StringObject>> GetWebTranslateItStrings(string apiKey)
        {
            var client = new HttpClient();
            var list = new List<StringObject>();
            var url = "https://webtranslateit.com/api/projects/" + apiKey + "/strings.json?locale=en&page=1";

            while (true)
            {
                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                var objects = Newtonsoft.Json.JsonConvert.DeserializeObject<StringObject[]>(json);
                list.AddRange(objects);

                var linkResponseHeader = response.Headers.GetValues("link").Single();
                var next = linkResponseHeader.Split(',').SingleOrDefault(x => x.Contains(@"rel=""next"""));
                if (string.IsNullOrWhiteSpace(next)) break;

                url = (next.Split(';')[0].Replace("<", string.Empty).Replace(">", string.Empty) + "&locale=en").Replace("//strings.json", $"/{apiKey}/strings.json");
            }

            return list;
        }

        private static List<StringObject> GetManagerStrings()
        {
            var list = new List<StringObject>();
            foreach (var property in typeof(ManagerServer.Globalization.Strings).GetProperties())
            {
                var stringObject = new StringObject();
                stringObject.key = property.Name;
                stringObject.translations[0].text = (string)property.GetValue(null);
                list.Add(stringObject);
            }
            return list;
        }

        private sealed class StringObject
        {
            public int id = 0;
            public string key;
            public Translations[] translations = new Translations[] { new Translations() };

            public sealed class Translations
            {
                public string text;
            }
        }
    }
}
#endif