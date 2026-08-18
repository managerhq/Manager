using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.BankRules
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Receipts), nameof(Payments))]
    [Title(nameof(Strings.BankRules))]
    [Guide("Bank rules automate the categorization of imported bank transactions, saving time and ensuring consistency in your bookkeeping.")]
    [Guide("When you import a bank statement, the system checks each transaction against your defined rules and automatically allocates matching transactions to the appropriate accounts.")]
    [Guide("Rules are processed in order of specificity - more detailed rules (with more conditions) are applied before general rules.")]
    [SettingsItemScreenshot("fa-ruler-triangle", nameof(Strings.BankRules))]
    [Header("Types of Bank Rules")]
    [Guide("**Payment Rules** - Automatically categorize money going out of your bank accounts:")]
    [Guide("• Regular supplier payments and recurring expenses")]
    [Guide("• Utility bills, rent, and other operational costs")]
    [Guide("• Bank fees, interest, and financial charges")]
    [LinkGuide("For more information, see:", typeof(PaymentRules.PaymentRules))]
    [Guide("**Receipt Rules** - Automatically categorize money coming into your bank accounts:")]
    [Guide("• Customer payments and sales receipts")]
    [Guide("• Interest income and investment returns")]
    [Guide("• Refunds, rebates, and other income sources")]
    [LinkGuide("For more information, see:", typeof(ReceiptRules.ReceiptRules))]
    [Header("Best Practices")]
    [Guide("To create effective bank rules that save time and reduce errors:")]
    [Guide("• Use specific keywords that uniquely identify transactions")]
    [Guide("• Test rules with a small import first to verify accuracy")]
    [Guide("• Review and update rules periodically as your vendors and transaction patterns change")]
    [Guide("• Create separate rules for different accounts if transaction patterns vary")]
    internal sealed class BankRules : NakedNamespaces
    {
    }
}
