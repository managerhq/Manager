using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.SalesOrder
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SalesOrders))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.SalesOrder))]
    [Guide("Configure the email template for sending sales orders to customers.")]
    [Guide("Customize subject line and message body with placeholders for dynamic content.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForSalesOrder))]
    internal sealed class EmailTemplateForSalesOrderForm : NakedVueForm<ManagerServer.Model.EmailTemplateForSalesOrder>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForSalesOrder>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}