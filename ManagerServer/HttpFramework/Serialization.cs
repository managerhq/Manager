using ManagerServer;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace HttpFramework
{
    public static class Serialization
    {
        public static object Deserialize2(Type t, string input)
        {
            var buffer = UrlStringToBytes(input);
            using (var ms = new System.IO.MemoryStream(buffer))
            {
                try
                {
                    return ProtoBuf.Serializer.Deserialize(t, ms);
                }
                catch (ProtoBuf.ProtoException)
                {
                    return Activator.CreateInstance(t);
                }
                catch (OverflowException)
                {
                    return Activator.CreateInstance(t);
                }
                catch (EndOfStreamException)
                {
                    return Activator.CreateInstance(t);
                }
            }
        }

        private static void Deserialize(object o, Dictionary<string, string> input)
        {
            foreach (MemberInfo p in o.GetType().GetFieldsAndProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (p.DeclaringType == typeof(HttpHandler)) continue;
                if (!input.ContainsKey(p.Name) && input.All(x => !x.Key.StartsWith(p.Name+"."))) continue;

                var memberType = p.GetMemberType();

                var getValue = new Func<string>(() =>
                {
                    return input[p.Name];
                });

                var getValues = new Func<string[]>(() =>
                {
                    return input[p.Name].Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                });

                if (memberType.IsEnum)
                {
                    var value = getValue();
                    if (!string.IsNullOrWhiteSpace(value)) p.SetMemberValue(o, Enum.Parse(memberType, value, true));
                }
                else if (memberType == typeof(Guid) || memberType == typeof(Guid?))
                {
                    var value = getValue();
                    Guid result;
                    if (Guid.TryParse(value, out result))
                    {
                        if (memberType == typeof(Guid)) p.SetMemberValue(o, result);
                        if (memberType == typeof(Guid?)) p.SetMemberValue(o, new Nullable<Guid>(result));
                    }
                }
                else if (memberType == typeof(bool) || memberType == typeof(bool?))
                {
                    var value = getValue();
                    if (value == "on" || value == "On" || value == "ON" || value == "true" || value == "True" || value == "TRUE") p.SetMemberValue(o, true);
                    else p.SetMemberValue(o, false);
                }
                else if (memberType == typeof(int) || memberType == typeof(int?))
                {
                    var value = getValue();
                    int result;
                    if (int.TryParse(value, out result))
                    {
                        if (memberType == typeof(int)) p.SetMemberValue(o, result);
                        if (memberType == typeof(Nullable<int>)) p.SetMemberValue(o, new Nullable<int>(result));
                    }
                }
                else if (memberType == typeof(long))
                {
                    var value = getValue();
                    long result;
                    if (long.TryParse(value, out result))
                    {
                        p.SetMemberValue(o, result);
                    }
                }
                else if (memberType == typeof(decimal) || memberType == typeof(decimal?))
                {
                    var value = getValue();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        var decimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                        var value2 = string.Join(string.Empty, value.Where(x => char.IsDigit(x) || x.Equals('-') || x.Equals(decimalSeparator[0])).ToArray());
                        value2 = value2.Replace(decimalSeparator[0], '.');

                        decimal result;
                        if (decimal.TryParse(value2, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out result))
                        {
                            if (memberType == typeof(decimal)) p.SetMemberValue(o, result);
                            if (memberType == typeof(Nullable<decimal>)) p.SetMemberValue(o, new Nullable<decimal>(result));
                        }
                    }
                }
                else if (memberType == typeof(string))
                {
                    var value = getValue();
                    if (value == null) p.SetMemberValue(o, string.Empty);
                    else p.SetMemberValue(o, value.Trim());
                }
                else if (memberType == typeof(string[]))
                {
                    var values = getValues();
                    p.SetMemberValue(o, values);
                }
                else if (memberType == typeof(int[]))
                {
                    var values = getValues();
                    p.SetMemberValue(o, values.Select(x => int.Parse(x.Trim())).ToArray());
                }
                else if (memberType == typeof(byte[]))
                {
                    p.SetMemberValue(o, UrlStringToBytes(getValue()));
                }
                else if (memberType == typeof(Guid[]))
                {
                    var values = getValues();
                    var guids = new List<Guid>();
                    foreach (var e in values)
                    {
                        if (string.IsNullOrWhiteSpace(e)) continue;
                        if (Guid.TryParse(e.Trim(), out Guid result)) guids.Add(result);
                    }
                    p.SetMemberValue(o, guids.ToArray());
                }
                else if (Nullable.GetUnderlyingType(memberType) != null && Nullable.GetUnderlyingType(memberType).IsEnum)
                {
                    var value = getValue();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        p.SetMemberValue(o, Enum.Parse(Nullable.GetUnderlyingType(memberType), value, true));
                    }
                }
                else if (memberType == typeof(DateTime) || memberType == typeof(DateTime?))
                {
                    try
                    {
                        var value = getValue();
                        var parts = value.Split('-');
                        var year = int.Parse(parts[0]);
                        if (year < 100) year += 2000;
                        DateTime date = new DateTime(year, int.Parse(parts[1]), int.Parse(parts[2]));
                        p.SetMemberValue(o, date);
                    }
                    catch
                    {
                    }
                }
                else if (memberType == typeof(Dictionary<Guid, string>))
                {
                    var items = input.FilterKeysStartsWith(p.Name + ".");
                    var dict = new Dictionary<Guid, string>();
                    foreach (var e in items)
                    {
                        if (string.IsNullOrWhiteSpace(e.Value)) continue;
                        dict.Add(new Guid(e.Key), e.Value);
                    }
                    p.SetMemberValue(o, dict);
                }
                else if (memberType.IsArray)
                {
                    var value = getValue();
                    Type elementType = memberType.GetElementType();
                    Array array = Array.CreateInstance(elementType, string.IsNullOrEmpty(value) ? 0 : int.Parse(value));
                    for (int i = 0; i < array.Length; i++)
                    {
                        object element = Activator.CreateInstance(elementType);
                        Deserialize(element, input.FilterKeysStartsWith(p.Name + @"[" + i.ToString() + @"]."));
                        array.SetValue(element, i);
                    }
                    p.SetMemberValue(o, array);
                }
                else
                {
                    object fieldValue = Activator.CreateInstance(memberType);
                    Deserialize(fieldValue, input.FilterKeysStartsWith(p.Name + "."));
                    p.SetMemberValue(o, fieldValue);
                }
            }
        }

        public static byte[] UrlStringToBytes(string value)
        {
            var value2 = value.Replace('_', '/').Replace('-', '+');
            switch (value2.Length % 4)
            {
                case 2: value2 += "=="; break;
                case 3: value2 += "="; break;
            }
            byte[] buffer = new byte[0];
            try
            {
                buffer = Convert.FromBase64String(value2);
            }
            catch (FormatException) { }
            return buffer;
        }

        private static Dictionary<string, string> FilterKeysStartsWith(this Dictionary<string, string> collection, string s)
        {
            var o = new Dictionary<string, string>();
            foreach (string key in collection.Keys)
            {
                if (key == null) continue;
                if (key.StartsWith(s)) o.Add(key.Substring(s.Length), collection[key]);
            }
            return o;
        }

        public static T Parse<T>(this IFormCollection o)
        {
            var dict = o.ToDictionary(x => x.Key, x => x.Value.ToString());
            T value = (T)Activator.CreateInstance(typeof(T), true);
            Serialization.Deserialize(value, dict);
            return value;
        }
    }
}