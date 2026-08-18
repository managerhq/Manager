using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.PurchaseInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of purchase invoices.")]
    [Guide("Use footers to add terms, conditions, or additional information to purchase invoices.")]
    [Fields(typeof(ManagerServer.Model.PurchaseInvoiceFooter))]
    internal sealed class PurchaseInvoiceFooterForm : NakedVueForm<ManagerServer.Model.PurchaseInvoiceFooter>
    {
    }
}