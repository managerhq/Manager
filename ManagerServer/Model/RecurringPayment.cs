using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Guide("Recurring payments automate regular outgoing payments, ensuring bills and other obligations are recorded on time.")]
    [Guide("Use recurring payments for rent, loan repayments, subscription fees, regular supplier payments, or any predictable cash outflow.")]
    [Guide("The system creates payment transactions automatically, helping you track cash flow and maintain accurate bank reconciliations.")]
    [CustomFields]
    [ProtoContract]
    [Guid("789d22dc-bc42-4952-a591-60123b344726")]
    [Currency(nameof(PaidFrom))]
    public sealed class RecurringPayment : Object, IRecurringTransactionFor<Payment>, ICustomFields
    {
        [Guide("The date when the next payment will be automatically created. This date advances automatically based on your frequency settings.")]
        [Guide("Set this to match your payment due dates. For example, if rent is due on the 1st, set the next issue date to the 1st.")]
        [ProtoMember(1), NoWrap] public DateTime? NextIssueDate { get; set; }
        [Guide("The frequency interval for creating payments. This number works with the period type to match your payment schedule.")]
        [Guide("Common examples: 1 Month = monthly payments, 2 Weeks = fortnightly wages, 3 Months = quarterly fees, 1 Year = annual subscriptions.")]
        [ProtoMember(2), NoWrap, Placeholder("1")] public int? Interval { get; set; }
        [Guide("The period type that works with the interval to determine frequency. Select the unit of time for your payment cycle.")]
        [Guide("Choose the option that matches how often you make this payment. Most regular payments are monthly.")]
        [ProtoMember(3), NoWrap, EmptyLabel] public Period PeriodType { get; set; }
        [Guide("For monthly recurring payments, specify which day of the month the payment should be created.")]
        [Guide("Match this to your actual payment dates. For example, if rent is always due on the 1st, select '1st day of month'.")]
        [ProtoMember(31), NoWrap, EmptyLabel, IfEnum(nameof(PeriodType), (int)Period.Month)] public MonthDay MonthDay { get; set; }
        [Guide("Determines when the recurring schedule ends. This controls how long the system will continue creating payments.")]
        [Guide("Choose 'Until further notice' for indefinite payments like rent, or 'Until date' for loan repayments with a fixed end date.")]
        [ProtoMember(4), NoWrap, EmptyLabel] public ExpirationType ExpirationType { get; set; }
        [Guide("The date when recurring payments will stop being created. The system will not create any payments after this date.")]
        [Guide("Use this for loan repayments, fixed-term leases, or any payment series with a known end date.")]
        [ProtoMember(5), EmptyLabel, IfEnum(nameof(ExpirationType), (int)ExpirationType.Custom)] public DateTime? UntilDate { get; set; }
        [Guide("The bank or cash account from which the payment will be made. This account will be credited (reduced) for each payment.")]
        [Guide("Select the account you typically use for this type of payment. The currency of this account determines the payment currency.")]
        [ProtoMember(6), NoWrap, Autocomplete(typeof(ManagerServer.Model.IBankOrCashAccount)), Prepend(nameof(Strings.Account))] public Guid? PaidFrom { get; set; }
        [Guide("The clearing status for bank transactions. Controls whether payments appear as pending or cleared in bank reconciliation.")]
        [Guide("Use 'Pending' if you want to review and clear payments manually, or 'Cleared' if they should immediately show as reconciled.")]
        [ProtoMember(7), IfTrue(nameof(PaidFrom), nameof(BankOrCashAccount.CanHavePendingTransactions))] public BankAccountClearStatus Cleared { get; set; }
        //[ProtoMember(8), EmptyLabel, IfTrue(nameof(PaidFrom), nameof(BankOrCashAccount.CanHavePendingTransactions)), IfEnum(nameof(Cleared), (int)BankAccountClearStatus.OnALaterDate), Placeholder(nameof(Strings.Pending)), Prepend(nameof(Strings.Date))] public DateTime? BankClearDate;
        [Guide("The type of payee receiving the payment. This determines which contact list to select from.")]
        [Guide("Choose `Customer` for refunds or credits, `Supplier` for most business payments, or `Other` for payments to non-business contacts.")]
        [ProtoMember(9), NoWrap, Prepend(nameof(Strings.Contact))] public PayerPayeeType Payee { get; set; }
        [Guide("Select the customer receiving the payment. This links the payment to their account for tracking.")]
        [Guide("Customer payments might include refunds, overpayment returns, or commission payments to customer-agents.")]
        [ProtoMember(10), EmptyLabel, NoWrap, IfEnum(nameof(Payee), (int)PayerPayeeType.Customer), Autocomplete(typeof(ManagerServer.Model.Customer))] public Guid? Customer { get; set; }
        [Guide("Select the supplier receiving the payment. This links the payment to their account for tracking.")]
        [Guide("Most recurring payments will be to suppliers for regular business expenses.")]
        [ProtoMember(11), EmptyLabel, NoWrap, IfEnum(nameof(Payee), (int)PayerPayeeType.Supplier), Autocomplete(typeof(ManagerServer.Model.Supplier))] public Guid? Supplier { get; set; }
        [Guide("Enter the name of the payee when they're not in your customer or supplier lists.")]
        [Guide("Use this for one-off payees or personal payments. Consider adding regular payees as suppliers for better tracking.")]
        [ProtoMember(12), EmptyLabel, IfEnum(nameof(Payee), (int)PayerPayeeType.Other), Placeholder(nameof(Strings.Optional)), Typeahead] public string Contact { get; set; }
        [Guide("A description that identifies this recurring payment and will be copied to each generated payment.")]
        [Guide("Be specific to help with record-keeping. Examples: 'Monthly office rent', 'Fortnightly payroll', 'Quarterly insurance premium'.")]
        [ProtoMember(13), Long, Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
        [Guide("The payment lines specifying which accounts to debit and the amounts. These lines will be copied to each generated payment.")]
        [Guide("Add a line for each expense category or account affected by this payment. The total of all lines equals the payment amount.")]
        [Guide("If amounts vary, you'll need to edit individual payments after they're created or update this template.")]
        [ProtoMember(14)] public Payment.Line[] Lines { get; set; }
        [ProtoMember(15), NoLabel, Prepend(nameof(Strings.InventoryLocation)), Autocomplete(typeof(CustomInventoryLocation)), IfAnyNotNull(nameof(Payment.Line.Item))] public Guid? InventoryLocation { get; set; }
        [ProtoMember(16), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [ProtoMember(17), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }
        [ProtoMember(18), Label(nameof(Strings.Column), nameof(Strings.Qty))] public bool QuantityColumn { get; set; }
        [ProtoMember(19), Label(nameof(Strings.Column), nameof(Strings.UnitPrice)), IfTrue(nameof(QuantityColumn))] public bool UnitPriceColumn { get; set; }
        [ProtoMember(20), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(21), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [ProtoMember(22), IfContains<TaxCode>] public bool AmountsAreTaxExclusive { get; set; }
        [Guide("When enabled, the system automatically assigns sequential reference numbers to each generated payment.")]
        [Guide("Recommended for maintaining a clear audit trail. You can still add payment details in the description field.")]
        [ProtoMember(23)] public bool AutomaticReference { get; set; }
        [ProtoMember(24), Label(nameof(Strings.CustomTitle))] public bool HasPaymentCustomTitle { get; set; }
        [ProtoMember(25), NoLabel, IfTrue(nameof(HasPaymentCustomTitle)), Placeholder(nameof(Strings.Payment))] public string PaymentCustomTitle { get; set; }
        [ProtoMember(26), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(27), Label(nameof(Strings.Footers))] public bool HasPaymentFooters { get; set; }
        [ProtoMember(28), Autocomplete(typeof(ManagerServer.Model.PaymentFooter)), NoLabel, IfTrue(nameof(HasPaymentFooters))] public Guid[] PaymentFooters { get; set; }
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