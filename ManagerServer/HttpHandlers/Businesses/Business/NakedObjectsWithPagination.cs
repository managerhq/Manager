using System;
using ManagerServer.Globalization;
using Newtonsoft.Json;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithPagination : NakedObjectsWithJsonOutput
    {
        [InheritedProtoMember(4030), JsonProperty("skip")] public int Skip;
        [InheritedProtoMember(4031), JsonProperty("pageSize")] public int? PageSize;

        protected int GetPageSize() => PageSize ?? 50;

        protected override void OnAfterHeader(Context context)
        {
            if (Skip > 0) WritePagination(context);
            base.OnAfterHeader(context);
        }

        protected override void OnBeforeFooter(Context context)
        {
            WritePagination(context);
            base.OnBeforeFooter(context);
        }

        internal sealed class Total
        {
            public int Value;
        }

        protected void WritePagination(Context context)
        {
            var total = context.Get<Total>();
            if (total == null) return;

            if (total.Value <= GetPageSize()) return;

            using (Div(@class: "card-header"))
            {
                var take = GetPageSize();

                var handler = (NakedObjectsWithPagination)this.MemberwiseClone();

                if (total.Value > take || Skip > 0)
                {
                    var totalPages = Math.DivRem(total.Value, take, out int lastPageCount);
                    if (lastPageCount > 0) totalPages++;
                    if (lastPageCount == 0) lastPageCount = take;
                    int currentPage = 1;
                    if (Skip > 0) currentPage = (Skip / take) + 1;

                    using (Div(@class: "text-center"))
                    {
                        using (Div(@class: "btn-group"))
                        {
                            handler.Skip = 0;
                            using (A(href: handler.ToUrl(), @class: "btn btn-sm" + (currentPage <= 1 ? " disabled" : ""), style: "font-weight: bold; min-width: 60px"))
                            {
                                I(@class: "fas fa-step-backward");
                            }
                            handler.Skip = (currentPage - 2) * take;
                            using (A(href: handler.ToUrl(), @class: "btn btn-sm" + (currentPage <= 1 ? " disabled" : ""), style: "font-weight: bold; min-width: 60px"))
                            {
                                I(@class: "fas fa-backward");
                            }
                        }
                        using (Span(style: "font-size: 12px; color: #ccc; font-weight: bold; margin-left: 10px; margin-right: 10px")) Write(string.Format(Strings.Page_XXX_of_XXX, currentPage.ToString(), totalPages.ToString()));
                        using (Div(@class: "btn-group"))
                        {
                            handler.Skip = currentPage * take;
                            using (A(href: handler.ToUrl(), @class: "btn btn-sm" + (currentPage >= totalPages ? " disabled" : ""), style: "font-weight: bold; min-width: 60px"))
                            {
                                I(@class: "fas fa-forward");
                            }
                            handler.Skip = total.Value - lastPageCount;
                            using (A(href: handler.ToUrl(), @class: "btn btn-sm" + (currentPage >= totalPages ? " disabled" : ""), style: "font-weight: bold; min-width: 60px"))
                            {
                                I(@class: "fas fa-step-forward");
                            }
                        }
                    }
                }
            }
        }

        protected void WriteTakeControl(Context context)
        {
            var total = context.Get<Total>();
            if (total == null) return;

            if (total.Value > 50)
            {
                using (Div(@class: "btn-group"))
                {
                    foreach (var e in new int[] { 50, 100, 250, 500, 1000 })
                    {
                        var handler = (NakedObjectsWithPagination)this.MemberwiseClone();
                        handler.PageSize = e;
                        handler.Skip = 0;
                        var take = PageSize ?? 50;
                        using (A(href: handler.ToUrl(), @class: "btn btn-xs" + (take == e ? " active" : null)))
                        {
                            if (total.Value < e) Write(total.Value.ToString());
                            else Write(e.ToString());
                        }

                        if (total.Value <= e) break;
                    }
                    if (total.Value > 1000)
                    {
                        var handler = (NakedObjectsWithPagination)this.MemberwiseClone();
                        handler.PageSize = int.MaxValue;
                        handler.Skip = 0;
                        var take = PageSize ?? 50;
                        using (A(href: handler.ToUrl(), @class: "btn btn-xs" + (take == int.MaxValue ? " active" : null)))
                        {
                            Write(total.Value.ToString());
                        }
                    }
                }
            }
            else if (total.Value > 0)
            {
                using (Span(@class: "btn btn-xs disabled"))
                {
                    Write(total.Value.ToString());
                }
            }
        }

        protected override void OnFooterStartSection(Context context)
        {
            WriteTakeControl(context);

            base.OnFooterStartSection(context);
        }
    }
}
