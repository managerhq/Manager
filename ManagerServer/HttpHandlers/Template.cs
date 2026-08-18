using ManagerServer.Globalization;
using ManagerServer.HttpHandlers.Businesses.Business;
using ManagerServer.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    public class Template : HttpHandler
    {
        [InheritedProtoMember(102)] public bool? ContentOnly;
        [InheritedProtoMember(103)] public bool? ContentOnlyForIframe;

        protected virtual void InnerHead()
        {
        }

        protected virtual Task InnerGet()
        {
            return Task.CompletedTask;
        }

        protected virtual Task InnerPost()
        {
            return Task.CompletedTask;
        }

        public override sealed Task Post()
        {
            return InnerPost();
        }

        protected void WriteHelp()
        {
            WriteHelp(false);
        }

        protected void WriteHelp(bool bounce)
        {
            var key = ConvertPascalToKebabCase(this.GetType()).Substring(1);
            WriteHelp(key, bounce);
        }

        protected string GetHelpUrl()
        {
            if (Whitelabel.IsEnabled) return string.Empty;

#if !DEBUG
            if (Edition.IsServer) return string.Empty;
#endif

            var hostname = Request.Host.Host?.ToString().ToLowerInvariant();
            if (hostname.EndsWith(".accounting.link")) return string.Empty;

            if (!this.GetType().GetCustomAttributes<GuideAttribute>().Any()) return string.Empty;

            var key = ConvertPascalToKebabCase(this.GetType()).Substring(1);
            var url = "https://www.manager.io";
            if (Strings.CurrentLanguage.Value != "en") url += "/" + Strings.CurrentLanguage.Value;
            url += "/guides/";
            url += key;
            return url;
        }

        protected void WriteHelp(string key, bool bounce)
        {
            if (Whitelabel.IsEnabled) return;

#if !DEBUG
            if (Edition.IsServer) return;
#endif

            var hostname = Request.Host.Host?.ToString().ToLowerInvariant();
            if (hostname.EndsWith(".accounting.link")) return;

            if (this.GetType().GetCustomAttributes<GuideAttribute>().Any())
            {
                var url = "https://www.manager.io";
                //if (Strings.CurrentLanguage.Value != "en") url += "/" + Strings.CurrentLanguage.Value;
                url += "/guides/";
                url += key;

                using (A(href: url, target: "_blank", @class: "text-(--muted-foreground) opacity-25 hover:opacity-50"))
                {
                    if (bounce)
                    {
                        I(@class: "fas fa-circle-question fa-bounce text-base");
                    }
                    else
                    {
                        I(@class: "fas fa-circle-question text-base");
                    }
                }
            }
        }

        private string GetTitle()
        {
            var titleText = string.Empty;
            var title = this.GetType().GetCustomAttribute<TitleAttribute>();
            if (title != null)
            {
                titleText = string.Join(" | ", title.Text.Select(x => ManagerServer.Globalization.Strings.GetPropertyValue(x)));
            }
            else
            {
                titleText = ManagerServer.Globalization.Strings.GetPropertyValue(this.GetType().Name);
            }

            if (this is Businesses.Business.BusinessTemplate businessTemplate)
            {
                titleText = businessTemplate.Business + " | " + titleText;
            }
            return titleText;
        }

        public override sealed async Task Get()
        {
            if (this is BusinessTemplate businessTemplate && !ApplicationData.Businesses.Exists(businessTemplate.Business))
            {
                using (Script()) Write($"window.location.href = '{new Businesses.Businesses().ToUrl()}';");
                return;
            }

            if (ContentOnly == true || ContentOnlyForIframe == true)
            {
                await InnerGet();
                return;
            }

            var entityName = string.Empty;
            var handlerFullName = this.GetType().FullName;

            Response.ContentType = "text/html; charset=UTF-8";

            using (Html(dir: Languages.IsRightToLeft() ? "rtl" : "ltr", translate: "no", lang: Strings.CurrentLanguage.Value))
            {
                using (Head())
                {
                    Write(@"<meta charset=""UTF-8"" />");
                    Write(@"<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" /> ");
                    Title(GetTitle());
                    if (!Whitelabel.IsEnabled)
                    {
                        Link(rel: "shortcut icon", type: "image/x-icon", href: "favicon.ico?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    }
                    Link(rel: "stylesheet", type: "text/css", href: "resources/fontawesome/fontawesome.css?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    Link(rel: "stylesheet", type: "text/css", href: "resources/fontawesome/solid.css?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    Link(rel: "stylesheet", type: "text/css", href: "resources/tailwind/output.css?version=" + typeof(Template).Assembly.GetName().Version.ToString());                    
                    Script("resources/htmx/htmx.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    Script("resources/htmx-extensions/pdf.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    Script("resources/htmx-extensions/form.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    Script("resources/htmx-extensions/error.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    Script("resources/custom/time.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    Script("resources/custom/tsv.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    Script("resources/custom/iframe.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    Script("resources/custom/api-proxy.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    using (Style())
                    {
                        CssRule("html:has(dialog:modal)", "overflow: hidden");
                    }
                    using (Script())
                    {
                        var options = new JsonSerializerOptions
                        {
                            IncludeFields = true,
                            Encoder = JavaScriptEncoder.Default,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                            TypeInfoResolver = new ProtoAwareResolver()
                        };

                        var payload = System.Text.Json.JsonSerializer.Serialize(new { handler = this.GetType().Name, path = Request?.Path.ToString(), query = (object)this }, options);

                        Write(@"window.addEventListener(""message"", async (event) => {
    const { data } = event;
    if (data?.type === ""reload"") {
        location.reload();
    }
    if (data?.type === ""page-request"") {
        const { requestId } = data;
        
        event.source.postMessage({
                type: ""page-response"",
                requestId,
                body: " + payload + @",
            }, event.origin);
    }
});");
                    }
                    InnerHead();

                    using (Script())
                    {
                        Write("if (localStorage.theme) document.documentElement.classList.add(localStorage.theme);");
                        Write("if (localStorage.compact === 'true') document.documentElement.classList.add('compact');");
                        Write("if (localStorage.observer === 'true') document.documentElement.classList.add('observer');");
                    }

                    using (Script())
                    {
                        Write("function onSubmit(target) {");
                        Write("var button = target.getElementsByTagName('button')[0];");
                        Write("button.disabled = true;");
                        Write("var spinner = document.createElement('i');");
                        foreach (var e in "fas fa-spinner-third fa-spin ltr:mr-4 rtl:ml-4".Split(' ')) Write($"spinner.classList.add('{e}');");
                        Write("button.prepend(spinner);");
                        Write("}");
                    }
                }

                using (Body())
                {
                    using (Dialog(@class: "card shadow-md shadow-(--muted-shadow) hidden open:block inset-0 m-auto", id: "error-dialog", onclick: "if (event.target === this) this.close()"))
                    {
                        using (Div(@class: "card-header", id: "error-title")) using (Div(@class: "card-title")) Write(Strings.Error);
                        using (Div(@class: "card-body")) using (Pre(id: "error-message")) { }
                        using (Div(@class: "card-header")) { }
                    }

                    Write(@"<noscript><div class=""text-bg-warning p-8 text-center print:hidden"" style=""border-bottom: 1px solid #ccc"">Javascript Error</div></noscript>");                    

                    if (IsAdministrator())
                    {
                        var banner = Environment.GetEnvironmentVariable("MANAGER_BANNER");
                        if (!string.IsNullOrWhiteSpace(banner))
                        {
                            using (Div(@class: "bg-(--accent) text-(--accent-foreground) flex items-center justify-center gap-2 p-4 print:hidden"))
                            {
                                using (Div()) Write(banner);
                                using (Form(action: new DismissBanner().ToUrl(), method: "POST"))
                                {
                                    InputHidden(name: "Location", value: this.ToUrl());
                                    InputSubmit(@class: "cursor-pointer bg-white border border-neutral-300 text-neutral-700 rounded py-1 px-4 hover:border-neutral-400 hover:text-neutral-800 hover:no-underline hover:bg-neutral-100 hover:shadow-inner", value: "Dismiss");
                                }
                            }
                        }
                    }

                    var currentUser = this.GetCurrentUser();

                    using (Div(@class: "p-4 lg:px-8 flex items-center justify-between gap-2 print:hidden inset-shadow-[var(--inset)] overflow-x-auto lg:overflow-visible no-scrollbar"))
                    {
                        using (Div())
                        {
                            var acceptLanguageHeader = Request.Headers["Accept-Language"].ToString();
                            var defaultLanguages = new HashSet<string>();
                            defaultLanguages.Add("en");
                            if (acceptLanguageHeader != null)
                            {
                                foreach (var e in acceptLanguageHeader.Split(',').Select(x => x.Split(';').First().ToLowerInvariant().Trim()))
                                {
                                    if (!string.IsNullOrWhiteSpace(e))
                                    {
                                        var language = e.Split('-').First();
                                        if (!string.IsNullOrWhiteSpace(language))
                                        {
                                            if (!defaultLanguages.Contains(language))
                                            {
                                                defaultLanguages.Add(language);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Edition.IsDesktop)
                            {
                                var language = System.Globalization.CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
                                if (!string.IsNullOrWhiteSpace(language))
                                {
                                    if (!defaultLanguages.Contains(language))
                                    {
                                        defaultLanguages.Add(language);
                                    }
                                }
                            }
                            var languages = ManagerServer.Globalization.Languages.GetLanguages();

                            if (languages.Length > 1)
                            {
                                using (Details(@class: "dropdown"))
                                {
                                    using (Summary(@class: "opacity-50 hover:opacity-75 whitespace-nowrap cursor-pointer"))
                                    {
                                        I(@class: "fas fa-language");
                                    }
                                    using (Div(@class: "dropdown-menu px-2"))
                                    {
                                        var current = ManagerServer.Globalization.Languages.GetLanguage();
                                        foreach (var e in ManagerServer.Globalization.Languages.GetLanguages().OrderByDescending(x => x.Code == "en").ThenBy(x => x.NativeName).GroupBy(x => defaultLanguages.Contains(x.Code)))
                                        {
                                            if (!e.Key) Hr(@class: "my-2");
                                            using (Div(@class: "lg:columns-4"))
                                            {
                                                foreach (var e2 in e)
                                                {
                                                    var css = "dropdown-item w-full cursor-pointer";
                                                    if (e2.Code == current) css += " font-bold";
                                                    using (Button(@class: css, title: e2.EnglishName, hxPost: new SwitchLanguage() { Language = e2.Code }.ToUrl(), hxDisabledElt: "this"))
                                                    {
                                                        using (Span()) Write(e2.NativeName);
                                                        I(@class: "htmx-indicator fas fa-cog fa-spin mx-2 !hidden");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (currentUser != null)
                        {
                            using (Div(@class: "flex gap-2 font-semibold"))
                            {
                                var profile = handlerFullName.StartsWith("ManagerServer.HttpHandlers.Profile.");
                                var businesses = handlerFullName.StartsWith("ManagerServer.HttpHandlers.Businesses.");
                                var support = handlerFullName.StartsWith("ManagerServer.HttpHandlers.Support.");
                                var users = handlerFullName.StartsWith("ManagerServer.HttpHandlers.Users.");
                                var extensions = handlerFullName.StartsWith("ManagerServer.HttpHandlers.Extensions.");

                                using (A(href: new Businesses.Businesses().ToUrl(), @class: $"navbar-item {(businesses ? "active" : null)}"))
                                {
                                    using (Span()) I(@class: "fas fa-building me-3 opacity-75");
                                    Write(Strings.Businesses);
                                }

                                if (currentUser.Type == UserType.Administrator)
                                {
                                    var showUsersTab = true;
                                    if (Edition.IsDesktop)
                                    {
                                        if (!File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Users.html")))
                                        {
                                            showUsersTab = false;
                                        }
                                    }

                                    if (showUsersTab)
                                    {
                                        using (A(href: new Users.Users().ToUrl(), @class: $"navbar-item {(users ? "active" : null)}"))
                                        {
                                            using (Span()) I(@class: "fas fa-people-group me-3 opacity-75");
                                            Write(Strings.Users);
                                        }
                                    }                                    
                                }

                                if (currentUser.Type == UserType.Administrator)
                                {
                                    using (A(href: new Extensions.Extensions().ToUrl(), @class: $"navbar-item {(extensions ? "active" : null)}"))
                                    {
                                        using (Span()) I(@class: "fas fa-plug me-3 opacity-75");
                                        Write(Strings.Extensions);
                                    }
                                }

                                if (currentUser.Type == UserType.Administrator)
                                {
                                    if (!string.IsNullOrWhiteSpace(Support.Support.Path))
                                    {
                                        using (A(href: new Support.Support().ToUrl(), @class: $"navbar-item {(support ? "active" : null)}"))
                                        {
                                            using (Span()) I(@class: "fas fa-question-circle me-3 opacity-75");
                                            Write(Strings.Support);
                                        }
                                    }
                                }

                                if (!Edition.IsDesktop)
                                {
                                    using (A(href: new Profile.ChangePasswordForm().ToUrl(), @class: $"navbar-item {(profile ? "active" : null)}"))
                                    {
                                        using (Span()) I(@class: "fas fa-user-circle mr-3 rtl:ml-3 opacity-75");

                                        Write(currentUser.Name);
                                    }
                                    using (A(href: new Logout().ToUrl(), @class: "navbar-item"))
                                    {
                                        using (Span()) I(@class: "fas fa-sign-out mr-3 rtl:ml-3 opacity-75");
                                        Write(Strings.Logout);
                                    }
                                }
                            }
                        }

                        using (Div())
                        {
                            using (Button(@class: "cursor-pointer text-(--foreground)/50 hover:text-(--foreground)", onclick: "document.documentElement.classList.toggle('dark'); localStorage.theme = document.documentElement.classList.contains('dark') ? 'dark' : 'light';"))
                            {
                                I(@class: "fas fa-fw fa-circle-half-stroke");
                            }                            
                        }
                    }                    

                    using (Div(@class: "pb-2 lg:px-8 print:p-0 print:lg:p-0"))
                    {
                        await InnerGet();

                        using (Div(@class: "p-4 print:hidden font-semibold text-center text-neutral-400"))
                        {
                            Write(typeof(Program).Assembly.GetName().Version.ToString());
                        }
                    }

#if DEBUG
                    using (Span(@class: "print:hidden fixed bottom-1 right-1 rtl:left-1 rtl:right-auto px-4 py-3 bg-black text-white"))
                    {
                        Write(Features.Get<Stopwatch>().ElapsedMilliseconds.ToString() + " ms");
                    }
#endif
                    using (Script())
                    {
                        var type = this.GetType();

                        while (type != typeof(Template))
                        {
                            if (!type.IsGenericType)
                            {
                                var s = type.Assembly.GetManifestResourceStream(type.FullName + ".js");
                                if (s != null)
                                {
                                    Write(new StreamReader(s).ReadToEnd());
                                }
                            }
                            type = type.BaseType;
                        }
                    }
                }
            }
        }

        protected void FormSuccessButton(string label) => FormButton(label, ManagerServer.Globalization.Strings.GetPropertyValue(label), "btn btn-success");
        protected void FormDangerButton(string label) => FormButton(label, ManagerServer.Globalization.Strings.GetPropertyValue(label), "btn btn-danger");
        protected void FormPrimaryButton(string label) => FormButton(label, ManagerServer.Globalization.Strings.GetPropertyValue(label), "btn btn-primary");

        private void FormButton(string form, string label, string @class)
        {
            using (Form(id: form, action: this.ToUrl(), method: "POST", hxBoost: true, hxDisabledElt: $"#{form} button"))
            {
                using (Button(@class: @class))
                {                    
                    Write(label);
                    I(@class: "htmx-indicator fas fa-circle-notch fa-spin ms-2 !hidden");
                }
            }
        }

        protected HttpFramework.HttpHandler.HtmlElement DefaultLink(string href)
        {
            return A(href: href, @class: "btn");
        }        

        protected HttpFramework.HttpHandler.HtmlElement DefaultButton(string onclick = null)
        {
            return Button(onclick: onclick, @class: "btn");
        }

        protected HttpFramework.HttpHandler.HtmlElement PrimaryButton(string onclick = null)
        {
            return Button(onclick: onclick, @class: "btn btn-primary");
        }

        protected HttpFramework.HttpHandler.HtmlElement DangerLink(string href)
        {
            return A(href: href, @class: "btn btn-danger");
        }

        protected HttpFramework.HttpHandler.HtmlElement DangerButton(string onclick = null)
        {
            return Button(onclick: onclick, @class: "btn btn-danger");
        }

        protected HttpFramework.HttpHandler.HtmlElement SuccessButton(string onclick = null, string form = null)
        {
            return Button(onclick: onclick, form: form, @class: "btn btn-success");
        }        

        protected HttpFramework.HttpHandler.HtmlElement PostForm(bool formData = false, string action = null)
        {
            return Form(method: "POST", onsubmit: "onSubmit(this)", action: action, enctype: formData ? HttpFramework.Enctype.multipartformdata : null);
        }

        /*
        private class ProtoMemberContractResolver : DefaultContractResolver
        {
            public static readonly ProtoMemberContractResolver Instance = new ProtoMemberContractResolver();

            protected override List<System.Reflection.MemberInfo> GetSerializableMembers(Type objectType)
            {
                var members = new List<System.Reflection.MemberInfo>();
                foreach (var field in objectType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (field.GetCustomAttribute<ProtoMemberAttribute>(true) != null) members.Add(field);
                }
                foreach (var property in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.GetCustomAttribute<ProtoMemberAttribute>(true) != null) members.Add(property);
                }
                return members;
            }
        }
        */
    }
}