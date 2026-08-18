using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagerServer.Helpers
{
    internal static class StringExtensions
    {
        internal static string IfEmptyReplaceWith(this string s, string emptyString)
        {
            return string.IsNullOrWhiteSpace(s) ? emptyString : s;
        }

        internal static string EncodeJsString(this string s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            if (!string.IsNullOrEmpty(s))
            {
                foreach (char c in s)
                {
                    switch (c)
                    {
                        case '\"':
                            sb.Append("\\\"");
                            break;
                        case '\\':
                            sb.Append("\\\\");
                            break;
                        case '\b':
                            sb.Append("\\b");
                            break;
                        case '\f':
                            sb.Append("\\f");
                            break;
                        case '\n':
                            sb.Append("\\n");
                            break;
                        case '\r':
                            sb.Append("\\r");
                            break;
                        case '\t':
                            sb.Append("\\t");
                            break;
                        default:
                            int i = (int)c;
                            if (i < 32 || i > 127)
                            {
                                sb.AppendFormat("\\u{0:X04}", i);
                            }
                            else
                            {
                                sb.Append(c);
                            }
                            break;
                    }
                }
            }
            sb.Append("\"");

            return sb.ToString().Replace("</script>", @"</scr""+""ipt>");
        }

        internal static string EncodeJsString2(this string s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('\'');
            if (!string.IsNullOrEmpty(s))
            {
                foreach (char c in s)
                {
                    switch (c)
                    {
                        case '\"':
                            sb.Append("\\\"");
                            break;
                        case '\\':
                            sb.Append("\\\\");
                            break;
                        case '\b':
                            sb.Append("\\b");
                            break;
                        case '\f':
                            sb.Append("\\f");
                            break;
                        case '\n':
                            sb.Append("\\n");
                            break;
                        case '\r':
                            sb.Append("\\r");
                            break;
                        case '\t':
                            sb.Append("\\t");
                            break;
                        default:
                            int i = (int)c;
                            if (i < 32 || i > 127)
                            {
                                sb.AppendFormat("\\u{0:X04}", i);
                            }
                            else
                            {
                                sb.Append(c);
                            }
                            break;
                    }
                }
            }
            sb.Append('\'');

            return sb.ToString().Replace("</script>", @"</scr""+""ipt>");
        }
    }
}