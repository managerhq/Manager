using ManagerServer.Model;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.BankRules.ReceiptRules
{
    [ProtoContract]
    [Title(nameof(Strings.ReceiptRule), nameof(Strings.Edit))]
    [Guide("The Receipt Rule form is used to create automatic categorization rules for receipts.")]
    [Guide("Rules automatically assign accounts and other fields based on transaction descriptions.")]
    [Fields(typeof(ManagerServer.Model.ReceiptRule))]
    internal sealed class ReceiptRuleForm : NakedVueForm<ManagerServer.Model.ReceiptRule>
    {
        [ProtoMember(1)] public string Description;
        [ProtoMember(2)] public Guid? BankAccount;

        protected override void OnSource(ReceiptRule form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                form.IfBankAccountIs = BankAccount;
                form.Conditions = new ReceiptRule.Condition[] { new ReceiptRule.Condition() { AndDescriptionContains = Description } };
            }
        }
    }
}