using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Helpers;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.CustomerPortal
{
    abstract class Table<T> : Template where T : Table<T>.Item
    {
        protected virtual IEnumerable<T> GetItems()
        {
            return null;
        }

        protected virtual string GetTitle()
        {
            return this.GetType().Name;
        }

        protected override void InnerGet()
        {
            var fields = typeof(T).GetFieldsAndProperties().Where(x => x.Name != nameof(Item.View)).ToArray();

            using (Style())
            {
                Write(".table-striped > tbody > tr:nth-child(2n+1) > td { background-color: #f9f9f9 !important; box-shadow: inset 0px 1px 0px #fff; }");
            }

            using (Div(@class: "p-3", style: "background-color: #F5F5F5; box-shadow: inset 1px 1px 0px #fff; border: 1px solid #ddd; border-top-right-radius: 3px; border-top-left-radius: 3px; border-bottom: none; font-size: .875rem"))
            {
                using (Span(@class: "fw-bold", style: "color: #ccc; text-shadow: 1px 1px 0 #fff")) Write(GetTitle());
            }
            using (Table(@class: "table table-bordered table-striped", style: "font-size: .75rem"))
            {
                using (THead())
                {
                    using (Tr())
                    {
                        var style = "background-color: #f5f5f5; box-shadow: inset 1px 1px 0px #fff; color: #555; text-shadow: 0 1px 0 #fff";
                        using (Th(style: style, @class: "text-center")) I(@class: "fas fa-print", style: "opacity: 0.2");
                        foreach (var e in fields)
                        {
                            string @class = "text-nowrap ";
                            if (e.GetMemberType() == typeof(DateTime)) @class += "; text-center";
                            if (e.GetMemberType() == typeof(decimal)) style += "; text-align: right";
                            if (e.GetMemberType().IsEnum) @class += "; text-center";
                            if (e.GetCustomAttribute<ManagerServer.Model.Attributes.LongAttribute>() != null) @class += "; w-100";
                            if (e.GetCustomAttribute<CenterAttribute>() != null) @class += "; text-center";
                            using (Th(@class: @class, style: style)) Write(ManagerServer.Globalization.Strings.GetPropertyValue(e.Name));
                        }
                    }
                }
                using (TBody())
                {
                    foreach (var e in GetItems())
                    {
                        using (Tr())
                        {
                            using (Td(@class: "text-center"))
                            {
                                using (A(href: e.View?.ToUrl(), @class: "btn btn-outline-secondary btn-sm btn-block d-inline", style: "font-size: 0.75rem; padding: .1rem .4rem; border-color: #ccc; color: #555; background-color: #fff")) Write(Strings.View);
                            }
                            foreach (var e2 in fields)
                            {
                                var value = e2.GetMemberValue(e);
                                if (value is DateTime date) using (Td(@class: "text-center text-nowrap")) Write(date.ToShortDateString());
                                else if (value is decimal @decimal)
                                {
                                    var cellStyle = "text-align: right";
                                    using (Td(@class: "text-nowrap", style: cellStyle))
                                    {
                                        Write(@decimal.ToNumberString());
                                    }
                                }
                                else if (value is Enum @enum)
                                {
                                    using (Td(@class: "text-nowrap"))
                                    {
                                        var bg = string.Empty;
                                        var style = string.Empty;
                                        if (@enum.GetType().GetMember(@enum.ToString())[0].GetCustomAttribute<SuccessAttribute>() != null) style = "background-color: #5cb85c";
                                        else if (@enum.GetType().GetMember(@enum.ToString())[0].GetCustomAttribute<DangerAttribute>() != null) bg = "bg-danger";
                                        else bg = "bg-secondary";
                                        using (Span(@class: "badge d-block " + bg, style: style)) Write(ManagerServer.Globalization.Strings.GetPropertyValue(value.ToString()));
                                    }
                                }
                                else
                                {
                                    var cellClass = string.Empty;
                                    if (e2.GetCustomAttribute<CenterAttribute>() != null) cellClass += "text-center";
                                    using (Td(@class: cellClass)) Write(value?.ToString());
                                }
                            }
                        }
                    }
                }
            }
        }

        public abstract class Item
        {
            public Template View;
        }
    }

    public sealed class SuccessAttribute : Attribute { }
    public sealed class DangerAttribute : Attribute { }
    public sealed class CenterAttribute : Attribute { }
}
