using System;
using System.Linq;
using System.Collections.Generic;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByCustomField;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomField
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByCustomField))]
    [Guide("The Sales Invoice Totals by Custom Field report analyzes sales by custom attributes.")]
    [Guide("It groups and totals sales amounts based on custom field values.")]
    [LinkGuide("For more information see:", typeof(SalesInvoiceTotalsByCustomFieldForm))]
    internal sealed class SalesInvoiceTotalsByCustomFieldView : DefaultView<GetSalesInvoiceTotalsByCustomFieldView>
    {
    }
}