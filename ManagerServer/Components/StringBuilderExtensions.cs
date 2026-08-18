using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ManagerComponents
{
    internal static class StringBuilderExtensions
    {
        public static IDisposable Html(this StringBuilder sb, string lang = null, string style = null, string dir = null) => new HtmlElement(sb, "html", lang: lang, dir: dir, style: style);
        public static void Hr(this StringBuilder sb) => sb.Append("<hr />");
        public static void Br(this StringBuilder sb) => sb.Append("<br />");
        public static void Title(this StringBuilder sb, string title) => sb.Append("<title>" + title + "</title>");
        public static void Comment(this StringBuilder sb, string s) => sb.Append("<!-- " + s + " -->");
        public static IDisposable Head(this StringBuilder sb) { return new HtmlElement(sb, "head"); }
        public static IDisposable Details(this StringBuilder sb, string @class = null) { return new HtmlElement(sb, "details", @class: @class); }
        public static IDisposable Summary(this StringBuilder sb, string @class = null, string style = null) { return new HtmlElement(sb, "summary", @class: @class, style: style); }
        public static IDisposable Legend(this StringBuilder sb) { return new HtmlElement(sb, "legend"); }
        public static IDisposable IFrame(this StringBuilder sb, string src = null, string @class = null, string style = null) => new HtmlElement(sb, "iframe", src: src, @class: @class, style: style);
        public static IDisposable Style(this StringBuilder sb) { return new HtmlElement(sb, "style"); }
        public static IDisposable Header(this StringBuilder sb) { return new HtmlElement(sb, "header"); }
        public static IDisposable Footer(this StringBuilder sb) { return new HtmlElement(sb, "footer"); }
        public static IDisposable Nav(this StringBuilder sb) { return new HtmlElement(sb, "nav"); }
        public static IDisposable Pre(this StringBuilder sb, string id = null, string @class = null, string style = null) { return new HtmlElement(sb, "pre", id: id, @class: @class, style: style); }
        public static IDisposable Code(this StringBuilder sb, string id = null, string @class = null, string style = null) { return new HtmlElement(sb, "code", id: id, @class: @class, style: style); }
        public static IDisposable H1(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "h1", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static IDisposable H2(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "h2", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static IDisposable H3(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "h3", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static IDisposable H4(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "h4", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static IDisposable H5(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "h5", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static IDisposable H6(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "h6", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static void LinkStylesheet(this StringBuilder sb, string href) { sb.Append(@"<link rel=""stylesheet"" type=""text/css"" href=""" + href + @""" />"); }
        public static IDisposable Script(this StringBuilder sb) { return new HtmlElement(sb, "script"); }
        public static IDisposable Body(this StringBuilder sb, string style = null, string @class = null, string hxBoost = null, string hxIndicator = null, string hxDisabledElt = null) { return new HtmlElement(sb, "body", style: style, @class: @class, hxBoost: hxBoost, hxIndicator: hxIndicator, hxDisabledElt: hxDisabledElt); }
        public static IDisposable Form(this StringBuilder sb, string action = null, string enctype = null, string method = null, string dataBind = null, string hxVals = null, string hxTarget = null, string hxBoost = null, string hxIndicator = null, string hxDisabledElt = null, string hxPost = null, string hxSwap = null, string name = null, string target = null, string @class = null, string id = null, string style = null, string title = null, string onsubmit = null) { return new HtmlElement(sb, "form", action: action, enctype: enctype, method: method, name: name, target: target, @class: @class, id: id, style: style, title: title, onsubmit: onsubmit, dataBind: dataBind, hxPost: hxPost, hxSwap: hxSwap, hxBoost: hxBoost, hxVals: hxVals, hxTarget: hxTarget, hxDisabledElt: hxDisabledElt, hxIndicator: hxIndicator); }
        public static IDisposable Fieldset(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null, string legend = null) { return new HtmlElement(sb, "fieldset", id: id, @class: @class, style: style, legend: legend, dataBind: dataBind); }
        public static IDisposable Div(this StringBuilder sb, string id = null, string hxGet = null, string hxPost = null, string hxTrigger = null, string hxSwap = null, string hxTarget = null, string hxIndicator = null, string hxDisabledElt = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "div", id: id, @class: @class, hxGet: hxGet, hxPost: hxPost, style: style, dataBind: dataBind, hxTrigger: hxTrigger, hxSwap: hxSwap, hxTarget: hxTarget, hxIndicator: hxIndicator, hxDisabledElt: hxDisabledElt); }
        public static IDisposable A(this StringBuilder sb, string href = null, string id = null, string @class = null, string style = null, string ariaCurrent = null, string hxIndicator = null, string title = null, string dataBind = null, string accessKey = null, int? tabIndex = null, string target = null, string rev = null, string rel = null, string name = null, string onclick = null, string itemprop = null, string role = null, string hreflang = null, string hxBoost = null, string hxDisabledElt = null, Tuple<string, string>[] data = null) { return new HtmlElement(sb, "a", href: href, id: id, @class: @class, style: style, title: title, target: target, name: name, onclick: onclick, dataBind: dataBind, hxBoost: hxBoost, hxDisabledElt: hxDisabledElt, ariaCurrent: ariaCurrent, hxIndicator: hxIndicator); }
        public static IDisposable Span(this StringBuilder sb, string id = null, string @class = null, string style = null, string title = null, string dataBind = null) { return new HtmlElement(sb, "span", id: id, @class: @class, style: style, title: title, dataBind: dataBind); }
        public static IDisposable P(this StringBuilder sb, string id = null, string @class = null, string style = null, string title = null, string dataBind = null) { return new HtmlElement(sb, "p", id: id, @class: @class, style: style, title: title, dataBind: dataBind); }
        public static IDisposable Ul(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "ul", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static IDisposable Ol(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "ol", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static IDisposable Li(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "li", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static IDisposable Label(this StringBuilder sb, string id = null, string @class = null, string style = null, string @for = null, string dataBind = null, string title = null) { return new HtmlElement(sb, "label", id: id, @class: @class, style: style, @for: @for, dataBind: dataBind, title: title); }
        public static IDisposable TBody(this StringBuilder sb, string dataBind = null, string @class = null) { return new HtmlElement(sb, "tbody", dataBind: dataBind, @class: @class); }
        public static IDisposable THead(this StringBuilder sb, string @class = null) { return new HtmlElement(sb, "thead", @class: @class); }
        public static IDisposable TFoot(this StringBuilder sb) { return new HtmlElement(sb, "tfoot"); }
        public static IDisposable Select(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null, bool? multiple = null, string name = null, bool disabled = false, Tuple<string, string>[] data = null) { return new HtmlElement(sb, "select", id: id, @class: @class, style: style, dataBind: dataBind, name: name); }
        public static IDisposable Button(this StringBuilder sb, string id = null, string @class = null, string style = null, bool disabled = false, string dataBind = null, string onclick = null, string hxIndicator = null, string hxGet = null, string hxSwap = null, string hxTarget = null, string title = null, string type = null, params string[] data) { return new HtmlElement(sb, "button", id: id, @class: @class, style: style, dataBind: dataBind, onclick: onclick, title: title, hxGet: hxGet, hxSwap: hxSwap, hxTarget: hxTarget, disabled: disabled, hxIndicator: hxIndicator); }
        public static IDisposable OptGroup(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null, string label = null, Tuple<string, string>[] data = null) { return new HtmlElement(sb, "optgroup", id: id, @class: @class, style: style, dataBind: dataBind, label: label); }
        public static IDisposable Table(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "table", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static IDisposable Caption(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(sb, "caption", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static IDisposable Tr(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null, params string[] data) { return new HtmlElement(sb, "tr", id: id, @class: @class, style: style, dataBind: dataBind); }
        public static IDisposable Td(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null, int? colspan = null, int? rowspan = null) { return new HtmlElement(sb, "td", id: id, @class: @class, style: style, colspan: colspan, rowspan: rowspan, dataBind: dataBind); }
        public static IDisposable Th(this StringBuilder sb, string id = null, string @class = null, string style = null, string dataBind = null, int? colspan = null, int? rowspan = null) { return new HtmlElement(sb, "th", id: id, @class: @class, style: style, colspan: colspan, rowspan: rowspan, dataBind: dataBind); }

        private sealed class HtmlElement : IDisposable
        {
            private readonly StringBuilder sb;
            private readonly string elementName;

            public HtmlElement(StringBuilder sb, string elementName, string action = null, string enctype = null, string method = null, string name = null, string dataBind = null, string target = null, string @class = null, string id = null, string style = null, string title = null, string onsubmit = null, string lang = null, string legend = null, string src = null, string data = null, string href = null, string @for = null, int? colspan = null, int? rowspan = null, string label = null, string onclick = null, string hxGet = null, string hxPost = null, string hxTrigger = null, string hxSwap = null, string hxTarget = null, string hxIndicator = null, string hxDisabledElt = null, string hxBoost = null, string hxVals = null, string ariaCurrent = null, string dir = null, bool disabled = false)
            {
                this.sb = sb;
                this.elementName = elementName;

                sb.Append('<');
                sb.Append(elementName);
                if (lang != null) sb.Append(@" lang=""" + lang + @"""");
                if (action != null) sb.Append(@" action=""" + action + @"""");
                if (legend != null) sb.Append(@" legend=""" + legend + @"""");
                if (src != null) sb.Append(@" src=""" + src + @"""");
                if (disabled) sb.Append(@" disabled");
                if (href != null) sb.Append(@" href =""" + href + @"""");
                if (data != null) sb.Append(@" data=""" + data + @"""");
                if (label != null) sb.Append(@" label=""" + label + @"""");
                if (onclick != null) sb.Append(@" onclick=""" + onclick + @"""");
                if (@for != null) sb.Append(@" for=""" + @for + @"""");
                if (colspan.HasValue) sb.Append(@" colspan=""" + colspan.Value.ToString() + @"""");
                if (rowspan.HasValue) sb.Append(@" rowspan=""" + rowspan.Value.ToString() + @"""");
                if (enctype != null) sb.Append(@" enctype="""" + enctype + @""");                
                if (method != null) sb.Append(@" method=""" + method + @"""");
                if (name != null) sb.Append(@" name=""" + name + @"""");
                if (target != null) sb.Append(@" target=""" + target + @"""");
                if (@class != null) sb.Append(@" class=""" + @class + @"""");
                if (id != null) sb.Append(@" id=""" + id + @"""");
                if (style != null) sb.Append(@" style=""" + style + @"""");
                if (title != null) sb.Append(@" title=""" + title + @"""");
                if (dir != null) sb.Append(@" dir=""" + dir + @"""");
                if (onsubmit != null) sb.Append(@" onsubmit=""" + onsubmit + @"""");
                if (dataBind != null) sb.Append(@" data-bind=""" + dataBind + @"""");
                if (hxGet != null) sb.Append(@" hx-get=""" + hxGet + @"""");
                if (hxPost != null) sb.Append(@" hx-post=""" + hxPost + @"""");
                if (hxTrigger != null) sb.Append(@" hx-trigger=""" + hxTrigger + @"""");
                if (hxSwap != null) sb.Append(@" hx-swap=""" + hxSwap + @"""");
                if (hxVals != null) sb.Append(@" hx-vals=""" + hxVals + @"""");
                if (hxTarget != null) sb.Append(@" hx-target=""" + hxTarget + @"""");
                if (hxIndicator != null) sb.Append(@" hx-indicator=""" + hxIndicator + @"""");
                if (hxDisabledElt != null) sb.Append(@" hx-disabled-elt=""" + hxDisabledElt + @"""");
                if (hxBoost != null) sb.Append(@" hx-boost=""" + hxBoost + @"""");
                if (ariaCurrent != null) sb.Append(@" aria-current=""" + ariaCurrent + @"""");
                sb.Append('>');
            }

            public void Dispose()
            {
                sb.Append("</");
                sb.Append(elementName);
                sb.Append(">");
            }
        }

        public static void Script(this StringBuilder sb, string src, string @class = null, Tuple<string, string>[] data = null)
        {
            var output = @"<script src=""" + src + @""" type=""text/javascript""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (data != null) foreach (var e in data) output += @" data-" + e.Item1 + @"=""" + e.Item2 + @"""";
            output += "></script>";
            sb.Append(output);
        }

        public static void I(this StringBuilder sb, string @class = null, string style = null)
        {
            string output = @"<i";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            output += "></i>";
            sb.Append(output);
        }

        public static void InputText(this StringBuilder sb, string id = null, string @class = null, string vmodel = null, string form = null, int? maxlength = null, string style = null, string name = null, string value = null, bool @readonly = false, string placeholder = null, string dataBind = null, int? tabindex = null, bool autofocus = false, bool? autocomplete = null, bool? autocorrect = null, bool? autocapitalize = null, bool? spellcheck = null, bool disabled = false, Tuple<string, string>[] data = null)
        {
            string output = @"<input type=""text""";
            if (id != null) output += @" id=""" + id + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (name != null) output += @" name=""" + name + @"""";
            if (form != null) output += @" form=""" + form + @"""";
            if (vmodel != null) output += @" v-model=""" + vmodel + @"""";
            if (tabindex != null) output += @" tabindex=""" + tabindex.Value.ToString() + @"""";
            if (value != null) output += @" value=""" + System.Net.WebUtility.HtmlEncode(value) + @"""";
            if (placeholder != null) output += @" placeholder=""" + placeholder + @"""";
            if (@readonly) output += @" readonly";
            if (disabled) output += @" disabled";
            if (maxlength != null) output += @" maxlength=""" + maxlength.Value + @"""";
            if (dataBind != null) output += @" data-bind=""" + dataBind + @"""";
            if (autofocus) output += @" autofocus";
            if (autocomplete.HasValue && !autocomplete.Value) output += @" autocomplete=""off""";
            if (autocorrect.HasValue && !autocorrect.Value) output += @" autocorrect=""off""";
            if (autocapitalize.HasValue && !autocapitalize.Value) output += @" autocapitalize=""off""";
            if (spellcheck.HasValue && !spellcheck.Value) output += @" spellcheck=""off""";
            if (data != null) foreach (var e in data) output += @" data-" + e.Item1 + @"=""" + e.Item2 + @"""";
            output += " />";
            sb.Append(output);
        }

        public static void InputDate(this StringBuilder sb, string id = null, string @class = null, string style = null, string name = null, DateTime? value = null, DateTime? min = null, DateTime? max = null, bool @readonly = false, int? tabindex = null, bool autofocus = false, bool disabled = false)
        {
            string output = @"<input type=""date""";
            if (id != null) output += @" id=""" + id + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (name != null) output += @" name=""" + name + @"""";
            if (tabindex != null) output += @" tabindex=""" + tabindex.Value.ToString() + @"""";
            if (value != null) output += @" value=""" + value.Value.ToString("yyyy-MM-dd") + @"""";
            if (min != null) output += @" min=""" + min.Value.ToString("yyyy-MM-dd") + @"""";
            if (max != null) output += @" max=""" + max.Value.ToString("yyyy-MM-dd") + @"""";
            if (@readonly) output += @" readonly";
            if (disabled) output += @" disabled";
            if (autofocus) output += @" autofocus";
            output += " />";
            sb.Append(output);
        }

        public static void InputFile(this StringBuilder sb, string id = null, string @class = null, string style = null, string name = null, int? tabindex = null, string accept = null)
        {
            string output = @"<input type=""file""";
            if (id != null) output += @" id=""" + id + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (name != null) output += @" name=""" + name + @"""";
            if (accept != null) output += @" accept=""" + accept + @"""";
            if (tabindex != null) output += @" tabindex=""" + tabindex.Value.ToString() + @"""";
            output += " />";
            sb.Append(output);
        }

        public static void InputPassword(this StringBuilder sb, string id = null, string @class = null, int? maxlength = null, string style = null, string name = null, string value = null, bool @readonly = false, string placeholder = null, string dataBind = null, int? tabindex = null, bool autofocus = false, bool? autocomplete = null, bool? autocorrect = null, bool? autocapitalize = null, bool? spellcheck = null, Tuple<string, string>[] data = null)
        {
            string output = @"<input type=""password""";
            if (id != null) output += @" id=""" + id + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (name != null) output += @" name=""" + name + @"""";
            if (tabindex != null) output += @" tabindex=""" + tabindex.Value.ToString() + @"""";
            if (value != null) output += @" value=""" + System.Net.WebUtility.HtmlEncode(value) + @"""";
            if (placeholder != null) output += @" placeholder=""" + placeholder + @"""";
            if (@readonly) output += @" readonly=""readonly""";
            if (maxlength != null) output += @" maxlength=""" + maxlength.Value + @"""";
            if (dataBind != null) output += @" data-bind=""" + dataBind + @"""";
            if (autofocus) output += @" autofocus";
            if (autocomplete.HasValue && !autocomplete.Value) output += @" autocomplete=""off""";
            if (autocorrect.HasValue && !autocorrect.Value) output += @" autocorrect=""off""";
            if (autocapitalize.HasValue && !autocapitalize.Value) output += @" autocapitalize=""off""";
            if (spellcheck.HasValue && !spellcheck.Value) output += @" spellcheck=""off""";
            if (data != null) foreach (var e in data) output += @" data-" + e.Item1 + @"=""" + e.Item2 + @"""";
            output += " />";
            sb.Append(output);
        }

        public static void InputSubmit(this StringBuilder sb, string id = null, string @class = null, string style = null, string name = null, int? tabindex = null, string value = null, string dataBind = null, string onClick = null, string title = null, Tuple<string, string>[] data = null)
        {
            string output = @"<input type=""submit""";
            if (id != null) output += @" id=""" + id + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (tabindex != null) output += @" tabindex=""" + tabindex.Value.ToString() + @"""";
            if (name != null) output += @" name=""" + name + @"""";
            if (value != null) output += @" value=""" + value + @"""";
            if (title != null) output += @" title=""" + title + @"""";
            if (dataBind != null) output += @" data-bind=""" + dataBind + @"""";
            if (onClick != null) output += @" onClick=""" + onClick + @"""";
            if (data != null) foreach (var e in data) output += @" data-" + e.Item1 + @"=""" + e.Item2 + @"""";
            output += " />";
            sb.Append(output);
        }

        public static void Img(this StringBuilder sb, string src = null, string style = null, string @class = null, string id = null, string alt = null, string itemprop = null, string dataBind = null, string[] data = null)
        {
            string output = @"<img";
            if (src != null) output += @" src=""" + src + @"""";
            if (id != null) output += @" id=""" + id + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (alt != null) output += @" alt=""" + alt + @"""";
            if (dataBind != null) output += @" data-bind=""" + dataBind + @"""";
            if (itemprop != null) output += @" itemprop=""" + itemprop + @"""";
            if (data != null)
            {
                foreach (var e in data)
                {
                    if (string.IsNullOrWhiteSpace(e)) continue;
                    var keyValue = e.Split('=');
                    var dataKey = keyValue[0];
                    var dataValue = (keyValue.Length > 0) ? keyValue[1] : "";
                    output += @" data-" + dataKey + @"=""" + dataValue + @"""";
                }
            }
            output += " />";
            sb.Append(output);
        }

        public static void Option(this StringBuilder sb, string id = null, string @class = null, string style = null, string value = null, bool? selected = null, string text = null, string disabled = null, string dataBind = null, Tuple<string, string>[] data = null)
        {
            string output = @"<option";
            if (id != null) output += @" id=""" + id + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (disabled != null) output += @" disabled=""" + disabled + @"""";
            if (value != null) output += @" value=""" + value + @"""";
            if (selected != null && selected.Value) output += @" selected=""selected""";
            if (dataBind != null) output += @" data-bind=""" + dataBind + @"""";
            if (data != null) foreach (var e in data) output += @" data-" + e.Item1 + @"=""" + e.Item2 + @"""";
            output += ">";
            if (text != null) output += text;
            output += "</option>";
            sb.Append(output);
        }

        public static void Textarea(this StringBuilder sb, string id = null, string @class = null, string vmodel = null, string style = null, string name = null, int? tabindex = null, string text = null, string placeholder = null, string dataBind = null, Tuple<string, string>[] data = null, bool? spellcheck = null, int? maxlength = null, bool @readonly = false, int? rows = null)
        {
            string output = @"<textarea";
            if (id != null) output += @" id=""" + id + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (name != null) output += @" name=""" + name + @"""";
            if (vmodel != null) output += @" v-model=""" + vmodel + @"""";
            if (rows != null) output += @" rows=""" + rows.Value.ToString() + @"""";
            if (tabindex != null) output += @" tabindex=""" + tabindex.Value.ToString() + @"""";
            if (maxlength != null) output += @" maxlength=""" + maxlength.Value.ToString() + @"""";
            if (placeholder != null) output += @" placeholder=""" + placeholder + @"""";
            if (dataBind != null) output += @" data-bind=""" + dataBind + @"""";
            if (spellcheck.HasValue) output += @" spellcheck=""" + (spellcheck.Value ? "true" : "false") + @"""";
            if (@readonly) output += @" readonly=""readonly""";
            if (data != null) foreach (var e in data) output += @" data-" + e.Item1 + @"=""" + e.Item2 + @"""";
            output += ">";
            if (text != null) output += text;//text.Replace(System.Environment.NewLine, "&#10;");
            output += "</textarea>";
            sb.Append(output);
        }

        public static void InputCheckbox(this StringBuilder sb, string id = null, bool disabled = false, string name = null, string form = null, string onclick = null, string value = null, string style = null, string dataBind = null, string @class = null, Tuple<string, string>[] data = null)
        {
            string output = @"<input type=""checkbox""";
            if (id != null) output += @" id=""" + id + @"""";
            if (name != null) output += @" name=""" + name + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (disabled) output += @" disabled=""disabled""";
            if (onclick != null) output += @" onclick=""" + onclick + @"""";
            if (form != null) output += @" form=""" + form + @"""";
            if (value != null) output += @" value=""" + System.Net.WebUtility.HtmlEncode(value) + @"""";
            if (dataBind != null) output += @" data-bind=""" + dataBind + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (data != null) foreach (var e in data) output += @" data-" + e.Item1 + @"=""" + e.Item2 + @"""";
            output += " />";
            sb.Append(output);
        }

        public static void InputHidden(this StringBuilder sb, string id = null, string name = null, string value = null, string style = null, string dataBind = null, string @class = null, Tuple<string, string>[] data = null)
        {
            string output = @"<input type=""hidden""";
            if (id != null) output += @" id=""" + id + @"""";
            if (name != null) output += @" name=""" + name + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (value != null) output += @" value=""" + System.Net.WebUtility.HtmlEncode(value) + @"""";
            if (dataBind != null) output += @" data-bind=""" + dataBind + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (data != null) foreach (var e in data) output += @" data-" + e.Item1 + @"=""" + e.Item2 + @"""";
            output += " />";
            sb.Append(output);
        }

        public static void InputRadio(this StringBuilder sb, string id = null, string name = null, string value = null, string style = null, string dataBind = null, string @class = null, Tuple<string, string>[] data = null)
        {
            string output = @"<input type=""radio""";
            if (id != null) output += @" id=""" + id + @"""";
            if (name != null) output += @" name=""" + name + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (value != null) output += @" value=""" + System.Net.WebUtility.HtmlEncode(value) + @"""";
            if (dataBind != null) output += @" data-bind=""" + dataBind + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (data != null) foreach (var e in data) output += @" data-" + e.Item1 + @"=""" + e.Item2 + @"""";
            output += " />";
            sb.Append(output);
        }
    }
}