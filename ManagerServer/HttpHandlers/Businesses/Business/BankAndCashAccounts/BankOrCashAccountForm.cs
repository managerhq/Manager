using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.BankAndCashAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.BankOrCashAccount), nameof(Strings.Edit))]
    [Guide("This form allows you to create a new bank or cash account or edit an existing one.")]
    [Guide("Bank accounts track money in your bank, while cash accounts track physical cash on hand.")]
    [Header("Form Fields")]
    [Guide("Complete the following fields:")]
    [Fields(typeof(BankOrCashAccount))]
    [Header("Setting Initial Balances")]
    [Guide("New accounts start with a zero balance. To set an initial balance:")]
    [Guide("• For a positive balance, create a receipt in the `Receipts` tab")]
    [Guide("• For a negative balance, create a payment in the `Payments` tab")]
    [Guide("• For bulk adjustments, use the `JournalEntries` tab to create a journal entry")]
    internal sealed class BankOrCashAccountForm : NakedVueForm<ManagerServer.Model.BankOrCashAccount>
    {
    }
}