using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Settings.BankRules.PaymentRules
{
    [ProtoContract]
    internal sealed class GetPaymentRuleView : GetObjectViewEndpoint<Model.PaymentRule>
    {
    }
}
