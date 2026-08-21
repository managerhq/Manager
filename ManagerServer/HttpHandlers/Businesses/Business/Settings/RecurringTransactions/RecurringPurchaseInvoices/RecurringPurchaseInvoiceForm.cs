using System;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPurchaseInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.RecurringPurchaseInvoice))]
    [Guide("Create purchase invoices that repeat on a regular schedule.")]
    [Guide("Useful for rent, subscriptions, and other regular supplier bills.")]
    [Fields(typeof(ManagerServer.Model.RecurringPurchaseInvoice))]
    internal sealed class RecurringPurchaseInvoiceForm : NakedVueForm<ManagerServer.Model.RecurringPurchaseInvoice>
    {
        protected override void OnSource(RecurringPurchaseInvoice form, ManagerServer.Model.Object source)
        {
            if (source is PurchaseInvoice purchaseInvoice)
            {
                Copy(purchaseInvoice, form);

                // Copy() only matches members by name; PurchaseInvoice exposes CustomTheme/CustomThemeId
                // while this form exposes its own uniquely-named fields, so bridge via IHasCustomTheme.
                if (purchaseInvoice is IHasCustomTheme sourceCustomTheme)
                {
                    form.HasPurchaseInvoiceCustomTheme = sourceCustomTheme.CustomTheme;
                    form.PurchaseInvoiceCustomTheme = sourceCustomTheme.CustomThemeId;
                }
            }
        }
    }
}