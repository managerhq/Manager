using ManagerServer;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerComponents;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Attachments;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    internal class BusinessTemplate : Template, IComparable
    {
        [InheritedProtoMember(100)] public string Business { get; set; }
        [InheritedProtoMember(101)] public string Referrer;

        protected sealed override void InnerHead()
        {
            using (Script())
            {
                Write(@"function decodeBase64Utf8ToHtml(base64) {
    const binary = atob(base64);
    const bytes = new Uint8Array([...binary].map(char => char.charCodeAt(0)));
    return new TextDecoder(""utf-8"").decode(bytes);
}");
            }

            using (Script())
            {
                Write(@"window.addEventListener(""message"", async (event) => {
    const { data } = event;
    if (data?.type === ""version-request"") {
        const { requestId } = data;
        
        event.source.postMessage({
                type: ""version-response"",
                requestId,
                body: """ + typeof(BusinessTemplate).Assembly.GetName().Version?.ToString() + @""",
            }, event.origin);
    }
});");
            }
        }

        protected virtual void InnerGet2()
        {
        }

        internal virtual bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            return false;
        }

        internal string GetTitle()
        {
            var titleAttribute = this.GetType().GetCustomAttribute<TitleAttribute>();
            if (titleAttribute != null)
            {
                return string.Join(" | ", titleAttribute.Text.Select(x => Strings.GetPropertyValue(x)));
            }
            return Strings.GetPropertyValue(this.GetType().Name);
        }

        protected sealed override Task InnerGet()
        {
            if (ContentOnly == true || ContentOnlyForIframe == true)
            {
                SetCulture(Business);
                InnerGet2();
                return Task.CompletedTask;
            }

            if (ApplicationData.Businesses.IsBackupInProgress(Business))
            {
                using (Div(@class: "bg-[var(--muted)] border border-b-0 border-[var(--border)] shadow-[inset_0_1px_var(--muted-inset)] print:hidden"))
                {
                    using (Div(@class: "flex items-center justify-between px-6 py-4"))
                    {
                        using (Div(@class: "flex items-center gap-4"))
                        {
                            I(@class: "fas fa-circle-notch fa-spin");
                            using (Div(@class: "font-semibold"))
                            {
                                Write("The business is currently in read-only mode while a backup is being downloaded. Data can be viewed, but changes will not be saved until the backup is complete.");
                            }
                            using (A(href: new BackupCancel() { Business = Business }.ToUrl())) I(@class: "fas fa-xmark text-neutral-300 hover:text-neutral-400");
                        }
                    }
                }
            }

            using (Div(@class: "hidden observer:block"))
            {
                using (Div(@class: "bg-[var(--muted)] border border-b-0 border-[var(--border)] shadow-[inset_0_1px_var(--muted-inset)] print:hidden"))
                {
                    using (Div(@class: "flex items-center justify-between px-6 py-4"))
                    {
                        using (Div(@class: "flex items-center gap-2"))
                        {
                            I(@class: "fas fa-mask");
                            using (Div(@class: "font-semibold")) Write(Strings.ObscureModeIsOnFinancialFiguresAreConcealed);
                        }
                        using (Button(onclick: "document.documentElement.classList.remove('observer'); localStorage.observer = 'false'")) I(@class: "fas fa-xmark cursor-pointer hover:opacity-75");
                    }
                }
            }
            
            UserPermissions userPermissions = null;

            using (Div(@class: "bg-[var(--muted)] border border-[var(--border)] shadow-[inset_0_1px_var(--muted-inset)] print:hidden overflow-x-auto lg:overflow-visible no-scrollbar"))
            {
                using (Div(@class: "flex items-center justify-between gap-4 px-4 py-6"))
                {
                    using (Div(@class: "flex items-center gap-4"))
                    {
                        using (A(href: new HttpHandlers.Businesses.Businesses().ToUrl(), @class: "text-[var(--muted-foreground)]/25 hover:text-[var(--muted-foreground)]/50"))
                        {
                            I(@class: "fas fa-xmark text-xl");
                        }
                        using (Div(@class: "text-lg font-bold whitespace-nowrap text-[var(--muted-foreground)] text-shadow-[var(--muted-foreground-shadow)]"))
                        {
                            Write(Business);
                        }

                        var progress = ApplicationData.Businesses.GetProgress(Business);
                        if (progress != null)
                        {
                            using (Div(@class: "flex items-center gap-2"))
                            {
                                I(@class: "fas fa-circle-notch fa-spin text-base");
                                using (Span(@class: "font-semibold", hxGet: new Progress() { Business = Business }.ToUrl(), hxTrigger: "load"))
                                {
                                    Write(progress);
                                }
                            }
                            return Task.CompletedTask;
                        }

                        var database = ApplicationData.Businesses.Get(Business);
                        if (database == null)
                        {
                            using (Script()) Write("window.location.href = " + new Businesses().ToUrl().EncodeJsString() + @";");
                            return Task.CompletedTask;
                        }
                        else if (database.Status == ManagerServer.Database.DatabaseStatus.Corrupted)
                        {
                            using (Script()) Write("window.location.href = " + new Corrupt() { Business = Business }.ToUrl().EncodeJsString());
                            return Task.CompletedTask;
                        }
                        else if (database.Status == ManagerServer.Database.DatabaseStatus.Invalid)
                        {
                            using (Script()) Write("window.location.href = " + new Invalid() { Business = Business }.ToUrl().EncodeJsString());
                            return Task.CompletedTask;
                        }
                        else if (database.Status == ManagerServer.Database.DatabaseStatus.Incompatible)
                        {
                            using (Script()) Write("window.location.href = " + new NewerVersionRequired() { Business = Business }.ToUrl().EncodeJsString());
                            return Task.CompletedTask;
                        }
                        else if (database.Status == ManagerServer.Database.DatabaseStatus.OutOfMemory)
                        {
                            using (Script()) Write("window.location.href = " + new NotEnoughMemory() { Business = Business }.ToUrl().EncodeJsString());
                            return Task.CompletedTask;
                        }

                        userPermissions = this.GetCurrentUserPermissions(Business);

                        if (userPermissions.FullAccess)
                        {
                            using (A(href: new HttpHandlers.Businesses.Business.BusinessForm() { Business = Business }.ToUrl(), @class: "text-sm")) Write(Strings.Rename);
                        }
                    }

                    if (userPermissions.FullAccess)
                    {
                        using (Div(@class: "flex items-center gap-1"))
                        {
                            using (A(href: new Emails.Emails() { Business = Business }.ToUrl(), @class: "btn")) Write(Strings.Emails);
                            if (ApplicationData.Businesses.Get(Business).Any<ManagerServer.Model.Attachment>())
                            {
                                using (A(href: new Attachments.Attachments() { Business = Business }.ToUrl(), @class: "btn")) Write(Strings.Attachments);
                            }
                            using (A(href: new History() { Business = Business }.ToUrl(), @class: "btn")) Write(Strings.History);
                            using (A(href: new Backup() { Business = Business }.ToUrl(), @class: "btn")) Write(Strings.Backup);

                            if (ApplicationData.Businesses.Get(Business).Exists<LockDate>())
                            {
                                var lockDate = ApplicationData.Businesses.Get(Business).Single<LockDate>();

                                var referrer = this.ToUrl();
                                var lockDateUrl = new Settings.LockDate.LockDateForm() { Business = Business, Key = lockDate.Key, Referrer = referrer }.ToUrl();

                                if (lockDate.GetLockDate().HasValue)
                                {
                                    using (A(href: lockDateUrl, title: lockDate.GetLockDate().Value.ToLocalShortDisplayString())) I(@class: "fas fa-lock text-muted");
                                }
                                else
                                {
                                    using (A(href: lockDateUrl)) I(@class: "fas fa-lock-open");
                                }
                            }
                        }
                    }
                }
            }

            SetCulture(Business);

            using (Div(@class: "flex flex-col lg:flex-row print:block"))
            {
                using (Div(@class: "flex lg:flex-col print:hidden overflow-x-auto lg:overflow-visible no-scrollbar", id: "sidebar"))
                {
                    var tabs = this.GetTabs(applyUserPermissions: true);
                    foreach (var e in tabs.GetAll())
                    {
                        var style = "group flex justify-between font-semibold px-3 py-2 gap-4 border-e lg:border-s border-b border-[var(--border)]";

                        if (this.GetType().FullName.StartsWith("ManagerServer.HttpHandlers.Businesses.Business." + e.Name + @"."))
                        {
                            style += " bg-[var(--card)] lg:border-e-[var(--card)]";
                        }
                        else
                        {
                            style += " bg-[var(--muted)] shadow-[inset_0_1px_var(--muted-inset)]";
                        }

                        if (this is TabsForm)
                        {
                            style += " text-[var(--muted-foreground)]/50";
                        }

                        var url = e.HttpHandler.ToUrl();
                        if (this is TabsForm) url = null;
                        if (!e.Visible)
                        {
                            if (this is TabsForm)
                            {
                                style += " hidden";
                            }
                            else
                            {
                                continue;
                            }
                        }

                        using (A(href: url, @class: style, id: "tab" + e.Name))
                        {
                            using (Span(@class: "flex gap-3 items-center"))
                            {
                                using (Span()) I(@class: "text-[var(--muted-foreground)] opacity-25 fas fa-fw " + Icons.GetIcon(e.Name));
                                using (Span(@class: $"whitespace-nowrap compact:hidden")) Write(e.DisplayName);
                            }

                            if (e.Count.HasValue)
                            {
                                using (Div(@class: $"flex items-center gap-2 compact:hidden"))
                                {
                                    var style2 = "bg-[var(--input)] border border-[var(--input-border)] text-[var(--input-foreground)]/60 text-xs whitespace-nowrap py-0 px-2 rounded tabular-nums observer:blur-sm observer:hover:blur-none observer:hover:transition";
                                    if (e.Count.Value == 0) style2 += " opacity-50";
                                    using (Span(@class: style2)) Write(e.Count.Value.ToString("N0", System.Threading.Thread.CurrentThread.CurrentCulture));
                                    if (e.PendingCount.HasValue && e.PendingCount.Value > 0)
                                    {
                                        using (Span(@class: "bg-[var(--input)] border border-[var(--input-border)] text-[var(--input-foreground)]/60 whitespace-nowrap border text-xs py-0 px-2 rounded tabular-nums observer:blur-sm observer:hover:blur-none observer:hover:transition")) Write("+" + e.PendingCount.Value.ToString("N0", System.Threading.Thread.CurrentThread.CurrentCulture));
                                    }
                                }
                            }

                            using (Div(@class: "absolute z-100 py-0.5 px-2 ltr:left-22 rtl:right-22 bg-neutral-900 text-white rounded hidden compact:lg:group-hover:block drop-shadow"))
                            {
                                Write(e.DisplayName);
                            }
                        }
                    }
                    using (Div(@class: "border-[var(--border)] lg:border-r rtl:border-l rtl:border-r-0 lg:grow lg:pb-32 lg:pt-6"))
                    {
                        if (userPermissions.FullAccess)
                        {
                            if (!(this is TabsForm))
                            {
                                using (Div(@class: $"text-center compact:hidden"))
                                {
                                    using (A(href: new TabsForm() { Business = Business, Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Tabs)) }.ToUrl(), @class: $"font-semibold compact:hidden"))
                                    {
                                        Write(Strings.Customize);
                                    }
                                    if (!ApplicationData.Businesses.Get(Business).Exists<ManagerServer.Model.Tabs>())
                                    {
                                        using (Div(@class: $"py-6 compact:hidden"))
                                        {
                                            I(@class: "fas fa-hand-pointer fa-bounce print:hidden text-neutral-400");
                                        }
                                    }
                                }                                
                            }
                        }
                    }
                }
                using (Div(@class: "bg-[var(--card)] border-[var(--border)] flex flex-col lg:gap-4 grow p-0 lg:p-6 lg:pb-32 border-e border-b print:lg:p-0 print:border-0 print:bg-transparent"))
                {
                    var isAdministrator = this.IsAdministrator();
                    if (!isAdministrator)
                    {
                        if (!userPermissions.FullAccess)
                        {
                            if (this is EmailForm)
                            {
                                // Pass
                            }
                            else if (this is Start)
                            {
                                // Pass
                            }
                            else if (this is History)
                            {
                                using (Div()) Write("You are not authorised to access this part of the system. Contact your administrator to elevate your permissions.");
                                return Task.CompletedTask;
                            }
                            else
                            {
                                if (!userPermissions.CanView(this.GetType().Namespace))
                                {
                                    using (Div()) Write("You are not authorised to access this part of the system. Contact your administrator to elevate your permissions.");
                                    return Task.CompletedTask;
                                }
                            }
                        }
                    }

                    var list = new List<Tuple<string, string, string[]>>();
                    if (Referrer != null)
                    {
                        var innerReferrer = Referrer;
                        while (!string.IsNullOrWhiteSpace(innerReferrer))
                        {
                            var parts = innerReferrer.Split('?');
                            if (parts.Length != 2) break;
                            var key = parts[0].Substring(1);
                            var query = parts[1].Split('&')[0];

                            if (!Assembly.ContainsHttpHandler(key)) break;

                            var httpHandlerType = Assembly.GetHttpHandlerType(key);
                            var httpHandler = HttpFramework.Serialization.Deserialize2(httpHandlerType, query) as BusinessTemplate;

                            if (httpHandler == null) break;

                            var title = httpHandlerType.Name;
                            var titleAttribute = httpHandlerType.GetCustomAttribute<TitleAttribute>();
                            if (titleAttribute != null) title = Strings.GetPropertyValue(titleAttribute.Text.Last());

                            list.Add(new Tuple<string, string, string[]>(title, innerReferrer, GetContextParts(httpHandler)));

                            if (httpHandler.Referrer == null) break;

                            innerReferrer = httpHandler.Referrer;
                        }
                    }

                    using (Div(@class: "print:hidden"))
                    {
                        using (Div(@class: "card"))
                        {
                            using (Div(@class: "card-header"))
                            {
                                using (Div(@class: "flex justify-between"))
                                {
                                    using (Div(@class: "flex gap-2 flex-wrap items-center text-sm"))
                                    {
                                        using (Button(@class: "block compact:hidden cursor-pointer", onclick: "document.documentElement.classList.add('compact'); localStorage.compact = 'true'")) I(@class: "fas fa-fw fa-bars opacity-25 hover:opacity-50");
                                        using (Button(@class: "hidden compact:block cursor-pointer", onclick: "document.documentElement.classList.remove('compact'); localStorage.compact = 'false'")) I(@class: "fas fa-fw fa-bars opacity-25 hover:opacity-50");

                                        I(@class: "fa fa-solid fa-caret-right opacity-25");

                                        list.Reverse();
                                        foreach (var e in list)
                                        {
                                            using (A(href: e.Item2, @class: "flex gap-2 items-center flex-wrap"))
                                            {
                                                if (!e.Item3.Any())
                                                {
                                                    using (Span()) Write(e.Item1);
                                                }
                                                else
                                                {
                                                    foreach (var e2 in e.Item3)
                                                    {
                                                        using (Span(@class: "max-w-[20ch] truncate", title: e2)) Write(e2);
                                                    }
                                                }
                                            }
                                            I(@class: "fa fa-solid fa-caret-right opacity-25");
                                        }

                                        using (Div(@class: "flex gap-2 items-center flex-wrap opacity-75"))
                                        {
                                            var title = this.GetType().Name;
                                            var titleAttribute = this.GetType().GetCustomAttribute<TitleAttribute>();
                                            if (titleAttribute != null) title = Strings.GetPropertyValue(titleAttribute.Text.Last());
                                            var contextParts = GetContextParts(this);
                                            if (!contextParts.Any())
                                            {
                                                using (Span()) Write(title);
                                            }
                                            else
                                            {
                                                foreach (var e2 in contextParts)
                                                {
                                                    using (Span(@class: "max-w-[20ch] truncate", title: e2)) Write(e2);
                                                }
                                            }
                                        }
                                    }
                                    using (Div(@class: "flex gap-2 items-center"))
                                    {
                                        using (Button(@class: "block observer:hidden cursor-pointer", onclick: "document.documentElement.classList.add('observer'); localStorage.observer = 'true'")) I(@class: "fas fa-fw fa-mask opacity-25 hover:opacity-50");
                                        using (Button(@class: "hidden observer:block cursor-pointer", onclick: "document.documentElement.classList.remove('observer'); localStorage.observer = 'false'")) I(@class: "fas fa-fw fa-mask opacity-25 hover:opacity-50");
                                    }
                                }
                            }
                        }
                    }

                    using (Div(id: "nonBatchView"))
                    {
                        InnerGet2();
                    }

                    using (Script())
                    {
                        var format = $"{CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern} {CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern}";
                        Write($"updateAllTimeElements('{format}')");
                    }

                    using (Script())
                    {
                        Write($"const MANAGER_BUSINESS = {JsonSerializer.Serialize(Business)};");
                    }

                    EmitCustomButtons();

                    using (Div(id: "batchView", style: "display: none"))
                    {
                        using (Div(@class: "card-header"))
                        {
                            using (Div(@class: "flex gap-4 items-center print:hidden"))
                            {
                                using (Div(@class: "card-title")) Write(Strings.BatchView);
                                using (A(href: "javascript:window.print()", @class: "btn btn-sm", style: "font-weight: bold")) Write(Strings.Print);
                            }
                        }
                        using (Div(@class: "card-inset", id: "batchViewContent"))
                        {
                        }
                    }                    
                }
            }

            var extensions = true;
            if (this is Summary.SummaryView) extensions = false;
            if (this is Settings.Settings) extensions = false;
            if (this is Settings.ObsoleteFeatures.ScriptExtensions.ScriptExtensions) extensions = false;
            if (this is Settings.ObsoleteFeatures.ScriptExtensions.ScriptExtensionForm) extensions = false;

            if (extensions)
            {
                var extensionList = new List<ManagerServer.Model.ScriptExtension>();
                var businessDetails = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BusinessDetails>();
                extensionList.AddRange(ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ScriptExtension>().Where(x => x.IsMatch(Request.Path)));

                foreach (var e in extensionList)
                {
                    if (string.IsNullOrWhiteSpace(e.Script)) continue;
                    using (Script()) Write(e.Script);
                }
            }

            using (Script())
            {
                // This is for mobile-friendly sidebar
                Write("var sidebar = document.getElementById('sidebar');");
                Write("var left = localStorage.getItem('sidebar-scroll');");
                Write("if (left !== null) sidebar.scrollLeft = parseInt(left, 10);");

                Write("sidebar.addEventListener('scroll', function() {");
                Write("localStorage.setItem('sidebar-scroll', sidebar.scrollLeft);");
                Write("});");
            }

            return Task.CompletedTask;
        }

        protected virtual void EmitCustomButtons()
        {
            var customButtons = GetCustomButtons();
            if (customButtons.Any())
            {
                using (Div(@class: "card print:hidden"))
                {
                    using (Div(@class: "card-header flex gap-4 items-center"))
                    {
                        using (Div(@class: "card-title")) Write(Strings.CustomButtons);
                        foreach (var e in customButtons)
                        {
                            EmitCustomButton(e, "btn");
                        }
                    }
                }
            }
        }

        protected virtual CustomButton[] GetCustomButtons()
        {
            return ApplicationData.Businesses.Get(Business).OfType<CustomButton>().Where(x => !x.Inactive && x.IsMatch(Request.Path)).ToArray();
        }

        protected virtual void EmitCustomButton(CustomButton customButton, string @class)
        {
            using (Dialog(id: customButton.Key.ToString(), @class: "m-0 ms-24 w-auto h-auto max-w-none max-h-none shadow-2xl transform transition-transform duration-300 ease-out starting:open:translate-x-full rtl:starting:open:-translate-x-full open:translate-x-0", onclick: "this.close()"))
            {
                if (customButton.Source == ManagerServer.Model.Enums.ExtensionSource.Url)
                {
                    var endpoint = customButton.Endpoint ?? string.Empty;
                    // Allow same-origin paths (e.g. "/extensions/au/business-activity-statement.html") to load
                    // relative to Manager itself; otherwise default to https://.
                    if (!endpoint.StartsWith("/") && !endpoint.StartsWith("http://") && !endpoint.StartsWith("https://")) endpoint = "https://" + endpoint;
                    using (IFrame(@class: "w-full h-full", src: endpoint, loading: "lazy")) { }
                }
                if (customButton.Source == ManagerServer.Model.Enums.ExtensionSource.Inline)
                {
                    using (IFrame(@class: "w-full h-full", src: new ManagerServer.Api.Businesses.Business.GetCustomButtonHtml() { Business = Business, Key = customButton.Key }.ToUrl(), loading: "lazy")) { }
                }
            }

            using (Button(@class: @class, onclick: $"document.getElementById('{customButton.Key.ToString()}').showModal()"))
            {
                Write(customButton.Name);
            }            
        }

        protected void WriteCodeExample(string header, string text, string javascript)
        {
            using (H4(@class: "font-semibold mt-8")) Write(header);
            using (P()) Write(text);

            var key = Guid.CreateVersion7().ToString();
            using (Div(id: key, @class: "border mb-4 ace-editor"))
            {
                Write(javascript);
            }

            using (Script())
            {
                Write(@"ace.edit('"+ key + "', { mode: 'ace/mode/javascript', maxLines: 25, useWorker: false }).renderer.setScrollMargin(10, 10, 10, 10);");
            }

            using (P(@class: "flex gap-3 items-center px-4"))
            {
                I(@class: "fas fa-fw fa-turn-up fa-rotate-90 text-neutral-400 text-4xl");
                using (DefaultButton(onclick: $"runCode('{key}')"))
                {
                    I(@class: "fas fa-play me-3");
                    Write("Run");
                }
            }
        }

        protected HttpHandler GetHttpHandlerFromReferrer()
        {
            if (Referrer == null) return null;
            var parts = Referrer.Split('?');
            if (parts.Length != 2) return null;
            var key = parts[0].Substring(1);
            var query = parts[1].Split('&')[0];

            if (!Assembly.ContainsHttpHandler(key)) return null;

            var httpHandler = HttpFramework.Serialization.Deserialize2(Assembly.GetHttpHandlerType(key), query) as BusinessTemplate;

            return httpHandler;
        }

        protected void ExportButton()
        {
            using (Button(id: "export-button", @class: "btn", onclick: "javascript:copyToClipboard("+Strings.Copied.EncodeJsString2()+")")) Write(Strings.Copy_to_clipboard);
        }

        protected void ShowAttachments(Guid key)
        {
            var referrer = this.ToUrl();

            using (Div(@class: "card-header print:hidden", id: "attachments"))
            {
                using (Div(@class: "flex flex-wrap gap-4 items-center"))
                {
                    I(@class: "fas fa-paperclip", style: "font-size: 16px; color: #ccc");

                    var userPermissions = this.GetCurrentUserPermissions(Business);
                    foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.Attachment>().Where(x => x.Object == key).OrderBy(x => x.Name))
                    {
                        using (Div(@class: "btn-group"))
                        {
                            using (A(@class: "btn btn-sm", href: new ViewAttachment() { Business = Business, Key = e.Key }.ToUrl()))
                            {
                                Write(e.Name);
                            }
                            if (!userPermissions.CanDelete(this.GetType().Namespace))
                            {
                                using (Button(@class: "btn btn-sm rounded-e-none", disabled: true)) Write("&times;");
                            }
                            else
                            {
                                using (Form(method: "POST", action: new Attachments.RemoveAttachment() { Business = Business, Key = e.Key, Referrer = referrer }.ToUrl(), hxBoost: true, hxDisabledElt: "button", hxSelect: "#attachments", hxTarget: "#attachments", hxSwap: "outerHTML"))
                                {
                                    using (Button(@class: "btn btn-sm rounded-s-none -ms-px hover:z-[2]")) Write("&times;");
                                }
                            }
                        }
                    }

                    if (!userPermissions.CanCreate(this.GetType().Namespace))
                    {
                        using (Button(@class: "btn btn-sm", disabled: true))
                        {
                            Write(Strings.NewAttachment);
                            Write("&nbsp;…");
                        }
                    }
                    else
                    {
                        using (Form(@class: "flex gap-4 items-center", method: "POST", action: new HttpHandlers.Businesses.Business.Attachments.NewAttachment() { Business = Business, Key = key }.ToUrl(), enctype: HttpFramework.Enctype.multipartformdata, hxBoost: true, hxDisabledElt: "button", hxSelect: "#attachments", hxTarget: "#attachments", hxSwap: "outerHTML"))
                        {
                            using (Label(@class: "btn btn-sm mb-0"))
                            {
                                Write(Strings.NewAttachment);
                                Write("&nbsp;…");
                                InputFile(name: "file", onchange: "htmx.trigger(this.form, 'submit');", @class: "hidden");
                            }

                            using (Progress(@class: "htmx-indicator", value: "0", max: 100)) Write("0%");
                        }
                    }
                }

                using (Script())
                {
                    Write(@"document.getElementById('attachments').addEventListener('dragover', e => e.preventDefault());");

                    Write(@"document.getElementById('attachments').addEventListener('drop', e => {
  e.preventDefault();
  const fileInput = document.getElementById('attachments').getElementsByTagName('input')[0];
  fileInput.files = e.dataTransfer.files;
  fileInput.dispatchEvent(new Event('change'));
});");

                    Write(@"htmx.on('body','htmx:xhr:progress', evt => {
  const bar  = evt.target.closest('form')?.querySelector('progress');
  if (bar && evt.detail.total) bar.value = Math.round(evt.detail.loaded / evt.detail.total * 100);
});");
                }
            }
        }

        protected void PrintEmailButtons(string subject, string to, string body, string variables, Guid? source)
        {
            using (Div(@class: "flex items-center gap-1"))
            {
                using (Script())
                {
                    Write("function getIframe() {");
                    Write("return document.getElementById('iframeView').contentWindow.document.getElementById('iframeView') ?? document.getElementById('iframeView');");
                    Write("}");
                    Write("function getIframeTitle() {");
                    Write("return getIframe().contentDocument?.title ?? '';");
                    Write("}");
                    // This workaround is required because of bug in Chromium: https://issues.chromium.org/issues/382394786
                    Write("function printIframe() {");
                    Write("const iframe = getIframe();");
                    Write("const originalTitle = document.title;");
                    Write("document.title = iframe.contentDocument?.title;");
                    Write("iframe.contentWindow.print();");
                    Write("document.title = originalTitle;");
                    Write("}");
                }
                using (Button(onclick: "printIframe()", @class: "btn")) Write(Strings.Print);

                using (Button(onclick: "getPdf(this, getIframeTitle() + '.pdf')", @class: "btn group"))
                {
                    Write("PDF");
                    I(@class: "ms-2 fas fa-circle-notch fa-spin !hidden group-disabled:!inline-block");
                }

                var emailSettings = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailSettings>();
                if (!emailSettings.IsInactive())
                {
                    using (Form(action: new EmailForm() { Business = Business, Source = source, Referrer = this.ToUrl() }.ToUrl(), method: "POST", hxBoost: true, hxDisabledElt: "find button", onsubmit: "this.elements.Filename.value = getIframeTitle();"))
                    {
                        InputHidden(name: "2be520d4-1fa1-4118-a5a5-627e0576a1c4");
                        InputHidden(name: nameof(SendEmail.FormData.To), value: to);
                        InputHidden(name: nameof(SendEmail.FormData.Subject), value: subject);
                        InputHidden(name: nameof(SendEmail.FormData.Body), value: body);
                        InputHidden(name: nameof(SendEmail.FormData.Filename));
                        InputHidden(name: "Variables", value: variables);
                        using (Button(@class: "btn group"))
                        {
                            Write(Strings.Email);
                            I(@class: "ms-2 fas fa-circle-notch fa-spin !hidden group-disabled:!inline-block");
                        }
                    }
                }
            }
        }

        protected void ShowPagination(int skip, int take, int total, Func<int, string> url)
        {
            if (total <= take) return;

            var totalPages = Math.DivRem(total, take, out int lastPageCount);
            if (lastPageCount > 0) totalPages++;
            if (lastPageCount == 0) lastPageCount = take;
            int currentPage = 1;
            if (skip > 0) currentPage = (skip / take) + 1;

            using (Div(@class: "card-header flex gap-4 justify-center items-center"))
            {
                using (Div(@class: "input-group"))
                {
                    if (currentPage == 1)
                    {
                        using (Button(@class: "btn px-8", disabled: true)) I(@class: "fas fa-step-backward text-base");
                        using (Button(@class: "btn px-8", disabled: true)) I(@class: "fas fa-backward text-base");
                    }
                    else
                    {
                        using (A(href: url(0), @class: "btn px-8")) I(@class: "fas fa-step-backward text-base");
                        using (A(href: url(skip-take), @class: "btn px-8")) I(@class: "fas fa-backward text-base");
                    }
                }
                using (Div()) Write($"<bdi>{currentPage} / {totalPages}</bdi>");
                using (Div(@class: "input-group"))
                {
                    if (currentPage == totalPages)
                    {
                        using (Button(@class: "btn px-8", disabled: true)) I(@class: "fas fa-forward text-base");
                        using (Button(@class: "btn px-8", disabled: true)) I(@class: "fas fa-step-forward text-base");
                    }
                    else
                    {
                        using (A(href: url(skip+take), @class: "btn px-8")) I(@class: "fas fa-forward text-base");
                        using (A(href: url(total-lastPageCount), @class: "btn px-8")) I(@class: "fas fa-step-forward text-base");
                    }
                }
            }
        }

        protected void Copy(object source, object target)
        {
            var targetMembers = target.GetType().GetFieldsAndProperties().Where(x => x.CanWrite()).ToDictionary(x => x.Name);
            var sourceMembers = source.GetType().GetFieldsAndProperties().Where(x => targetMembers.ContainsKey(x.Name)).ToArray();

            var crossTypeCopy = target.GetType() != source.GetType();

            if (targetMembers.ContainsKey(source.GetType().Name) && source is ManagerServer.Model.NamedObject namedObject)
            {
                if (!string.IsNullOrWhiteSpace(namedObject.GetCodeAndName()))
                {
                    var targetMember = targetMembers[source.GetType().Name];
                    if (targetMember.GetMemberType() == typeof(Guid?))
                    {
                        targetMember.SetMemberValue(target, namedObject.Key);
                    }
                }
            }

            foreach (var sourceMember in sourceMembers)
            {
                var name = sourceMember.Name;
                if (name == nameof(ManagerServer.Model.Object.Key)) continue; // Do not copy Keys
                if (sourceMember.GetMemberType() == typeof(DateTime)) continue; // Do not copy dates
                if (crossTypeCopy && name == nameof(IHasCustomTheme.CustomTheme)) continue; // Do not copy custom themes
                if (crossTypeCopy && name == nameof(IHasCustomTheme.CustomThemeId)) continue; // Do not copy custom themes
                if (sourceMember.GetCustomAttribute<ManagerServer.Model.Attributes.DoNotCopyAttribute>() != null) continue;
                if (name == "Reference") continue; // Do not copy references

                if (sourceMember.GetMemberType() == targetMembers[name].GetMemberType())
                {
                    var sourceValue = sourceMember.GetMemberValue(source);
                    var targetMember = targetMembers[name];

                    if (sourceValue is ManagerServer.Model.CustomFields sourceCustomFields)
                    {
                        var targetValue = targetMembers[name].GetMemberValue(target) as ManagerServer.Model.CustomFields;
                        if (targetValue == null)
                        {
                            targetMember.SetMemberValue(target, ProtoBuf.Serializer.DeepClone<CustomFields>(sourceCustomFields));
                        }
                        else
                        {
                            if (targetValue.Booleans == null) targetValue.Booleans = new Dictionary<Guid, bool>();
                            if (targetValue.Dates == null) targetValue.Dates = new Dictionary<Guid, DateTime?>();
                            if (targetValue.Decimals == null) targetValue.Decimals = new Dictionary<Guid, decimal?>();
                            if (targetValue.StringArrays == null) targetValue.StringArrays = new Dictionary<Guid, string[]>();
                            if (targetValue.Strings == null) targetValue.Strings = new Dictionary<Guid, string>();

                            if (sourceCustomFields.Booleans != null) sourceCustomFields.Booleans.Where(x => x.Value).ToList().ForEach(x => targetValue.Booleans[x.Key] = x.Value);
                            if (sourceCustomFields.Dates != null) sourceCustomFields.Dates.Where(x => x.Value.HasValue).ToList().ForEach(x => targetValue.Dates[x.Key] = x.Value);
                            if (sourceCustomFields.Decimals != null) sourceCustomFields.Decimals.Where(x => x.Value.HasValue).ToList().ForEach(x => targetValue.Decimals[x.Key] = x.Value);
                            if (sourceCustomFields.StringArrays != null) sourceCustomFields.StringArrays.Where(x => x.Value != null && x.Value.Length > 0).ToList().ForEach(x => targetValue.StringArrays[x.Key] = x.Value);
                            if (sourceCustomFields.Strings != null) sourceCustomFields.Strings.Where(x => !string.IsNullOrWhiteSpace(x.Value)).ToList().ForEach(x => targetValue.Strings[x.Key] = x.Value);
                        }
                    }
                    else
                    {
                        targetMember.SetMemberValue(target, sourceValue);

                        if (sourceValue is Guid)
                        {
                            var onChangeAttributes = targetMember.GetCustomAttributes<ManagerServer.Model.Attributes.OnChangeSetDefaultAttribute>();
                            if (onChangeAttributes != null)
                            {
                                object sourceValueObject = null;
                                foreach (var e in onChangeAttributes)
                                {
                                    if (sourceValueObject == null) sourceValueObject = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Object>((Guid)sourceValue);
                                    if (sourceValueObject != null)
                                    {
                                        var hasDefaultMember = sourceValueObject.GetType().GetFieldOrProperty("HasDefault" + e.Field);
                                        var defaultMember = sourceValueObject.GetType().GetFieldOrProperty("Default" + e.Field);

                                        var hasDefaultValue = hasDefaultMember?.GetMemberValue(sourceValueObject) as bool?;
                                        if (hasDefaultValue == true)
                                        {
                                            var defaultValue = defaultMember?.GetMemberValue(sourceValueObject);
                                            if (defaultValue != null)
                                            {
                                                if (targetMembers[e.Field].GetMemberType() == defaultMember?.GetMemberType())
                                                {
                                                    targetMembers[e.Field].SetMemberValue(target, defaultValue);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (sourceMember.GetMemberType().IsArray && targetMembers[name].GetMemberType().IsArray)
                {
                    var sourceArray = sourceMember.GetMemberValue(source) as Array;
                    var targetArray = Array.CreateInstance(targetMembers[name].GetMemberType().GetElementType(), sourceArray.Length);
                    for (int i = 0; i < sourceArray.Length; i++)
                    {
                        var sourceElement = sourceArray.GetValue(i);
                        var targetElement = Activator.CreateInstance(targetMembers[name].GetMemberType().GetElementType());
                        Copy(sourceElement, targetElement);
                        targetArray.SetValue(targetElement, i);
                    }
                    targetMembers[name].SetMemberValue(target, targetArray);
                }
            }
        }

        private string[] GetContextParts(BusinessTemplate businessTemplate)
        {
            var context = new List<string>();

            if (businessTemplate is VueForm) return context.ToArray();

            foreach (var e in businessTemplate.GetType().GetFieldsAndProperties())
            {
                if (e.Name == nameof(Business)) continue;
                if (e.Name == nameof(Referrer)) continue;

                /*
                if (e.FieldType == typeof(string))
                {
                    var value = e.GetValue(httpHandler) as string;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        context.Add("<q>"+value+"</q>");
                    }
                }
                */

                /*
                if (e.FieldType == typeof(bool) || e.FieldType == typeof(bool?))
                {
                    var value = e.GetValue(httpHandler) as bool?;
                    if (value == true)
                    {
                        context.Add(Manager.Globalization.Strings.GetPropertyValue(e.Name));
                    }
                }
                */

                if (e.GetMemberType() == typeof(Guid) || e.GetMemberType() == typeof(Guid?))
                {
                    var value = e.GetMemberValue(businessTemplate) as Guid?;
                    if (value.HasValue)
                    {
                        var contextObject = ApplicationData.Businesses.Get(Business).Single(value.Value) as NamedObject ?? ApplicationData.Businesses.Get(Business).SingleOrDefault(value.Value) as NamedObject;
                        if (contextObject != null)
                        {
                            var name = contextObject.GetName();
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                context.Add(name);
                            }
                        }
                    }
                }
            }

            return context.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }

        int IComparable.CompareTo(object obj)
        {
            return 0;
        }
    }
}