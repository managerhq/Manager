using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.SalesInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of sales invoices.")]
    [Guide("Use footers to add terms, conditions, or additional information to sales invoices.")]
    [Fields(typeof(ManagerServer.Model.SalesInvoiceFooter))]
    internal sealed class SalesInvoiceFooterForm : NakedVueForm<ManagerServer.Model.SalesInvoiceFooter>
    {        
    }
}
