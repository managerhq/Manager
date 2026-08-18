using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.CustomerPortal
{
    abstract class Template : HttpFramework.HttpHandler
    {
        [InheritedProtoMember(100)] public string Business;
        [InheritedProtoMember(101)] public Guid CustomerPortal;

        public override Task ProcessRequest()
        {
            switch (Request.Method)
            {
                case "GET": return Get();
                default: return Task.CompletedTask;
            }
        }

        protected virtual void InnerGet()
        {
        }

        private void UpdateDateNumberFormat()
        {
            var culture = new System.Globalization.CultureInfo("en");
            if (ApplicationData.Businesses.Exists(Business))
            {
                var database = ApplicationData.Businesses.Get(Business);
                if (database != null)
                {
                    var regionFormats = database.Single<ManagerServer.Model.DateAndNumberFormat>();

                    var shortDatePattern = "yyyy-MM-dd";
                    if (!string.IsNullOrWhiteSpace(regionFormats.DateFormat)) shortDatePattern = regionFormats.DateFormat;

                    var shortTimePattern = "HH:mm:ss";
                    if (!string.IsNullOrWhiteSpace(regionFormats.TimeFormat)) shortTimePattern = regionFormats.TimeFormat;

                    culture.DateTimeFormat.LongDatePattern = shortDatePattern;
                    culture.DateTimeFormat.ShortDatePattern = shortDatePattern;
                    culture.DateTimeFormat.ShortTimePattern = shortTimePattern;
                    culture.DateTimeFormat.LongTimePattern = shortTimePattern;

                    var firstDayOfTheWeek = 0;
                    if (database.Exists<ManagerServer.Model.DateAndNumberFormat>()) firstDayOfTheWeek = (int)regionFormats.FirstDayOfWeek;
                    culture.DateTimeFormat.FirstDayOfWeek = (DayOfWeek)firstDayOfTheWeek;

                    if (!string.IsNullOrWhiteSpace(regionFormats.NumberFormat))
                    {
                        using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(regionFormats.NumberFormat)))
                        {
                            var numberFormat = ProtoBuf.Serializer.Deserialize<ManagerServer.Model.DateAndNumberFormat.NumberFormatParts>(ms);
                            culture.NumberFormat.NumberDecimalSeparator = numberFormat.DecimalSeparator;
                            culture.NumberFormat.NumberGroupSeparator = numberFormat.GroupSeparator;
                            culture.NumberFormat.NumberGroupSizes = numberFormat.GroupSizes;
                        }
                    }
                }
            }
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        }

        private void UpdateLanguage()
        {
            var currentLanguage = string.Empty;
            if (Request.Cookies["language"] != null)
            {
                currentLanguage = Request.Cookies["language"].ToString();
            }

            ManagerServer.Globalization.Languages.SetLanguage(currentLanguage);
        }

        private Task Get()
        {
            UpdateDateNumberFormat();
            UpdateLanguage();

            if (!ApplicationData.Businesses.Exists(Business))
            {
                Write("Business not found");
                return Task.CompletedTask;
            }

            var customerPortal = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.CustomerPortal>(CustomerPortal);
            if (customerPortal == null && this is GenericView view)
            {
                using (Html(dir: ManagerServer.Globalization.Languages.IsRightToLeft() ? "rtl" : null))
                {
                    using (Head())
                    {
                        Write(@"<meta charset=""utf-8"" />");
                        Title("Customer Portal");
                        Link(rel: "shortcut icon", type: "image/x-icon", href: "favicon.ico");
                        if (ManagerServer.Globalization.Languages.IsRightToLeft())
                        {
                            Link(rel: "stylesheet", type: "text/css", href: "/resources/bootstrap5/css/bootstrap-rtl.css?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                        }
                        else
                        {
                            Link(rel: "stylesheet", type: "text/css", href: "/resources/bootstrap5/css/bootstrap.css?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                        }
                        Link(rel: "stylesheet", type: "text/css", href: "/resources/fontawesome/fontawesome.css?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                        Link(rel: "stylesheet", type: "text/css", href: "/resources/fontawesome/solid.css?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                        Script("/resources/htmx/htmx.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    }

                    using (Body(@class: "print-reset", style: "padding: 20px"))
                    {
                        view.InnerGet();
                    }
                }
                return Task.CompletedTask;
            }
            if (customerPortal == null)
            {
                Write("Customer portal not found");
                return Task.CompletedTask;
            }

            var customer = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Customer>(customerPortal.Customer);

            if (customer == null)
            {
                Write("Customer not set");
                return Task.CompletedTask;
            }

            using (Html(dir: ManagerServer.Globalization.Languages.IsRightToLeft() ? "rtl" : null))
            {
                using (Head())
                {
                    Write(@"<meta charset=""utf-8"" />");
                    Title("Customer Portal");
                    Link(rel: "shortcut icon", type: "image/x-icon", href: "favicon.ico");
                    if (ManagerServer.Globalization.Languages.IsRightToLeft())
                    {
                        Link(rel: "stylesheet", type: "text/css", href: "/resources/bootstrap5/css/bootstrap-rtl.css?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    }
                    else
                    {
                        Link(rel: "stylesheet", type: "text/css", href: "/resources/bootstrap5/css/bootstrap.css?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    }
                    Link(rel: "stylesheet", type: "text/css", href: "/resources/fontawesome/fontawesome.css?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    Link(rel: "stylesheet", type: "text/css", href: "/resources/fontawesome/solid.css?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                    using (Style())
                    {
                        Write(@"@media print {");
                        Write(".print-reset { padding: 0 !important; margin: 0 !important; border: none !important; background: none !important; box-shadow: none !important; float: none !important }");
                        Write("}");
                    }
                    Script("/resources/htmx/htmx.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                }

                using (Body(@class: "print-reset", style: "background-color: #eee"))
                {
                    using (Div(@class: "print-reset container-fluid p-4"))
                    {                       
                        using (Div(@class: "row d-print-none"))
                        {
                            using (Div(@class: "col-12"))
                            {
                                using (Div(@class: "navbar navbar-light border", style: "background-color: #f3f3f3; box-shadow: inset 1px 1px 0px #fff; border-color: #ccc !important"))
                                {
                                    using (Div(@class: "container-fluid"))
                                    {
                                        using (Span(@class: "navbar-brand py-2 fw-bold", style: "color: #555; text-shadow: 1px 1px 0 #fff"))
                                        {
                                            Write(customer.Name);
                                        }
                                    }
                                }
                            }
                        }
                        using (Div(@class: "row g-0"))
                        {
                            using (Div(@class: "col-auto d-print-none"))
                            {
                                using (Div(@class: "list-group list-group-flush border-start border-bottom fw-bold", style: "border-color: #ccc !important"))
                                {
                                    using (A(href: new Summary.CustomerPortal() { Business = Business, CustomerPortal = CustomerPortal }.ToUrl(), @class: "text-nowrap nav-link list-group-item d-flex justify-content-between align-items-center" + GetClassNamesIf(nameof(Summary)), style: "border-color: #ccc !important" + GetStyleIf(nameof(Summary))))
                                    {
                                        using (Span())
                                        {
                                            I(@class: "fas fa-fw fa-presentation me-2", style: "color: #ccc");
                                            using (Span(style: "color: #428bca; font-size: 0.75rem")) Write(Strings.Summary);
                                        }
                                    }
                                    if (customerPortal.SalesQuotes)
                                    {
                                        var salesQuotes = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.SalesQuote>().Where(x => x.Customer == customerPortal.Customer.Value);
                                        using (A(href: new Quotes.CustomerPortalQuotes() { Business = Business, CustomerPortal = CustomerPortal }.ToUrl(), @class: "text-nowrap list-group-item d-flex justify-content-between align-items-center" + GetClassNamesIf(nameof(Quotes)), style: "border-color: #ccc !important" + GetStyleIf(nameof(Quotes))))
                                        {
                                            using (Span())
                                            {
                                                I(@class: "fas fa-fw fa-file-check me-2", style: "color: #ccc");
                                                using (Span(style: "color: #428bca; font-size: 0.75rem")) Write(Strings.Quotes);
                                            }
                                            var count = salesQuotes.Count();
                                            using (Span(@class: "ms-2 badge bg-white border", style: (count > 0 ? "color: #666" : "color: #ccc") + "; border-color: #ccc; font-size: .625rem")) Write(count.ToString());
                                        }
                                    }
                                    if (customerPortal.SalesOrders)
                                    {
                                        var salesOrders = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.SalesOrder>().Where(x => x.Customer == customerPortal.Customer.Value);
                                        using (A(href: new Orders.CustomerPortalOrders() { Business = Business, CustomerPortal = CustomerPortal }.ToUrl(), @class: "text-nowrap list-group-item d-flex justify-content-between align-items-center" + GetClassNamesIf(nameof(Orders)), style: "border-color: #ccc !important" + GetStyleIf(nameof(Orders))))
                                        {
                                            using (Span())
                                            {
                                                I(@class: "fas fa-fw fa-shopping-cart me-2", style: "color: #ccc");
                                                using (Span(style: "color: #428bca; font-size: 0.75rem")) Write(Strings.Orders);
                                            }
                                            var count = salesOrders.Count();
                                            using (Span(@class: "ms-2 badge bg-white border", style: (count > 0 ? "color: #666" : "color: #ccc") + "; border-color: #ccc; font-size: .625rem")) Write(count.ToString());
                                        }
                                    }
                                    if (customerPortal.SalesInvoices)
                                    {
                                        var salesInvoices = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.Customer == customerPortal.Customer.Value);
                                        using (A(href: new Invoices.CustomerPortalInvoices() { Business = Business, CustomerPortal = CustomerPortal }.ToUrl(), @class: "text-nowrap list-group-item d-flex justify-content-between align-items-center" + GetClassNamesIf(nameof(Invoices)), style: "border-color: #ccc !important" + GetStyleIf(nameof(Invoices))))
                                        {
                                            using (Span())
                                            {
                                                I(@class: "fas fa-fw fa-file-alt me-2", style: "color: #ccc");
                                                using (Span(style: "color: #428bca; font-size: 0.75rem")) Write(Strings.Invoices);
                                            }
                                            var count = salesInvoices.Count();
                                            using (Span(@class: "ms-2 badge bg-white border", style: (count > 0 ? "color: #666" : "color: #ccc") + "; border-color: #ccc; font-size: .625rem")) Write(count.ToString());
                                        }
                                    }
                                    if (customerPortal.CreditNotes)
                                    {
                                        var creditNotes = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.CreditNote>().Where(x => x.Customer == customerPortal.Customer.Value);
                                        using (A(href: new CreditNotes.CustomerPortalCreditNotes() { Business = Business, CustomerPortal = CustomerPortal }.ToUrl(), @class: "text-nowrap list-group-item d-flex justify-content-between align-items-center" + GetClassNamesIf(nameof(CreditNotes)), style: "border-color: #ccc !important" + GetStyleIf(nameof(CreditNotes))))
                                        {
                                            using (Span())
                                            {
                                                I(@class: "fas fa-fw fa-cut me-2", style: "color: #ccc");
                                                using (Span(style: "color: #428bca; font-size: 0.75rem")) Write(Strings.CreditNotes);
                                            }
                                            var count = creditNotes.Count();
                                            using (Span(@class: "ms-2 badge bg-white border", style: (count > 0 ? "color: #666" : "color: #ccc") + "; border-color: #ccc; font-size: .625rem")) Write(count.ToString());
                                        }
                                    }
                                    if (customerPortal.DeliveryNotes)
                                    {
                                        var deliveryNotes = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.DeliveryNote>().Where(x => x.Customer == customerPortal.Customer.Value);
                                        using (A(href: new DeliveryNotes.CustomerPortalDeliveryNotes() { Business = Business, CustomerPortal = CustomerPortal }.ToUrl(), @class: "text-nowrap list-group-item d-flex justify-content-between align-items-center" + GetClassNamesIf(nameof(DeliveryNotes)), style: "border-color: #ccc !important" + GetStyleIf(nameof(DeliveryNotes))))
                                        {
                                            using (Span())
                                            {
                                                I(@class: "fas fa-fw fa-truck me-2", style: "color: #ccc");
                                                using (Span(style: "color: #428bca; font-size: 0.75rem")) Write(Strings.DeliveryNotes);
                                            }
                                            var count = deliveryNotes.Count();
                                            using (Span(@class: "ms-2 badge bg-white border", style: (count > 0 ? "color: #666": "color: #ccc")+"; border-color: #ccc; font-size: .625rem")) Write(count.ToString());
                                        }
                                    }
                                }
                            }

                            var boxShadow = "box-shadow: -1px 0px 0px #ccc";
                            if (ManagerServer.Globalization.Languages.IsRightToLeft()) boxShadow = "box-shadow: 1px 0px 0px #ccc";
                            using (Div(@class: "col border-end border-bottom p-4 bg-white print-reset", style: boxShadow+"; border-color: #ccc !important"))
                            {
                                InnerGet();
                            }
                        }
                        using (Div(@class: "row d-print-none"))
                        {
                            using (Div(@class: "col"))
                            {
                                var acceptLanguageHeader = Request.Headers["Accept-Language"].ToString();
                                var defaultLanguages = new HashSet<string>();
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
                                    using (Script())
                                    {
                                        Write(@"function showLanguages() { var elements = document.getElementsByClassName('hideOnDefault'); for (var i = 0; i < elements.length; i++) { elements[i].style.display = 'inline-block'; } document.getElementById('language-expand-link').style.display = 'none'; }");
                                    }
                                }

                                if (languages.Length > 1)
                                {
                                    using (Div(@class: "print:hidden", style: "line-height: 200%; width: 600px; margin: 20px auto; margin-bottom: 10px; text-align: center; color: #ccc; text-shadow: 1px 1px 0px #fff"))
                                    {
                                        InputHidden(name: "Location", value: this.ToUrl());
                                        foreach (var e in ManagerServer.Globalization.Languages.GetLanguages().OrderByDescending(x => x.Code == ManagerServer.Globalization.Strings.CurrentLanguage.Value).ThenByDescending(x => x.Code == "en").ThenBy(x => x.NativeName))
                                        {
                                            if (e.Code == ManagerServer.Globalization.Strings.CurrentLanguage.Value)
                                            {
                                                using (Span(@class: "btn fw-bold", title: e.EnglishName, style: "font-size: 0.875rem; color: #999; display: inline-block; background-color: transparent; text-decoration: none")) Write(e.NativeName);
                                            }
                                            else
                                            {
                                                var visible = defaultLanguages.Contains(e.Code.Split('-').First());
                                                using (Button(hxPost: new SwitchLanguage() { Language = e.Code }.ToUrl(), title: e.EnglishName, @class: "btn btn-link" + (!visible ? " hideOnDefault" : null), style: "font-size: 0.875rem; text-decoration: none; color: #428bca; display: " + (visible ? "inline-block" : "none")))
                                                {
                                                    Write(e.NativeName);
                                                }
                                            }
                                        }
                                        using (Style())
                                        {
                                            Write("a#language-expand-link { color: #ccc; padding: 3px 6px; margin-left: 5px; border: 1px solid #ccc; border-radius: 3px }");
                                            Write("a#language-expand-link:hover { text-decoration: none; background-color: #fff; color: #333; border: 1px solid #999; text-shadow: none }");
                                        }
                                        using (A(href: "javascript:showLanguages()", id: "language-expand-link", style: "text-decoration: none")) Write("+");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        private string GetClassNamesIf(string name)
        {
            if (!this.GetType().FullName.StartsWith(typeof(Template).Namespace + "." + name))
            {
                return " border-end";
            }
            return null;
        }

        private string GetStyleIf(string name)
        {
            if (!this.GetType().FullName.StartsWith(typeof(Template).Namespace + "." + name))
            {
                return "; background-color: #fafafa; box-shadow: inset 1px 1px 0px #fff";
            }
            return null;
        }
    }
}
