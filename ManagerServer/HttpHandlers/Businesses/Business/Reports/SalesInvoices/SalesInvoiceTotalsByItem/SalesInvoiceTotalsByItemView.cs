using System;
using System.Linq;
using System.Collections.Generic;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByItem;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByItem
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByItem))]
    [Guide("The Sales Invoice Totals by Item report shows revenue by product or service.")]
    [Guide("It displays total sales amounts for each item over specified periods.")]
    [LinkGuide("For more information see:", typeof(SalesInvoiceTotalsByItemForm))]
    internal sealed class SalesInvoiceTotalsByItemView : DefaultView<GetSalesInvoiceTotalsByItemView>
    {
    }
}