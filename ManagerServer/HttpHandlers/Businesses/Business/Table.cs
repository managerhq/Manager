using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerComponents;
using ManagerServer.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class Table<T> : BusinessTemplate
    {
        [InheritedProtoMember(201)] public int Skip { get; set; }
        [InheritedProtoMember(202)] public int? Take { get; set; }
        [InheritedProtoMember(203)] public string SortBy { get; set; }
        [InheritedProtoMember(204)] public bool Desc { get; set; }
        [InheritedProtoMember(205)] public string Term { get; set; }
        [InheritedProtoMember(206)] public bool BatchView { get; set; }
        [InheritedProtoMember(207)] public int? Redirect;

        private int GetTakeOrDefault() => Take ?? 50;
        protected virtual T[] GetObjects() => Array.Empty<T>();
        protected virtual bool IsInactive(T row) => false;
        protected virtual HeaderButton GetPrimaryButton() => null;
        protected virtual decimal? GetBalanceMovement(T row) => null;

        protected override void InnerGet2()
        {
            if (Request.HasFormContentType)
            {
                var form = Request.ReadFormAsync().GetAwaiter().GetResult();

                if (form.TryGetValue(nameof(Term), out var term))
                {
                    this.Skip = 0;
                    this.Term = term;
                    Response.Redirect(this.ToUrl());
                    return;
                }

                if (form.TryGetValue(nameof(BatchView), out var batchView))
                {
                    using (Div(@class: "card"))
                    {
                        using (Div(@class: "card-header flex items-center gap-2 print:hidden"))
                        {
                            using (Div(@class: "card-title")) Write(Strings.BatchView);
                            using (A(href: "javascript:window.print()", @class: "btn")) Write(Strings.Print);
                        }

                        using (Div(@class: "card-inset flex flex-col gap-4"))
                        {
                            var batchViewUrls = batchView.Select(x => UTF8Encoding.UTF8.GetString(Convert.FromBase64String(x))).ToArray();
                            foreach (var e in batchViewUrls)
                            {
                                using (IFrame(src: e, @class: "w-full", onload: "autoResizeIframe(this)", loading: "lazy"))
                                {
                                }
                            }
                        }
                    }

                    return;
                }
            }

            var referrer = this.ToUrl();

            var titleAttribute = this.GetType().GetCustomAttribute<TitleAttribute>();

            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header flex justify-between gap-4"))
                {
                    using (Div(@class: "flex items-center gap-4"))
                    {
                        using (Div(@class: "flex items-center gap-2"))
                        {
                            using (Div(@class: "card-title"))
                            {
                                Write(Strings.GetPropertyValue(titleAttribute.Text.Last()));
                            }
                            WriteHelp();
                        }
                        var primaryButton = GetPrimaryButton();
                        if (primaryButton != null)
                        {
                            using (A(href: primaryButton.Url, @class: "btn")) Write(primaryButton.Text);
                        }
                    }
                    using (Form(method: "POST", action: this.ToUrl(), hxBoost: true, hxDisabledElt: "find button"))
                    {
                        using (Div(@class: "input-group"))
                        {
                            InputText(name: nameof(Term), value: Term, @class: "form-control", placeholder: Strings.Search);
                            using (Button(@class: "btn"))
                            {
                                Write(Strings.Search);
                                I(@class: "htmx-indicator fas fa-circle-notch fa-spin ms-2 !hidden");
                            }
                        }
                    }
                }

                var rows = GetObjects();
                var rowsBeforeFilter = rows.Length;
                rows = Filter(rows);
                var rowsAfterFilter = rows.Length;

                if (rowsBeforeFilter != rowsAfterFilter)
                {
                    using (Div(@class: "card-header p-0"))
                    {
                        using (Div(@class: "flex items-center"))
                        {
                            var cancelUrl = (Table<T>)this.MemberwiseClone();
                            cancelUrl.Term = null;
                            cancelUrl.Skip = 0;
                            using (A(href: cancelUrl.ToUrl(), @class: "py-4 px-6 text-(--muted-foreground) opacity-25 hover:opacity-50"))
                            {
                                I(@class: "fas fa-xmark text-base");
                            }
                            using (Div(@class: "vertical-divider"))
                            {
                            }
                            using (Div(@class: "px-4"))
                            {
                                Write(string.Format(Strings.HiddenRowsCount, "<b>"+(rowsBeforeFilter-rowsAfterFilter).ToString()+"</b>", "<q>"+Term+"</q>"));
                            }
                        }
                    }
                }

                if (Redirect.HasValue)
                {
                    var row = rows.ElementAtOrDefault(Redirect.Value);
                    if (row != null)
                    {
                        var clone = (Table<T>)this.MemberwiseClone();
                        clone.Redirect = null;
                        var view = WithIndex(GetView(row, clone.ToUrl()), Redirect.Value, rows.Length - 1);
                        if (view != null)
                        {
                            StringBuilder.Clear();
                            Response.Redirect(view.ToUrl());
                            return;
                        }
                    }
                }

                var columns = GetColumns();
                foreach (var e in columns.ToArray())
                {
                    if (e.HideColumnIfAllEmpty)
                    {
                        if (rows.All(x => string.IsNullOrWhiteSpace(e.GetHtml(x))))
                        {
                            columns.Remove(e);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(SortBy))
                {
                    columns.SingleOrDefault(x => x.Key == SortBy)?.Sort(rows, Desc);
                }

                var pageSizes = new InputGroup();
                var pageSize = GetTakeOrDefault();
                if (rows.Length <= 50)
                {
                    pageSizes.Children.Add(new FooterButton() { Text = rows.Length.ToString() });
                }
                else
                {
                    var clone = (Table<T>)this.MemberwiseClone();
                    foreach (var e in new[] { 50, 100, 250, 500, 1000, int.MaxValue })
                    {
                        clone.Skip = 0;
                        clone.Take = e;
                        var url = clone.ToUrl();
                        if (clone.Take == pageSize) url = null;

                        if (e == int.MaxValue)
                        {
                            pageSizes.Children.Add(new FooterButton() { Text = rows.Length.ToString(), Url = url });
                        }
                        else if (e <= rows.Length)
                        {
                            pageSizes.Children.Add(new FooterButton() { Text = e.ToString(), Url = url });
                        }
                        if (rows.Length == e) break;
                    }
                }

                decimal? closingBalance = null;
                if (string.IsNullOrWhiteSpace(SortBy) && string.IsNullOrWhiteSpace(Term) && rows.Any(x => GetBalanceMovement(x).HasValue))
                {
                    closingBalance = rows.Skip(Skip).Sum(x => GetBalanceMovement(x) ?? 0m);
                }

                var totalRows = rows.Length;
                var pagination = GetPagination(totalRows);

                rows = rows.Skip(Skip).Take(GetTakeOrDefault()).ToArray();

                if (rows.Length == 0)
                {
                    using (Div(@class: "card-inset p-24 text-center"))
                    {
                        using (Span(@class: "card-title text-xl"))
                        {
                            Write(Strings.Empty);
                        }
                    }
                }
                else
                {
                    if (Skip > 0 && pagination != null) Write(pagination);

                    var table = new ManagerComponents.Table();
                    table.Term = Term;

                    if (BatchView)
                    {
                        table.Columns.Insert(0, new TableColumn()
                        {
                            Text = string.Empty,
                            Checkbox = true,
                            MinWidth = true,
                            WhitespaceNoWrap = true,
                            Cells = rows.Select(x => new TableCell()
                            {
                                Checkbox = new Tuple<string, byte[]>(nameof(BatchView), UTF8Encoding.UTF8.GetBytes(WithIframeContentOnly(GetView(x, null)) ?? string.Empty))
                            }).ToArray()
                        });
                    }
                    else
                    {
                        var checkboxColumn = new TableColumn()
                        {
                            Text = string.Empty,
                            Checkbox = true,
                            MinWidth = true,
                            WhitespaceNoWrap = true,
                            Cells = rows.Select(x => new TableCell()
                            {
                                Checkbox = new Tuple<string, byte[]>("CustomCheckbox", GetCheckbox(x))
                            }).ToArray()
                        };

                        if (checkboxColumn.Cells.Any(x => x.Checkbox.Item2 != null)) table.Columns.Insert(0, checkboxColumn);
                    }

                    var attachmentColumn = new TableColumn()
                    {
                        Icon = "fa-paperclip",
                        Text = Strings.Attachment,
                        MinWidth = true,
                        WhitespaceNoWrap = true,
                        Cells = rows.Select(x => new TableCell()
                        {
                            Inactive = IsInactive(x),
                            Icon = GetAttachment(x) ? "fa-paperclip" : null
                        }).ToArray()
                    };

                    if (attachmentColumn.Cells.Any(x => x.TextValue != null)) table.Columns.Add(attachmentColumn);

                    var editColumn = new TableColumn()
                    {
                        Icon = "fa-edit",
                        Text = Strings.Edit,
                        MinWidth = true,
                        WhitespaceNoWrap = true,
                        Cells = rows.Select(x => new TableCell()
                        {
                            Inactive = IsInactive(x),
                            CellButton = new TableCellButton()
                            {
                                Text = Strings.Edit,
                                Url = GetEdit(x, referrer)?.ToUrl()
                            }
                        }).ToArray()
                    };

                    if (editColumn.Cells.Any(x => x.CellButton.Url != null)) table.Columns.Add(editColumn);

                    var viewColumn = new TableColumn()
                    {
                        Icon = "fa-eye",
                        Text = Strings.View,
                        MinWidth = true,
                        WhitespaceNoWrap = true,
                        Cells = rows.Index().Select(x => new TableCell()
                        {
                            Inactive = IsInactive(x.Item),
                            CellButton = new TableCellButton()
                            {
                                Text = Strings.View,
                                Url = WithIndex(GetView(x.Item, referrer), x.Index + Skip, totalRows - 1)?.ToUrl()
                            }
                        }).ToArray()
                    };

                    if (viewColumn.Cells.Any(x => x.CellButton.Url != null)) table.Columns.Add(viewColumn);

                    foreach (var e in columns)
                    {
                        var link = (Table<T>)this.MemberwiseClone();
                        link.Skip = 0;
                        if (link.SortBy == e.Key)
                        {
                            link.Desc = !link.Desc;
                        }
                        else
                        {
                            link.SortBy = e.Key;
                            link.Desc = false;
                        }
                        table.Columns.Add(new TableColumn()
                        {
                            Checkbox = e is Column<byte[]>,
                            Text = Strings.GetPropertyValue(e.Key),
                            Url = link.ToUrl(),
                            Desc = SortBy == e.Key ? Desc : null,
                            Center = e.Center,
                            MinWidth = e.MinWidth,
                            TabularNums = e.TabularNums,
                            WhitespaceNoWrap = e.WhitespaceNoWrap,
                            Bold = e.Bold,
                            Right = e.Right,
                            Sum = e.Sum ? rows.Sum(x => e.GetValue(x)).ToNumberString() : null,
                            Cells = rows.Select(x => new TableCell()
                            {
                                TextValue = e.GetHtml(x),
                                Inactive = IsInactive(x)
                            }).ToArray()
                        });
                    }

                    if (closingBalance.HasValue)
                    {
                        var cells = new List<TableCell>();
                        foreach (var e in rows)
                        {
                            var movement = GetBalanceMovement(e) ?? 0m;

                            var textValue = @"<span class=""opacity-50"">" + closingBalance.ToNumberString() + "<span>";
                            if (closingBalance < 0m) textValue = @$"<span class=""text-red-600"">{textValue}</span>";

                            cells.Add(new TableCell()
                            {
                                TextValue = textValue
                            });
                            closingBalance -= movement;
                        }

                        table.Columns.Add(new TableColumn()
                        {
                            Text = Strings.Balance,
                            TabularNums = true,
                            WhitespaceNoWrap = true,
                            Right = true,
                            Cells = cells.ToArray()
                        });
                    }

                    Write(table);

                    if (pagination != null)
                    {
                        Write(pagination);
                    }
                }

                if (BatchView)
                {
                    var batchViewFooter = new ManagerComponents.Panel();
                    batchViewFooter.IsActionBar = true;
                    batchViewFooter.StartElements.Add(new HeaderButton() { Style = HeaderButton.ButtonStyle.Info, Text = Strings.BatchView, Url = this.ToUrl(), Form = nameof(BatchView) });

                    var clone = (Table<T>)this.MemberwiseClone();
                    clone.BatchView = false;
                    batchViewFooter.StartElements.Add(new HeaderButton() { Style = HeaderButton.ButtonStyle.Secondary, Text = Strings.Cancel, Url = clone.ToUrl() });

                    Write(batchViewFooter);
                }
                else
                {
                    if (rows.Any(x => GetCheckbox(x) != null))
                    {
                        using (Div(@class: "card-header flex items-center gap-4"))
                        {
                            I(@class: "fas fa-fw fa-turn-up fa-rotate-90 text-2xl opacity-50");
                            using (Form(method: "POST", action: this.ToUrl(), hxBoost: true, hxDisabledElt: "find button", id: "CustomCheckbox"))
                            {
                                using (Button(@class: "btn btn-success"))
                                {
                                    Write(Strings.BatchUpdate);
                                    I(@class: "htmx-indicator fas fa-circle-notch fa-spin ms-2 !hidden");
                                }
                            }
                        }
                    }
                }

                var footer = new ManagerComponents.Footer();
                footer.StartElements.Add(pageSizes);

                var batchOperationsButton = new FooterButton() { Text = Strings.BatchOperations };

                if (!BatchView)
                {
                    var clone = (Table<T>)this.MemberwiseClone();
                    clone.BatchView = true;
                    batchOperationsButton.Menu.Add(new Tuple<string, string>(Strings.BatchView, clone.ToUrl()));
                }

                OnBatchOperationsButton(batchOperationsButton);
                footer.EndElements.Add(batchOperationsButton);

                footer.EndElements.Add(new CopyToClipboardButton() { Text = Strings.Copy_to_clipboard });

                Write(footer);
            }
        }

        protected override async Task InnerPost()
        {
            var form = await Request.ReadFormAsync();
            if (form.ContainsKey("CustomCheckbox"))
            {
                var values = form["CustomCheckbox"].Select(x => Convert.FromBase64String(x)).ToArray();
                OnCustomCheckbox(values);
            }

            await Get();
        }

        protected virtual void OnCustomCheckbox(byte[][] values)
        {
        }

        protected virtual void OnBatchOperationsButton(FooterButton batchOperationsButton)
        {
        }

        protected virtual T[] Filter(T[] rows)
        {
            if (string.IsNullOrWhiteSpace(Term)) return rows;

            var keywords = Term.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            var methods = GetColumns();

            var output = new List<T>();

            foreach (var e in rows)
            {
                var keywords2 = new bool[keywords.Length];
                foreach (var e2 in methods)
                {
                    var text = e2.GetHtml(e);

                    if (string.IsNullOrWhiteSpace(text)) continue;

                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (text.Contains(keywords[i], StringComparison.OrdinalIgnoreCase))
                        {
                            keywords2[i] = true;
                        }
                    }
                }

                if (keywords2.All(x => x))
                {
                    output.Add(e);
                }
            }

            return output.ToArray();
        }

        private BusinessTemplate WithIndex(BusinessTemplate businessTemplate, int position, int maxPosition)
        {
            if (businessTemplate is BaseView3 baseView3)
            {
                baseView3.Position = position;
                baseView3.MaxPosition = maxPosition;
            }
            return businessTemplate;
        }

        private string WithIframeContentOnly(BusinessTemplate businessTemplate)
        {
            if (businessTemplate is BaseView3 baseView3)
            {
                baseView3.HttpContext = HttpContext;
                return baseView3.GetIframeUrl();
            }

            businessTemplate.ContentOnlyForIframe = true;
            return businessTemplate.ToUrl();
        }

        private Panel GetPagination(int total)
        {
            if (total <= GetTakeOrDefault()) return null;

            var skip = Skip;
            var take = GetTakeOrDefault();
            var totalPages = Math.DivRem(total, take, out int lastPageCount);
            if (lastPageCount > 0) totalPages++;
            if (lastPageCount == 0) lastPageCount = take;
            int currentPage = 1;
            if (Skip > 0) currentPage = (Skip / take) + 1;

            var panel = new Panel();
            var startInputGroup = new InputGroup();
            var endInputGroup = new InputGroup();
            panel.CenterElements.Add(startInputGroup);
            panel.CenterElements.Add(new PanelLabel() { Text = $"<bdi>{currentPage} / {totalPages}</bdi>" });
            panel.CenterElements.Add(endInputGroup);            

            if (total > take || Skip > 0)
            {
                this.Skip = 0;
                startInputGroup.Children.Add(new HeaderButton() { Icon = "fa-step-backward", Url = currentPage > 1 ? this.ToUrl() : null });
                this.Skip = skip-take;
                startInputGroup.Children.Add(new HeaderButton() { Icon = "fa-backward", Url = currentPage > 1 ? this.ToUrl() : null });
                this.Skip = skip+take;
                endInputGroup.Children.Add(new HeaderButton() { Icon = "fa-forward", Url = currentPage < totalPages ? this.ToUrl() : null });
                this.Skip = (totalPages-1)*take;
                endInputGroup.Children.Add(new HeaderButton() { Icon = "fa-step-forward", Url = currentPage < totalPages ? this.ToUrl() : null });
                this.Skip = skip;
            }

            return panel;
        }

        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            return GetObjects().Length == 0;
        }

        public abstract class BaseColumn
        {
            public bool Center { get; set; }
            public bool Right { get; set; }
            public bool MinWidth { get; set; }
            public bool WhitespaceNoWrap { get; set; }
            public bool HideColumnIfAllEmpty { get; set; }
            public bool TabularNums { get; set; }
            public bool Bold { get; set; }
            public bool Sum { get; set; }
            public bool RedIfNegative { get; set; }
            public string Key { get; set; }
            public abstract void Sort(T[] rows, bool desc);
            public abstract string GetHtml(T row);
            public abstract decimal GetValue(T row);
        }

        public class Column<TOut> : BaseColumn
        {
            public Func<T, TOut> Function { get; set; }

            public Column(PropertyInfo propertyInfo)
            {
                Center = propertyInfo.GetCustomAttribute<CenterAttribute>() != null;
                Right = propertyInfo.GetCustomAttribute<RightAttribute>() != null;
                MinWidth = propertyInfo.GetCustomAttribute<MinWidthAttribute>() != null;
                WhitespaceNoWrap = propertyInfo.GetCustomAttribute<WhitespaceNoWrapAttribute>() != null;
                HideColumnIfAllEmpty = propertyInfo.GetCustomAttribute<HideColumnIfAllEmptyAttribute>() != null;
                Bold = propertyInfo.GetCustomAttribute<BoldAttribute>() != null;
                TabularNums = propertyInfo.GetCustomAttribute<TabularNumsAttribute>() != null;
                RedIfNegative = propertyInfo.GetCustomAttribute<RedIfNegativeAttribute>() != null;
                Sum = propertyInfo.GetCustomAttribute<SumAttribute>() != null;
                Key = propertyInfo.Name;
                Function = propertyInfo.GetGetMethod().CreateDelegate<Func<T, TOut>>();
            }

            public override void Sort(T[] rows, bool desc)
            {
                Array.Sort(rows, (a, b) => Comparer<TOut>.Default.Compare(Function(a), Function(b)));
                if (desc) Array.Reverse(rows);
            }

            public override decimal GetValue(T row)
            {
                var value = Function(row);
                if (value is decimal d) return d;
                if (value is int i) return i;
                if (value is Tuple<decimal, string> d2) return d2.Item1;
                if (value is Tuple<decimal, string, string> d3) return d3.Item1;
                if (value is Tuple<decimal, Currency> d4) return d4.Item1;
                return 0m;
            }

            public override string GetHtml(T row)
            {
                var value = Function(row);
                if (value == null)
                {
                    return string.Empty;
                }
                else if (value is NamedObject namedObject)
                {
                    return namedObject.GetCodeAndName();
                }
                else if (value.GetType().IsEnum)
                {
                    return Strings.GetPropertyValue(value.ToString());
                }
                else if (value is DateTime date)
                {
                    return date.ToLocalShortDisplayString();
                }
                else if (value is byte[] buffer)
                {
                    return $@"<input type=""checkbox"" name=""{Key}"" value=""{Convert.ToBase64String(buffer)}"">";
                }
                else if (value is decimal d)
                {
                    if (RedIfNegative && d < 0m)
                    {
                        return @$"<span class=""text-red-600"">{d.ToNumberString()}</span>";
                    }
                    else
                    {
                        return d.ToNumberString();
                    }
                }
                else if (value is Tuple<decimal, Currency> decimalCurrency)
                {
                    if (RedIfNegative && decimalCurrency.Item1 < 0m)
                    {
                        return @$"<span class=""text-red-600"">{decimalCurrency.Item1.ToCurrencyString(decimalCurrency.Item2, CurrencySymbol.Short)}</span>";
                    }
                    else
                    {
                        return decimalCurrency.Item1.ToCurrencyString(decimalCurrency.Item2, CurrencySymbol.Short);
                    }
                }
                else if (value is Tuple<decimal, string> item)
                {
                    if (RedIfNegative && item.Item1 < 0m)
                    {
                        return @$"<span class=""text-red-600"">{item.Item2}</span>";
                    }
                    else
                    {
                        return item.Item2;
                    }
                }
                else if (value is Tuple<decimal, string, string> linkItem)
                {
                    return @$"<a href=""{linkItem.Item3}"">{linkItem.Item2}</a>";
                }
                else if (value is ICell cell)
                {
                    return cell.ToHtml();
                }
                else
                {
                    return value.ToString();
                }
            }
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
        public sealed class SumAttribute : Attribute { }
        public sealed class RedIfNegativeAttribute : Attribute { }
        public sealed class TabularNumsAttribute : Attribute { }        

        protected virtual bool GetAttachment(T o)
        {
            return false;
        }

        protected virtual byte[] GetCheckbox(T o)
        {
            return null;
        }

        protected virtual BusinessTemplate GetEdit(T o, string referrer)
        {
            return null;
        }

        protected virtual BusinessTemplate GetView(T o, string referrer)
        {
            return null;
        }

        private List<BaseColumn> GetColumns()
        {
            var columns = new List<BaseColumn>();

            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var e in properties)
            {
                var columnType = typeof(Column<>).MakeGenericType(typeof(T), e.PropertyType);
                var column = (BaseColumn)Activator.CreateInstance(columnType, [ e ]);
                columns.Add(column);
            }

            return columns;
        }

        public interface ICell : IComparable<ICell>
        {
            public string ToHtml();
            public object GetComparisonKey();

            int IComparable<ICell>.CompareTo(ICell other)
            {
                if (other is null) return 1;

                var k1 = GetComparisonKey();
                var k2 = other.GetComparisonKey();

                if (k1 is null) return k2 is null ? 0 : -1;
                if (k2 is null) return 1;

                // Prefer same-type comparable
                if (k1.GetType() == k2.GetType() && k1 is IComparable c1)
                {
                    return c1.CompareTo(k2);
                }

                throw new InvalidOperationException("Keys are not comparable.");
            }
        }

        public record LinkButton(string url, string text) : ICell
        {
            public string ToHtml() => $@"<a href=""{url}"" class=""btn btn-sm"">{text}</a>";

            public object GetComparisonKey() => text;
        }

        public record CurrencyAmount(decimal amount, Currency currency) : ICell
        {
            public string ToHtml() => amount.ToCurrencyString(currency, CurrencySymbol.Short);

            public object GetComparisonKey() => amount;
        }

        public record Delta(string oldValue, string newValue) : ICell
        {
            public string ToHtml()
            {
                if (oldValue == newValue) return oldValue;
                return $"<del>{oldValue}</del> <ins>{newValue}</ins>";
            }

            public object GetComparisonKey() => (oldValue, newValue);
        }

        public record StringWithLinkButton(string s, LinkButton linkButton) : ICell
        {
            public string ToHtml() => $@"{s} {linkButton.ToHtml()}";
            public object GetComparisonKey() => s;
        }
    }
}