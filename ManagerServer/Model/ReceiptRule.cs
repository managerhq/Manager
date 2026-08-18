using ManagerServer.Globalization;
using System.Linq;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("72ed576f-e78b-41eb-8188-73285f32c2d2")]
    public sealed class ReceiptRule : Object
    {
        [Guide("Select the specific bank or cash account where this receipt rule should apply.")]
        [Guide("Leave this field empty to apply the rule to receipts in any bank or cash account.")]
        [Guide("Account-specific rules are useful when different accounts receive different types of income.")]
        [ProtoMember(1), Autocomplete(typeof(ManagerServer.Model.BankOrCashAccount)), TableColumn] public Guid? IfBankAccountIs { get; set; }
        [Guide("Choose how to match receipt amounts for this rule:")]
        [Guide("`AnyAmount` - Matches receipts of any value")]
        [Guide("`Exactly` - Matches only the exact amount specified")]
        [Guide("`MoreThan` - Matches amounts greater than specified")]
        [Guide("`LessThan` - Matches amounts less than specified")]
        [ProtoMember(21), NoWrap] public AmountType AndAmountIs { get; set; }
        [Guide("Enter the amount value for the matching condition you selected.")]
        [Guide("This field is only required when using `Exactly`, `MoreThan`, or `LessThan` conditions.")]
        [ProtoMember(22), IfEnumNot(nameof(AndAmountIs), (int)AmountType.AnyAmount), AppendCurrency(nameof(IfBankAccountIs)), EmptyLabel] public decimal AndAmountIsAmount { get; set; }
        [Guide("Enter keywords that must appear in the transaction description for this rule to match.")]
        [Guide("Add multiple conditions to create precise matching rules - all conditions must be met.")]
        [Guide("Use specific keywords like customer names, invoice numbers, or payment references for accurate matching.")]
        [ProtoMember(18), TableColumn] public Condition[] Conditions { get; set; }
        [Fieldset(nameof(Strings._then_allocate_to))]
        [Guide("Select who made this payment:")]
        [Guide("`Customer` - For payments from your customers")]
        [Guide("`Supplier` - For refunds or credits from suppliers")]
        [Guide("`Other` - For payments from other sources not in your contact lists")]
        [ProtoMember(13), NoWrap] public PayerPayeeType PaidBy { get; set; }
        [Guide("Select the specific customer who made this payment.")]
        [Guide("This will automatically allocate the receipt to the customer's account.")]
        [ProtoMember(14), NoWrap, EmptyLabel, IfEnum(nameof(PaidBy), (int)PayerPayeeType.Customer), Autocomplete(typeof(ManagerServer.Model.Customer))] public Guid? Customer { get; set; }
        [Guide("Select the supplier who issued this refund or credit.")]
        [Guide("This might be used for supplier refunds, rebates, or returned goods credits.")]
        [ProtoMember(15), NoWrap, EmptyLabel, IfEnum(nameof(PaidBy), (int)PayerPayeeType.Supplier), Autocomplete(typeof(ManagerServer.Model.Supplier))] public Guid? Supplier { get; set; }
        [Guide("Enter the name of the payer when they are not in your customer or supplier lists.")]
        [Guide("Use this for one-time receipts or payments from parties you don't regularly transact with.")]
        [ProtoMember(16), EmptyLabel, IfEnum(nameof(PaidBy), (int)PayerPayeeType.Other)] public string OtherContact { get; set; }
        [Guide("Configure how matched receipts will be categorized in your accounts.")]
        [Guide("You can allocate the entire receipt to one account or split it across multiple accounts.")]
        [Guide("Use the `AddLine` button to split receipts that contain multiple income types.")]
        [ProtoMember(17)] public Line[] Lines { get; set; }
        [Guide("Enable the `Description` column to add detailed descriptions for each line item.")]
        [Guide("Useful for providing additional context about different income components in split receipts.")]
        [ProtoMember(20), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool DescriptionColumn { get; set; }
        [Guide("Enable the `Qty` column to record quantities for items sold or services provided.")]
        [Guide("Essential when the receipt involves inventory sales or billable services with quantity tracking.")]
        [ProtoMember(19), Label(nameof(Strings.Column), nameof(Strings.Qty))] public bool QuantityColumn { get; set; }

        [ProtoContract]
        public sealed class Line
        {
            [Guide("Select an inventory or non-inventory item if this receipt relates to a specific product or service sold.")]
            [Guide("The associated income account will be automatically selected when you choose an item.")]
            [ProtoMember(21), Autocomplete(typeof(ISaleItem)), Short] public Guid? Item { get; set; }
            [Guide("Select the income or asset account where this receipt should be recorded.")]
            [Guide("Choose the appropriate account based on the source and nature of the income.")]
            [ProtoMember(1), Autocomplete(typeof(IReceiptOrPaymentAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(ISaleItem.SaleItemAccount))] public Guid? Account { get; set; }
            [ProtoMember(38), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsInterAccountTransfers)), Autocomplete(typeof(BankOrCashAccount)), Prepend(nameof(Strings.BankOrCashAccount))] public Guid? InterAccountTransferAccount { get; set; }
            [ProtoMember(2), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsReceivable)), Substitute(nameof(Customer)), Autocomplete(typeof(Customer), Filter = nameof(Account)), Prepend(nameof(Strings.Customer))] public Guid? AccountsReceivableCustomer { get; set; }
            [ProtoMember(3), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), Autocomplete(typeof(Customer)), Prepend(nameof(Strings.Customer))] public Guid? BillableExpenseCustomer { get; set; }
            [ProtoMember(4), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsPayable)), Substitute(nameof(Supplier)), Autocomplete(typeof(Supplier), Filter = nameof(Account)), Prepend(nameof(Strings.Supplier))] public Guid? AccountsPayableSupplier { get; set; }
            [ProtoMember(19), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsWithholdingTaxPayablePayable)), Autocomplete(typeof(Supplier)), Prepend(nameof(Strings.Supplier))] public Guid? WithholdingTaxPayableSupplier { get; set; }
            [ProtoMember(5), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), Autocomplete(typeof(CapitalAccount), Filter = nameof(Account)), Prepend(nameof(Strings.CapitalAccount))] public Guid? CapitalAccount { get; set; }
            [ProtoMember(6), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), IfNotNull(nameof(CapitalAccount)), Autocomplete(typeof(SubAccount)), Prepend(nameof(Strings.SubAccount))] public Guid? SubAccount { get; set; }
            [ProtoMember(7), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsEmployeeClearingAccount)), Autocomplete(typeof(Employee), Filter = nameof(Account)), Prepend(nameof(Strings.Employee))] public Guid? Employee { get; set; }
            [ProtoMember(10), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForSpecialAccounts)), Autocomplete(typeof(SpecialAccount), Filter = nameof(Account)), Prepend(nameof(Strings.SpecialAccount))] public Guid? SpecialAccount { get; set; }
            [ProtoMember(11), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForFixedAssets)), Autocomplete(typeof(FixedAsset), Filter = nameof(Account)), Prepend(nameof(Strings.FixedAsset))] public Guid? FixedAsset { get; set; }
            [ProtoMember(12), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForIntangibleAssets)), Autocomplete(typeof(IntangibleAsset), Filter = nameof(Account)), Prepend(nameof(Strings.IntangibleAsset))] public Guid? IntangibleAsset { get; set; }
            [ProtoMember(13), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.HasExpenseClaimPayers)), Autocomplete(typeof(ExpenseClaimsPayer)), Prepend(nameof(Strings.ExpenseClaimsPayer))] public Guid? ExpenseClaimsPayer { get; set; }
            [ProtoMember(26), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForInvestments)), Autocomplete(typeof(Investment), Filter = nameof(Account)), Prepend(nameof(Strings.Investment))] public Guid? Investment { get; set; }
            [Guide("Enter a description for this line item to provide context about this portion of the receipt.")]
            [Guide("Helpful for identifying specific income sources when receipts are split across multiple categories.")]
            [Guide("This field is only visible when the `Description` column option is enabled.")]
            [ProtoMember(22), IfTrue(nameof(DescriptionColumn)), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [Guide("Enter the quantity of items sold or services provided for this line.")]
            [Guide("The unit of measurement is based on the selected item's configuration.")]
            [Guide("This field is only visible when the `Qty` column option is enabled.")]
            [ProtoMember(20), AppendValue(nameof(Item), nameof(ManagerServer.Model.InventoryItem.UnitName)), Short, IfTrue(nameof(QuantityColumn))] public decimal? Qty { get; set; }
            [Guide("Choose how to allocate amounts when splitting receipts:")]
            [Guide("`ExactAmount` - Specify a fixed amount for this line")]
            [Guide("`Percentage` - Allocate a percentage of the total receipt")]
            [Guide("When mixing methods, percentages apply to the balance after exact amounts.")]
            [ProtoMember(16), IfMultiple(nameof(Lines))] public DiscountType Amount { get; set; }
            [ProtoMember(17), IfEnum(nameof(Amount), (int)DiscountType.ExactAmount), IfMultiple(nameof(Lines)), Sum, AppendCurrency(nameof(IfBankAccountIs))] public decimal ExactAmount { get; set; }
            [ProtoMember(18), IfEnum(nameof(Amount), (int)DiscountType.Percentage), Append("%"), IfMultiple(nameof(Lines)), Sum] public decimal Percentage { get; set; }
            [Guide("Select the tax code that applies to this line item for accurate tax reporting.")]
            [Guide("The tax code determines the rate and how this income appears in tax reports.")]
            [Guide("This field only appears if tax codes are enabled in settings.")]
            [ProtoMember(14), Autocomplete(typeof(TaxCode)), IfTrue(nameof(Account), nameof(NamedObject.TaxCodeEnabled)), Short] public Guid? TaxCode { get; set; }
            [Guide("Assign this line to a division for tracking income by business segment or location.")]
            [Guide("Helps analyze profitability across different parts of your organization.")]
            [Guide("This field only appears if divisions are enabled in settings.")]
            [ProtoMember(15), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(NamedObject.DivisionEnabled)), Short] public Guid? Division { get; set; }
        }

        [ProtoMember(2)] public string Obsolete_AndDescriptionContains { get; set; }

        public int GetRuleLength()
        {
            if (Conditions == null) return 0;
            return Conditions.Where(x => x?.AndDescriptionContains != null).Sum(x => x.AndDescriptionContains.Length);
        }

        public bool IsMatch(Guid? bankAccount, string description, decimal amount)
        {
            if (Conditions == null) return false;
            if (Conditions.All(x => string.IsNullOrWhiteSpace(x?.AndDescriptionContains))) return false;
            if (IfBankAccountIs.HasValue && IfBankAccountIs.Value != bankAccount) return false;
            if (AndAmountIs == AmountType.Exactly && AndAmountIsAmount != amount) return false;
            if (AndAmountIs == AmountType.LessThan && AndAmountIsAmount <= amount) return false;
            if (AndAmountIs == AmountType.MoreThan && AndAmountIsAmount >= amount) return false;
            foreach (var e in Conditions)
            {
                if (string.IsNullOrWhiteSpace(e?.AndDescriptionContains)) continue;
                if (!Search(description, e.AndDescriptionContains)) return false;
            }

            return true;
        }

        private static bool Search(string value, string term)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (string.IsNullOrWhiteSpace(term)) return false;
            return (IndexOf2(value, term) != -1);
        }

        private static int IndexOf2(string value, string term)
        {
            if (value == null) return -1;
            return value.IndexOf(term, StringComparison.OrdinalIgnoreCase);
        }

        [ProtoContract]
        public sealed class Condition
        {
            [ProtoMember(1)] public string AndDescriptionContains { get; set; }

            public override string ToString()
            {
                return AndDescriptionContains;
            }
        }
    }
}
