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
    [Guid("71ac4a94-6a53-4c0a-990c-e8174ab398d1")]
    public sealed class PaymentRule : Object
    {
        [Guide("Select a specific bank account to apply this payment rule to that account only.")]
        [Guide("If you leave this field empty, the payment rule will match transactions from any bank account.")]
        [ProtoMember(1), Autocomplete(typeof(ManagerServer.Model.BankOrCashAccount)), TableColumn] public Guid? IfBankAccountIs { get; set; }
        [Guide("Select how to match transactions based on their amount.")]
        [Guide("Options include: **Any Amount** (matches all amounts), **Exactly** (matches a specific amount), **More Than** (matches amounts greater than specified), or **Less Than** (matches amounts less than specified).")]
        [ProtoMember(21), NoWrap] public AmountType AndAmountIs { get; set; }
        [ProtoMember(22), IfEnumNot(nameof(AndAmountIs), (int)AmountType.AnyAmount), AppendCurrency(nameof(IfBankAccountIs)), EmptyLabel] public decimal AndAmountIsAmount { get; set; }
        [Guide("Enter text that must appear in the transaction description for this rule to match.")]
        [Guide("To match transactions containing multiple specific terms, click **Add Line** to add additional description criteria.")]
        [Guide("All specified terms must be present in the transaction description for the rule to apply.")]
        [ProtoMember(18), TableColumn, Label(nameof(Strings.AndDescriptionContains))] public Condition[] Conditions { get; set; }
        [Fieldset(nameof(Strings._then_allocate_to))]
        [Guide("Select the type of *payee* this payment should be allocated to.")]
        [ProtoMember(13), NoWrap] public PayerPayeeType Payee { get; set; }
        [ProtoMember(14), NoWrap, EmptyLabel, IfEnum(nameof(Payee), (int)PayerPayeeType.Customer), Autocomplete(typeof(ManagerServer.Model.Customer))] public Guid? Customer { get; set; }
        [ProtoMember(15), NoWrap, EmptyLabel, IfEnum(nameof(Payee), (int)PayerPayeeType.Supplier), Autocomplete(typeof(ManagerServer.Model.Supplier))] public Guid? Supplier { get; set; }
        [ProtoMember(16), EmptyLabel, IfEnum(nameof(Payee), (int)PayerPayeeType.Other)] public string OtherContact { get; set; }
        [Guide("Configure how matched payments will be categorized in your accounts.")]
        [Guide("You can allocate the entire payment to a single account, or split it across multiple accounts using the **Add Line** button.")]
        [Guide("Splitting payments is useful for transactions that include multiple expense categories, such as a credit card payment covering various business expenses.")]
        [Guide("The *Lines* section contains the following columns:")]
        [Fields(typeof(Line))]
        [ProtoMember(17)] public Line[] Lines { get; set; }
        [Guide("Check this option to show the *Description* column in the *Lines* section.")]
        [ProtoMember(20), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool DescriptionColumn { get; set; }
        [Guide("Check this option to show the *Qty* column in the *Lines* section.")]
        [ProtoMember(19), Label(nameof(Strings.Column), nameof(Strings.Qty))] public bool QuantityColumn { get; set; }

        [ProtoContract]
        public sealed class Line
        {
            [Guide("Select an inventory or non-inventory item if this payment relates to a specific product or service.")]
            [Guide("The associated purchase account will be automatically selected when you choose an item.")]
            [ProtoMember(21), Autocomplete(typeof(IPurchaseItem)), OnChangeSetDefault(nameof(Qty)), Short] public Guid? Item { get; set; }
            [Guide("Select the general ledger account where this payment should be recorded.")]
            [Guide("Choose the appropriate expense, asset, or liability account based on the nature of the payment.")]
            [ProtoMember(1), Autocomplete(typeof(IReceiptOrPaymentAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(IPurchaseItem.PurchaseItemAccount))] public Guid? Account { get; set; }
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
            [Guide("Enter a description for this line item to provide additional context about the payment.")]
            [Guide("Descriptions help identify specific expenses when payments are split across multiple categories.")]
            [Guide("This field is only visible when the *Description* column option is enabled.")]
            [ProtoMember(22), IfTrue(nameof(DescriptionColumn)), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [Guide("Enter the quantity purchased if this line item involves inventory or measurable items.")]
            [Guide("The unit of measurement is determined by the selected inventory item's settings.")]
            [Guide("This field is only visible when the *Qty* column option is enabled.")]
            [ProtoMember(20), AppendValue(nameof(Item), nameof(ManagerServer.Model.InventoryItem.UnitName)), Short, IfTrue(nameof(QuantityColumn))] public decimal? Qty { get; set; }
            [Guide("Choose how to allocate amounts when splitting payments across multiple lines:")]
            [Guide("**Exact Amount** - Specify a fixed amount for this line")]
            [Guide("**Percentage** - Allocate a percentage of the total payment")]
            [Guide("When mixing exact amounts and percentages, percentages are calculated on the remaining balance after deducting all exact amounts.")]
            [ProtoMember(16), IfMultiple(nameof(Lines))] public DiscountType Amount { get; set; }
            [ProtoMember(17), IfEnum(nameof(Amount), (int)DiscountType.ExactAmount), IfMultiple(nameof(Lines)), Sum, AppendCurrency(nameof(IfBankAccountIs))] public decimal ExactAmount { get; set; }
            [ProtoMember(18), IfEnum(nameof(Amount), (int)DiscountType.Percentage), Append("%"), IfMultiple(nameof(Lines)), Sum] public decimal Percentage { get; set; }
            [Guide("Select the appropriate tax code for this line item to ensure correct tax calculation and reporting.")]
            [Guide("Tax codes determine the tax rate and how the transaction appears in tax reports.")]
            [Guide("This field only appears if tax codes are enabled in your business settings.")]
            [ProtoMember(14), Autocomplete(typeof(TaxCode)), IfTrue(nameof(Account), nameof(NamedObject.TaxCodeEnabled)), Short] public Guid? TaxCode { get; set; }
            [Guide("Assign this line item to a division for tracking profitability by business segment or location.")]
            [Guide("Divisions help analyze income and expenses for different parts of your business.")]
            [Guide("This field only appears if divisions are enabled in your business settings.")]
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
