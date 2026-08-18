using ManagerServer.Attributes;
using ManagerServer.Model;
using System;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.BankRules.PaymentRules
{
    [ProtoContract]
    [Title(nameof(Strings.PaymentRule), nameof(Strings.Edit))]
    [Guide("`PaymentRule` form allows to create new payment rule or edit the existing one.")]
    [Guide("The form contains the following fields:")]
    [Fields(typeof(PaymentRule))]
    internal sealed class PaymentRuleForm : NakedVueForm<ManagerServer.Model.PaymentRule>
    {
        [ProtoMember(1)] public string Description;
        [ProtoMember(2)] public Guid? BankAccount;

        protected override void OnSource(PaymentRule form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                form.IfBankAccountIs = BankAccount;
                form.Conditions = new PaymentRule.Condition[] { new PaymentRule.Condition() { AndDescriptionContains = Description } };
            }
        }
    }
}