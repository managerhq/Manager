using ManagerServer.Globalization;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Extensions
{
    [ProtoContract]
    [Title(nameof(Strings.Extensions))]
    internal sealed class Extensions : Template
    {
        public sealed class Entry
        {
            public string Language { get; set; } = string.Empty;
            public string Region { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
        }

        public static List<Entry> Entries = new List<Entry>();

        static Extensions()
        {
            // External directory entries (e-invoicing portals etc.) and any other manually-curated
            // names live on disk so they can be edited without rebuilding.
            var file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Extensions.json");
            if (System.IO.File.Exists(file))
            {
                var external = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Entry>>(System.IO.File.ReadAllText(file));
                if (external != null) Entries.AddRange(external);
            }

            // Local country extensions live under wwwroot/extensions and are embedded in the
            // assembly. Their index file is the source of truth for those entries.
            try
            {
                var provider = new ManifestEmbeddedFileProvider(typeof(Extensions).Assembly, "wwwroot");
                var localIndex = provider.GetFileInfo("extensions/extensions.json");
                if (localIndex.Exists)
                {
                    using var stream = localIndex.CreateReadStream();
                    using var reader = new StreamReader(stream);
                    var local = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Entry>>(reader.ReadToEnd());
                    if (local != null) Entries.AddRange(local);
                }
            }
            catch { /* embedded resource missing — fall back to the on-disk file alone */ }
        }

        protected override Task InnerGet()
        {
            using (Div(@class: "p-8")) using (Div(@class: "max-w-prose mx-auto"))
            {
                using (Div(@class: "flex justify-between items-end"))
                {
                    using (Div(@class: "font-bold text-lg text-neutral-400 px-3"))
                    {
                        using (Div(@class: "flex gap-4 items-center"))
                        {
                            using (Span()) Write(Strings.Extensions);
                        }
                    }
                }

                // Show only extensions matching the viewer's language (ISO code). English is the
                // fallback: when viewing in English we also include entries with no language set.
                var currentLanguage = ManagerServer.Globalization.Languages.GetLanguage();
                var visible = Entries
                    .Where(e =>
                        string.Equals(e.Language, currentLanguage, StringComparison.OrdinalIgnoreCase)
                        || (currentLanguage == "en" && string.IsNullOrEmpty(e.Language)))
                    .OrderBy(e => string.IsNullOrEmpty(e.Region))
                    .ThenBy(e => e.Region, StringComparer.CurrentCultureIgnoreCase)
                    .ThenBy(e => e.Name, StringComparer.CurrentCultureIgnoreCase)
                    .ToList();

                using (Table(@class: "font-semibold w-full", style: "margin-top: 10px"))
                {
                    string lastRegion = null;
                    foreach (var e in visible)
                    {
                        var region = e.Region ?? string.Empty;
                        var newGroup = lastRegion != region;
                        if (newGroup)
                        {
                            using (Tr())
                            {
                                using (Td(colspan: 2, @class: "p-0")) Hr();
                            }
                        }
                        using (Tr())
                        {
                            using (Td(@class: "p-4 text-neutral-400 w-0 whitespace-nowrap align-top"))
                            {
                                if (newGroup) Write(region);
                            }
                            using (Td(style: "padding: 10px"))
                            {
                                var id = System.Buffers.Text.Base64Url.EncodeToString(Encoding.UTF8.GetBytes(e.Url));

                                using (Button(onclick: $"document.getElementById('{id}').showModal()", style: "font-size: 14px", @class: "block cursor-pointer font-semibold p-2 bg-transparent border-0 text-(--primary-foreground)/75 hover:text-(--primary-foreground)"))
                                {
                                    Write(e.Name);
                                }

                                using (Dialog(id: id, @class: "m-0 ms-24 w-auto h-auto max-w-none max-h-none shadow-2xl transform transition-transform duration-300 ease-out starting:open:translate-x-full rtl:starting:open:-translate-x-full open:translate-x-0", onclick: "this.close()"))
                                {
                                    using (IFrame(@class: "w-full h-full", src: e.Url, loading: "lazy")) { }
                                }
                            }
                        }
                        lastRegion = region;
                    }
                    using (Tr())
                    {
                        using (Td(colspan: 2, @class: "p-0")) Hr();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
