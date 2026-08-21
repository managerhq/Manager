using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringSalesInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.RecurringSalesInvoice))]
    [Guide("Create sales invoices that repeat on a regular schedule.")]
    [Guide("Ideal for subscription services, maintenance contracts, or regular billing.")]
    [Fields(typeof(ManagerServer.Model.RecurringSalesInvoice))]
    internal sealed class RecurringSalesInvoiceForm : NakedVueForm<ManagerServer.Model.RecurringSalesInvoice>
    {
        protected override void OnSource(RecurringSalesInvoice form, ManagerServer.Model.Object source)
        {
            if (source is SalesInvoice salesInvoice)
            {
                Copy(salesInvoice, form);

                // Copy() only matches members by name; SalesInvoice exposes CustomTheme/CustomThemeId
                // while this form exposes its own uniquely-named fields, so bridge via IHasCustomTheme.
                if (salesInvoice is IHasCustomTheme sourceCustomTheme)
                {
                    form.HasSalesInvoiceCustomTheme = sourceCustomTheme.CustomTheme;
                    form.SalesInvoiceCustomTheme = sourceCustomTheme.CustomThemeId;
                }
            }
        }
    }
}