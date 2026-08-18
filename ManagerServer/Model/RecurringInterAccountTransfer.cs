using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Guide("Recurring inter account transfers automate regular movements of money between your bank and cash accounts.")]
    [Guide("Use recurring transfers for regular savings deposits, loan repayments between accounts, petty cash top-ups, or scheduled investment transfers.")]
    [Guide("The system creates transfer transactions automatically, maintaining accurate balances across all your accounts.")]
    [CustomFields]
    [ProtoContract]
    [Guid("10ac4ab8-df74-4faf-b7fc-eb343556fb1b")]
    public sealed class RecurringInterAccountTransfer : Object, IRecurringTransactionFor<InterAccountTransfer>, ICustomFields
    {
        [Guide("The date when the next inter account transfer will be automatically created. This date advances automatically based on your frequency settings.")]
        [Guide("Set this to the date of your first scheduled transfer. The system checks daily for transfers due to be created.")]
        [ProtoMember(1), NoWrap, TableColumn] public DateTime? NextIssueDate { get; set; }
        [Guide("The frequency interval for creating transfers. This number works with the period type to set your transfer schedule.")]
        [Guide("Common examples: 1 Week = weekly transfers, 1 Month = monthly transfers, 3 Months = quarterly movements.")]
        [ProtoMember(2), NoWrap, Placeholder("1")] public int? Interval { get; set; }
        [Guide("The period type that works with the interval to determine frequency. Select the unit of time for your transfer cycle.")]
        [Guide("Choose based on how often you need to move money between accounts.")]
        [ProtoMember(3), NoWrap, EmptyLabel] public Period PeriodType { get; set; }
        [Guide("For monthly recurring transfers, specify which day of the month the transfer should occur.")]
        [Guide("Useful for scheduling transfers to coincide with specific dates like salary deposits or bill payments.")]
        [ProtoMember(20), NoWrap, EmptyLabel, IfEnum(nameof(PeriodType), (int)Period.Month)] public MonthDay MonthDay { get; set; }
        [Guide("Determines when the recurring schedule ends. This controls how long the system will continue creating transfers.")]
        [Guide("Choose 'Until further notice' for indefinite transfers like regular savings, or 'Until date' for transfers with a known end point.")]
        [ProtoMember(4), NoWrap, EmptyLabel] public ExpirationType ExpirationType { get; set; }
        [Guide("The date when recurring transfers will stop being created. The system will not create any transfers after this date.")]
        [Guide("Use this for transfers related to loans with fixed end dates or temporary cash management needs.")]
        [ProtoMember(5), EmptyLabel, IfEnum(nameof(ExpirationType), (int)ExpirationType.Custom)] public DateTime? UntilDate { get; set; }
        [Guide("A description that identifies this recurring transfer and will be copied to each generated transfer.")]
        [Guide("Be descriptive to help track money movements. Examples: 'Weekly petty cash top-up', 'Monthly savings deposit', 'Loan repayment transfer'.")]
        [ProtoMember(6), Long, TableColumn] public string Description { get; set; }
        [Guide("The bank or cash account from which money will be transferred. This account's balance will decrease by the transfer amount.")]
        [Guide("Select the account that has the funds to transfer. This could be your main operating account or any account with surplus funds.")]
        [ProtoMember(7), NoWrap, Autocomplete(typeof(IBankOrCashAccount)), TableColumn] public Guid? PaidFrom { get; set; }
        [Guide("The amount to transfer from the source account in its currency.")]
        [Guide("Enter the exact amount to transfer each time. This amount will be deducted from the source account.")]
        [ProtoMember(8), NoWrap, AppendCurrency(nameof(PaidFrom)), EmptyLabel, Prepend(nameof(Strings.Amount))] public decimal CreditAmount { get; set; }
        [Guide("The clearing status for the source account transaction. Controls how the transfer appears in bank reconciliation.")]
        [Guide("Use 'Pending' for transfers that need to clear through the banking system, or 'Cleared' for internal transfers.")]
        [ProtoMember(9), Prepend(nameof(Strings.Cleared)), EmptyLabel, IfNotNull(nameof(PaidFrom))] public BankAccountClearStatus CreditClearStatus { get; set; }
        [Guide("The bank or cash account that will receive the transferred money. This account's balance will increase by the transfer amount.")]
        [Guide("Select the destination account. This could be a savings account, loan account, or any account receiving the funds.")]
        [ProtoMember(11), NoWrap, Autocomplete(typeof(IBankOrCashAccount)), TableColumn] public Guid? ReceivedIn { get; set; }
        [Guide("The amount to receive in the destination account. Only shown when transferring between accounts with different currencies.")]
        [Guide("For foreign currency transfers, enter the amount in the destination account's currency after exchange rate conversion.")]
        [ProtoMember(12), NoWrap, AppendCurrency(nameof(ReceivedIn)), EmptyLabel, Prepend(nameof(Strings.Amount)), IfNotEqual(nameof(PaidFrom) +"."+nameof(Currency), nameof(ReceivedIn) + "." + nameof(Currency))] public decimal DebitAmount { get; set; }
        [Guide("The clearing status for the destination account transaction. Controls how the transfer appears in bank reconciliation.")]
        [Guide("Usually matches the source account clearing status, but can differ for transfers between different bank institutions.")]
        [ProtoMember(13), Prepend(nameof(Strings.Cleared)), EmptyLabel, IfNotNull(nameof(ReceivedIn))] public BankAccountClearStatus DebitClearStatus { get; set; }
        [Guide("When enabled, the system automatically assigns sequential reference numbers to each generated transfer.")]
        [Guide("Recommended for tracking transfers systematically. The reference helps identify transfers in bank statements.")]
        [ProtoMember(15)] public bool AutomaticReference { get; set; }
        [ProtoMember(16), Label(nameof(Strings.Footers))] public bool HasInterAccountTransferFooters { get; set; }
        [ProtoMember(17), Autocomplete(typeof(ManagerServer.Model.InterAccountTransferFooter)), NoLabel, IfTrue(nameof(HasInterAccountTransferFooters))] public Guid[] InterAccountTransferFooters { get; set; }
        [ProtoMember(18)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(19)] public CustomFields CustomFields2 { get; set; }

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