using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.Payments
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of payments.")]
    [Guide("Use footers to add terms, conditions, or additional information to payments.")]
    [Fields(typeof(ManagerServer.Model.PaymentFooter))]
    internal sealed class PaymentFooterForm : NakedVueForm<ManagerServer.Model.PaymentFooter>
    {
    }
}