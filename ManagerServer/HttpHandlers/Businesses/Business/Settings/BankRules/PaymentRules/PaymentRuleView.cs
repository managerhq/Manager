using ManagerServer.Api.Businesses.Business.Settings.BankRules.PaymentRules;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.BankRules.PaymentRules
{
    [ProtoContract]
    internal sealed class PaymentRuleView : DefaultView<GetPaymentRuleView>
    {
    }
}
