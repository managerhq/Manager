using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByItem
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByItem))]
    [Guide("The Sales Invoice Totals by Item form configures item sales analysis.")]
    [Guide("Set date ranges to analyze sales revenue broken down by item.")]
    [Fields(typeof(ManagerServer.Model.SalesInvoiceTotalsByItem))]
    internal sealed class SalesInvoiceTotalsByItemForm : NakedVueForm<ManagerServer.Model.SalesInvoiceTotalsByItem>
    {
    }
}
