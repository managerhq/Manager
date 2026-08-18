using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Guide("Recurring receipts automate regular incoming payments, ensuring consistent income is recorded accurately and on time.")]
    [Guide("Use recurring receipts for rental income, membership fees, regular customer payments, investment income, or any predictable cash inflow.")]
    [Guide("The system creates receipt transactions automatically, helping you track income and maintain accurate cash flow records.")]
    [CustomFields]
    [ProtoContract]
    [Guid("1c7ecd01-eb32-47b1-8ccd-e85f77969b03")]
    [Currency(nameof(ReceivedIn))]
    public sealed class RecurringReceipt : Object, IRecurringTransactionFor<Receipt>, ICustomFields
    {
        [Guide("The date when the next receipt will be automatically created. This date advances automatically based on your frequency settings.")]
        [Guide("Set this to match when you expect to receive payments. For example, if rent is collected on the 1st, set this to the 1st.")]
        [ProtoMember(1), NoWrap, TableColumn] public DateTime? NextIssueDate { get; set; }
        [Guide("The frequency interval for creating receipts. This number works with the period type to match your income schedule.")]
        [Guide("Common examples: 1 Month = monthly income, 1 Week = weekly collections, 3 Months = quarterly dividends, 1 Year = annual fees.")]
        [ProtoMember(2), NoWrap, Placeholder("1")] public int? Interval { get; set; }
        [Guide("The period type that works with the interval to determine frequency. Select the unit of time for your income cycle.")]
        [Guide("Most recurring income is received monthly, but adjust this to match your actual collection schedule.")]
        [ProtoMember(3), NoWrap, EmptyLabel] public Period PeriodType { get; set; }
        [Guide("For monthly recurring receipts, specify which day of the month the receipt should be created.")]
        [Guide("Match this to when payments are typically received. For rent collected on the 1st, select '1st day of month'.")]
        [ProtoMember(31), NoWrap, EmptyLabel, IfEnum(nameof(PeriodType), (int)Period.Month)] public MonthDay MonthDay { get; set; }
        [Guide("Determines when the recurring schedule ends. This controls how long the system will continue creating receipts.")]
        [Guide("Choose 'Until further notice' for indefinite income like rent, or 'Until date' for fixed-term contracts.")]
        [ProtoMember(4), NoWrap, EmptyLabel] public ExpirationType ExpirationType { get; set; }
        [Guide("The date when recurring receipts will stop being created. The system will not create any receipts after this date.")]
        [Guide("Use this for fixed-term leases, time-limited contracts, or any income stream with a known end date.")]
        [ProtoMember(5), EmptyLabel, IfEnum(nameof(ExpirationType), (int)ExpirationType.Custom)] public DateTime? UntilDate { get; set; }
        [Guide("The type of payer making the payment. This determines which contact list to select from.")]
        [Guide("Choose `Customer` for most business income, `Supplier` for supplier refunds or rebates, or `Other` for non-business income.")]
        [ProtoMember(6), NoWrap, Prepend(nameof(Strings.Contact))] public PayerPayeeType PaidBy { get; set; }
        [Guide("Select the customer making the payment. This links the receipt to their account for tracking.")]
        [Guide("This is typically used for regular customer payments, rent from tenants, or subscription fees.")]
        [ProtoMember(7), EmptyLabel, NoWrap, IfEnum(nameof(PaidBy), (int)PayerPayeeType.Customer), Autocomplete(typeof(ManagerServer.Model.Customer))] public Guid? Customer { get; set; }
        [Guide("Select the supplier making the payment. This links the receipt to their account for tracking.")]
        [Guide("Supplier receipts might include rebates, refunds, or commission income from suppliers.")]
        [ProtoMember(8), EmptyLabel, NoWrap, IfEnum(nameof(PaidBy), (int)PayerPayeeType.Supplier), Autocomplete(typeof(ManagerServer.Model.Supplier))] public Guid? Supplier { get; set; }
        [Guide("Enter the name of the payer when they're not in your customer or supplier lists.")]
        [Guide("Use this for one-time payers or personal income. Consider adding regular payers as customers for better tracking.")]
        [ProtoMember(9), EmptyLabel, IfEnum(nameof(PaidBy), (int)PayerPayeeType.Other), Placeholder(nameof(Strings.Optional)), Typeahead] public string Contact { get; set; }
        [Guide("The bank or cash account that will receive the payment. This account will be debited (increased) for each receipt.")]
        [Guide("Select the account where you typically deposit this type of income. The currency of this account determines the receipt currency.")]
        [ProtoMember(10), NoWrap, Autocomplete(typeof(ManagerServer.Model.IBankOrCashAccount)), Prepend(nameof(Strings.Account))] public Guid? ReceivedIn { get; set; }
        [Guide("The clearing status for bank transactions. Controls whether receipts appear as pending or cleared in bank reconciliation.")]
        [Guide("Use 'Pending' if payments need to clear through the bank, or 'Cleared' for immediate deposits like cash or electronic transfers.")]
        [ProtoMember(11), IfTrue(nameof(ReceivedIn), nameof(BankOrCashAccount.CanHavePendingTransactions))] public BankAccountClearStatus Cleared { get; set; }
        //[ProtoMember(12), EmptyLabel, IfTrue(nameof(ReceivedIn), nameof(BankOrCashAccount.CanHavePendingTransactions)), IfEnum(nameof(Cleared), (int)BankAccountClearStatus.OnALaterDate), Placeholder(nameof(Strings.Pending)), Prepend(nameof(Strings.Date))] public DateTime? BankClearDate;
        [Guide("A description that identifies this recurring receipt and will be copied to each generated receipt.")]
        [Guide("Be specific to help identify the income source. Examples: 'Monthly rent - Unit 5', 'Weekly service fee', 'Quarterly dividend income'.")]
        [ProtoMember(13), Long, Placeholder(nameof(Strings.Optional)), TableColumn] public string Description { get; set; }
        [Guide("The receipt lines specifying which accounts to credit and the amounts. These lines will be copied to each generated receipt.")]
        [Guide("Add a line for each income category or account affected by this receipt. The total of all lines equals the receipt amount.")]
        [Guide("If amounts vary, you'll need to edit individual receipts after they're created or update this template.")]
        [ProtoMember(14)] public Receipt.Line[] Lines { get; set; }
        [ProtoMember(15), NoLabel, Prepend(nameof(Strings.InventoryLocation)), Autocomplete(typeof(CustomInventoryLocation)), IfAnyNotNull(nameof(Receipt.Line.Item))] public Guid? InventoryLocation { get; set; }
        [ProtoMember(16), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [ProtoMember(17), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }
        [ProtoMember(18), Label(nameof(Strings.Column), nameof(Strings.Qty))] public bool QuantityColumn { get; set; }
        [ProtoMember(19), Label(nameof(Strings.Column), nameof(Strings.UnitPrice)), IfTrue(nameof(QuantityColumn))] public bool UnitPriceColumn { get; set; }
        [ProtoMember(20), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(21), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [ProtoMember(22), IfContains<TaxCode>] public bool AmountsAreTaxExclusive { get; set; }
        [Guide("When enabled, the system automatically assigns sequential reference numbers to each generated receipt.")]
        [Guide("Recommended for maintaining a clear audit trail of all income. You can still add receipt details in the description field.")]
        [ProtoMember(23)] public bool AutomaticReference { get; set; }
        [ProtoMember(24), Label(nameof(Strings.CustomTitle))] public bool HasReceiptCustomTitle { get; set; }
        [ProtoMember(25), NoLabel, IfTrue(nameof(HasReceiptCustomTitle)), Placeholder(nameof(Strings.Receipt))] public string ReceiptCustomTitle { get; set; }
        [ProtoMember(26), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(27), Label(nameof(Strings.Footers))] public bool HasReceiptFooters { get; set; }
        [ProtoMember(28), Autocomplete(typeof(ManagerServer.Model.ReceiptFooter)), NoLabel, IfTrue(nameof(HasReceiptFooters))] public Guid[] ReceiptFooters { get; set; }
        [ProtoMember(29)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(30)] public CustomFields CustomFields2 { get; set; }

        DateTime? IRecurringTransaction.NextIssueDate { get => NextIssueDate; set => NextIssueDate = value; }
        int? IRecurringTransaction.Interval => Interval;
        Period IRecurringTransaction.PeriodType => PeriodType;
        ExpirationType IRecurringTransaction.ExpirationType => ExpirationType;
        DateTime? IRecurringTransaction.UntilDate => UntilDate;

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        MonthDay IRecurringTransaction.MonthDay => MonthDay;
    }
}