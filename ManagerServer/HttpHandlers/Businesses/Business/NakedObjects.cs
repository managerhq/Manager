using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using Newtonsoft.Json.Linq;
using ManagerServer.Model;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Model.Enums;
using System.Threading;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjects : BusinessTemplate
    {
        private Dictionary<string, object> methodCache = new Dictionary<string, object>();

        protected object Retrieve(MethodInfo methodInfo, object parameter)
        {
            var key = methodInfo.Name;
            if (!methodCache.ContainsKey(key))
            {
                try
                {
                    methodCache.Add(key, methodInfo.Invoke(this, [parameter]));
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                    throw;
                }
            }
            return methodCache[key];
        }

        private string GetAttributeTitle()
        {
            var titleAttribute = this.GetType().GetCustomAttribute<TitleAttribute>(false);
            if (titleAttribute != null)
            {
                return string.Join(" — ", titleAttribute.Text.Select(x => Strings.GetPropertyValue(x)));
            }
            return null;
        }

        protected virtual void OnTitle(Context context)
        {
            using (Div(@class: "flex gap-3 items-center"))
            {
                using (Div(@class: "card-title"))
                {
                    Write(GetAttributeTitle() ?? Strings.GetPropertyValue(this.GetType().Name));
                }
                WriteHelp();
            }
        }

        protected virtual void OnEmpty(Context context)
        {
            using (Span(@class: "card-title")) Write(Strings.Empty);
        }

        protected virtual void OnColumnHeaderCell(Column column)
        {
            var icon = column.Attributes.OfType<IconAttribute>().SingleOrDefault()?.Value;
            if (!string.IsNullOrWhiteSpace(icon))
            {
                I(@class: $"fas {icon} text-base");
            }
            else
            {
                Write(column.Label);
            }
        }

        protected virtual void OnAfterHeader(Context context) { }
        protected virtual void OnBeforeFooter(Context context) { }
        protected virtual void OnBeforeFooter2(Context context) { }
        protected virtual void OnBeforeBeforeFooter(Context context) { }
        protected virtual void OnAfterFooter(Context context) { }
        protected virtual void OnFooterEndSection(Context context) { }
        protected virtual void OnColumnCell(Column column, object row) => Write(column.GetValueAsHtml(row));
        protected virtual void OnColumnFooterCell(Column column, Array rows) { }
        protected virtual void OnHeaderStartSection(Context context) { }
        protected virtual void OnHeaderMiddleSection(Context context) { }
        protected virtual void OnHeaderEndSection(Context context) { }
        protected virtual void OnFooterStartSection(Context context) { }

        protected interface IsInactive
        {
            bool IsInactive { get; }
        }

        protected sealed class Context
        {
            private readonly Dictionary<Type, object> objects = new Dictionary<Type, object>();
            public void Set<T>(T value) { if (!objects.TryAdd(typeof(T), value)) objects[typeof(T)] = value; }
            public T Get<T>() => objects.TryGetValue(typeof(T), out object value) ? (T)value : default;
        }

        protected override sealed void InnerGet2()
        {
            var context = new Context();
            context.Set(GetColumns());
            InnerGet4(context);
        }

        protected virtual void OnPostForm(Context context)
        {
            var rows = context.Get<Array>();
            var columns = context.Get<Column[]>();

            using (PostForm())
            {
                if (rows == null || rows.Length == 0)
                {
                    using (Div(@class: "card-inset text-center p-32 rounded-t-none rounded-b-none"))
                    {
                        OnEmpty(context);
                    }
                }
                else
                {
                    var visibleColumns = columns.Where(x => x.Visible).Where(x => x.CanEnsureCells(rows)).ToArray();

                    foreach (var e in visibleColumns)
                    {
                        e.EnsureCells(rows);

                        if (e.Attributes.OfType<HideColumnIfAllEmptyAttribute>().Any())
                        {
                            // This block makes sure that when sorting by column that has the attribute above, the column won't hide
                            if (this is NakedObjectsWithSorting nakedObjectsWithSorting)
                            {
                                if (nakedObjectsWithSorting.SortBy.HasValue && e.Key.HasValue)
                                {
                                    if (nakedObjectsWithSorting.SortBy == e.Key)
                                    {
                                        continue;
                                    }
                                }
                            }

                            e.Visible = false;
                            var defaultValue = e.GetDefaultValue();
                            foreach (var e2 in rows)
                            {
                                var cellValue = e.GetValue(e2);
                                if (!object.Equals(cellValue, defaultValue))
                                {
                                    e.Visible = true;
                                    break;
                                }
                            }
                        }
                    }
                    visibleColumns = columns.Where(x => x.Visible).Where(x => x.CanEnsureCells(rows)).ToArray();

                    var prefixColumn = true;
                    var firstColumn = visibleColumns.FirstOrDefault();
                    if (firstColumn != null)
                    {
                        if (firstColumn is Column<Tuple<string, byte[]>> || firstColumn.Attributes.OfType<IconAttribute>().Any()) prefixColumn = false;
                    }

                    using (Div(@class: "overflow-x-auto lg:overflow-visible no-scrollbar"))
                    {
                        using (Table(@class: "card-table"))
                        {
                            var stickyCss = "lg:sticky lg:top-[-1px] z-40";
                            using (THead(@class: stickyCss))
                            {
                                if (visibleColumns.Any(x => x.Action != null))
                                {
                                    if (prefixColumn) using (Th(style: "width: 34px")) { }

                                    foreach (var e in visibleColumns)
                                    {
                                        using (Th())
                                        {
                                            if (e.Action != null)
                                            {
                                                using (Div(@class: "flex gap-2 items-center ltr:justify-end-safe"))
                                                {
                                                    if (e.Action.Item2 != null)
                                                    {
                                                        if (e.Action.Item3)
                                                        {
                                                            using (Button(hxPost: e.Action.Item2.ToUrl(), hxDisabledElt: "this", @class: "btn btn-sm"))
                                                            {
                                                                I(@class: "htmx-indicator fas fa-circle-notch fa-spin me-2 !hidden");
                                                                Write(e.Action.Item1);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            using (A(href: e.Action.Item2.ToUrl(), @class: "btn btn-sm"))
                                                            {
                                                                Write(e.Action.Item1);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        using (Span(@class: "font-normal cursor-not-allowed bg-white border border-neutral-300 text-neutral-400 rounded py-1 px-4 whitespace-nowrap"))
                                                        {
                                                            Write(e.Action.Item1);
                                                        }
                                                    }
                                                    I(@class: "fas fa-turn-down text-base");
                                                }
                                            }
                                        }
                                    }
                                }

                                using (Tr())
                                {
                                    if (prefixColumn) using (Th(style: "width: 34px")) { }

                                    foreach (var e in visibleColumns)
                                    {
                                        var tailwind = string.Empty;
                                        if (e.Attributes.OfType<CenterAttribute>().Any()) tailwind += " text-center";
                                        if (e.Attributes.OfType<RightAttribute>().Any()) tailwind += " text-right";

                                        using (Th(@class: tailwind))
                                        {
                                            OnColumnHeaderCell(e);
                                        }
                                    }
                                }
                            }

                            using (TBody())
                            {
                                foreach (var e in rows)
                                {
                                    var inactiveSuffix = string.Empty;
                                    if (e is ManagerServer.Model.Object o && o.IsInactive()) inactiveSuffix = " *:opacity-35";
                                    if (e is IsInactive isInactive && isInactive.IsInactive) inactiveSuffix = " *:opacity-35";

                                    using (Tr())
                                    {
                                        if (prefixColumn) using (Td()) { }

                                        foreach (var e2 in visibleColumns)
                                        {
                                            var tailwind = inactiveSuffix;
                                            if (e2.Attributes.OfType<CenterAttribute>().Any()) tailwind += " text-center";
                                            if (e2.Attributes.OfType<RightAttribute>().Any()) tailwind += " text-right";
                                            if (e2.Attributes.OfType<MinWidthAttribute>().Any()) tailwind += " w-px";
                                            if (e2.Attributes.OfType<WhitespaceNoWrapAttribute>().Any()) tailwind += " whitespace-nowrap";
                                            if (e2.Attributes.OfType<BoldAttribute>().Any()) tailwind += " font-semibold";
                                            using (Td(@class: tailwind))
                                            {
                                                OnColumnCell(e2, e);
                                            }
                                        }
                                    }
                                }
                            }
                            using (TFoot())
                            {
                                if (prefixColumn) using (Th()) { }

                                foreach (var e in visibleColumns)
                                {
                                    var tailwind = "whitespace-nowrap";
                                    if (e.Attributes.OfType<CenterAttribute>().Any()) tailwind += " text-center";
                                    if (e.Attributes.OfType<RightAttribute>().Any()) tailwind += " text-right";

                                    using (Th(@class: tailwind))
                                    {
                                        OnColumnFooterCell(e, rows);
                                    }
                                }
                            }
                        }
                    }
                }

                OnBeforeBeforeFooter(context);
                OnBeforeFooter(context);
            }

            OnBeforeFooter2(context);
        }

        protected override Task InnerPost()
        {
            Response.Redirect(this.ToUrl());
            return Task.CompletedTask;
        }

        protected virtual void InnerGet4(Context context)
        {
            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header print:hidden"))
                {
                    using (Div(@class: "flex justify-between gap-8"))
                    {
                        using (Div(@class: "flex items-center gap-6"))
                        {                            
                            OnTitle(context);                            

                            OnHeaderStartSection(context);                            
                        }
                        using (Div(@class: "flex items-center gap-6"))
                        {
                            OnHeaderMiddleSection(context);
                        }
                        using (Div(@class: "flex items-center gap-4"))
                        {
                            OnHeaderEndSection(context);                            
                        }
                    }
                }

                OnAfterHeader(context);

                OnPostForm(context);

                using (Div(@class: "card-header print:hidden"))
                {
                    using (Div(@class: "flex justify-between"))
                    {
                        using (Div())
                        {
                            OnFooterStartSection(context);
                        }

                        using (Div(@class: "flex items-center gap-2"))
                        {
                            OnFooterEndSection(context);
                        }
                    }
                }
            }

            OnAfterFooter(context);
        }

        internal Column[] GetColumns()
        {
            var columns = new List<Column>();
            foreach (var e in this.GetType().GetMethods())
            {
                if (!e.Name.StartsWith("Get")) continue;
                if (!e.ReturnType.IsArray) continue;

                var column = (Column)Activator.CreateInstance(typeof(InvokeMethodColumn<>).MakeGenericType(e.ReturnType.GetElementType()), e, this);
                column.Visible = e.GetCustomAttribute<DefaultAttribute>() != null;
                column.Attributes = e.GetCustomAttributes().ToArray();
                column.Key = e.GetCustomAttribute<GuidAttribute>()?.Value;
                column.Name = e.Name.Substring(3);
                column.MergeTag = "@@"+e.Name.Substring(3)+"@@";
                var priorityAttribute = e.GetCustomAttribute<PriorityAttribute>();
                if (priorityAttribute != null) column.Priority = priorityAttribute.Value;

                column.Label = Strings.GetPropertyValue(e.Name.Substring(3));
                if (e.GetCustomAttribute<NameAttribute>() != null)
                {
                    column.Label = string.Join(" - ", e.GetCustomAttribute<NameAttribute>().Value.Select(x => Strings.GetPropertyValue(x)));
                }

                columns.Add(column);
            }
            return columns.OrderBy(x => x.Priority).ToArray();
        }

        internal abstract class Column
        {
            public Guid? Key;
            public Attribute[] Attributes = [];
            public Tuple<string, HttpHandler, bool> Action;
            public bool Visible;
            public int Priority;
            public string Label;
            public string Name;
            public string MergeTag;

            public virtual bool CanEnsureCells(Array rows) => true;
            public abstract void EnsureCells(Array rows);

            public abstract object GetValue(object row);
            public abstract string GetValueAsHtml(object row);
            public abstract JToken GetValueAsJToken(object row);
            public abstract string GetValueAsPlainText(object row);
            public abstract object GetDefaultValue();

            public abstract bool CanConvertToJson { get; }
            public abstract bool CanConvertToPlainText { get; }
        }

        internal abstract class Column<T> : Column
        {
            private Dictionary<object, T> cells;
            private Converter<T> converter;

            public Column()
            {
                var nestedTypes = typeof(NakedObjects).GetNestedTypes(BindingFlags.NonPublic);
                var converterType = nestedTypes.SingleOrDefault(x => x.BaseType == typeof(Converter<T>));
                if (converterType == null && typeof(T).IsEnum) converterType = typeof(EnumConverter<>).MakeGenericType(typeof(T));
                if (converterType != null) converter = (Converter<T>)Activator.CreateInstance(converterType);
            }

            public override bool CanConvertToPlainText => converter != null ? converter.CanConvertToPlainText() : false;
            public override bool CanConvertToJson => converter != null ? converter.CanConvertToJson() : false;
            public override object GetDefaultValue() => default(T);

            protected bool CanAddValues() => (cells == null);

            protected void AddValues(Array rows, T[] values)
            {
                if (cells == null)
                {
                    var dict = new Dictionary<object, T>();
                    for (int i = 0; i < rows.Length; i++)
                    {
                        dict.Add(rows.GetValue(i), values[i]);
                    }
                    cells = dict;
                }
            }

            public override object GetValue(object row)
            {
                if (cells.TryGetValue(row, out T value)) return value;
                return default(T);
            }

            public override string GetValueAsHtml(object row)
            {
                if (cells == null) return null;
                if (cells.TryGetValue(row, out T value))
                {
                    if (converter == null || value == null) return value?.ToString();
                    return converter.ToHtml(value);
                }
                else
                {
                    if (converter == null) return default(T)?.ToString();
                    return converter.ToHtml(default(T));
                }
            }

            public override string GetValueAsPlainText(object row)
            {
                if (cells.TryGetValue(row, out T value))
                {
                    if (converter == null || value == null) return value?.ToString();
                    return converter.ToPlainText(value);
                }
                else
                {
                    if (converter == null) return default(T)?.ToString();
                    return converter.ToPlainText(default(T));
                }
            }

            public override JToken GetValueAsJToken(object row)
            {
                if (cells.TryGetValue(row, out T value))
                {
                    if (converter == null || value == null) return value?.ToString();
                    return converter.ToJson(value);
                }
                else
                {
                    if (converter == null) return default(T)?.ToString();
                    return converter.ToJson(default(T));
                }
            }

            public T GetRowValue(object row)
            {
                if (cells == null) return default(T);
                if (cells.TryGetValue(row, out T value)) return value;
                else return default(T);
            }
        }

        private class InvokeMethodColumn<T> : Column<T>
        {
            private readonly MethodInfo methodInfo;
            private readonly NakedObjects target;

            public InvokeMethodColumn(MethodInfo methodInfo, NakedObjects target)
            {
                this.methodInfo = methodInfo;
                this.target = target;
            }

            public override bool CanEnsureCells(Array rows)
            {
                if (rows == null) return false;
                var firstParameterType = methodInfo.GetParameters().First().ParameterType;
                return firstParameterType == rows.GetType();
            }

            public override void EnsureCells(Array rows)
            {
                if (CanAddValues())
                {
                    var result = (T[])target.Retrieve(methodInfo, rows);
                    if (result == null) result = new T[rows.Length];
                    AddValues(rows, result);
                }
            }
        }        

        protected abstract class Converter<T>
        {
            public abstract bool CanConvertToPlainText();
            public abstract bool CanConvertToJson();

            public abstract string ToPlainText(T value);
            public abstract string ToHtml(T value);
            public abstract JToken ToJson(T value);
        }

        private sealed class StringConverter : Converter<string>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(string value) => value;
            public override string ToHtml(string value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(string value) => value;
        }

        private sealed class BooleanConverter : Converter<bool>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(bool value) => value ? "&#10003;" : null;
            public override string ToHtml(bool value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(bool value) => value;
        }

        private sealed class DecimalConverter : Converter<decimal>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(decimal value) => value.ToNumberString();
            public override string ToHtml(decimal value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(decimal value) => value;
        }

        private sealed class NullableDecimalConverter : Converter<decimal?>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(decimal? value) => value.ToNumberString();
            public override string ToHtml(decimal? value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(decimal? value) => value;
        }

        private sealed class PercentageConverter : Converter<Percentage>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(Percentage value)
            {
                if (value == null) return string.Empty;
                return value.Value.ToString() + "%";
            }

            public override string ToHtml(Percentage value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(Percentage value) => value.Value;
        }

        private sealed class QrCodeConverter : Converter<QrCode>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(QrCode value)
            {
                if (value == null) return string.Empty;
                return value.Value;
            }

            public override string ToHtml(QrCode value) => @$"<div class=""QrCode"">{ToPlainText(value)}</div>";
            public override JToken ToJson(QrCode value) => value.Value;
        }

        private sealed class DecimalDebitCreditConverter : Converter<Tuple<decimal, DebitCredit>>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => false;

            public override string ToPlainText(Tuple<decimal, DebitCredit> value)
            {
                if (value == null) return null;
                if (value.Item1 == 0m) return value.Item1.ToNumberString();
                if (value.Item2 == DebitCredit.Debit) return string.Format(Strings.XXX_Dr, value.Item1.ToNumberString());
                if (value.Item2 == DebitCredit.Credit) return string.Format(Strings.XXX_Cr, value.Item1.ToNumberString());
                return null;
            }
            public override string ToHtml(Tuple<decimal, DebitCredit> value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(Tuple<decimal, DebitCredit> value) => null;
        }

        private sealed class NullableIntegerConverter : Converter<int?>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(int? value) => value.ToString();
            public override string ToHtml(int? value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(int? value) => value;
        }

        private sealed class StringArrayConverter : Converter<string[]>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(string[] value) => value != null ? string.Join(", ", value) : null;
            public override string ToHtml(string[] value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(string[] value) => new JArray(value);
        }

        private static string GetHtml(DateTime value)
        {
            if (value == DateTime.MinValue) return string.Empty;
            if (value == DateTime.MaxValue) return string.Empty;

            if (value.Kind == DateTimeKind.Utc)
            {
                return $@"<time datetime=""{value.ToString("yyyy-MM-ddTHH:mm:ssZ")}"" data-format=""{Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern} {Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern}""></time>";
            }
            var s = value.ToLocalShortDisplayString();
            if (value != value.Date) s += " " + value.ToLongTimeString();
            return s;
        }

        private sealed class DateConverter : Converter<DateTime>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(DateTime value)
            {
                if (value == DateTime.MinValue) return string.Empty;
                if (value == DateTime.MaxValue) return string.Empty;
                var s = value.ToLocalShortDisplayString();
                if (value != value.Date) s += " " + value.ToLongTimeString();
                return s;
            }
            public override string ToHtml(DateTime value) => GetHtml(value);

            public override JToken ToJson(DateTime value)
            {
                if (value == DateTime.MinValue) return string.Empty;
                if (value == DateTime.MaxValue) return string.Empty;
                var s = value.ToString("yyyy-MM-dd");
                if (value != value.Date) s += "T" + value.ToString("hh:mm:ssZ");
                return s;
            }
        }

        private sealed class NullableDateConverter : Converter<DateTime?>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(DateTime? value)
            {
                if (!value.HasValue) return null;
                var s = value.Value.ToLocalShortDisplayString();
                if (value != value.Value.Date) s += " " + value.Value.ToLongTimeString();
                return s;
            }
            public override string ToHtml(DateTime? value) => GetHtml(value ?? DateTime.MinValue);
            public override JToken ToJson(DateTime? value) => value?.ToString("yyyy-MM-dd");
        }

        private sealed class ByteArrayConverter : Converter<Tuple<string, byte[]>>
        {
            public override bool CanConvertToPlainText() => false;
            public override bool CanConvertToJson() => false;

            public override string ToPlainText(Tuple<string, byte[]> value) => null;
            public override string ToHtml(Tuple<string, byte[]> value)
            {
                if (value == null) return string.Empty;
                else if (value.Item2 == null || value.Item2.Length == 0) return @"<input type=""checkbox"" disabled>";
                return $@"<input type=""checkbox"" name=""{value.Item1}"" value=""{Convert.ToBase64String(value.Item2)}"">";
            }
            public override JToken ToJson(Tuple<string, byte[]> value) => null;
        }

        private sealed class CurrencyAmountConverter : Converter<Tuple<decimal, ManagerServer.Model.Currency>>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(Tuple<decimal, ManagerServer.Model.Currency> value) => value?.Item1.ToCurrencyString(value.Item2, CurrencySymbol.Short);
            public override string ToHtml(Tuple<decimal, ManagerServer.Model.Currency> value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(Tuple<decimal, ManagerServer.Model.Currency> value)
            {
                var output = new JObject();
                output["value"] = value.Item1;
                output["currency"] = value.Item2?.DisplayCode;
                return output;
            }
        }

        private sealed class DebitCreditAmountConverter : Converter<DebitCreditAmount>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(DebitCreditAmount value) => value.Value.ToCurrencyStringAsDrCr(null, CurrencySymbol.None);
            public override string ToHtml(DebitCreditAmount value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(DebitCreditAmount value) => value.Value;
        }

        private sealed class CurrencyDebitCreditAmountConverter : Converter<Tuple<DebitCreditAmount, Currency>>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(Tuple<DebitCreditAmount, Currency> value) => value.Item1.Value.ToCurrencyStringAsDrCr(value.Item2, CurrencySymbol.Short);
            public override string ToHtml(Tuple<DebitCreditAmount, Currency> value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(Tuple<DebitCreditAmount, Currency> value) => value.Item1.Value;
        }

        private sealed class CurrencyDebitCreditAmountWithHyperlinkConverter : Converter<Tuple<DebitCreditAmount, Currency, BusinessTemplate>>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(Tuple<DebitCreditAmount, Currency, BusinessTemplate> value) => value.Item1.Value.ToCurrencyStringAsDrCr(value.Item2, CurrencySymbol.Short);
            public override string ToHtml(Tuple<DebitCreditAmount, Currency, BusinessTemplate> value) => @$"<a href=""{value.Item3.ToUrl()}"">{ToPlainText(value)}</a>";
            public override JToken ToJson(Tuple<DebitCreditAmount, Currency, BusinessTemplate> value) => value.Item1.Value;
        }

        private sealed class NullableCurrencyAmountWithHyperlinkConverter : Converter<Tuple<decimal?, Currency, BusinessTemplate>>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(Tuple<decimal?, Currency, BusinessTemplate> value) => value.Item1.HasValue ? value.Item1.Value.ToCurrencyString(value.Item2, CurrencySymbol.Short) : "?";
            public override string ToHtml(Tuple<decimal?, Currency, BusinessTemplate> value) => @$"<a href=""{value.Item3.ToUrl()}"">{ToPlainText(value)}</a>";
            public override JToken ToJson(Tuple<decimal?, Currency, BusinessTemplate> value) => value.Item1.Value;
        }

        private sealed class NamedObjectConverter : Converter<NamedObject>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(NamedObject value) => value?.GetCodeAndName();
            public override string ToHtml(NamedObject value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(NamedObject value)
            {
                var output = new JObject();
                output["key"] = value.Key;
                output["name"] = value.GetCodeAndName();
                return output;
            }
        }

        private sealed class DoubleCurrencyAmountConverter : Converter<Tuple<Tuple<decimal, ManagerServer.Model.Currency>, Tuple<decimal, ManagerServer.Model.Currency>>>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => false;

            public override string ToPlainText(Tuple<Tuple<decimal, ManagerServer.Model.Currency>, Tuple<decimal, ManagerServer.Model.Currency>> value)
            {
                if (value == null) return null;
                var currencyAmounts = new [] { value.Item1, value.Item2 };
                return string.Join(" = ", currencyAmounts.Where(x => x != null).Select(x => x.Item1.ToCurrencyString(x.Item2, CurrencySymbol.Short)));
            }

            public override string ToHtml(Tuple<Tuple<decimal, ManagerServer.Model.Currency>, Tuple<decimal, ManagerServer.Model.Currency>> value) => $"<span>{ToPlainText(value)}</span>";
            public override JToken ToJson(Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>> value)
            {
                return null;
            }

            /*
            public override JToken ToJson(Tuple<Tuple<decimal, Manager.Model.Currency>, Tuple<decimal, Manager.Model.Currency>> value)
            {
                var output = new JObject();
                output["value"] = value.Item1;
                output["currency"] = value.Item2?.DisplayCode;
                return output;
            }
            */
        }

        private sealed class DoubleCurrencyAmountWithHyperlinkConverter : Converter<Tuple<Tuple<decimal, ManagerServer.Model.Currency>, Tuple<decimal, ManagerServer.Model.Currency>, BusinessTemplate>>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => false;

            public override string ToPlainText(Tuple<Tuple<decimal, ManagerServer.Model.Currency>, Tuple<decimal, ManagerServer.Model.Currency>, BusinessTemplate> value)
            {
                if (value == null) return null;
                var currencyAmounts = new[] { value.Item1, value.Item2 };
                var text = string.Join(" = ", currencyAmounts.Where(x => x != null).Select(x => x.Item1.ToCurrencyString(x.Item2, CurrencySymbol.Short)));
                if (string.IsNullOrWhiteSpace(text)) text = "?";
                return text;
            }

            public override string ToHtml(Tuple<Tuple<decimal, ManagerServer.Model.Currency>, Tuple<decimal, ManagerServer.Model.Currency>, BusinessTemplate> value)
            {
                if (value.Item3 != null)
                {
                    return @$"<a href=""{value.Item3.ToUrl()}"">{ToPlainText(value)}</a>";
                }
                return ToPlainText(value);
            }

            public override JToken ToJson(Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate> value)
            {
                return null;
            }

            /*
            public override JToken ToJson(Tuple<Tuple<decimal, Manager.Model.Currency>, Tuple<decimal, Manager.Model.Currency>> value)
            {
                var output = new JObject();
                output["value"] = value.Item1;
                output["currency"] = value.Item2?.DisplayCode;
                return output;
            }
            */
        }

        private sealed class CurrencyAmountWithHyperlinkConverter : Converter<Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> value) => value?.Item1.ToCurrencyString(value.Item2, CurrencySymbol.Short);
            public override string ToHtml(Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> value)
            {
                if (value.Item3 != null) return @$"<a href=""{value.Item3.ToUrl()}"">{ToPlainText(value)}</a>";
                else return ToPlainText(value);
            }
            public override JToken ToJson(Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> value)
            {
                var output = new JObject();
                output["value"] = value.Item1;
                output["currency"] = value.Item2?.DisplayCode;
                return output;
            }
        }

        private sealed class NumberWithHyperlinkConverter : Converter<Tuple<int, BusinessTemplate>>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(Tuple<int, BusinessTemplate> value) => value?.Item1.ToString();
            public override string ToHtml(Tuple<int, BusinessTemplate> value) => @$"<a href=""{value.Item2.ToUrl()}"">{ToPlainText(value)}</a>";
            public override JToken ToJson(Tuple<int, BusinessTemplate> value) => value?.Item1;
        }

        private sealed class DecimalWithHyperlinkConverter : Converter<Tuple<decimal, BusinessTemplate>>
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(Tuple<decimal, BusinessTemplate> value) => value?.Item1.ToNumberString();
            public override string ToHtml(Tuple<decimal, BusinessTemplate> value) => value.Item2 != null ? @$"<a href=""{value.Item2.ToUrl()}"">{ToPlainText(value)}</a>" : ToPlainText(value);
            public override JToken ToJson(Tuple<decimal, BusinessTemplate> value) => value?.Item1;
        }

        private sealed class EnumConverter<TEnum> : Converter<TEnum> where TEnum : Enum
        {
            public override bool CanConvertToPlainText() => true;
            public override bool CanConvertToJson() => true;

            public override string ToPlainText(TEnum value) => ManagerServer.Globalization.Strings.GetPropertyValue(value.ToString());
            public override string ToHtml(TEnum value)
            {
                var text = ToPlainText(value);

                string @class = null;
                if (value.GetType().GetMember(value.ToString()).FirstOrDefault()?.GetCustomAttribute<ManagerServer.Model.Attributes.DangerAttribute>() != null)
                {
                    @class = "bg-red-500";
                }
                else if (value.GetType().GetMember(value.ToString()).FirstOrDefault()?.GetCustomAttribute<ManagerServer.Model.Attributes.SuccessAttribute>() != null)
                {
                    @class = "bg-green-500";
                }
                else if (value.GetType().GetMember(value.ToString()).FirstOrDefault()?.GetCustomAttribute<ManagerServer.Model.Attributes.PrimaryAttribute>() != null)
                {
                    @class = "bg-blue-500";
                }
                else if (value.GetType().GetMember(value.ToString()).FirstOrDefault()?.GetCustomAttribute<ManagerServer.Model.Attributes.WarningAttribute>() != null)
                {
                    @class = "bg-amber-500";
                }
                else
                {
                    @class = "bg-neutral-400";
                }

                return $@"<div class=""inline-block text-white rounded font-semibold px-2.5 py-0.5 rounded {@class}"">{text}</div>";
            }
            public override JToken ToJson(TEnum value) => value.ToString();
        }

        private sealed class BusinessTemplateConverter : Converter<BusinessTemplate>
        {
            public override bool CanConvertToPlainText() => false;
            public override bool CanConvertToJson() => false;

            public override string ToPlainText(BusinessTemplate value) => null;
            public override string ToHtml(BusinessTemplate value)
            {
                if (value != null)
                {
                    var text = Strings.Edit;
                    if (value.GetType().Name.EndsWith("View")) text = Strings.View;

                    return $@"<a href=""{value.ToUrl()}"" class=""btn btn-sm"">{text}</a>";
                }
                else
                {
                    return null;
                }
            }
            public override JToken ToJson(BusinessTemplate value) => null;
        }

        public sealed class DefaultAttribute : Attribute { }
        public sealed class BoldAttribute : Attribute { }
        public sealed class WarnIfNotUniqueAttribute : Attribute { }
        public sealed class WarnIfNegativeAttribute : Attribute { }
        public sealed class CenterAttribute : Attribute { }
        public sealed class RightAttribute : Attribute { }
        public sealed class MinWidthAttribute : Attribute { }        
        public sealed class HideColumnIfAllEmptyAttribute : Attribute { }
        public sealed class WhitespaceNoWrapAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
        public sealed class GuidAttribute : Attribute
        {
            public Guid Value { get; init; }
            public GuidAttribute(string guid) => Value = new Guid(guid);
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
        public sealed class NameAttribute : Attribute
        {
            public string[] Value { get; init; }
            public NameAttribute(params string[] value) => Value = value;
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public sealed class PriorityAttribute : Attribute
        {
            public int Value { get; init; }
            public PriorityAttribute(int value) => Value = value;
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
        public sealed class IconAttribute : Attribute
        {
            public string Value { get; init; }
            public IconAttribute(string value) => Value = value;
        }

        public class Percentage : IComparable
        {
            public decimal Value;

            int IComparable.CompareTo(object obj)
            {
                if (obj is Percentage percentage) return Value.CompareTo(percentage.Value);
                return 0;
            }
        }

        public class QrCode : IComparable
        {
            public string Value;

            int IComparable.CompareTo(object obj)
            {
                if (obj is QrCode qrCode) return Value.CompareTo(qrCode.Value);
                return 0;
            }
        }

        private sealed class ValueChange : Converter<Tuple<decimal, decimal, Currency>>
        {
            public override bool CanConvertToPlainText() => false;
            public override bool CanConvertToJson() => false;

            public override string ToPlainText(Tuple<decimal, decimal, Currency> value) => null;
            public override string ToHtml(Tuple<decimal, decimal, Currency> value)
            {
                var currencyAmountConverter = new CurrencyAmountConverter();

                if (value.Item1 != value.Item2)
                {
                    return @$"{currencyAmountConverter.ToHtml(new Tuple<decimal, Currency>(value.Item1, value.Item3))}<i class=""fas fa-arrow-right mx-2""></i><span class=""bg-green-600 text-white rounded px-2 py-1"">{currencyAmountConverter.ToHtml(new Tuple<decimal, Currency>(value.Item2, value.Item3))}<span>";
                }
                else
                {
                    return @$"{currencyAmountConverter.ToHtml(new Tuple<decimal, Currency>(value.Item1, value.Item3))}";
                }
            }
            public override JToken ToJson(Tuple<decimal, decimal, Currency> value) => null;
        }

        public struct DebitCreditAmount : IComparable
        {
            public decimal Value;

            public DebitCreditAmount(decimal value) => Value = value;

            int IComparable.CompareTo(object obj)
            {
                if (obj is DebitCreditAmount debitCreditAmount) return Value.CompareTo(debitCreditAmount.Value);
                return 0;
            }
        }
    }
}