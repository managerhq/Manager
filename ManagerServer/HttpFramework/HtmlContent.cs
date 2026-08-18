using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;

namespace HttpFramework
{
    public abstract class HtmlContent
    {
        public StringBuilder StringBuilder;

        public void Write(string s)
        {
            StringBuilder.Append(s);
        }

        public void Write(StringBuilder sb)
        {
            StringBuilder.Append(sb);
        }

        public HtmlElement Html(string lang = null, string @class = null, string dir = null, string style = null, string translate = null) { return new HtmlElement(StringBuilder, "html", @class: @class, lang: lang, dir: dir, style: style, translate: translate, doctype: "<!DOCTYPE html>"); }
        public HtmlElement Head() { return new HtmlElement(StringBuilder, "head"); }
        public HtmlElement Template(string id = null, string @class = null) { return new HtmlElement(StringBuilder, "template", id: id, @class: @class); }
        public HtmlElement Main() { return new HtmlElement(StringBuilder, "main"); }
        public HtmlElement Data(string id = null, string value = null) { return new HtmlElement(StringBuilder, "data", id: id, value: value); }
        public HtmlElement Dl(string style = null) { return new HtmlElement(StringBuilder, "dl", style: style); }
        public HtmlElement Dt(string style = null) { return new HtmlElement(StringBuilder, "dt", style: style); }
        public HtmlElement Dd(string style = null, string id = null, string onload = null) { return new HtmlElement(StringBuilder, "dd", style: style, id: id, onload: onload); }
        public HtmlElement Del() { return new HtmlElement(StringBuilder, "del"); }
        public HtmlElement Ins() { return new HtmlElement(StringBuilder, "ins"); }
        public HtmlElement Strong() { return new HtmlElement(StringBuilder, "strong"); }
        public HtmlElement Section(string style = null) { return new HtmlElement(StringBuilder, "section", style: style); }
        public HtmlElement Address(string id = null, string style = null) { return new HtmlElement(StringBuilder, "address", id: id, style: style); }
        public HtmlElement Time(DateTime? datetime = null) { return new HtmlElement(StringBuilder, "time", datetime: datetime); }
        public HtmlElement Legend(string style = null) { return new HtmlElement(StringBuilder, "legend", style: style); }
        public HtmlElement Style() { return new HtmlElement(StringBuilder, "style"); }
        public HtmlElement Pre(string id = null, string @class = null, string style = null) { return new HtmlElement(StringBuilder, "pre", id: id, @class: @class, style: style); }
        public HtmlElement Code(string id = null, string @class = null, string style = null, string onclick = null) { return new HtmlElement(StringBuilder, "code", id: id, @class: @class, style: style, onclick: onclick); }
        public HtmlElement Summary(string id = null, string @class = null, string style = null) { return new HtmlElement(StringBuilder, "summary", id: id, @class: @class, style: style); }
        public HtmlElement Footer(string id = null, string @class = null, string style = null) { return new HtmlElement(StringBuilder, "footer", id: id, @class: @class, style: style); }
        public HtmlElement Header(string id = null, string @class = null, string style = null) { return new HtmlElement(StringBuilder, "header", id: id, @class: @class, style: style); }
        public HtmlElement Details(string id = null, string @class = null, string style = null, bool open = false) { return new HtmlElement(StringBuilder, "details", id: id, @class: @class, style: style, open: open); }
        public HtmlElement Dialog(string id = null, string @class = null, string closedby = null, string onclick = null) { return new HtmlElement(StringBuilder, "dialog", id: id, @class: @class, closedby: closedby, onclick: onclick); }
        public HtmlElement H1(string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(StringBuilder, "h1", id: id, @class: @class, style: style, dataBind: dataBind); }
        public HtmlElement H2(string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(StringBuilder, "h2", id: id, @class: @class, style: style, dataBind: dataBind); }
        public HtmlElement H3(string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(StringBuilder, "h3", id: id, @class: @class, style: style, dataBind: dataBind); }
        public HtmlElement H4(string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(StringBuilder, "h4", id: id, @class: @class, style: style, dataBind: dataBind); }
        public HtmlElement H5(string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(StringBuilder, "h5", id: id, @class: @class, style: style, dataBind: dataBind); }
        public HtmlElement H6(string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(StringBuilder, "h6", id: id, @class: @class, style: style, dataBind: dataBind); }
        public HtmlElement Script(bool isModule = false, string type = null, string id = null) { return new HtmlElement(StringBuilder, "script", type: type, id: id, isModule: isModule); }
        public HtmlElement Body(string style = null, string @class = null) { return new HtmlElement(StringBuilder, "body", style: style, @class: @class); }
        public HtmlElement Form(string action = null, Enctype? enctype = null, string method = null, string hxSelect = null, bool? hxBoost = null, string hxDisabledElt = null, string dataBind = null, string name = null, string target = null, string @class = null, string id = null, string style = null, string title = null, string onsubmit = null, string hxPost = null, string hxTrigger = null, string hxInclude = null, string hxTarget = null, string hxSwap = null, string hxVals = null) { return new HtmlElement(StringBuilder, "form", action: action, enctype: enctype, method: method, name: name, target: target, @class: @class, id: id, style: style, title: title, dataBind: dataBind, onsubmit: onsubmit, hxBoost: hxBoost, hxDisabledElt: hxDisabledElt, hxPost: hxPost, hxTrigger: hxTrigger, hxInclude: hxInclude, hxTarget: hxTarget, hxSwap: hxSwap, hxVals: hxVals, hxSelect: hxSelect); }
        public HtmlElement Fieldset(string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(StringBuilder, "fieldset", id: id, @class: @class, style: style, dataBind: dataBind); }
        public HtmlElement IFrame(string src = null, StringBuilder srcdoc = null, string loading = null, string name = null, string id = null, string @class = null, string style = null, string dataBind = null, bool? frameBorder = null, string onload = null, bool? scrolling = null) { return new HtmlElement(StringBuilder, "iframe", src: src, name: name, id: id, @class: @class, style: style, dataBind: dataBind, frameBorder: frameBorder, onload: onload, loading: loading, scrolling: scrolling, srcdoc: srcdoc); }
        public HtmlElement Div(string id = null, string hxGet = null, string hxTrigger = null, string hxSelect = null, string dir = null, string v_if = null, string v_show = null, bool? v_cloak = null, string v_bind_style = null, string v_for = null, string @class = null, string style = null, string dataBind = null, string hxDisabledElt = null, string[] data = null) { return new HtmlElement(StringBuilder, "div", v_if: v_if, v_show: v_show, v_cloak: v_cloak, v_bind_style: v_bind_style, v_for: v_for, id: id, dir: dir, @class: @class, style: style, dataBind: dataBind, hxDisabledElt: hxDisabledElt, hxGet: hxGet, hxTrigger: hxTrigger, dataArray: data, hxSelect: hxSelect); }
        public HtmlElement A(string href = null, string v_href = null, string appearance = null, bool? hxBoost = null, string id = null, string @class = null, string v_on_click = null, string style = null, string title = null, string dataBind = null, string accessKey = null, int? tabIndex = null, string target = null, string rev = null, string rel = null, string name = null, string onclick = null, string itemprop = null, string role = null, string hreflang = null, Tuple<string, string>[] data = null) { return new HtmlElement(StringBuilder, "a", href: href, v_href: v_href, appearance: appearance, id: id, @class: @class, v_on_click: v_on_click, style: style, title: title, accessKey: accessKey, tabIndex: tabIndex, target: target, rev: rev, rel: rel, name: name, onclick: onclick, itemprop: itemprop, dataBind: dataBind, hreflang: hreflang, hxBoost: hxBoost, data: data); }
        public HtmlElement Span(string id = null, string @class = null, string v_if = null, string style = null, string title = null, string onclick = null, string dataBind = null, string hxGet = null, string hxTrigger = null, string hxInclude = null, string hxSelect = null, string hxTarget = null, string hxSwap = null, Tuple<string, string>[] data = null) { return new HtmlElement(StringBuilder, "span", id: id, @class: @class, v_if: v_if, style: style, title: title, onclick: onclick, dataBind: dataBind, hxGet: hxGet, hxTrigger: hxTrigger, hxTarget: hxTarget, hxSwap: hxSwap, hxInclude: hxInclude, hxSelect: hxSelect, data: data); }
        public HtmlElement P(string id = null, string @class = null, string style = null, string title = null, string dataBind = null) { return new HtmlElement(StringBuilder, "p", id: id, @class: @class, style: style, title: title, dataBind: dataBind); }
        public HtmlElement Ul(string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(StringBuilder, "ul", id: id, @class: @class, style: style, dataBind: dataBind); }
        public HtmlElement Ol(string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(StringBuilder, "ol", id: id, @class: @class, style: style, dataBind: dataBind); }
        public HtmlElement Li(string id = null, string v_for = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(StringBuilder, "li", v_for: v_for, id: id, @class: @class, style: style, dataBind: dataBind); }
        public HtmlElement Label(string id = null, string @class = null, string style = null, string @for = null, string dataBind = null, string title = null) { return new HtmlElement(StringBuilder, "label", id: id, @class: @class, style: style, @for: @for, dataBind: dataBind, title: title); }
        public HtmlElement TBody(string dataBind = null, string id = null, string @class = null, string v_model = null, string @is = null, string tag = null, string handle = null) { return new HtmlElement(StringBuilder, "tbody", id: id, @class: @class, dataBind: dataBind, v_model: v_model, @is: @is, tag: tag, handle: handle); }
        public HtmlElement THead(string @class = null) { return new HtmlElement(StringBuilder, "thead", @class: @class); }
        public HtmlElement TFoot() { return new HtmlElement(StringBuilder, "tfoot"); }
        public HtmlElement If(string v_if) { return new HtmlElement(StringBuilder, "template", v_if: v_if); }
        public HtmlElement Select(string id = null, string v_model = null, string form = null, string v_model_number = null, string hxTrigger = null, string @class = null, string style = null, string dataBind = null, bool? multiple = null, string name = null, bool disabled = false, string onchange = null, Tuple<string, string>[] data = null) { return new HtmlElement(StringBuilder, "select", id: id, v_model: v_model, v_model_number: v_model_number, @class: @class, style: style, dataBind: dataBind, onchange: onchange, multiple: multiple, name: name, disabled: disabled, form: form, hxTrigger: hxTrigger, data: data); }
        public HtmlElement Button(string id = null, string form = null, string v_on_click = null, bool? disabled = null, string @class = null, string type = null, string style = null, string dataBind = null, string onclick = null, string title = null, string dataUrl = null, string dataId = null, string hxPost = null, string hxGet = null, string hxTrigger = null, string hxInclude = null, string hxTarget = null, string hxSwap = null, string hxVals = null, string hxRedirect = null, string hxDisabledElt = null, string hxOnBeforeRequest = null, string hxOnAfterRequest = null, params string[] data) { return new HtmlElement(StringBuilder, "button", id: id, form: form, @class: @class, style: style, dataBind: dataBind, v_on_click: v_on_click, type: type, onclick: onclick, title: title, dataUrl: dataUrl, dataId: dataId, dataArray: data, hxPost: hxPost, hxTrigger: hxTrigger, hxInclude: hxInclude, hxTarget: hxTarget, hxSwap: hxSwap, hxRedirect: hxRedirect, hxVals: hxVals, hxDisabledElt: hxDisabledElt, disabled: disabled, hxGet: hxGet, hxOnAfterRequest: hxOnAfterRequest, hxOnBeforeRequest: hxOnBeforeRequest); }
        public HtmlElement OptGroup(string id = null, string @class = null, string style = null, string dataBind = null, string label = null, Tuple<string, string>[] data = null) { return new HtmlElement(StringBuilder, "optgroup", id: id, @class: @class, style: style, dataBind: dataBind, label: label, data: data); }
        public HtmlElement Table(string id = null, string @class = null, string style = null, string dataBind = null) { return new HtmlElement(StringBuilder, "table", id: id, @class: @class, style: style, dataBind: dataBind); }
        public HtmlElement Tr(string id = null, string v_for = null, string v_key = null, string @class = null, string style = null, string dataBind = null, params string[] data) { return new HtmlElement(StringBuilder, "tr", id: id, v_for: v_for, v_key: v_key, @class: @class, style: style, dataBind: dataBind, dataArray: data); }
        public HtmlElement Td(string id = null, string v_for = null, string @class = null, string v_if = null, string v_show = null, string v_colspan = null, string style = null, string dataBind = null, int? colspan = null, int? rowspan = null, string data_tsv = null, string data_value = null) { return new HtmlElement(StringBuilder, "td", id: id, v_for: v_for, v_if: v_if, v_show: v_show, v_colspan: v_colspan, @class: @class, style: style, colspan: colspan, rowspan: rowspan, dataBind: dataBind, data_tsv: data_tsv, data_value: data_value); }
        public HtmlElement Th(string id = null, string @class = null, string v_if = null, string style = null, string dataBind = null, int? colspan = null, int? rowspan = null) { return new HtmlElement(StringBuilder, "th", id: id, v_if: v_if, @class: @class, style: style, colspan: colspan, rowspan: rowspan, dataBind: dataBind); }
        public HtmlElement Textarea2(string id = null, string @class = null, string style = null, string name = null, int? tabindex = null, string placeholder = null, string dataBind = null, Tuple<string, string>[] data = null, bool? spellcheck = null, bool @readonly = false, string onclick = null, string wrap = null) { return new HtmlElement(StringBuilder, "textarea", id: id, @class: @class, style: style, name: name, tabIndex: tabindex, dataBind: dataBind, data: data, onclick: onclick); }
        public HtmlElement Progress(string id = null, string value = null, string @class = null, int? max = null) { return new HtmlElement(StringBuilder, "progress", id: id, @class: @class, value: value, max: max); }

        public void Link(string rel, string type, string href)
        {
            StringBuilder.Append(@"<link rel=""").Append(rel).Append(@""" type=""").Append(type).Append(@""" href=""").Append(href).Append(@""" />");
        }

        public void Hr(string style = null, string @class = null)
        {
            StringBuilder.Append("<hr");
            if (!string.IsNullOrWhiteSpace(style)) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (!string.IsNullOrWhiteSpace(@class)) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            StringBuilder.Append(" />");
        }

        public void Br() { StringBuilder.Append("<br />"); }

        public void Title(string title)
        {
            StringBuilder.Append("<title>").Append(title).Append("</title>");
        }

        public void Comment(string s)
        {
            StringBuilder.Append("<!-- ").Append(s).Append(" -->");
        }

        public void I(string @class, string id = null, string style = null)
        {
            StringBuilder.Append(@"<i class=""").Append(@class).Append('"');
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            StringBuilder.Append("></i>");
        }


        public void Script(string src, string @class = null, Tuple<string, string>[] data = null)
        {
            StringBuilder.Append(@"<script src=""").Append(src).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append("></script>");
        }

        public void Meta(string name = null, string charset = null, string content = null)
        {
            StringBuilder.Append(@"<meta");
            if (name != null) StringBuilder.Append(@" name=""").Append(name).Append('"');
            if (content != null) StringBuilder.Append(@" content=""").Append(content).Append('"');
            if (charset != null) StringBuilder.Append(@" charset=""").Append(charset).Append('"');
            StringBuilder.Append(" />");
        }

        public void Img(string src = null, string style = null, string @class = null, string id = null, string alt = null, string itemprop = null, string dataBind = null, string[] data = null)
        {
            StringBuilder.Append(@"<img");
            if (src != null) StringBuilder.Append(@" src=""").Append(src).Append('"');
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (alt != null) StringBuilder.Append(@" alt=""").Append(alt).Append('"');
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (itemprop != null) StringBuilder.Append(@" itemprop=""").Append(itemprop).Append('"');
            if (data != null)
            {
                foreach (var e in data)
                {
                    if (string.IsNullOrWhiteSpace(e)) continue;
                    var keyValue = e.Split('=');
                    var dataKey = keyValue[0];
                    var dataValue = (keyValue.Length > 1) ? keyValue[1] : "";
                    StringBuilder.Append(@" data-").Append(dataKey).Append(@"=""").Append(dataValue).Append('"');
                }
            }
            StringBuilder.Append(" />");
        }

        public void Option(string id = null, string @class = null, string style = null, string value = null, bool? selected = null, string text = null, bool? disabled = null, string dataBind = null, Tuple<string, string>[] data = null)
        {
            StringBuilder.Append(@"<option");
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (disabled == true) StringBuilder.Append(@" disabled=""disabled""");
            if (value != null) StringBuilder.Append(@" value=""").Append(value).Append('"');
            if (selected != null && selected.Value) StringBuilder.Append(@" selected=""selected""");
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append(">");
            if (text != null) StringBuilder.Append(text);
            StringBuilder.Append("</option>");
        }

        public void Textarea(string id = null, string v_model = null, string form = null, string @class = null, string style = null, string name = null, int? tabindex = null, string text = null, string placeholder = null, string dataBind = null, Tuple<string, string>[] data = null, bool? spellcheck = null, bool @readonly = false, string wrap = null, int? rows = null)
        {
            StringBuilder.Append(@"<textarea");
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (name != null) StringBuilder.Append(@" name=""").Append(name).Append('"');
            if (form != null) StringBuilder.Append(@" form=""").Append(form).Append('"');
            if (v_model != null) StringBuilder.Append(@" v-model=""").Append(v_model).Append('"');
            if (tabindex != null) StringBuilder.Append(@" tabindex=""").Append(tabindex.Value).Append('"');
            if (placeholder != null) StringBuilder.Append(@" placeholder=""").Append(placeholder).Append('"');
            if (wrap != null) StringBuilder.Append(@" wrap=""").Append(wrap).Append('"');
            if (rows.HasValue) StringBuilder.Append(@" rows=""").Append(rows.Value).Append('"');
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (spellcheck.HasValue) StringBuilder.Append(@" spellcheck=""").Append(spellcheck.Value ? "true" : "false").Append('"');
            if (@readonly) StringBuilder.Append(@" readonly=""readonly""");
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append(">");
            if (text != null) StringBuilder.Append(text.Replace(System.Environment.NewLine, "&#10;"));
            StringBuilder.Append("</textarea>");
        }

        public void InputHidden(string id = null, string v_model = null, string v_bind_value = null, string form = null, string name = null, string value = null, string style = null, string dataBind = null, string @class = null, Tuple<string, string>[] data = null)
        {
            StringBuilder.Append(@"<input type=""hidden""");
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (name != null) StringBuilder.Append(@" name=""").Append(name).Append('"');
            if (form != null) StringBuilder.Append(@" form=""").Append(form).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (v_model != null) StringBuilder.Append(@" v-model=""").Append(v_model).Append('"');
            if (v_bind_value != null) StringBuilder.Append(@" :value=""").Append(v_bind_value).Append('"');
            if (value != null) StringBuilder.Append(@" value=""").Append(System.Net.WebUtility.HtmlEncode(value)).Append('"');
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append(" />");
        }

        public void InputText(string id = null, string v_model = null, string form = null, string hxGet = null, string hxVals = null, string hxTrigger = null, string hxTarget = null, string hxInclude = null, string list = null, string v_if = null, string @class = null, int? maxlength = null, string style = null, string name = null, string value = null, bool @readonly = false, string placeholder = null, string dataBind = null, int? tabindex = null, bool autofocus = false, string autocomplete = null, bool? autocorrect = null, bool? autocapitalize = null, bool? spellcheck = null, Tuple<string, string>[] data = null)
        {
            StringBuilder.Append(@"<input type=""text""");
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (name != null) StringBuilder.Append(@" name=""").Append(name).Append('"');
            if (form != null) StringBuilder.Append(@" form=""").Append(form).Append('"');
            if (v_model != null) StringBuilder.Append(@" v-model=""").Append(v_model).Append('"');
            if (v_if != null) StringBuilder.Append(@" v-if=""").Append(v_if).Append('"');
            if (tabindex != null) StringBuilder.Append(@" tabindex=""").Append(tabindex.Value).Append('"');
            if (value != null) StringBuilder.Append(@" value=""").Append(System.Net.WebUtility.HtmlEncode(value)).Append('"');
            if (placeholder != null) StringBuilder.Append(@" placeholder=""").Append(placeholder).Append('"');
            if (@readonly) StringBuilder.Append(@" readonly=""readonly""");
            if (maxlength != null) StringBuilder.Append(@" maxlength=""").Append(maxlength.Value).Append('"');
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (autofocus) StringBuilder.Append(@" autofocus");
            if (autocomplete != null) StringBuilder.Append(@" autocomplete=""").Append(autocomplete).Append('"');
            if (hxGet != null) StringBuilder.Append(@" hx-get=""").Append(hxGet).Append('"');
            if (hxTrigger != null) StringBuilder.Append(@" hx-trigger=""").Append(hxTrigger).Append('"');
            if (hxTarget != null) StringBuilder.Append(@" hx-target=""").Append(hxTarget).Append('"');
            if (hxInclude != null) StringBuilder.Append(@" hx-include=""").Append(hxInclude).Append('"');
            if (hxVals != null) StringBuilder.Append(@" hx-vals=""").Append(hxVals).Append('"');
            if (list != null) StringBuilder.Append(@" list=""").Append(list).Append('"');
            if (autocorrect.HasValue && !autocorrect.Value) StringBuilder.Append(@" autocorrect=""off""");
            if (autocapitalize.HasValue && !autocapitalize.Value) StringBuilder.Append(@" autocapitalize=""off""");
            if (spellcheck.HasValue && !spellcheck.Value) StringBuilder.Append(@" spellcheck=""off""");
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append(" />");
        }

        public void InputSearch(string id = null, string v_model = null, string hxTrigger = null, string hxTarget = null, string hxSelect = null, string hxGet = null, string v_if = null, string @class = null, int? maxlength = null, string style = null, string name = null, string value = null, bool @readonly = false, string placeholder = null, string dataBind = null, int? tabindex = null, bool autofocus = false, bool? autocomplete = null, bool? autocorrect = null, bool? autocapitalize = null, bool? spellcheck = null, Tuple<string, string>[] data = null)
        {
            StringBuilder.Append(@"<input type=""search""");
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (name != null) StringBuilder.Append(@" name=""").Append(name).Append('"');
            if (hxTrigger != null) StringBuilder.Append(@" hx-trigger=""").Append(hxTrigger).Append('"');
            if (hxTarget != null) StringBuilder.Append(@" hx-target=""").Append(hxTarget).Append('"');
            if (hxSelect != null) StringBuilder.Append(@" hx-select=""").Append(hxSelect).Append('"');
            if (hxGet != null) StringBuilder.Append(@" hx-get=""").Append(hxGet).Append('"');
            if (v_model != null) StringBuilder.Append(@" v-model=""").Append(v_model).Append('"');
            if (v_if != null) StringBuilder.Append(@" v-if=""").Append(v_if).Append('"');
            if (tabindex != null) StringBuilder.Append(@" tabindex=""").Append(tabindex.Value).Append('"');
            if (value != null) StringBuilder.Append(@" value=""").Append(System.Net.WebUtility.HtmlEncode(value)).Append('"');
            if (placeholder != null) StringBuilder.Append(@" placeholder=""").Append(placeholder).Append('"');
            if (@readonly) StringBuilder.Append(@" readonly=""readonly""");
            if (maxlength != null) StringBuilder.Append(@" maxlength=""").Append(maxlength.Value).Append('"');
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (autofocus) StringBuilder.Append(@" autofocus");
            if (autocomplete.HasValue && !autocomplete.Value) StringBuilder.Append(@" autocomplete=""off""");
            if (autocorrect.HasValue && !autocorrect.Value) StringBuilder.Append(@" autocorrect=""off""");
            if (autocapitalize.HasValue && !autocapitalize.Value) StringBuilder.Append(@" autocapitalize=""off""");
            if (spellcheck.HasValue && !spellcheck.Value) StringBuilder.Append(@" spellcheck=""off""");
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append(" />");
        }

        public void InputDate(string id = null, string v_model = null, string form = null, string @class = null, string style = null, string name = null, int? tabindex = null, DateTime? value = null, bool @readonly = false, string dataBind = null, string placeholder = null, Tuple<string, string>[] data = null)
        {
            StringBuilder.Append(@"<input type=""date""");
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (v_model != null) StringBuilder.Append(@" v-model=""").Append(v_model).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (tabindex != null) StringBuilder.Append(@" tabindex=""").Append(tabindex.Value).Append('"');
            if (name != null) StringBuilder.Append(@" name=""").Append(name).Append('"');
            if (form != null) StringBuilder.Append(@" form=""").Append(form).Append('"');
            if (value != null) StringBuilder.Append(@" value=""").Append(value.Value.ToString("yyyy-MM-dd")).Append('"');
            if (placeholder != null) StringBuilder.Append(@" placeholder=""").Append(placeholder).Append('"');
            if (@readonly) StringBuilder.Append(@" readonly=""readonly""");
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append(" />");
        }

        public void InputFile(string id = null, string @class = null, string form = null, string onchange = null, string style = null, string hxVals = null, string hxPost = null, string hxTrigger = null, string hxEncoding = null, string hxTarget = null, string hxSwap = null, string hxDisabledElt = null, string accept = null, string name = null, int? tabindex = null, string dataBind = null, string vIf = null, string vShow = null, Tuple<string, string>[] data = null)
        {
            StringBuilder.Append(@"<input type=""file""");
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (tabindex != null) StringBuilder.Append(@" tabindex=""").Append(tabindex.Value).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (accept != null) StringBuilder.Append(@" accept=""").Append(accept).Append('"');
            if (name != null) StringBuilder.Append(@" name=""").Append(name).Append('"');
            if (form != null) StringBuilder.Append(@" form=""").Append(form).Append('"');
            if (hxPost != null) StringBuilder.Append(@" hx-post=""").Append(hxPost).Append('"');
            if (hxTrigger != null) StringBuilder.Append(@" hx-trigger=""").Append(hxTrigger).Append('"');
            if (hxEncoding != null) StringBuilder.Append(@" hx-encoding=""").Append(hxEncoding).Append('"');
            if (hxTarget != null) StringBuilder.Append(@" hx-target=""").Append(hxTarget).Append('"');
            if (hxSwap != null) StringBuilder.Append(@" hx-swap=""").Append(hxSwap).Append('"');
            if (hxVals != null) StringBuilder.Append(@" hx-vals=""").Append(hxVals).Append('"');
            if (hxDisabledElt != null) StringBuilder.Append(@" hx-disabled-elt=""").Append(hxDisabledElt).Append('"');
            if (onchange != null) StringBuilder.Append(@" onchange=""").Append(onchange).Append('"');
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (vIf != null) StringBuilder.Append(@" v-if=""").Append(vIf).Append('"');
            if (vShow != null) StringBuilder.Append(@" v-show=""").Append(vShow).Append('"');
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append(" />");
        }

        public void InputRadio(string id = null, string v_model = null, string @class = null, string style = null, string name = null, string value = null, int? tabindex = null, bool? @checked = null, string dataBind = null, Tuple<string, string>[] data = null, string onclick = null)
        {
            StringBuilder.Append(@"<input type=""radio""");
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (name != null) StringBuilder.Append(@" name=""").Append(name).Append('"');
            if (tabindex != null) StringBuilder.Append(@" tabindex=""").Append(tabindex.Value).Append('"');
            if (value != null) StringBuilder.Append(@" value=""").Append(value).Append('"');
            if (v_model != null) StringBuilder.Append(@" v-model=""").Append(v_model).Append('"');
            if (onclick != null) StringBuilder.Append(@" onclick=""").Append(onclick).Append('"');
            if (@checked != null && @checked.Value) StringBuilder.Append(@" checked=""checked""");
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append(" />");
        }

        public void InputSubmit(string id = null, string @class = null, string style = null, string name = null, int? tabindex = null, string value = null, string dataBind = null, string onClick = null, string title = null, Tuple<string, string>[] data = null)
        {
            StringBuilder.Append(@"<input type=""submit""");
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (tabindex != null) StringBuilder.Append(@" tabindex=""").Append(tabindex.Value).Append('"');
            if (name != null) StringBuilder.Append(@" name=""").Append(name).Append('"');
            if (value != null) StringBuilder.Append(@" value=""").Append(value).Append('"');
            if (title != null) StringBuilder.Append(@" title=""").Append(title).Append('"');
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (onClick != null) StringBuilder.Append(@" onClick=""").Append(onClick).Append('"');
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append(" />");
        }

        public void InputCheckbox(string id = null, string @class = null, string form = null, string v_model = null, string v_on_change = null, string style = null, string name = null, string value = null, int? tabindex = null, bool? @checked = null, string dataBind = null, bool? disabled = null, Tuple<string, string>[] data = null, string onClick = null)
        {
            StringBuilder.Append(@"<input type=""checkbox""");
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (form != null) StringBuilder.Append(@" form=""").Append(form).Append('"');
            if (v_model != null) StringBuilder.Append(@" v-model=""").Append(v_model).Append('"');
            if (v_on_change != null) StringBuilder.Append(@" v-on:change=""").Append(v_on_change).Append('"');
            if (name != null) StringBuilder.Append(@" name=""").Append(name).Append('"');
            if (value != null) StringBuilder.Append(@" value=""").Append(value).Append('"');
            if (tabindex != null) StringBuilder.Append(@" tabindex=""").Append(tabindex.Value).Append('"');
            if (disabled != null && disabled.Value) StringBuilder.Append(@" disabled=""disabled""");
            if (@checked != null && @checked.Value) StringBuilder.Append(@" checked=""checked""");
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (onClick != null) StringBuilder.Append(@" onClick=""").Append(onClick).Append('"');
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append(" />");
        }

        public void InputPassword(string id = null, string v_model = null, string @class = null, int? maxlength = null, string style = null, string name = null, int? tabindex = null, string value = null, string dataBind = null, string placeholder = null, bool autofocus = false, Tuple<string, string>[] data = null)
        {
            StringBuilder.Append(@"<input type=""password""");
            if (id != null) StringBuilder.Append(@" id=""").Append(id).Append('"');
            if (@class != null) StringBuilder.Append(@" class=""").Append(@class).Append('"');
            if (style != null) StringBuilder.Append(@" style=""").Append(style).Append('"');
            if (name != null) StringBuilder.Append(@" name=""").Append(name).Append('"');
            if (v_model != null) StringBuilder.Append(@" v-model=""").Append(v_model).Append('"');
            if (value != null) StringBuilder.Append(@" value=""").Append(value).Append('"');
            if (tabindex != null) StringBuilder.Append(@" tabindex=""").Append(tabindex.Value).Append('"');
            if (placeholder != null) StringBuilder.Append(@" placeholder=""").Append(placeholder).Append('"');
            if (maxlength != null) StringBuilder.Append(@" maxlength=""").Append(maxlength.Value).Append('"');
            if (dataBind != null) StringBuilder.Append(@" data-bind=""").Append(dataBind).Append('"');
            if (autofocus) StringBuilder.Append(@" autofocus");
            if (data != null) foreach (var e in data) StringBuilder.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
            StringBuilder.Append(" />");
        }

        public void CssRule(string selector, string declaration)
        {
            StringBuilder.Append(selector).Append(" { ").Append(declaration).Append(" }");
        }

        public MediaQueryBlock MediaQuery(string condition) => new MediaQueryBlock(StringBuilder, condition);

        public sealed class MediaQueryBlock : IDisposable
        {
            private StringBuilder sb;

            public MediaQueryBlock(StringBuilder sb, string condition)
            {
                this.sb = sb;
                sb.Append("@media ").Append(condition).Append(" { ");
            }

            public void Dispose()
            {
                sb.Append('}');
            }
        }

        public sealed class HtmlElement : IDisposable
        {
            private StringBuilder sb;
            private string elementName;

            public HtmlElement(StringBuilder sb, string elementName, DateTime? datetime = null, string loading = null, string action = null, string value = null, Enctype? enctype = null, string method = null, string name = null, string target = null, string @class = null, string id = null, string style = null, string title = null, string lang = null, string legend = null, string src = null, string href = null, string @for = null, int? colspan = null, int? rowspan = null, string label = null, string hxTrigger = null, string hxSwap = null, string hxTarget = null, string hxIndicator = null, string hxDisabledElt = null, bool? hxBoost = null, string ariaCurrent = null, string dir = null, int? max = null, string translate = null, string dataBind = null, string v_if = null, string v_show = null, bool? v_cloak = null, string v_for = null, string v_bind_style = null, string v_model = null, string v_model_number = null, string v_on_click = null, string v_key = null, string v_colspan = null, string hxPost = null, string hxGet = null, string hxInclude = null, string hxVals = null, string hxRedirect = null, string hxSelect = null, string hxOnBeforeRequest = null, string hxOnAfterRequest = null, string onclick = null, string onsubmit = null, string onchange = null, string onload = null, string accessKey = null, int? tabIndex = null, string rev = null, string rel = null, string itemprop = null, string role = null, string hreflang = null, string appearance = null, bool? multiple = null, bool? disabled = null, string type = null, string dataUrl = null, string dataId = null, Tuple<string, string>[] data = null, string[] dataArray = null, string @is = null, string tag = null, string handle = null, string closedby = null, bool? open = false, bool? frameBorder = null, bool? scrolling = null, string data_tsv = null, string data_value = null, string v_href = null, string form = null, bool? isModule = null, string doctype = null, StringBuilder srcdoc = null)
            {
                this.sb = sb;
                this.elementName = elementName;

                if (doctype != null) sb.Append(doctype);
                sb.Append('<');
                sb.Append(elementName);
                if (lang != null) sb.Append(@" lang=""").Append(lang).Append('"');
                if (translate != null) sb.Append(@" translate=""").Append(translate).Append('"');
                if (action != null) sb.Append(@" action=""").Append(action).Append('"');
                if (legend != null) sb.Append(@" legend=""").Append(legend).Append('"');
                if (loading != null) sb.Append(@" loading=""").Append(loading).Append('"');
                if (datetime.HasValue) sb.Append(@" datetime=""").Append(datetime.Value.ToString("yyyy-MM-ddThh:mm:ssZ")).Append('"');
                if (src != null) sb.Append(@" src=""").Append(src).Append('"');
                if (srcdoc != null)
                {
                    sb.Append(@" srcdoc=""");
                    var encoder = HtmlEncoder.Default;
                    using (var writer = new StringWriter(sb))
                    {
                        foreach (var chunk in srcdoc.GetChunks())
                        {
                            var buffer = chunk.ToArray();
                            encoder.Encode(writer, buffer, 0, buffer.Length);
                        }
                    }
                    sb.Append('"');
                }
                if (href != null) sb.Append(@" href=""").Append(href).Append('"');
                if (v_href != null) sb.Append(@" v-bind:href=""").Append(v_href).Append('"');
                if (value != null) sb.Append(@" value=""").Append(value).Append('"');
                if (label != null) sb.Append(@" label=""").Append(label).Append('"');
                if (@for != null) sb.Append(@" for=""").Append(@for).Append('"');
                if (colspan.HasValue) sb.Append(@" colspan=""").Append(colspan.Value).Append('"');
                if (rowspan.HasValue) sb.Append(@" rowspan=""").Append(rowspan.Value).Append('"');
                if (max.HasValue) sb.Append(@" max=""").Append(max.Value).Append('"');
                if (type != null) sb.Append(@" type=""").Append(type).Append('"');
                if (isModule.HasValue && isModule.Value) sb.Append(@" type=""module""");
                if (enctype != null)
                {
                    switch (enctype.Value)
                    {
                        case Enctype.applicationxwwwformurlencoded: sb.Append(@" enctype=""application/x-www-form-urlencoded"""); break;
                        case Enctype.multipartformdata: sb.Append(@" enctype=""multipart/form-data"""); break;
                        case Enctype.textplain: sb.Append(@" enctype=""text/plain"""); break;
                    }
                }
                if (method != null) sb.Append(@" method=""").Append(method).Append('"');
                if (name != null) sb.Append(@" name=""").Append(name).Append('"');
                if (target != null) sb.Append(@" target=""").Append(target).Append('"');
                if (form != null) sb.Append(@" form=""").Append(form).Append('"');
                if (@class != null) sb.Append(@" class=""").Append(@class).Append('"');
                if (id != null) sb.Append(@" id=""").Append(id).Append('"');
                if (style != null) sb.Append(@" style=""").Append(style).Append('"');
                if (title != null) sb.Append(@" title=""").Append(title).Append('"');
                if (dir != null) sb.Append(@" dir=""").Append(dir).Append('"');
                if (dataBind != null) sb.Append(@" data-bind=""").Append(dataBind).Append('"');
                if (v_cloak.HasValue && v_cloak.Value) sb.Append(@" v-cloak");
                if (v_if != null) sb.Append(@" v-if=""").Append(v_if).Append('"');
                if (v_show != null) sb.Append(@" v-show=""").Append(v_show).Append('"');
                if (v_for != null) sb.Append(@" v-for=""").Append(v_for).Append('"');
                if (v_key != null) sb.Append(@" v-key=""").Append(v_key).Append('"');
                if (v_bind_style != null) sb.Append(@" v-bind:style=""").Append(v_bind_style).Append('"');
                if (v_model != null) sb.Append(@" v-model=""").Append(v_model).Append('"');
                if (v_model_number != null) sb.Append(@" v-model.number=""").Append(v_model_number).Append('"');
                if (v_on_click != null) sb.Append(@" v-on:click=""").Append(v_on_click).Append('"');
                if (v_colspan != null) sb.Append(@" v-bind:colspan=""").Append(v_colspan).Append('"');
                if (hxTrigger != null) sb.Append(@" hx-trigger=""").Append(hxTrigger).Append('"');
                if (hxSwap != null) sb.Append(@" hx-swap=""").Append(hxSwap).Append('"');
                if (hxTarget != null) sb.Append(@" hx-target=""").Append(hxTarget).Append('"');
                if (hxIndicator != null) sb.Append(@" hx-indicator=""").Append(hxIndicator).Append('"');
                if (hxDisabledElt != null) sb.Append(@" hx-disabled-elt=""").Append(hxDisabledElt).Append('"');
                if (hxBoost.HasValue) sb.Append(@" hx-boost=""").Append(hxBoost.Value.ToString().ToLowerInvariant()).Append('"');
                if (hxPost != null) sb.Append(@" hx-post=""").Append(hxPost).Append('"');
                if (hxGet != null) sb.Append(@" hx-get=""").Append(hxGet).Append('"');
                if (hxInclude != null) sb.Append(@" hx-include=""").Append(hxInclude).Append('"');
                if (hxVals != null) sb.Append(@" hx-vals=""").Append(hxVals).Append('"');
                if (hxRedirect != null) sb.Append(@" hx-redirect=""").Append(hxRedirect).Append('"');
                if (hxSelect != null) sb.Append(@" hx-select=""").Append(hxSelect).Append('"');
                if (hxOnBeforeRequest != null) sb.Append(@" hx-on::before-request=""").Append(hxOnBeforeRequest).Append('"');
                if (hxOnAfterRequest != null) sb.Append(@" hx-on::after-request=""").Append(hxOnAfterRequest).Append('"');
                if (ariaCurrent != null) sb.Append(@" aria-current=""").Append(ariaCurrent).Append('"');
                if (onclick != null) sb.Append(@" onclick=""").Append(onclick).Append('"');
                if (onsubmit != null) sb.Append(@" onsubmit=""").Append(onsubmit).Append('"');
                if (onchange != null) sb.Append(@" onchange=""").Append(onchange).Append('"');
                if (onload != null) sb.Append(@" onload=""").Append(onload).Append('"');
                if (accessKey != null) sb.Append(@" accesskey=""").Append(accessKey).Append('"');
                if (tabIndex.HasValue) sb.Append(@" tabindex=""").Append(tabIndex.Value).Append('"');
                if (rev != null) sb.Append(@" rev=""").Append(rev).Append('"');
                if (rel != null) sb.Append(@" rel=""").Append(rel).Append('"');
                if (itemprop != null) sb.Append(@" itemprop=""").Append(itemprop).Append('"');
                if (role != null) sb.Append(@" role=""").Append(role).Append('"');
                if (hreflang != null) sb.Append(@" hreflang=""").Append(hreflang).Append('"');
                if (appearance != null) sb.Append(@" appearance=""").Append(appearance).Append('"');
                if (multiple.HasValue && multiple.Value) sb.Append(@" multiple=""multiple""");
                if (disabled.HasValue && disabled.Value) sb.Append(@" disabled=""disabled""");
                if (dataUrl != null) sb.Append(@" data-url=""").Append(dataUrl).Append('"');
                if (dataId != null) sb.Append(@" data-id=""").Append(dataId).Append('"');
                if (@is != null) sb.Append(@" is=""").Append(@is).Append('"');
                if (tag != null) sb.Append(@" tag=""").Append(tag).Append('"');
                if (handle != null) sb.Append(@" handle=""").Append(handle).Append('"');
                if (closedby != null) sb.Append(@" closedby=""").Append(closedby).Append('"');
                if (open.HasValue && open.Value) sb.Append(@" open");
                if (frameBorder.HasValue) sb.Append(@" frameborder=""").Append(frameBorder.Value ? "1" : "0").Append('"');
                if (scrolling.HasValue) sb.Append(@" scrolling=""").Append(scrolling.Value ? "yes" : "no").Append('"');
                if (data_tsv != null) sb.Append(@" data-tsv=""").Append(data_tsv).Append('"');
                if (data_value != null) sb.Append(@" data-value=""").Append(data_value).Append('"');
                if (data != null)
                {
                    foreach (var e in data)
                    {
                        sb.Append(@" data-").Append(e.Item1).Append(@"=""").Append(e.Item2).Append('"');
                    }
                }
                if (dataArray != null)
                {
                    foreach (var e in dataArray)
                    {
                        if (string.IsNullOrWhiteSpace(e)) continue;
                        var keyValue = e.Split('=');
                        var dataKey = keyValue[0];
                        var dataValue = (keyValue.Length > 1) ? keyValue[1] : "";
                        sb.Append(@" data-").Append(dataKey).Append(@"=""").Append(dataValue).Append('"');
                    }
                }
                sb.Append('>');
            }

            public void Dispose()
            {
                sb.Append("</").Append(elementName).Append('>');
            }
        }
    }
}
