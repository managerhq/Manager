using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ManagerServer.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public abstract class ScreenshotAttribute : AbstractGuideAttribute
    {
        private StringBuilder sb = new StringBuilder();

        public string GetHtml()
        {
            return sb.ToString();
        }

        protected void Keyword(string keyword)
        {
            Write($@"<span translate=""true"" keyword=""{keyword}"">");
            Write(ManagerServer.Globalization.Strings.GetPropertyValue(keyword));
            Write("</span>");
        }

        public HtmlElement Div(string @class = null) => new HtmlElement(sb, "div", @class);
        public HtmlElement Label(string @class = null) => new HtmlElement(sb, "label", @class);
        public HtmlElement Span(string @class = null) => new HtmlElement(sb, "span", @class);
        public HtmlElement Summary(string @class = null) => new HtmlElement(sb, "summary", @class);
        public HtmlElement Details(string @class = null) => new HtmlElement(sb, "details", @class);
        public void I(string @class = null) => new HtmlElement(sb, "i", @class).Dispose();
        public void Hr(string @class = null) => new HtmlElement(sb, "hr", @class).Dispose();
        public void Write(string value) => sb.Append(value);

        public sealed class HtmlElement : IDisposable
        {
            private StringBuilder sb;
            private string name;

            public HtmlElement(StringBuilder sb, string name, string @class = null)
            {
                this.sb = sb;
                this.name = name;

                sb.Append($"<{name}");
                if (name == "details") sb.Append(" open");
                if (!string.IsNullOrWhiteSpace(@class)) sb.Append(@$" class=""{@class}""");

                if (IsSelfClosingTag(name)) sb.Append(" /");
                sb.Append(">");
            }

            public void Dispose()
            {
                if (IsSelfClosingTag(name)) return;
                sb.Append($"</{name}>");
            }

            private static bool IsSelfClosingTag(string name)
            {
                if (name == "hr") return true;
                return false;
            }
        }
    }
}
