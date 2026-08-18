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
        private static async Task<IEnumerable<Model.Object>> Upgrade332(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.BusinessDetails>().ToArray())
            {
                if (!string.IsNullOrWhiteSpace(e.Obsolete_Country))
                {
                    var country = e.Obsolete_Country;
                    switch (country)
                    {
                        case "English|Angola": e.Obsolete_Country = "pt-AO"; break;
                        case "English|Australia": e.Obsolete_Country = "en-AU"; break;
                        case "English|Bahrain": e.Obsolete_Country = "en-BH"; break;
                        case "English|Ghana": e.Obsolete_Country = "en-GH"; break;
                        case "English|India": e.Obsolete_Country = "en-IN"; break;
                        case "English|Ireland": e.Obsolete_Country = "en-IE"; break;
                        case "English|Kenya": e.Obsolete_Country = "en-KE"; break;
                        case "English|Namibia": e.Obsolete_Country = "en-NA"; break;
                        case "English|New Zealand": e.Obsolete_Country = "en-NZ"; break;
                        case "English|Nigeria": e.Obsolete_Country = "en-NG"; break;
                        case "English|Pakistan": e.Obsolete_Country = "en-PK"; break;
                        case "English|Philippines": e.Obsolete_Country = "en-PH"; break;
                        case "English|South Africa": e.Obsolete_Country = "en-ZA"; break;
                        case "English|Uganda": e.Obsolete_Country = "en-UG"; break;
                        case "English|United Kingdom": e.Obsolete_Country = "en-GB"; break;
                        case "English|United States": e.Obsolete_Country = "en-US"; break;
                        case "English|Zambia": e.Obsolete_Country = "en-ZM"; break;
                        case "العربية|مِصر‎,‎‎": e.Obsolete_Country = "ar-EG"; break;
                        case "العربية|الْعِرَاق‎": e.Obsolete_Country = "ar-IQ"; break;
                        case "العربية|الكويت‎‎": e.Obsolete_Country = "ar-KW"; break;
                        case "العربية|عُمَان‎‎": e.Obsolete_Country = "ar-OM"; break;
                        case "العربية|المملكة العربية السعودية": e.Obsolete_Country = "ar-SA"; break;
                        case "العربية|سُورِيَا": e.Obsolete_Country = "ar-SY"; break;
                        case "العربية|الإمارات العربية المتحدة": e.Obsolete_Country = "ar-AE"; break;
                        case "Eesti|Eesti": e.Obsolete_Country = "et-EE"; break;
                        case "Deutsch|Deutschland": e.Obsolete_Country = "de-DE"; break;
                        case "Ελληνικά|Ελλάδα": e.Obsolete_Country = "el-GR"; break;
                        case "中文(香港)|香港": e.Obsolete_Country = "zh-HK"; break;
                        case "Íslenska|Ísland": e.Obsolete_Country = "is-IS"; break;
                        case "Bahasa Indonesia|Indonesia": e.Obsolete_Country = "id-ID"; break;
                        case "Italiano|Italia": e.Obsolete_Country = "it-IT"; break;
                        case "Nederlands|Nederland": e.Obsolete_Country = "nl-NL"; break;
                        case "Македонски|Македонија": e.Obsolete_Country = "mk-MK"; break;
                        case "Srpski|Srbija": e.Obsolete_Country = "sr-RS"; break;
                        case "Slovenščina|Slovenija": e.Obsolete_Country = "sl-SL"; break;
                    }
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
