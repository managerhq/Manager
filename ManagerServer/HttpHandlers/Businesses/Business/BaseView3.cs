using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    internal abstract class BaseView3 : BusinessTemplate
    {
        [InheritedProtoMember(200)] public Guid Key;
        [InheritedProtoMember(201)] public int? Position;
        [InheritedProtoMember(202)] public int? MaxPosition;
        [InheritedProtoMember(203)] public bool ForceDefaultTheme;

        internal abstract string GetIframeUrl();

        protected virtual Type[] GetCopyToOptions() => null;
        protected virtual bool CanHaveAttachments() => false;
        protected virtual Guid? GetHistoryKey() => Key;
        protected virtual IEmailTemplate GetEmailTemplate() { return null; }
        protected virtual string GetRecipient() { return null; }

        protected virtual Guid? GetCustomTheme()
        {
            if (HttpContext?.RequestServices == null) return null;
            var business = ApplicationData.Businesses.Get(Business);
            if (business != null)
            {
                var o = business.Single(Key) ?? business.SingleOrDefault(Key);
                if (o is IHasCustomTheme hasCustomTheme)
                {
                    return hasCustomTheme.GetCustomTheme();
                }
            }
            return null;
        }

        protected virtual void EditCloneButtons()
        {
            var referrer = this.ToUrl();
            var formType = typeof(Program).Assembly.GetTypes().Where(x => x.Namespace == this.GetType().Namespace).SingleOrDefault(x => x.IsSubclassOf(typeof(VueForm)));
            if (formType != null)
            {
                var form = (VueForm)Activator.CreateInstance(formType);
                form.Business = Business;
                form.Key = Key;
                form.DeleteReferrer = Referrer;
                form.Referrer = referrer;

                using (Div(@class: "flex items-center gap-1"))
                {
                    using (A(href: form.ToUrl(), @class: "btn")) Write(Strings.Edit);

                    if (ManagerServer.Model.Object.GetTypeByGuid(Key) == null)
                    {
                        form.Clone = form.Key;
                        form.Key = null;
                        using (A(href: form.ToUrl(), @class: "btn")) Write(Strings.Clone);
                    }
                }
                using (Div(@class: "vertical-divider")) { }
            }
        }

        protected override sealed void InnerGet2()
        {
            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header print:hidden"))
                {
                    using (Div(@class: "flex justify-between"))
                    {
                        using (Div(@class: "flex items-center flex-wrap gap-4"))
                        {
                            using (Div(@class: "card-title"))
                            {
                                var titleAttribute = this.GetType().GetCustomAttribute<TitleAttribute>();
                                var title = titleAttribute?.Text?.FirstOrDefault() ?? this.GetType().Name;
                                Write(Strings.GetPropertyValue(title));
                            }

                            var referrer = this.ToUrl();

                            EditCloneButtons();

                            var copyToOptions = (GetCopyToOptions() ?? new Type[0]).ToList();
                            var tabs = this.GetTabs(true);
                            if (!tabs.Receipts.Visible) copyToOptions.Remove(typeof(ManagerServer.Model.Receipt));
                            if (!tabs.Payments.Visible) copyToOptions.Remove(typeof(ManagerServer.Model.Payment));
                            if (!tabs.SalesInvoices.Visible)
                            {
                                copyToOptions.Remove(typeof(ManagerServer.Model.RecurringSalesInvoice));
                            }
                            if (!tabs.PurchaseInvoices.Visible)
                            {
                                copyToOptions.Remove(typeof(ManagerServer.Model.RecurringPurchaseInvoice));
                            }

                            if (copyToOptions != null && copyToOptions.Count > 0)
                            {
                                var copyToReferrer = this.ToUrl();
                                using (Details(@class: "dropdown"))
                                {
                                    using (Summary(@class: "btn")) Write(Strings.CopyTo);
                                    using (Div(@class: "dropdown-menu"))
                                    {
                                        foreach (var e in copyToOptions)
                                        {
                                            if (e == typeof(ManagerServer.Model.JournalEntry)) using (A(@class: "dropdown-item", href: new JournalEntries.JournalEntryForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewJournalEntry);
                                            if (e == typeof(ManagerServer.Model.Receipt)) using (A(@class: "dropdown-item", href: new Receipts.ReceiptForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewReceipt);
                                            if (e == typeof(ManagerServer.Model.Payment)) using (A(@class: "dropdown-item", href: new Payments.PaymentForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewPayment);
                                            if (e == typeof(ManagerServer.Model.Transaction))
                                            {
                                                if (tabs.SalesQuotes.Visible) using (A(@class: "dropdown-item", href: new SalesQuotes.SalesQuoteForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewSalesQuote);
                                                if (tabs.SalesOrders.Visible) using (A(@class: "dropdown-item", href: new SalesOrders.SalesOrderForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewSalesOrder);
                                                if (tabs.SalesInvoices.Visible) using (A(@class: "dropdown-item", href: new SalesInvoices.SalesInvoiceForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewSalesInvoice);
                                                if (tabs.DeliveryNotes.Visible) using (A(@class: "dropdown-item", href: new DeliveryNotes.DeliveryNoteForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewDeliveryNote);
                                                if (tabs.CreditNotes.Visible) using (A(@class: "dropdown-item", href: new CreditNotes.CreditNoteForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewCreditNote);
                                                if (tabs.SalesQuotes.Visible || tabs.SalesOrders.Visible || tabs.SalesInvoices.Visible || tabs.DeliveryNotes.Visible || tabs.CreditNotes.Visible)
                                                {
                                                    Hr(@class: "dropdown-menu-divider");
                                                }
                                                if (tabs.PurchaseQuotes.Visible) using (A(@class: "dropdown-item", href: new PurchaseQuotes.PurchaseQuoteForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewPurchaseQuote);
                                                if (tabs.PurchaseOrders.Visible) using (A(@class: "dropdown-item", href: new PurchaseOrders.PurchaseOrderForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewPurchaseOrder);
                                                if (tabs.PurchaseInvoices.Visible) using (A(@class: "dropdown-item", href: new PurchaseInvoices.PurchaseInvoiceForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewPurchaseInvoice);
                                                if (tabs.GoodsReceipts.Visible) using (A(@class: "dropdown-item", href: new GoodsReceipts.GoodsReceiptForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewGoodsReceipt);
                                                if (tabs.DebitNotes.Visible) using (A(@class: "dropdown-item", href: new DebitNotes.DebitNoteForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewDebitNote);
                                            }
                                            if (e == typeof(ManagerServer.Model.RecurringPayment))
                                            {
                                                Hr(@class: "dropdown-menu-divider");
                                                using (A(@class: "dropdown-item", href: new Settings.RecurringTransactions.RecurringPayments.RecurringPaymentForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewRecurringPayment);
                                            }
                                            if (e == typeof(ManagerServer.Model.RecurringReceipt))
                                            {
                                                Hr(@class: "dropdown-menu-divider");
                                                using (A(@class: "dropdown-item", href: new Settings.RecurringTransactions.RecurringReceipts.RecurringReceiptForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewRecurringReceipt);
                                            }
                                            if (e == typeof(ManagerServer.Model.InterAccountTransfer))
                                            {
                                                using (A(@class: "dropdown-item", href: new InterAccountTransfers.InterAccountTransferForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewInterAccountTransfer);
                                            }
                                            if (e == typeof(ManagerServer.Model.RecurringInterAccountTransfer))
                                            {
                                                Hr(@class: "dropdown-menu-divider");
                                                using (A(@class: "dropdown-item", href: new Settings.RecurringTransactions.RecurringInterAccountTransfers.RecurringInterAccountTransferForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewRecurringInterAccountTransfer);
                                            }
                                            if (e == typeof(ManagerServer.Model.RecurringSalesQuote))
                                            {
                                                Hr(@class: "dropdown-menu-divider");
                                                using (A(@class: "dropdown-item", href: new Settings.RecurringTransactions.RecurringSalesQuotes.RecurringSalesQuoteForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewRecurringSalesQuote);
                                            }
                                            if (e == typeof(ManagerServer.Model.RecurringSalesOrder))
                                            {
                                                Hr(@class: "dropdown-menu-divider");
                                                using (A(@class: "dropdown-item", href: new Settings.RecurringTransactions.RecurringSalesOrders.RecurringSalesOrderForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewRecurringSalesOrder);
                                            }
                                            if (e == typeof(ManagerServer.Model.RecurringSalesInvoice))
                                            {
                                                Hr(@class: "dropdown-menu-divider");
                                                using (A(@class: "dropdown-item", href: new Settings.RecurringTransactions.RecurringSalesInvoices.RecurringSalesInvoiceForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewRecurringSalesInvoice);
                                            }
                                            if (e == typeof(ManagerServer.Model.RecurringPurchaseOrder))
                                            {
                                                Hr(@class: "dropdown-menu-divider");
                                                using (A(@class: "dropdown-item", href: new Settings.RecurringTransactions.RecurringPurchaseOrders.RecurringPurchaseOrderForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewRecurringPurchaseOrder);
                                            }
                                            if (e == typeof(ManagerServer.Model.RecurringPurchaseInvoice))
                                            {
                                                Hr(@class: "dropdown-menu-divider");
                                                using (A(@class: "dropdown-item", href: new Settings.RecurringTransactions.RecurringPurchaseInvoices.RecurringPurchaseInvoiceForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewRecurringPurchaseInvoice);
                                            }
                                            if (e == typeof(ManagerServer.Model.Payslip))
                                            {
                                                using (A(@class: "dropdown-item", href: new Payslips.PayslipForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewPayslip);
                                            }
                                            if (e == typeof(ManagerServer.Model.RecurringPayslip))
                                            {
                                                Hr(@class: "dropdown-menu-divider");
                                                using (A(@class: "dropdown-item", href: new Settings.RecurringTransactions.RecurringPayslips.RecurringPayslipForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewRecurringPayslip);
                                            }
                                            if (e == typeof(ManagerServer.Model.RecurringJournalEntry))
                                            {
                                                Hr(@class: "dropdown-menu-divider");
                                                using (A(@class: "dropdown-item", href: new Settings.RecurringTransactions.RecurringJournalEntries.RecurringJournalEntryForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewRecurringJournalEntry);
                                            }
                                            if (e == typeof(ManagerServer.Model.CustomTheme))
                                            {
                                                using (A(@class: "dropdown-item", href: new Settings.CustomThemes.CustomThemeForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewTheme);
                                            }
                                            if (e == typeof(ManagerServer.Model.BillableTime))
                                            {
                                                using (A(@class: "dropdown-item", href: new BillableTime.BillableTimeEntryForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewBillableTime);
                                            }
                                            if (e == typeof(ManagerServer.Model.Customer))
                                            {
                                                using (A(@class: "dropdown-item", href: new Customers.CustomerForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewCustomer);
                                            }
                                            if (e == typeof(ManagerServer.Model.Supplier))
                                            {
                                                using (A(@class: "dropdown-item", href: new Suppliers.SupplierForm() { Business = Business, Source = Key, Referrer = referrer }.ToUrl())) Write(Strings.NewSupplier);
                                            }
                                        }
                                    }
                                }
                                using (Div(@class: "vertical-divider")) { }
                            }

                            var emailTitle = GetTitle();

                            var emailTemplate = GetEmailTemplate();
                            var subject = emailTitle ?? string.Empty;
                            var body = string.Empty;
                            if (emailTemplate != null)
                            {
                                subject = emailTemplate.GetSubject() ?? subject;
                                body = emailTemplate.GetBody() ?? string.Empty;

                                var business = ApplicationData.Businesses.Get(Business);
                                var o = business.Single(Key) ?? business.SingleOrDefault(Key);
                                if (o is IHasAutomaticReference hasAutomaticReference)
                                {
                                    if (subject != null) subject = subject.Replace("{{reference}}", hasAutomaticReference.Reference);
                                    if (body != null) body = body.Replace("{{reference}}", hasAutomaticReference.Reference);
                                }
                            }

                            PrintEmailButtons(subject, GetRecipient(), body, null, Key);
                        }
                        using (Div(@class: "flex gap-2 items-center"))
                        {
                            if (Position.HasValue && MaxPosition.HasValue && Referrer != null)
                            {
                                var innerReferrer = Referrer;
                                if (!string.IsNullOrWhiteSpace(innerReferrer))
                                {
                                    var parts = innerReferrer.Split('?');
                                    var key = parts[0].Substring(1);
                                    var query = parts[1].Split('&')[0];

                                    if (Assembly.ContainsHttpHandler(key))
                                    {
                                        var httpHandlerType = Assembly.GetHttpHandlerType(key);
                                        if (httpHandlerType != null)
                                        {
                                            if (httpHandlerType.GetFieldOrProperty("Redirect") != null)
                                            {
                                                var httpHandler2 = HttpFramework.Serialization.Deserialize2(httpHandlerType, query) as BusinessTemplate;
                                                if (httpHandler2 != null)
                                                {
                                                    var oa = FastMember.ObjectAccessor.Create(httpHandler2);

                                                    using (Div(@class: "flex gap-2 items-center"))
                                                    {
                                                        using (Div(@class: "btn-group"))
                                                        {
                                                            oa["Redirect"] = 0;
                                                            if (Position.Value > 0) using (A(href: httpHandler2.ToUrl(), @class: "btn px-6")) I(@class: "fas fa-step-backward text-xs");
                                                            else using (Button(@class: "btn px-6", disabled: true)) I(@class: "fas fa-step-backward text-xs");

                                                            oa["Redirect"] = Position.Value - 1;
                                                            if (Position.Value > 0) using (A(href: httpHandler2.ToUrl(), @class: "btn px-6")) I(@class: "fas fa-backward text-xs");
                                                            else using (Button(@class: "btn px-6", disabled: true)) I(@class: "fas fa-backward text-xs");
                                                        }
                                                        using (Span(@class: "font-semibold whitespace-nowrap opacity-25"))
                                                        {
                                                            Write((Position.Value + 1).ToString("N0", System.Threading.Thread.CurrentThread.CurrentCulture) + " / " + (MaxPosition.Value + 1).ToString("N0", System.Threading.Thread.CurrentThread.CurrentCulture));
                                                        }

                                                        using (Div(@class: "btn-group"))
                                                        {
                                                            oa["Redirect"] = Position.Value + 1;
                                                            if (Position < MaxPosition.Value) using (A(href: httpHandler2.ToUrl(), @class: "btn px-6")) I(@class: "fas fa-forward text-xs");
                                                            else using (Button(@class: "btn px-6", disabled: true)) I(@class: "fas fa-forward text-xs");

                                                            oa["Redirect"] = MaxPosition;
                                                            if (Position < MaxPosition.Value) using (A(href: httpHandler2.ToUrl(), @class: "btn px-6")) I(@class: "fas fa-step-forward text-xs");
                                                            else using (Button(@class: "btn px-6", disabled: true)) I(@class: "fas fa-step-forward text-xs");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var customTheme = GetCustomTheme();
                if (customTheme.HasValue)
                {
                    using (Div(@class: "card-header flex items-center gap-4 print:hidden"))
                    {
                        if (!ForceDefaultTheme)
                        {
                            this.ForceDefaultTheme = true;
                            using (A(href: this.ToUrl(), @class: "py-1"))
                            {
                                I(@class: "fas fa-toggle-on fa-lg");
                            }
                            this.ForceDefaultTheme = false;
                        }
                        else
                        {
                            this.ForceDefaultTheme = false;
                            using (A(href: this.ToUrl(), @class: "py-1"))
                            {
                                I(@class: "fas fa-toggle-off fa-lg");
                            }
                            this.ForceDefaultTheme = true;
                        }

                        var customThemeName = ApplicationData.Businesses.Get(Business).SingleOrDefault<CustomTheme>(customTheme.Value)?.Name;
                        using (Label(@class: "mb-0")) Write(Strings.CustomTheme);
                        using (Span()) Write(customThemeName);

                        using (A(href: new Settings.CustomThemes.CustomThemeForm() { Business = Business, Key = customTheme.Value, Referrer = this.ToUrl() }.ToUrl()))
                        {
                            I(@class: "fa fa-pencil");
                        }
                    }
                }

                using (Div(@class: "card-inset p-0"))
                {
                    using (Div(@class: "hidden print:block text-base text-black border border-black p-2 leading-6 text-center font-semibold"))
                    {
                        Write("This page can be printed using the Print button within the application. Due to technical limitations, printing directly from the browser may not include all required content. Please close this dialog and use the in-app Print button to continue.");
                    }

                    var src = GetIframeUrl();
                    using (IFrame(id: "iframeView", @class: "border-0 rounded w-full print:hidden", onload: "autoResizeIframe(this)", src: src))
                    {
                    }
                }

                if (CanHaveAttachments())
                {
                    ShowAttachments(Key);
                }

                using (Div(@class: "card-header flex justify-between gap-2 print:hidden"))
                {
                    using (Div(@class: "flex items-center gap-2"))
                    {
                        var footerAction = GetFooterAction();
                        if (footerAction != null)
                        {
                            footerAction.Item2.Referrer = this.ToUrl();
                            I(@class: "fas fa-turn-up fa-rotate-90 text-xl opacity-50");
                            using (A(href: footerAction.Item2.ToUrl(), @class: "btn")) Write(footerAction.Item1);
                        }
                    }
                    using (Div(@class: "flex gap-2 items-center"))
                    {
                        var historyKey = GetHistoryKey();
                        if (historyKey != null)
                        {
                            using (A(href: new History() { Business = Business, Object = historyKey.Value, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.History);
                        }
                        foreach (var e in GetFooterButtons())
                        {
                            using (A(href: e.Item2.ToUrl(), @class: "btn btn-xs"))
                            {
                                Write(e.Item1);
                            }
                        }
                        using (Button(@class: "btn btn-xs", onclick: "this.disabled = true; navigator.clipboard.writeText(elementToTSV(document.querySelector('#iframeView'))); setTimeout(()=>this.disabled = false, 1000);"))
                        {
                            Write(Strings.Copy_to_clipboard);
                        }
                    }
                }
            }
        }

        protected virtual Tuple<string, BusinessTemplate> GetFooterAction()
        {
            return null;
        }

        protected virtual IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            return [];
        }
    }
}
