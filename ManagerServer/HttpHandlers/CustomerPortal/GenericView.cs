using ManagerServer.Api.Businesses.Business;
using ManagerServer.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.CustomerPortal
{
    abstract class GenericView : Template
    {
    }

    abstract class View<T> : GenericView where T : ViewEndpoint<TransactionView>, new()
    {
        [InheritedProtoMember(200)] public Guid Key;

        protected override void InnerGet()
        {
            var view = Activator.CreateInstance<T>();
            view.Context = HttpContext;
            view.Business = Business;
            view.Key = Key;
            var viewData = view.AuthorizedHandle();

            using (Style())
            {
                Write("@media print { .d-print-reset { border: none !important; padding: 0px !important; margin: 0px !important; box-shadow: none !important; display: block !important; min-width: none !important; background-color: none !important; width: auto !important } }");
            }

            using (Div(@class: "rounded-top p-3 border fw-bold d-print-none", style: "background-color: #f5f5f5; box-shadow: inset 1px 1px 0px #fff; border-color: #ddd; color: #ccc; text-shadow: 1px 1px 0 #FFFFFF; font-size: .875rem"))
            {
                Write(viewData.title);
                using (Button(onclick: "print()", @class: "btn btn-light border bg-white btn-sm ms-3 fw-bold")) Write(Strings.Print);
            }

            using (Div(@class: "d-print-reset", style: "background-color: #f5f5f5; display: flex; box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.05); border: 1px solid #ddd; border-top: none; padding: 20px"))
            {
                using (Style())
                {
                    CssRule("main", "padding: 2rem; display: flex; flex-direction: column; gap: 1rem; background-color: #fff; border: 1px solid #ddd");
                    using (MediaQuery("print"))
                    {
                        CssRule("main", "padding: 0; border: none; display: block");
                    }
                }

                using (Main())
                {
                    using (Header(style: "display: flex; justify-content: space-between; align-items: flex-start"))
                    {
                        using (H1(style: "font-size: 2rem; font-weight: bold; margin: 0")) Write(viewData.title);
                        if (viewData.business.logo != null)
                        {
                            Img(src: viewData.business.logo, style: "max-height: 150px; max-width: 300px; display: inline");
                        }
                    }
                    using (Section(style: "display: flex; align-items: flex-start; gap: 20px"))
                    {
                        using (Address(style: "flex: 1; font-style: normal; line-height: 1.5em"))
                        {
                            using (Div()) using (Strong()) Write(viewData.recipient.name);
                            using (Div()) Write(viewData.recipient.address?.ReplaceLineEndings("<br />"));
                        }
                        using (Dl(style: "display: block; margin: 0px; flex: 1; text-align: end"))
                        {
                            foreach (var e in viewData.fields)
                            {
                                using (Dt(style: "margin: 0 0 2px 0")) using (Data(value: e.key)) using (Strong()) Write(e.label);
                                using (Dd(style: "margin: 0 0 16px 0")) Write(e.text);
                            }
                            foreach (var e in viewData.custom_fields.Where(x => x.displayAtTheTop))
                            {
                                using (Dt(style: "margin: 0 0 2px 0")) using (Data(value: e.key)) using (Strong()) Write(e.label);
                                using (Dd(style: "margin: 0 0 16px 0")) Write(e.text);
                            }
                        }
                        using (Div(style: "width: 1px; border-left: 1px solid #000; align-self: stretch"))
                        {
                        }
                        using (Address(style: "white-space: nowrap; font-style: normal; line-height: 1.5em"))
                        {
                            using (Div()) using (Strong()) Write(viewData.business.name);
                            using (Div()) Write(viewData.business.address?.ReplaceLineEndings("<br />"));
                        }
                    }

                    if (viewData.description != null)
                    {
                        using (Div(style: "font-weight: bold")) Write(viewData.description);
                    }

                    using (Table(style: "min-width: 80ch; border-inline-end: 1px solid #000; border-collapse: collapse"))
                    {
                        using (THead())
                        {
                            using (Tr())
                            {
                                foreach (var e in viewData.table.columns)
                                {
                                    var style = "padding: 0.25rem 0.50rem; border: 1px solid #000";
                                    style += "; text-align: " + e.align;
                                    if (e.minWidth)
                                    {
                                        style += "; white-space: nowrap; width: 1px";
                                    }
                                    else if (e.nowrap)
                                    {
                                        style += "; white-space: nowrap";
                                    }
                                    using (Th(style: style)) Write(e.label);
                                }
                            }
                        }
                        using (TBody())
                        {
                            foreach (var e in viewData.table.rows)
                            {
                                using (Tr())
                                {
                                    for (int i = 0; i < viewData.table.columns.Count; i++)
                                    {
                                        var col = viewData.table.columns[i];
                                        var style = "padding: 0.25rem 0.50rem; border-left: 1px solid #000; border-right: 1px solid #000; vertical-align: top";
                                        style += "; text-align: " + col.align;
                                        if (col.minWidth)
                                        {
                                            style += "; white-space: nowrap; width: 1px";
                                        }
                                        else if (col.nowrap)
                                        {
                                            style += "; white-space: nowrap";
                                        }

                                        using (Td(style: style))
                                        {
                                            var cell = e.cells[i];
                                            if (cell.value != null)
                                            {
                                                using (Data(value: cell.value.ToString()))
                                                {
                                                    Write(cell.text?.ReplaceLineEndings("<br />"));
                                                }
                                            }
                                            else
                                            {
                                                Write(cell.text?.ReplaceLineEndings("<br />"));
                                            }
                                        }
                                    }
                                }
                            }
                            using (Tr())
                            {
                                for (int i = 0; i < viewData.table.columns.Count; i++)
                                {
                                    using (Td(style: "border: 1px solid #000; border-top: none")) Write("&nbsp;");
                                }
                            }
                            if (viewData.table.columns.Any(x => x.sumText != null))
                            {
                                using (Tr())
                                {
                                    foreach (var e in viewData.table.columns)
                                    {
                                        var style = "border: 1px solid #000; font-weight: bold; padding: 0.25rem 0.50rem";
                                        style += "; text-align: " + e.align;
                                        using (Td(style: style))
                                        {
                                            Write(e.sumText);
                                        }
                                    }
                                }
                            }
                            foreach (var e in viewData.table.totals)
                            {
                                using (Tr())
                                {
                                    var style1 = "padding: 0.25rem 0.50rem; text-align: end";
                                    if (e.emphasis) style1 += "; font-weight: bold";

                                    var style2 = "padding: 0.25rem 0.50rem; text-align: right; border: 1px solid #000; white-space: nowrap";
                                    if (e.emphasis) style2 += "; font-weight: bold";

                                    using (Td(style: style1, colspan: viewData.table.columns.Count - 1)) Write(e.label);
                                    using (Td(style: style2))
                                    {
                                        using (Data(value: e.number.ToString()))
                                        {
                                            Write(e.text);
                                        }
                                    }
                                }
                            }
                        }
                    }                    

                    if (viewData.custom_fields.Any(x => !x.displayAtTheTop))
                    {
                        using (Dl())
                        {
                            foreach (var e in viewData.custom_fields.Where(x => !x.displayAtTheTop))
                            {
                                using (Dt(style: "margin: 0 0 2px 0"))
                                {
                                    using (Strong())
                                    {
                                        using (Data(value: e.key))
                                        {
                                            Write(e.label);
                                        }
                                    }
                                }
                                using (Dd(style: "margin: 0 0 16px 0; white-space: pre-line"))
                                {
                                    Write(e.text);
                                }
                            }
                        }
                    }

                    if (viewData.footers != null)
                    {
                        foreach (var e in viewData.footers)
                        {
                            using (Div()) Write(e);
                        }
                    }

                    if (viewData.emphasis?.text != null)
                    {
                        using (Div(style: "text-align: center; display: flex; justify-content: center"))
                        {
                            var color = "#000";
                            if (viewData.emphasis.positive) color = "green";
                            if (viewData.emphasis.negative) color = "red";

                            using (Div(style: $"border-width: 5px; border-color: {color}; border-style: solid; padding: 10px; font-size: 20px; text-transform: uppercase; color: {color};"))
                            {
                                Write(viewData.emphasis.text);
                            }
                        }
                    }
                }
            }
        }
    }
}