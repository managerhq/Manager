using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringSalesOrders
{
    [ProtoContract]
    [Title(nameof(Strings.RecurringSalesOrder))]
    [Guide("Create sales orders that repeat on a regular schedule.")]
    [Guide("Useful for standing orders from customers or regular deliveries.")]
    [Fields(typeof(ManagerServer.Model.RecurringSalesOrder))]
    internal sealed class RecurringSalesOrderForm : NakedVueForm<ManagerServer.Model.RecurringSalesOrder>
    {
        protected override void OnSource(ManagerServer.Model.RecurringSalesOrder form, ManagerServer.Model.Object source)
        {
            if (source is ManagerServer.Model.SalesOrder salesOrder)
            {
                Copy(salesOrder, form);

                // Copy() only matches members by name; SalesOrder exposes CustomTheme/CustomThemeId
                // while this form exposes its own uniquely-named fields, so bridge via IHasCustomTheme.
                if (salesOrder is ManagerServer.Model.IHasCustomTheme sourceCustomTheme)
                {
                    form.HasSalesOrderCustomTheme = sourceCustomTheme.CustomTheme;
                    form.SalesOrderCustomTheme = sourceCustomTheme.CustomThemeId;
                }
            }
        }
    }
}
