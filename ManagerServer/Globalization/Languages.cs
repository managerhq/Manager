using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ManagerServer.Globalization
{
    public static class Languages
    {
        private static LanguageWithCode[] languages;

        public static LanguageWithCode[] GetLanguages()
        {
            if (languages == null)
            {
                var dict = new Dictionary<string, Language>();
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Translations.json");
                if (File.Exists(path))
                {
                    dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Language>>(File.ReadAllText(path)) ?? new Dictionary<string, Language>();
                }
                if (!dict.ContainsKey("en")) dict.Add("en", new Language() { EnglishName = "English", NativeName = "English" });
                languages = dict.Select(x => new LanguageWithCode() { Code = x.Key, EnglishName = x.Value.EnglishName, NativeName = x.Value.NativeName }).ToArray();
            }
            return languages;
        }

        public static string GetLanguageNativeName(string code)
        {
            if (code == "zh") return "中文";
            return GetLanguages().SingleOrDefault(x => x.Code == code)?.NativeName ?? code;
        }

        public static bool IsRightToLeft()
        {
            switch (Strings.CurrentLanguage.Value)
            {
                case "dv":
                case "ar":
                case "he":
                case "ku":
                case "fa":
                case "ur":
                case "ps":
                    return true;
                default:
                    return false;
            }
        }

        public static string GetLanguage()
        {
            return Strings.CurrentLanguage.Value;
        }

        public static void SetLanguage(string code)
        {
            Strings.CurrentLanguage.Value = "en";

            if (string.IsNullOrWhiteSpace(code)) return;

            lock (ManagerServer.Globalization.Strings.translations)
            {
                if (ManagerServer.Globalization.Strings.translations.ContainsKey(code))
                {
                    Strings.CurrentLanguage.Value = code;
                }
                else
                {
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Translations.json");
                    if (File.Exists(path))
                    {
                        using (var fs = File.OpenRead(path))
                        {
                            using (var s = new StreamReader(fs))
                            {
                                using (var reader = new JsonTextReader(s))
                                {
                                    while (reader.Read())
                                    {
                                        if (reader.TokenType == JsonToken.PropertyName)
                                        {
                                            if (reader.Value as string == code)
                                            {
                                                reader.Read();
                                                var serializer = new JsonSerializer();
                                                var language = serializer.Deserialize<LanguageWithStrings>(reader);
                                                ManagerServer.Globalization.Strings.translations.Add(code, language.Strings.ToFrozenDictionary());
                                                Strings.CurrentLanguage.Value = code;
                                                return;
                                            }
                                            reader.Skip();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public class Language
        {
            public string NativeName;
            public string EnglishName;
        }

        public sealed class LanguageWithCode : Language
        {
            public string Code;
        }

        public sealed class LanguageWithStrings : Language
        {
            public Dictionary<string, string> Strings;
        }
    }
}
