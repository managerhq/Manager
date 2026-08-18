using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Obsolete.Obsolete32;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("274fc6d0-2eac-43d0-8286-79c856e644aa")]
    [Currency(nameof(Supplier))]
    public sealed class DebitNote : Transaction, IHasAutomaticReference, IComparable<DebitNote>, ICustomFields, IForeignCurrencyTransaction, ICode, IHasCustomTheme
    {
        [Guide("Enter the date when this debit note is issued to the supplier.")]
        [Guide("This date determines when the debit reduces what you owe the supplier and appears in your accounts payable.")]
        [ProtoMember(1), NoWrap] public DateTime IssueDate { get; set; }
        [Guide("Enter a reference number for this debit note. This helps track and identify the debit note in supplier communications.")]
        [Guide("You can use automatic numbering or enter your own reference, such as the supplier's credit memo number.")]
        [ProtoMember(2)] public string Reference { get; set; }
        [Guide("Select the supplier to whom this debit note is issued. This determines the currency and affects your payables.")]
        [Guide("The debit note will reduce what you owe this supplier and can be applied against their outstanding invoices.")]
        [ProtoMember(3), Autocomplete(typeof(Supplier)), OnChangeSetNull(nameof(PurchaseInvoice)), NoWrap] public Guid? Supplier { get; set; }
        [Guide("Optionally, link this debit note to a specific purchase invoice. This helps track which invoice is being adjusted and can automatically apply the debit.")]
        [Guide("If linked to an invoice, the debit will automatically reduce that specific invoice's balance.")]
        [ProtoMember(6), Autocomplete(typeof(PurchaseInvoice), Filter = nameof(Supplier)), Placeholder(nameof(Strings.Automatic))] public Guid? PurchaseInvoice { get; set; }
        [Guide("If the supplier uses a foreign currency, enter the exchange rate for converting to your base currency.")]
        [Guide("The exchange rate should match the rate used when recording the original purchase to avoid exchange differences.")]
        [ProtoMember(22), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(Supplier), nameof(Model.Supplier.Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : getCurrencyCode()) }} = "), Append("{{ (ExchangeRateIsInverse ? getCurrencyCode() : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(23), IfNotNull(nameof(Supplier), nameof(Model.Supplier.Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }
        [Guide("Optionally, add a description or reason for this debit note, such as 'Goods returned' or 'Price adjustment agreed'.")]
        [Guide("This description helps document why the debit was issued and appears in reports for audit trail purposes.")]
        [ProtoMember(7), Placeholder(nameof(Strings.Optional)), Long] public string Description { get; set; }
        [Guide("Enter the line items being debited. Each line represents a product or service with the amount to be deducted from what you owe.")]
        [Guide("Line items should match what was originally purchased, using the same accounts and tax codes for accuracy.")]
        [ProtoMember(16)] public Line[] Lines { get; set; }
        [Guide("Check this box if the amounts you enter already include tax. Leave unchecked if tax should be calculated separately.")]
        [Guide("This setting should match how the original purchase invoice was recorded to ensure correct tax calculations.")]
        [ProtoMember(5)] public bool AmountsIncludeTax { get; set; }
        [Guide("Check this box to display line numbers on the debit note. This helps reference specific items with suppliers.")]
        [Guide("Line numbers are useful when discussing specific items with suppliers or matching to their credit memo.")]
        [ProtoMember(20), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [Guide("Check this box to show a description column for each line item, allowing detailed explanations for each debit.")]
        [Guide("Descriptions help explain why each item is being debited, especially for quality issues or agreed adjustments.")]
        [ProtoMember(15), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }
        [Guide("Check this box to enable a discount column if the debit relates to discount adjustments.")]
        [Guide("Use this when the debit note is for retrospective discounts or pricing adjustments agreed with the supplier.")]
        [ProtoMember(10), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(11), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [ProtoMember(12), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(13), Autocomplete(typeof(CustomTheme)), IfTrue(nameof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(14), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(17), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [Guide("Check this box if this debit note also represents a return of physical goods to the supplier, updating inventory levels.")]
        [Guide("When checked, the debit note will also reduce your inventory quantities for the returned items.")]
        [ProtoMember(24)] public bool AlsoActsAsGoodsReceipt { get; set; }
        [Guide("Select the inventory location from which goods are being returned to the supplier.")]
        [Guide("This determines which warehouse or location's inventory will be reduced for the returned items.")]
        [ProtoMember(9), IfTrue(nameof(AlsoActsAsGoodsReceipt)), NoLabel, Prepend(nameof(Strings.InventoryLocation)), Autocomplete(typeof(ManagerServer.Model.CustomInventoryLocation))] public Guid? PurchaseInventoryLocation { get; set; }
        [ProtoMember(18), Label(nameof(Strings.Footers))] public bool HasDebitNoteFooters { get; set; }
        [ProtoMember(19), Autocomplete(typeof(ManagerServer.Model.DebitNoteFooter)), NoLabel, IfTrue(nameof(HasDebitNoteFooters))] public Guid[] DebitNoteFooters { get; set; }
        [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(21)] public CustomFields CustomFields2 { get; set; }

        public override string GetReference() => Reference;

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        DateTime IForeignCurrencyTransaction.Date => IssueDate;
        Guid? IForeignCurrencyTransaction.Currency => Supplier;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }
        string ICode.Code => Reference;

        public override bool GetHasLineDescription() => HasLineDescription;
        public override bool HasLineQty() => true;
        public override bool HasLineUnitPrice() => true;
        public override DiscountType? GetLineDicountType() => Discount ? DiscountType : null;

        [CustomFields]
        [ProtoContract]
        [Guid("8c867cbb-cafd-4b34-b1af-f4187247bb82")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }
            [ProtoMember(1), Autocomplete(typeof(IPurchaseItem)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(PurchaseUnitPrice)), OnChangeSetDefault(nameof(TaxCode)), OnChangeSetDefault(nameof(Division)), Short] public Guid? Item { get; set; }
            [ProtoMember(2), Autocomplete(typeof(IPurchaseInvoiceAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(IPurchaseItem.PurchaseItemAccount)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Account { get; set; }
            [ProtoMember(5), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), Autocomplete(typeof(Customer)), Prepend(nameof(Strings.Customer))] public Guid? BillableExpenseCustomer { get; set; }
            [ProtoMember(6), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), IfNotNull(nameof(BillableExpenseCustomer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(BillableExpenseCustomer)), Prepend(nameof(Strings.Invoice)), Placeholder(nameof(Strings.Uninvoiced)), Short] public Guid? BillableExpenseSalesInvoice { get; set; }
            [ProtoMember(9), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), Autocomplete(typeof(CapitalAccount), Filter = nameof(Account)), Prepend(nameof(Strings.CapitalAccount))] public Guid? CapitalAccount { get; set; }
            [ProtoMember(10), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), IfNotNull(nameof(CapitalAccount)), Autocomplete(typeof(SubAccount)), Prepend(nameof(Strings.SubAccount))] public Guid? SubAccount { get; set; }
            [ProtoMember(11), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsEmployeeClearingAccount)), Autocomplete(typeof(Employee), Filter = nameof(Account)), Prepend(nameof(Strings.Employee))] public Guid? Employee { get; set; }
            [ProtoMember(13), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForSpecialAccounts)), Autocomplete(typeof(SpecialAccount), Filter = nameof(Account)), Prepend(nameof(Strings.SpecialAccount))] public Guid? SpecialAccount { get; set; }
            [ProtoMember(14), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForFixedAssets)), Autocomplete(typeof(FixedAsset), Filter = nameof(Account)), Prepend(nameof(Strings.FixedAsset))] public Guid? FixedAsset { get; set; }
            [ProtoMember(15), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForIntangibleAssets)), Autocomplete(typeof(IntangibleAsset), Filter = nameof(Account)), Prepend(nameof(Strings.IntangibleAsset))] public Guid? IntangibleAsset { get; set; }
            [ProtoMember(17), IfTrue(nameof(HasLineDescription)), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(25)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(27)] public CustomFields CustomFields2 { get; set; }
            [ProtoMember(18), AppendValue(nameof(Item), nameof(ManagerServer.Model.InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }
            [ProtoMember(19), NoPlaceholder, AppendCurrency(nameof(Supplier)), Label(nameof(Strings.UnitPrice))] public decimal PurchaseUnitPrice { get; set; }
            [ProtoMember(20), IfDifferentCurrency, NoPlaceholder] public decimal CurrencyAmount { get; set; }
            [ProtoMember(23), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(24), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(PurchaseUnitPrice), Times, nameof(Qty), Round, Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(PurchaseOrder.Supplier))] public object TotalBeforeTax { get; }
            [ProtoMember(21), Autocomplete(typeof(TaxCode)), Short] public Guid? TaxCode { get; set; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfFalse(nameof(AmountsIncludeTax))] public object TaxAmount { get; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(Supplier)), IfFalse(nameof(SalesOrder.AmountsIncludeTax))] public object Total { get; }
            [ProtoMember(26), Autocomplete(typeof(Project)), IfTrue(nameof(Account), nameof(NamedObject.ProjectEnabled)), Short] public Guid? Project { get; set; }
            [ProtoMember(22), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(NamedObject.DivisionEnabled)), Short] public Guid? Division { get; set; }

            public override Guid? GetItem() => Item;
            public override Guid? GetAccount() => Account;
            public override Guid? GetBillableExpenseCustomer() => BillableExpenseCustomer;
            public override Guid? GetBillableExpenseSalesInvoice() => BillableExpenseSalesInvoice;
            public override Guid? GetEmployee() => Employee;
            public override Guid? GetFixedAsset() => FixedAsset;
            public override Guid? GetIntangibleAsset() => IntangibleAsset;
            public override Guid? GetCapitalAccount() => CapitalAccount;
            public override Guid? GetSpecialAccount() => SpecialAccount;
            public override Guid? GetSubAccount() => SubAccount;
            protected override decimal? GetUnitPrice() => PurchaseUnitPrice;
            protected override decimal? GetQty() => Qty;
            protected override decimal? GetDiscountPercentage() => DiscountPercentage;
            protected override decimal? GetDiscountAmount() => DiscountAmount;
            protected override string GetLineDescription() => LineDescription;
            public override Guid? GetTaxCode() => TaxCode;
            public override Guid? GetDivision() => Division;
            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
            public override decimal? GetProposedAccountAmount() => CurrencyAmount;
            protected override Guid? GetProject() => Project;
        }

        [ProtoMember(4)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }

        public override string GetDescriptionOrNull()
        {
            if (!string.IsNullOrWhiteSpace(Description)) return Description;
            return null;
        }

        public override string GetName()
        {
            return Reference;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        public override ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var supplier = database.SingleOrDefault<Supplier>(Supplier);
            var purchaseInvoice = database.SingleOrDefault<PurchaseInvoice>(PurchaseInvoice);
            var inventoryLocation = AlsoActsAsGoodsReceipt ? database.SingleOrDefault<CustomInventoryLocation>(PurchaseInventoryLocation) : null;

            var baseCurrency = database.Single<BaseCurrency>();
            var transactionCurrency = database.SingleOrDefault<ForeignCurrency>(supplier?.Currency) as Currency ?? baseCurrency;

            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            if (Lines != null)
            {
                for (int i = 0; i < Lines.Length; i++)
                {
                    list.AddRange(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.From(
                        database: database,
                        date: IssueDate,
                        transaction: this,
                        transactionCurrency: transactionCurrency,
                        transactionLine: Lines[i],
                        exchangeRate: ExchangeRate,
                        exchangeRateIsInverse: ExchangeRateIsInverse,
                        amountsIncludeTax: AmountsIncludeTax,
                        inventoryLocation: inventoryLocation,
                        reverseSign: true,
                        supplier: supplier,
                        lineNumber: i
                    ));
                }
            }

            var contraTransactions = list.ToArray();

            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: IssueDate,
                generalLedgerAccount: database.Single<BalanceSheetAccountsPayableAccount>(),
                supplier: supplier,
                purchaseInvoice: purchaseInvoice,
                baseAmount: list.Sum(x => x.BaseAmount) * -1m,
                transactionAmount: list.Sum(x => x.TransactionAmount) * -1m,
                transactionCurrency: transactionCurrency,
                contraTransactions: contraTransactions,
                isBalancing: true,
                trackingCode: database.SingleOrDefault<Division>(supplier?.Division)
            ));

            return list.ToArray();
        }

        int IComparable<DebitNote>.CompareTo(DebitNote other)
        {
            return (other.IssueDate, other.Reference).CompareTo((IssueDate, Reference));
        }
    }
}