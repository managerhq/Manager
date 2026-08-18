using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.Payments
{
    [ProtoContract]
    [Title(nameof(Strings.Payment), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This page displays the current *payment footer* that appears at the bottom of payment transactions.")]
    [Guide("The footer text shown here will be automatically included on all new payment forms you create, providing consistent information across your payment documentation.")]
    [Guide("You can preview exactly how the footer will appear on printed or emailed payment receipts, ensuring the formatting meets your requirements before using it in actual transactions.")]
    [LinkGuide("To modify the payment footer content, see:", typeof(PaymentFooterForm))]
    internal class PaymentFooterView : DefaultView<GetPaymentFooterView>
    {
    }
}
