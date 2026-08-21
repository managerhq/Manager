using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPurchaseOrders
{
    [ProtoContract]
    [Title(nameof(Strings.RecurringPurchaseOrder))]
    [Guide("Create purchase orders that repeat on a regular schedule.")]
    [Guide("Useful for standing orders with suppliers or regular inventory replenishments.")]
    [Fields(typeof(ManagerServer.Model.RecurringPurchaseOrder))]
    internal sealed class RecurringPurchaseOrderForm : NakedVueForm<ManagerServer.Model.RecurringPurchaseOrder>
    {
        protected override void OnSource(ManagerServer.Model.RecurringPurchaseOrder form, ManagerServer.Model.Object source)
        {
            if (source is ManagerServer.Model.PurchaseOrder purchaseOrder)
            {
                Copy(purchaseOrder, form);

                // Copy() only matches members by name; PurchaseOrder exposes CustomTheme/CustomThemeId
                // while this form exposes its own uniquely-named fields, so bridge via IHasCustomTheme.
                if (purchaseOrder is ManagerServer.Model.IHasCustomTheme sourceCustomTheme)
                {
                    form.HasPurchaseOrderCustomTheme = sourceCustomTheme.CustomTheme;
                    form.PurchaseOrderCustomTheme = sourceCustomTheme.CustomThemeId;
                }
            }
        }
    }
}
