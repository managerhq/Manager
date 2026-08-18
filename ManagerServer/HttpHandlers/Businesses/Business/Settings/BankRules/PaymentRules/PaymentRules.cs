using ManagerServer.Attributes;
using System.Linq;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.BankRules.PaymentRules
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Payments))]
    [Guid("0f50ef3e-b1f0-435a-a5bd-8a355ab09b66")]
    [Title(nameof(Strings.PaymentRules))]
    [Guide("The **Payment Rules** screen allows you to manage rules that automatically categorize your *uncategorized payments* under the **Payments** tab.")]
    [Header("Accessing Payment Rules")]
    [Guide("To access **Payment Rules**, go to the **Settings** tab. Then click **Bank Rules**.")]
    [SettingsItemScreenshot(icon: "fa-ruler-triangle", name: nameof(Strings.BankRules))]
    [Guide("Within the **Bank Rules** screen, click **Payment Rules**.")]
    [Header("Creating Payment Rules")]
    [Guide("To create a new payment rule, click the **New Payment Rule** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.PaymentRules), name: nameof(Strings.NewPaymentRule))]
    [Guide("This action takes you to the **New Payment Rule** form, where you can define the conditions and actions for your rule.")]
    [LinkGuide("For more information, see:", typeof(PaymentRuleForm))]
    [Header("Alternative Method")]
    [Guide("Another approach to create new payment rules is from the **Uncategorized Payments** screen.")]
    [Guide("The *uncategorized payments* screen shows a list of payments that haven't been categorized yet (usually after importing a bank statement).")]
    [Guide("For uncategorized payments, there is a **New Payment Rule** button which will automatically pre-fill the new payment rule with necessary details from the transaction, making it easier to create payment rules.")]
    [LinkGuide("For more information, see:", typeof(Payments.UncategorizedPayments))]
    internal sealed class PaymentRules : NakedObjectsWithAutomaticRows<ManagerServer.Model.PaymentRule>
    {
        [Default]
        [Guid("af9c687d-3207-4e97-addc-ddcf3cec2ee7")]
        public string[] GetIfBankAccountIs(ManagerServer.Model.PaymentRule[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.BankOrCashAccount>(x.IfBankAccountIs)?.GetName()).ToArray();
        }

        [Default]
        [Guid("5d31032c-867d-47ab-938b-0c5b15b052e6")]
        public string[] GetAndDescriptionContains(ManagerServer.Model.PaymentRule[] rows)
        {
            return rows.Select(x => string.Join(", ", x.Conditions?.Select(x => x.AndDescriptionContains) ?? new string[0])).ToArray();
        }
    }
}
