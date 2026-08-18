using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.LatePaymentFees
{
    [ProtoContract]
    [Title(nameof(Strings.LatePaymentFee), nameof(Strings.Edit))]
    [Guide("Late payment fees are charges applied to customers when they fail to pay invoices by the due date.")]
    [Guide("Use this form to record late payment fees that will be added to the customer's outstanding balance.")]
    [Guide("The fee amount can be a fixed charge or calculated as interest on the overdue invoice amount.")]
    [Guide("When you create a late payment fee, it automatically increases the customer's `Accounts receivable` balance and records income under `Late payment fees` in your profit and loss statement.")]
    [Fields(typeof(ManagerServer.Model.LatePaymentFee))]
    internal sealed class LatePaymentFeeForm : NakedVueForm<ManagerServer.Model.LatePaymentFee>
    {
        protected override bool CanHaveImage() => true;
    }
}