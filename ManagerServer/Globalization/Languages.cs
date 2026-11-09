using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;
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
                var list = new List<LanguageWithCode>();
                foreach (var file in EmbeddedAssets.GetDirectory("Translations"))
                {
                    if (file.IsDirectory) continue;
                    if (!file.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) continue;
                    var language = ReadNames(file);
                    if (language == null) continue;
                    list.Add(new LanguageWithCode() { Code = Path.GetFileNameWithoutExtension(file.Name), EnglishName = language.EnglishName, NativeName = language.NativeName });
                }
                if (!list.Any(x => x.Code == "en")) list.Add(new LanguageWithCode() { Code = "en", EnglishName = "English", NativeName = "English" });
                languages = list.ToArray();
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
                    return;
                }

                // `code` arrives from request parameters, so it must never be allowed to form a path
                // of its own. Only codes we actually enumerated from the manifest get that far.
                if (!GetLanguages().Any(x => x.Code == code)) return;

                var language = ReadLanguage(EmbeddedAssets.GetFile("Translations/" + code + ".json"));
                if (language?.Strings == null) return;

                ManagerServer.Globalization.Strings.translations[code] = language.Strings.ToFrozenDictionary();
                Strings.CurrentLanguage.Value = code;
            }
        }

        // Reads only the name properties, stopping before `strings` so that listing the available
        // languages never parses the translations themselves.
        private static Language ReadNames(IFileInfo file)
        {
            try
            {
                using (var stream = file.CreateReadStream())
                using (var text = new StreamReader(stream))
                using (var reader = new JsonTextReader(text))
                {
                    var language = new Language();
                    while (reader.Read())
                    {
                        if (reader.TokenType != JsonToken.PropertyName) continue;
                        switch (reader.Value as string)
                        {
                            case "englishName":
                                language.EnglishName = reader.ReadAsString();
                                break;
                            case "nativeName":
                                language.NativeName = reader.ReadAsString();
                                break;
                            case "strings":
                                return language;
                            default:
                                reader.Skip();
                                break;
                        }
                    }
                    return language;
                }
            }
            catch (JsonException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }

        private static LanguageWithStrings ReadLanguage(IFileInfo file)
        {
            if (!file.Exists) return null;
            using (var stream = file.CreateReadStream())
            using (var text = new StreamReader(stream))
            using (var reader = new JsonTextReader(text))
            {
                return new JsonSerializer().Deserialize<LanguageWithStrings>(reader);
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
