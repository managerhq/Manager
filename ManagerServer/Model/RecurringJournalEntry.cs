using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Guide("Recurring journal entries automate repetitive accounting adjustments, saving time and ensuring consistency in your financial records.")]
    [Guide("Use recurring journal entries for depreciation, amortization, accruals, prepaid expense allocations, or any regular accounting adjustment.")]
    [Guide("Journal entries must balance (total debits equal total credits). The system will create these entries automatically on your specified schedule.")]
    [CustomFields]
    [ProtoContract]
    [Guid("b4c1b390-351e-4579-b43b-412b920cddaf")]
    [Currency]
    public sealed class RecurringJournalEntry : Object, IRecurringTransactionFor<JournalEntry>, ICustomFields
    {
        [Guide("The date when the next journal entry will be automatically created. This date advances automatically based on your frequency settings.")]
        [Guide("For month-end adjustments, set this to the last day of the month. The system checks daily for entries due to be created.")]
        [ProtoMember(13), NoWrap] public DateTime? NextIssueDate { get; set; }
        [Guide("The frequency interval for creating journal entries. This number works with the period type to set your adjustment schedule.")]
        [Guide("Common examples: 1 Month = monthly adjustments, 3 Months = quarterly entries, 1 Year = annual adjustments.")]
        [ProtoMember(12), NoWrap, Placeholder("1")] public int? Interval { get; set; }
        [Guide("The period type that works with the interval to determine frequency. Select the unit of time for your accounting cycle.")]
        [Guide("Most accounting adjustments are monthly. Use 'Days' for daily accruals or 'Years' for annual adjustments.")]
        [ProtoMember(11), NoWrap, EmptyLabel] public Period PeriodType { get; set; }
        [Guide("For monthly recurring entries, specify which day of the month the journal entry should be created.")]
        [Guide("'Last day of month' is common for month-end adjustments. Choose specific days for mid-month allocations.")]
        [ProtoMember(25), NoWrap, EmptyLabel, IfEnum(nameof(PeriodType), (int)Period.Month)] public MonthDay MonthDay { get; set; }
        [Guide("Determines when the recurring schedule ends. This controls how long the system will continue creating journal entries.")]
        [Guide("Choose 'Until further notice' for perpetual adjustments like depreciation, or 'Until date' for temporary accruals.")]
        [ProtoMember(15), NoWrap, EmptyLabel] public ExpirationType ExpirationType { get; set; }
        [Guide("The date when recurring journal entries will stop being created. The system will not create any entries after this date.")]
        [Guide("Use this for prepaid expenses that will be fully amortized or accruals that will reverse by a specific date.")]
        [ProtoMember(16), EmptyLabel, IfEnum(nameof(ExpirationType), (int)ExpirationType.Custom)] public DateTime? UntilDate { get; set; }
        [Guide("The currency for this journal entry template. Leave blank to use the base currency.")]
        [Guide("Only specify a foreign currency if you need to record adjustments in a currency other than your base currency.")]
        [ProtoMember(8), Autocomplete(typeof(ForeignCurrency))] public Guid? Currency { get; set; }
        [Guide("A description or explanation for the journal entry that will be copied to each generated entry.")]
        [Guide("Be descriptive to help with audit trails. Examples: 'Monthly depreciation - Equipment', 'Quarterly insurance expense allocation', 'Monthly payroll accrual'.")]
        [ProtoMember(3), Long] public string Narration { get; set; }
        [Guide("The debit and credit lines that make up the journal entry. Total debits must equal total credits for the entry to be valid.")]
        [Guide("Each line represents either a debit or credit to a specific account. Add multiple lines as needed for complex adjustments.")]
        [Guide("The same amounts will be used for each generated entry unless you edit this template.")]
        [ProtoMember(17), InitialSize(2)] public JournalEntry.Line[] Lines { get; set; }
        [Guide("Specify the tax transaction type when tax codes are used in the journal entry lines.")]
        [Guide("This determines how the transaction affects your tax reports. Only required if you're using tax codes in the entry.")]
        [ProtoMember(19), IfAnyNotNull(nameof(JournalEntry.Line.TaxCode)), Prepend(nameof(Strings.ForTaxPurposesThisIs)), NoLabel] public TaxTransactionType ForTaxPurposesThisIs { get; set; }
        [ProtoMember(26), Label(nameof(Strings.Column), nameof(Strings.Item))] public bool ItemColumn { get; set; }
        [ProtoMember(20), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }
        [ProtoMember(24), Label(nameof(Strings.Column), nameof(Strings.Qty))] public bool QuantityColumn { get; set; }
        [ProtoMember(9), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(10), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? Theme { get; set; }
        [ProtoMember(21), Label(nameof(Strings.Footers))] public bool HasJournalEntryFooters { get; set; }
        [ProtoMember(22), Autocomplete(typeof(ManagerServer.Model.JournalEntryFooter)), NoLabel, IfTrue(nameof(HasJournalEntryFooters))] public Guid[] JournalEntryFooters { get; set; }
        [Guide("When enabled, the system automatically assigns sequential reference numbers to each generated journal entry.")]
        [Guide("Recommended for audit trail purposes. The system will ensure each entry has a unique identifier.")]
        [ProtoMember(14)] public bool AutomaticReference { get; set; }
        [ProtoMember(7)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(23)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(18)] public Guid? Obsolete_InventoryLocation { get; set; }
        [ProtoMember(4)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }

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