using ManagerComponents;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model;
using MimeKit.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class ObjectTable<T> : BusinessTemplate
    {
        [InheritedProtoMember(201)] public int Skip { get; set; }
        [InheritedProtoMember(202)] public int? Take { get; set; }
        [InheritedProtoMember(203)] public Guid? SortBy { get; set; }
        [InheritedProtoMember(204)] public bool Desc { get; set; }
        [InheritedProtoMember(205)] public string Term { get; set; }
        [InheritedProtoMember(206)] public bool BatchView { get; set; }
        [InheritedProtoMember(207)] public int? Redirect;

        private int GetTakeOrDefault() => Take ?? 50;
        protected override Task InnerPost() => Get();
        protected virtual T[] Filter(T[] rows) => rows;
        protected virtual T[] GetObjects() => Array.Empty<T>();
        protected virtual bool IsInactive(T row) => false;
        protected virtual HeaderButton GetPrimaryButton() => null;
        protected virtual decimal? GetBalanceMovement(T row) => null;
        protected virtual void OnTable(ManagerComponents.Table table, T[] rows) {}
        protected virtual void OnFooter(ManagerComponents.Footer footer) {}

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
                        Write(new Panel()
                        {
                            Text = Strings.BatchView,
                            PrimaryButton = new HeaderButton() { Text = Strings.Print, Url = "javascript:window.print()" }
                        });


                        var batchViewUrls = batchView.Select(x => UTF8Encoding.UTF8.GetString(Convert.FromBase64String(x))).ToArray();
                        using (Div(@class: "card-inset flex flex-col gap-4"))
                        {
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
                var header = new ManagerComponents.Panel()
                {
                    Text = string.Join(" &mdash; ", titleAttribute.Text.Select(x => Strings.GetPropertyValue(x))),
                    HelpUrl = GetHelpUrl(),
                    PrimaryButton = GetPrimaryButton()
                };

                header.EndElements.Add(new InputText() { Form = nameof(Term), Name = nameof(Term), Placeholder = Strings.Search, Value = Term });
                header.EndElements.Add(new HeaderButton() { Form = nameof(Term), Text = Strings.Search, Url = this.ToUrl() });

                Write(header);

                var rows = GetObjects();
                rows = Filter(rows);

                if (!string.IsNullOrWhiteSpace(Term))
                {
                    var total = rows.Length;
                    rows = FilterByTerm(rows, Term);
                    var excluded = total - rows.Length;

                    if (excluded > 0)
                    {
                        var text = string.Format(Strings.HiddenRowsCount, $"<b>{excluded}</b>", $"<b>{HtmlUtils.HtmlEncode(Term)}</b>");

                        var term = this.Term;
                        this.Term = null;
                        var cancelUrl = this.ToUrl();
                        this.Term = term;

                        Write(new Notice() { Text = text, CancelUrl = cancelUrl });
                    }
                }

                if (Redirect.HasValue)
                {
                    var row = rows.ElementAtOrDefault(Redirect.Value);
                    if (row != null)
                    {
                        var clone = (ObjectTable<T>)this.MemberwiseClone();
                        clone.Redirect = null;
                        var view = WithIndex(GetView(row, clone.ToUrl()), Redirect.Value, rows.Length - 1);
                        if (view != null)
                        {
                            using (Script()) Write($"window.location.href = '{view.ToUrl()}'");
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

                if (SortBy.HasValue)
                {
                    columns.SingleOrDefault(x => x.Key == SortBy.Value)?.Sort(rows, Desc);
                }

                var pageSizes = new InputGroup();
                var pageSize = GetTakeOrDefault();
                if (rows.Length <= 50)
                {
                    pageSizes.Children.Add(new FooterButton() { Text = rows.Length.ToString() });
                }
                else
                {
                    var clone = (ObjectTable<T>)this.MemberwiseClone();
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
                if (!SortBy.HasValue && string.IsNullOrWhiteSpace(Term) && rows.Any(x => GetBalanceMovement(x).HasValue))
                {
                    closingBalance = rows.Skip(Skip).Sum(x => GetBalanceMovement(x) ?? 0m);
                }

                var totalRows = rows.Length;
                var pagination = GetPagination(totalRows);

                rows = rows.Skip(Skip).Take(GetTakeOrDefault()).ToArray();

                if (rows.Length == 0)
                {
                    Write(new Empty() { Text = Strings.Empty });
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
                            Text = Strings.BatchView,
                            Checkbox = true,
                            MinWidth = true,
                            WhitespaceNoWrap = true,
                            Cells = rows.Select(x => new TableCell()
                            {
                                Checkbox = new Tuple<string, byte[]>(nameof(BatchView), UTF8Encoding.UTF8.GetBytes(WithIframeContentOnly(GetView(x, null)) ?? string.Empty))
                            }).ToArray()
                        });
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

                    if (attachmentColumn.Cells.Any(x => x.Icon != null)) table.Columns.Add(attachmentColumn);

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
                        var link = (ObjectTable<T>)this.MemberwiseClone();
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
                            Text = e.Name,
                            Url = e.Key.HasValue ? link.ToUrl() : null,
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

                    OnTable(table, rows);

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

                    var clone = (ObjectTable<T>)this.MemberwiseClone();
                    clone.BatchView = false;
                    batchViewFooter.StartElements.Add(new HeaderButton() { Style = HeaderButton.ButtonStyle.Secondary, Text = Strings.Cancel, Url = clone.ToUrl() });

                    Write(batchViewFooter);
                }

                var footer = new ManagerComponents.Footer();
                footer.StartElements.Add(pageSizes);

                var batchOperationsButton = new FooterButton() { Text = Strings.BatchOperations };

                if (!BatchView)
                {
                    var clone = (ObjectTable<T>)this.MemberwiseClone();
                    clone.BatchView = true;
                    batchOperationsButton.Menu.Add(new Tuple<string, string>(Strings.BatchView, clone.ToUrl()));
                }

                OnBatchOperationsButton(batchOperationsButton);
                footer.EndElements.Add(batchOperationsButton);

                footer.EndElements.Add(new CopyToClipboardButton() { Text = Strings.Copy_to_clipboard });

                OnFooter(footer);
                Write(footer);
            }
        }

        protected virtual void OnBatchOperationsButton(FooterButton batchOperationsButton)
        {
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
            if (businessTemplate == null) return null;

            if (businessTemplate is BaseView3 baseView3)
            {
                baseView3.HttpContext = HttpContext;
                return baseView3.GetIframeUrl();
            }
            else
            {
                businessTemplate.ContentOnlyForIframe = true;
                return businessTemplate.ToUrl();
            }
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
            public Guid? Key { get; set; }
            public string Name { get; set; }
            public abstract void Sort(T[] rows, bool desc);
            public abstract string GetHtml(T row);
            public abstract decimal GetValue(T row);
            public abstract string GetSum(IEnumerable<T> rows);
        }

        public class Column<TOut> : BaseColumn
        {
            public Func<T, TOut> Function { get; set; }

            public Column(object instance, MethodInfo methodInfo)
            {
                Center = methodInfo.GetCustomAttribute<CenterAttribute>() != null;
                Right = methodInfo.GetCustomAttribute<RightAttribute>() != null;
                MinWidth = methodInfo.GetCustomAttribute<MinWidthAttribute>() != null;
                WhitespaceNoWrap = methodInfo.GetCustomAttribute<WhitespaceNoWrapAttribute>() != null;
                HideColumnIfAllEmpty = methodInfo.GetCustomAttribute<HideColumnIfAllEmptyAttribute>() != null;
                Bold = methodInfo.GetCustomAttribute<BoldAttribute>() != null;
                TabularNums = methodInfo.GetCustomAttribute<TabularNumsAttribute>() != null;
                RedIfNegative = methodInfo.GetCustomAttribute<RedIfNegativeAttribute>() != null;
                Sum = methodInfo.GetCustomAttribute<SumAttribute>() != null;
                Key = methodInfo.GetCustomAttribute<GuidAttribute>()?.Value;

                var nameAttribute = methodInfo.GetCustomAttribute<NameAttribute>()?.Value ?? [ methodInfo.Name.Substring(3) ];
                Name = string.Join(" &mdash ", nameAttribute.Select(x => Strings.GetPropertyValue(x)));

                Function = methodInfo.CreateDelegate<Func<T, TOut>>(instance);
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
                return 0m;
            }

            public override string GetSum(IEnumerable<T> rows)
            {
                if (this is Column<decimal> decimalColumn)
                {
                    return rows.Sum(x => decimalColumn.Function(x)).ToNumberString();
                }
                if (this is Column<int> intColumn)
                {
                    return rows.Sum(x => intColumn.Function(x)).ToString();
                }
                if (this is Column<Tuple<decimal, Currency>> currencyColumn)
                {
                    var lines = rows
                        .Select(currencyColumn.Function)
                        .GroupBy(t => t.Item2.Key)
                        .Select(g =>
                        {
                            var sum = g.Sum(t => t.Item1);
                            var currency = g.First().Item2;
                            return sum.ToCurrencyString(currency, CurrencySymbol.Short);
                        });

                    return string.Join("<br />", lines);
                }
                return string.Empty;
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

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public sealed class NewButtonAttribute : Attribute
        {
            public string Value { get; init; }
            public NewButtonAttribute(string value) => Value = value;
        }

        protected virtual bool GetAttachment(T o)
        {
            return false;
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

            var methods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetParameters().Length == 1)
                .Where(x => x.GetParameters()[0].ParameterType == typeof(T))
                .ToArray();

            foreach (var e in methods)
            {
                var columnType = typeof(Column<>).MakeGenericType(typeof(T), e.ReturnType);
                var column = (BaseColumn)Activator.CreateInstance(columnType, [ this, e ]);
                columns.Add(column);
            }

            return columns;
        }

        private T[] FilterByTerm(T[] rows, string term)
        {
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
    }
}