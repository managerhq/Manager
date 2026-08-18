using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Globalization
{
    public static class Locales
    {
        public static string GetNativeName(string code)
        {
            switch (code)
            {
                case "en-AU": return "Australia";
                case "en-BH": return "Bahrain";
                case "en-GB": return "United Kingdom";
                case "en-GH": return "Ghana";
                case "en-IN": return "India";
                case "en-IE": return "Ireland";
                case "en-NZ": return "New Zealand";
                case "en-NG": return "Nigeria";
                case "en-ZA": return "South Africa";
                case "en-UG": return "Uganda";
                case "en-KE": return "Kenya";
                case "en-NA": return "Namibia";
                case "en-PH": return "Philippines";
                case "en-PK": return "Pakistan";
                case "en-US": return "United States";
                case "en-ZM": return "Zambia";
                case "en-MY": return "Malaysia";
                case "zh-HK": return "香港";
                case "et-EE": return "Eesti";
                case "de-DE": return "Deutschland";
                case "el-GR": return "Ελλάδα";
                case "is-IS": return "Ísland";
                case "in-ID": return "Indonesia";
                case "it-IT": return "Italia";
                case "nl-NL": return "Nederland";
                case "mk-MK": return "Македонија";
                case "ar-SA": return "المملكة العربية السعودية";
                case "sr-RS": return "Srbija";
                case "sl-SL": return "Slovenija";
                case "ar-SY": return "سُورِيَا";
                case "ar-AE": return "الإمارات العربية المتحدة";
                case "ar-IQ": return "الْعِرَاق‎";
                case "ar-KW": return "الكويت‎‎";
                case "ar-EG": return "مِصر‎,‎‎";
                case "ar-OM": return "عُمَان‎‎";
                case "id-ID": return "Indonesia";
                case "pt-AO": return "Angola";
                case "sk-SK": return "Slovensko";
                default: return code;
            }
        }

        public static string GetLanguage(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            return code.Split('-').First();
        }
    }
}