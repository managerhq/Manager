using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.Attributes
{
    public class AdvancedQueryAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public AdvancedQueryAttribute(string[] select = null, string[] where = null, string[] orderBy = null, string[] groupBy = null)
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none"))
            {
                using (Div(@class: "bg-yellow-50 p-6 pb-6 text-sm"))
                {
                    if (select != null)
                    {
                        using (Div(@class: "font-semibold"))
                        {
                            Keyword(nameof(Strings.Select));
                        }
                        using (Div(@class: "mt-4 ps-6 flex flex-wrap gap-2"))
                        {
                            foreach (var e in select)
                            {
                                using (Span(@class: "bg-amber-100 py-2 px-4 rounded"))
                                {
                                    Keyword(e);
                                }
                            }
                        }
                    }

                    if (where != null)
                    {
                        using (Div(@class: "font-semibold mt-4"))
                        {
                            Keyword(nameof(Strings.HasWhere));
                        }
                        for (int i = 0; i < where.Length; i += 3)
                        {
                            using (Div(@class: "mt-4 ps-6 flex items-center gap-2"))
                            {
                                using (Span(@class: "bg-amber-100 py-2 px-4 rounded")) Keyword(where[i]);
                                using (Span()) Keyword(where[i + 1]);
                                if (where[i + 2] != null)
                                {
                                    using (Span(@class: "bg-white border-amber-100 border-2 py-2 px-4 rounded"))
                                    {
                                        Keyword(where[i + 2]);
                                    }
                                }
                            }
                        }
                    }

                    if (orderBy != null)
                    {
                        using (Div(@class: "font-semibold mt-4"))
                        {
                            Keyword(nameof(Strings.HasOrderBy));
                        }
                        for (int i = 0; i < orderBy.Length; i += 2)
                        {
                            using (Div(@class: "mt-4 ps-6 flex items-center gap-2"))
                            {
                                using (Span(@class: "bg-amber-100 py-2 px-4 rounded"))
                                {
                                    Keyword(orderBy[i]);
                                }
                                using (Span())
                                {
                                    Keyword(orderBy[i + 1]);
                                }
                            }
                        }
                    }

                    if (groupBy != null)
                    {
                        using (Div(@class: "font-semibold mt-4"))
                        {
                            Keyword(nameof(Strings.HasGroupBy));
                        }
                        using (Div(@class: "mt-4 ps-6 flex flex-wrap gap-2"))
                        {
                            foreach (var e in groupBy)
                            {
                                using (Span(@class: "bg-amber-100 py-2 px-4 rounded"))
                                {
                                    Keyword(e);
                                }
                            }
                        }
                    }
                }
            }
        }    
    }
}