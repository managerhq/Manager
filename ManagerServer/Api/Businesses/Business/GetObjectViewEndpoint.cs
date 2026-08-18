using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class GetObjectViewEndpoint<T> : ViewEndpoint<View>, IView where T : Model.Object, new()
    {
        public sealed override View AuthorizedHandle()
        {
            var business = GetApplicationData().Businesses.Get(Business);
            var obj = business.SingleOrDefault<T>(Key);
            if (obj == null) return null;

            Languages.SetLanguage(Language);

            var view = Build(business, obj);
            if (view == null) return null;

            if (string.IsNullOrEmpty(view.Title)) view.Title = DefaultTitle;

            var businessDetails = business.Single<Model.BusinessDetails>();
            view.BusinessName = businessDetails.Name;
            if (string.IsNullOrWhiteSpace(view.BusinessName)) view.BusinessName = Business;
            view.Direction = Languages.IsRightToLeft() ? Direction.Rtl : Direction.Ltr;
            view.Language = Languages.GetLanguage();

            return view;
        }

        protected virtual View Build(Database business, T obj) => BuildFromMembers(business, obj);

        protected virtual string DefaultTitle => Strings.GetPropertyValue(typeof(T).Name);

        protected View BuildFromMembers(Database business, T obj)
        {
            var view = new View { Title = DefaultTitle };

            var classicCustomFieldDefs = business.OfType<Model.CustomField>().Where(x => x.Contains(typeof(T))).ToDictionary(x => x.Key);
            var customFieldDefs2 = business.GetCustomFields(typeof(T));

            foreach (var property in typeof(T).GetProperties().OrderBy(x => x.Name == "CustomFields"))
            {
                if (property.Name == nameof(Model.Object.Key)) continue;
                if (property.Name == nameof(Model.Object.Timestamp)) continue;
                if (property.Name.StartsWith("Obsolete_")) continue;
                if (property.Name.StartsWith("StartingBalance")) continue;
                if (property.GetCustomAttribute<HiddenAttribute>() != null) continue;
                if (property.GetCustomAttribute<SecretAttribute>() != null) continue;
                if (!property.CanWrite) continue;

                var value = property.GetMemberValue(obj);
                if (value == null) continue;

                if (value is Dictionary<Guid, string> classicCustomFields)
                {
                    var children = new List<View.FieldInfo>();
                    foreach (var def in classicCustomFieldDefs.Values.OrderBy(x => x.Position).ThenBy(x => x.Name))
                    {
                        if (!classicCustomFields.TryGetValue(def.Key, out var raw)) continue;
                        if (string.IsNullOrWhiteSpace(raw)) continue;

                        string formatted;
                        if (def.Type == Model.Enums.CustomFieldStyle.Date && DateTime.TryParseExact(raw, "yyyy-M-d", null, System.Globalization.DateTimeStyles.None, out var date))
                        {
                            formatted = date.ToShortDateString();
                        }
                        else if (def.Type == Model.Enums.CustomFieldStyle.Number && decimal.TryParse(raw, out var number))
                        {
                            formatted = number.ToNumberString();
                        }
                        else
                        {
                            formatted = raw.Replace("\r", string.Empty);
                        }

                        children.Add(new View.FieldInfo { Label = def.Name, Text = formatted });
                    }

                    if (children.Count > 0)
                    {
                        view.Fields.Add(new View.FieldInfo { Label = Strings.CustomFields, Fields = children });
                    }
                    continue;
                }

                if (value is Model.CustomFields customFields2)
                {
                    var children = new List<View.FieldInfo>();
                    foreach (var def in customFieldDefs2.Where(x => x.DisplayOnView))
                    {
                        var v = customFields2.GetValue(def);
                        if (v == null) continue;

                        string formatted = v switch
                        {
                            DateTime d => d.ToShortDateString(),
                            decimal n => n.ToNumberString(),
                            string s => s.Replace("\r", string.Empty),
                            bool b => b ? Strings.Yes : "-",
                            string[] strings => string.Join(", ", strings),
                            _ => v.ToString(),
                        };

                        View.ImageInfo image = null;
                        if (def is ImageCustomField imageCustomField)
                        {
                            image = new View.ImageInfo() {
                                Url = new Image() { Business = Business }.ToUrl() + "&key=" + v.ToString(),
                                Height = imageCustomField.Height,
                                Width = imageCustomField.Width
                            };
                        }

                        children.Add(new View.FieldInfo { Label = def.Name, Text = formatted, Image = image });
                    }

                    if (children.Count > 0)
                    {
                        view.Fields.Add(new View.FieldInfo { Label = Strings.CustomFields, Fields = children });
                    }
                    continue;
                }

                if (value is Array arrayValue)
                {
                    var arrayField = BuildArrayField(business, property, arrayValue);
                    if (arrayField != null) view.Fields.Add(arrayField);
                    continue;
                }

                if (value is int intValue && intValue == 0) continue;
                if (value is bool boolValue && !boolValue) continue;
                if (value is decimal decimalValue && decimalValue == 0m) continue;

                string textValue;
                if (value is Guid guidValue)
                {
                    var named = business.Single(guidValue) ?? business.SingleOrDefault(guidValue);
                    if (named is NamedObject namedObject)
                    {
                        textValue = namedObject.GetName();
                    }
                    else
                    {
                        textValue = guidValue.ToString();
                    }
                }
                else if (value is bool)
                {
                    textValue = Strings.IsChecked;
                }
                else if (value is Enum)
                {
                    textValue = Strings.GetPropertyValue(value.ToString());
                }
                else if (value is DateTime date)
                {
                    textValue = date.ToLocalShortDisplayString();
                }
                else
                {
                    textValue = value.ToString();
                }

                if (string.IsNullOrWhiteSpace(textValue)) continue;

                view.Fields.Add(new View.FieldInfo
                {
                    Key = property.Name,
                    Label = Strings.GetPropertyValue(property.Name),
                    Text = textValue.Replace("\r", string.Empty),
                });
            }

            return view;
        }

        private static View.FieldInfo BuildArrayField(Database business, PropertyInfo property, Array arrayValue)
        {
            if (arrayValue.Length == 0) return null;

            var elementType = arrayValue.GetType().GetElementType();
            var itemFields = new List<View.FieldInfo>();

            if (IsLeafType(elementType))
            {
                for (int i = 0; i < arrayValue.Length; i++)
                {
                    var raw = arrayValue.GetValue(i);
                    var text = FormatLeafValue(business, raw);
                    if (string.IsNullOrWhiteSpace(text)) continue;
                    itemFields.Add(new View.FieldInfo { Label = (i+1).ToString(), Text = text });
                }
            }
            else
            {
                var properties = elementType.GetProperties().Where(x => !x.Name.StartsWith("Obsolete_")).ToArray();
                for (int i = 0; i < arrayValue.Length; i++)
                {
                    var line = arrayValue.GetValue(i);
                    if (line == null) continue;

                    var lineFields = new List<View.FieldInfo>();
                    foreach (var f in properties)
                    {
                        var raw = f.GetMemberValue(line);
                        if (raw == null) continue;
                        if (raw is int n && n == 0) continue;
                        if (raw is bool b && !b) continue;
                        if (raw is decimal d && d == 0m) continue;

                        var text = FormatLeafValue(business, raw);
                        if (string.IsNullOrWhiteSpace(text)) continue;

                        lineFields.Add(new View.FieldInfo
                        {
                            Key = f.Name,
                            Label = Strings.GetPropertyValue(f.Name),
                            Text = text,
                        });
                    }

                    if (lineFields.Count == 0) continue;
                    itemFields.Add(new View.FieldInfo
                    {
                        Label = (i + 1).ToString(),
                        Fields = lineFields,
                    });
                }
            }

            if (itemFields.Count == 0) return null;

            return new View.FieldInfo
            {
                Key = property.Name,
                Label = Strings.GetPropertyValue(property.Name),
                Fields = itemFields,
            };
        }

        private static bool IsLeafType(Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            return t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(Guid);
        }

        private static string FormatLeafValue(Database business, object value)
        {
            if (value == null) return null;
            if (value is Guid g)
            {
                var named = business.SingleOrDefault<Model.NamedObject>(g);
                return named != null ? named.GetName() : g.ToString();
            }
            if (value is bool b) return b ? Strings.IsChecked : null;
            if (value is DateTime dt) return dt.ToShortDateString();
            if (value is decimal dec) return dec.ToNumberString();
            return value.ToString();
        }

        public View GetView()
        {
            return AuthenticatedHandle();
        }
    }
}
