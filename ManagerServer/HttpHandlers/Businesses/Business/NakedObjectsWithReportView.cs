using System.Linq;
using System.Reflection;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithReportView : NakedObjectsWithFutureDateWarning
    {
        [InheritedProtoMember(199)] public bool ReportView;

        protected override void InnerGet4(Context context)
        {
            if (ReportView)
            {
                CustomInnerGetForView(context);
                return;
            }

            base.InnerGet4(context);
        }

        protected override void OnBeforeBeforeFooter(Context context)
        {
            if (context.Get<ReportInfo>() != null)
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "flex items-center gap-4"))
                    {
                        I(@class: "fas fa-fw fa-turn-up fa-rotate-90", style: "font-size: 24px; color: #ccc");

                        var httpHandler = (NakedObjectsWithReportView)this.MemberwiseClone();
                        httpHandler.ReportView = true;
                        httpHandler.Referrer = this.ToUrl();

                        using (A(@class: "btn", href: httpHandler.ToUrl())) Write(Strings.Print);
                    }
                }
            }
            base.OnBeforeBeforeFooter(context);
        }

        private void CustomInnerGetForView(Context context)
        {
            var reportInfo = context.Get<ReportInfo>();

            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "flex items-center space-x-6 rtl:space-x-reverse"))
                    {
                        using (Div(@class: "card-title")) Write(Strings.AdvancedQuery);
                        using (A(@class: "btn", href: "javascript:print()")) Write(Strings.Print);
                    }
                }
                using (Div(@class: "lg:flex bg-neutral-100 border-t border-b shadow-inner p-0 lg:p-8 print:shadow-none print:p-0 print:border-0"))
                {
                    using (Div(@class: "bg-white border border-neutral-300 shadow rounded-lg print:shadow-none print:grow p-8 print:p-0 print:border-0 overflow-x-auto lg:overflow-visible"))
                    {
                        using (Table(@class: "print:w-full print:p-0 w-full lg:min-w-[600px]"))
                        {
                            var columns = context.Get<Column[]>();
                            var rows = context.Get<Array>();
                            var visibleColumns = columns
                                .Where(x => x.Visible)
                                .Where(x => x.CanEnsureCells(rows))
                                .Where(x => x.Key.HasValue)
                                .Where(x => x.Key != new Guid("e86f12dd-2bfc-4eef-a7b0-71e4e9caeda9")) // Attachment column
                                .ToArray();

                            using (THead())
                            {
                                var businessDetails = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BusinessDetails>();
                                var businessName = businessDetails.Name;
                                if (string.IsNullOrWhiteSpace(businessName)) businessName = Business;

                                using (Tr()) using (Th(colspan: columns.Length + 1, style: "font-weight: bold; font-size: 16px; text-align: center; padding-bottom: 10px")) Write(businessName);
                                using (Tr()) using (Th(colspan: columns.Length + 1, style: "font-weight: bold; font-size: 24px; text-align: center; padding-bottom: 10px"))
                                {
                                    var titleAttribute = this.GetType().GetCustomAttribute<TitleAttribute>(false);
                                    if (titleAttribute != null)
                                    {
                                        var title = string.Join(" - ", this.GetType().GetCustomAttribute<TitleAttribute>(false).Text.Select(x => ManagerServer.Globalization.Strings.GetPropertyValue(x)));
                                        Write(title);
                                    }
                                    else
                                    {
                                        Write(base.GetTitle());
                                    }
                                }
                                using (Tr()) using (Th(colspan: columns.Length + 1, style: "font-weight: bold; font-size: 16px; text-align: center; padding-bottom: 10px"))
                                {
                                    var title = Strings.Unnamed;
                                    if (reportInfo != null)
                                    {
                                        if (!string.IsNullOrWhiteSpace(reportInfo.Title)) title = reportInfo.Title;
                                    }
                                    Write(title);
                                }                                

                                using (Tr())
                                {
                                    foreach (var e in visibleColumns)
                                    {
                                        e.EnsureCells(rows);

                                        var tailwind = "border-b border-b-black p-2";
                                        if (e.Attributes.OfType<CenterAttribute>().Any()) tailwind += " text-center";
                                        if (e.Attributes.OfType<RightAttribute>().Any()) tailwind += " text-right";

                                        using (Th(@class: tailwind))
                                        {
                                            Write(e.Label);
                                        }
                                    }
                                }
                                using (Tr()) using (Th(colspan: columns.Length + 1)) Write("&nbsp;");
                            }

                            using (TBody())
                            {
                                foreach (var e in rows)
                                {
                                    using (Tr())
                                    {
                                        foreach (var e2 in visibleColumns)
                                        {
                                            var tailwind = "p-2";
                                            if (e2.Attributes.OfType<CenterAttribute>().Any()) tailwind += " text-center";
                                            if (e2.Attributes.OfType<RightAttribute>().Any()) tailwind += " text-right";
                                            if (e2.Attributes.OfType<MinWidthAttribute>().Any()) tailwind += " whitespace-nowrap w-px";
                                            if (e2.Attributes.OfType<WhitespaceNoWrapAttribute>().Any()) tailwind += " whitespace-nowrap";
                                            if (e2.Attributes.OfType<BoldAttribute>().Any()) tailwind += " font-semibold";
                                            using (Td(@class: tailwind))
                                            {
                                                Write(e2.GetValueAsPlainText(e));
                                            }
                                        }
                                    }
                                }

                                var hasTotalRow = visibleColumns.Any(x => x.Attributes.OfType<NakedObjectsWithColumnTotals.SumAttribute>().Any());
                                if (hasTotalRow)
                                {
                                    using (Tr()) using (Th(colspan: columns.Length + 1)) Write("&nbsp;");

                                    foreach (var e in visibleColumns)
                                    {
                                        var tailwind = "font-semibold whitespace-nowrap border-y border-black p-2";
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
                }
                /*
                using (Panel(padding: "p-3 print:hidden"))
                {
                    using (Div(@class: "flex justify-between"))
                    {
                        using (Div())
                        {
                        }

                        using (Div(@class: "flex items-center gap-2"))
                        {
                        }
                    }
                }
                */
            }
        }

        public sealed class ReportInfo
        {
            public string Title;
        }
    }
}