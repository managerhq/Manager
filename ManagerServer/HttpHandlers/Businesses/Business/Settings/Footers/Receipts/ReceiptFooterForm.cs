using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.Receipts
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of receipts.")]
    [Guide("Use footers to add terms, conditions, or additional information to receipts.")]
    [Fields(typeof(ManagerServer.Model.ReceiptFooter))]
    internal sealed class ReceiptFooterForm : NakedVueForm<ManagerServer.Model.ReceiptFooter>
    {
    }
}