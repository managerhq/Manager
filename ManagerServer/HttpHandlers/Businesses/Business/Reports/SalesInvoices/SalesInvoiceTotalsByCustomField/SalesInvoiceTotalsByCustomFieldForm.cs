using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomField
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByCustomField))]
    [Guide("The Sales Invoice Totals by Custom Field form configures custom field analysis.")]
    [Guide("Set parameters to analyze sales by custom field values.")]
    [Fields(typeof(ManagerServer.Model.SalesInvoiceTotalsByCustomField))]
    internal sealed class SalesInvoiceTotalsByCustomFieldForm : NakedVueForm<ManagerServer.Model.SalesInvoiceTotalsByCustomField>
    {
    }
}
