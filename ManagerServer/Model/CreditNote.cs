using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Obsolete.Obsolete32;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("245e5943-0092-409d-96ae-e2ee10eac75b")]
    [Currency(nameof(Customer))]
    public sealed class CreditNote : Transaction, IHasAutomaticReference, IComparable<CreditNote>, ICustomFields, IForeignCurrencyTransaction, ICode, IHasCustomTheme
    {
        [Guide("Enter the date when this credit note is issued to the customer.")]
        [Guide("This date determines when the credit reduces the customer's account balance and appears in financial reports.")]
        [ProtoMember(1), NoWrap] public DateTime IssueDate { get; set; }
        [Guide("Enter a reference number for this credit note. This helps track and identify the credit note in communications.")]
        [Guide("You can use automatic numbering by leaving this field blank, or enter your own reference number for better organization.")]
        [ProtoMember(2)] public string Reference { get; set; }
        [Guide("Select the customer who will receive this credit note. Their billing address will automatically populate.")]
        [Guide("The credit note will reduce what this customer owes you and can be applied against their outstanding invoices.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(ManagerServer.Model.Customer)), OnChangeSetNull(nameof(SalesInvoice)), OnChangeSetDefault(nameof(BillingAddress))] public Guid? Customer { get; set; }
        [Guide("Optionally, link this credit note to a specific sales invoice. This helps track which invoice is being credited and can automatically apply the credit.")]
        [Guide("If linked to an invoice, the credit will be automatically allocated against that specific invoice's balance.")]
        [ProtoMember(8), Autocomplete(typeof(ManagerServer.Model.SalesInvoice), Filter = nameof(Customer)), Placeholder(nameof(Strings.Automatic))] public Guid? SalesInvoice { get; set; }
        [Guide("Enter the customer's billing address. This is automatically filled from the customer record but can be modified.")]
        [Guide("The address appears on the printed credit note and should match where you send correspondence to this customer.")]
        [ProtoMember(4), Textarea] public string BillingAddress { get; set; }
        [Guide("If the customer uses a foreign currency, enter the exchange rate for converting to your base currency.")]
        [Guide("The exchange rate determines how the foreign currency amount converts to your base currency for accounting purposes.")]
        [ProtoMember(30), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(Customer), nameof(Model.Customer.Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : getCurrencyCode()) }} = "), Append("{{ (ExchangeRateIsInverse ? getCurrencyCode() : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(31), IfNotNull(nameof(Customer), nameof(Model.Customer.Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }
        [Guide("Optionally, add a description or reason for this credit note, such as 'Product return' or 'Price adjustment'.")]
        [Guide("This description helps document why the credit was issued and appears on reports but not on the credit note itself.")]
        [ProtoMember(9), Typeahead, Long] public string Description { get; set; }
        [Guide("Enter the line items being credited. Each line represents a product or service with the amount to be credited.")]
        [Guide("Line items should match what was originally sold, including the same accounts, tax codes, and tracking categories.")]
        [ProtoMember(22)] public Line[] Lines { get; set; }
        [Guide("Check this box to display line numbers on the credit note. This helps reference specific items.")]
        [Guide("Line numbers make it easier to discuss specific items with customers and match them to the original invoice.")]
        [ProtoMember(26), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [Guide("Check this box to show a description column for each line item, allowing detailed explanations for each credit.")]
        [Guide("Descriptions help explain why each item is being credited, which is especially useful for partial credits or adjustments.")]
        [ProtoMember(21), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }
        [ProtoMember(15), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(16), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [Guide("Check this box if the amounts you enter already include tax. Leave unchecked if tax should be calculated on top.")]
        [Guide("This setting should match how the original invoice was created to ensure accurate tax calculations.")]
        [ProtoMember(6)] public bool AmountsIncludeTax { get; set; }
        [Guide("Check this box if withholding tax applies to this credit note. This adjusts the withholding tax previously recorded.")]
        [Guide("Withholding tax credits reverse the withholding tax that was recorded on the original invoice.")]
        [ProtoMember(13), IfWithholdingTaxReceivable] public bool WithholdingTax { get; set; }
        [ProtoMember(10), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(11), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel, IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount), AppendCurrency(nameof(Customer))] public decimal WithholdingTaxAmount { get; set; }
        [ProtoMember(12), IfTrue(nameof(WithholdingTax)), NoLabel, IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate), Append("%")] public decimal WithholdingTaxRate { get; set; }
        [ProtoMember(17), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(18), IfTrue(nameof(CustomTheme)), NoLabel, Autocomplete(typeof(CustomTheme))] public Guid? CustomThemeId { get; set; }
        [ProtoMember(20), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(23), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(28), Label(nameof(Strings.CustomTitle))] public bool HasCreditNoteCustomTitle { get; set; }
        [ProtoMember(29), NoLabel, IfTrue(nameof(HasCreditNoteCustomTitle)), Placeholder(nameof(Strings.CreditNote))] public string CreditNoteCustomTitle { get; set; }
        [Guide("Check this box if this credit note also represents a return of physical goods, updating inventory levels.")]
        [Guide("When checked, the credit note will also function as a goods receipt, adding the returned items back into inventory.")]
        [ProtoMember(34)] public bool AlsoActsAsDeliveryNote { get; set; }
        [Guide("Select the inventory location where returned goods will be received back into stock.")]
        [Guide("This determines which warehouse or location receives the returned inventory items for future sale.")]
        [ProtoMember(14), IfTrue(nameof(AlsoActsAsDeliveryNote)), NoLabel, Prepend(nameof(Strings.InventoryLocation)), Autocomplete(typeof(CustomInventoryLocation))] public Guid? SalesInventoryLocation { get; set; }
        [ProtoMember(24), Label(nameof(Strings.Footers))] public bool HasCreditNoteFooters { get; set; }
        [ProtoMember(25), Autocomplete(typeof(ManagerServer.Model.CreditNoteFooter)), NoLabel, IfTrue(nameof(HasCreditNoteFooters))] public Guid[] CreditNoteFooters { get; set; }
        [ProtoMember(7)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(27)] public CustomFields CustomFields2 { get; set; }
        [ProtoMember(19), Hidden] public CreditNoteType Type { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        DateTime IForeignCurrencyTransaction.Date => IssueDate;
        Guid? IForeignCurrencyTransaction.Currency => Customer;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }
        string ICode.Code => Reference;

        public override string GetReference() => Reference;

        public override string TransactionTitle => HasCreditNoteCustomTitle ? CreditNoteCustomTitle : null;

        public override bool GetHasLineDescription() => HasLineDescription;
        public override bool HasLineQty() => true;
        public override bool HasLineUnitPrice() => true;
        public override DiscountType? GetLineDicountType() => Discount ? DiscountType : null;

        [CustomFields]
        [ProtoContract]
        [Guid("4f819a77-dabc-4e28-8c99-172a1f595f3f")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }
            [ProtoMember(1), Autocomplete(typeof(ISaleItem)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(SalesUnitPrice)), OnChangeSetDefault(nameof(TaxCode)), OnChangeSetDefault(nameof(Division)), Short] public Guid? Item { get; set; }
            [ProtoMember(2), Autocomplete(typeof(ISalesInvoiceAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(ISaleItem.SaleItemAccount)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Account { get; set; }
            [ProtoMember(9), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), Autocomplete(typeof(CapitalAccount), Filter = nameof(Account)), Prepend(nameof(Strings.CapitalAccount))] public Guid? CapitalAccount { get; set; }
            [ProtoMember(10), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), IfNotNull(nameof(CapitalAccount)), Autocomplete(typeof(SubAccount)), Prepend(nameof(Strings.SubAccount))] public Guid? SubAccount { get; set; }
            [ProtoMember(13), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForSpecialAccounts)), Autocomplete(typeof(SpecialAccount), Filter = nameof(Account)), Prepend(nameof(Strings.SpecialAccount))] public Guid? SpecialAccount { get; set; }
            [ProtoMember(14), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForFixedAssets)), Autocomplete(typeof(FixedAsset), Filter = nameof(Account)), Prepend(nameof(Strings.FixedAsset))] public Guid? FixedAsset { get; set; }
            [ProtoMember(15), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForIntangibleAssets)), Autocomplete(typeof(IntangibleAsset), Filter = nameof(Account)), Prepend(nameof(Strings.IntangibleAsset))] public Guid? IntangibleAsset { get; set; }
            [ProtoMember(17), IfTrue(nameof(HasLineDescription)), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(25)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(27)] public CustomFields CustomFields2 { get; set; }
            [ProtoMember(18), AppendValue(nameof(Item), nameof(ManagerServer.Model.InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }
            [ProtoMember(19), NoPlaceholder, AppendCurrency(nameof(Model.Customer)), Label(nameof(Strings.UnitPrice))] public decimal SalesUnitPrice { get; set; }
            [ProtoMember(20), IfDifferentCurrency, NoPlaceholder] public decimal CurrencyAmount { get; set; }
            [ProtoMember(23), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(24), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(SalesUnitPrice), Times, nameof(Qty), Round, Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(Model.Customer))] public object TotalBeforeTax { get; }
            [ProtoMember(21), Autocomplete(typeof(TaxCode)), Short] public Guid? TaxCode { get; set; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfFalse(nameof(AmountsIncludeTax))] public object TaxAmount { get; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(Model.Customer)), IfFalse(nameof(SalesOrder.AmountsIncludeTax))] public object Total { get; }
            //[ProtoMember(28), IfTrue(nameof(Item), nameof(ISaleItem.HasCostOfGoodsSold))] public AutomaticManual CostOfGoodsSold;
            //[ProtoMember(29), IfEnum(nameof(CostOfGoodsSold), 1), AppendBaseCurrency, EmptyLabel] public decimal CostOfGoodsSoldAmount;
            [ProtoMember(26), Autocomplete(typeof(Project)), IfTrue(nameof(Account), nameof(NamedObject.ProjectEnabled)), Short] public Guid? Project { get; set; }
            [ProtoMember(22), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(NamedObject.DivisionEnabled)), Short] public Guid? Division { get; set; }

            public override Guid? GetItem() => Item;
            public override Guid? GetAccount() => Account;
            public override Guid? GetFixedAsset() => FixedAsset;
            public override Guid? GetIntangibleAsset() => IntangibleAsset;
            public override Guid? GetCapitalAccount() => CapitalAccount;
            public override Guid? GetSpecialAccount() => SpecialAccount;
            public override Guid? GetSubAccount() => SubAccount;
            protected override decimal? GetUnitPrice() => SalesUnitPrice;
            protected override decimal? GetDiscountPercentage() => DiscountPercentage;
            protected override decimal? GetDiscountAmount() => DiscountAmount;
            protected override decimal? GetQty() => Qty;
            protected override string GetLineDescription() => LineDescription;
            public override Guid? GetTaxCode() => TaxCode;
            public override Guid? GetDivision() => Division;
            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
            public override decimal? GetProposedAccountAmount() => CurrencyAmount;
            protected override Guid? GetProject() => Project;
            //public override decimal? GetProposedCostOfGoodsSoldAmount() => (CostOfGoodsSold == AutomaticManual.Manual ? CostOfGoodsSoldAmount : null);
        }
        
        [ProtoMember(5)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }
        [ProtoMember(32)] public bool Obsolete_HasRelay { get; set; }
        [ProtoMember(33)] public string Obsolete_Relay { get; set; }


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
            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            var customer = database.SingleOrDefault<Customer>(Customer);
            var salesInvoice = database.SingleOrDefault<SalesInvoice>(SalesInvoice);

            var baseCurrency = database.Single<BaseCurrency>();
            var transactionCurrency = database.SingleOrDefault<ForeignCurrency>(customer?.Currency) as Currency ?? baseCurrency;
            var inventoryLocation = AlsoActsAsDeliveryNote ? database.SingleOrDefault<CustomInventoryLocation>(SalesInventoryLocation) : null;

            if (Lines != null)
            {
                foreach (var line in Lines)
                {
                    list.AddRange(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.From(
                        database: database,
                        date: IssueDate,
                        transaction: this,
                        transactionCurrency: transactionCurrency,
                        transactionLine: line,
                        exchangeRate: ExchangeRate,
                        exchangeRateIsInverse: ExchangeRateIsInverse,
                        customer: customer,
                        inventoryLocation: inventoryLocation,
                        amountsIncludeTax: AmountsIncludeTax
                    ));
                }
            }

            var total = list.Sum(x => x.TransactionAmount);

            if (WithholdingTax)
            {
                var withholdingTax = database.Single<ManagerServer.Model.WithholdingTax>();
                if (withholdingTax.WithholdingTaxReceivable)
                {
                    if (WithholdingTaxType == WithholdingTaxType.Rate && WithholdingTaxRate > 0m && WithholdingTaxRate <= 100m)
                    {
                        var withholdingTaxAmount = transactionCurrency.Round(total / 100m * WithholdingTaxRate);
                        if (withholdingTaxAmount != 0m)
                        {
                            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                transaction: this,
                                date: IssueDate,
                                generalLedgerAccount: database.Single<BalanceSheetWithholdingTaxReceivableAccount>(),
                                customer: customer,
                                salesInvoice: salesInvoice,
                                transactionAmount: withholdingTaxAmount * -1m,
                                exchangeRate: ExchangeRate,
                                isExchangeRateInverse: ExchangeRateIsInverse,
                                transactionCurrency: transactionCurrency
                            ));
                        }
                    }
                    if (WithholdingTaxType == WithholdingTaxType.Amount && WithholdingTaxAmount > 0m)
                    {
                        var withholdingTaxAmount = transactionCurrency.Round(WithholdingTaxAmount);

                        list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                transaction: this,
                                date: IssueDate,
                                generalLedgerAccount: database.Single<BalanceSheetWithholdingTaxReceivableAccount>(),
                                customer: customer,
                                salesInvoice: salesInvoice,
                                transactionAmount: withholdingTaxAmount * -1m,
                                exchangeRate: ExchangeRate,
                                isExchangeRateInverse: ExchangeRateIsInverse,
                                transactionCurrency: transactionCurrency
                            ));
                    }
                }
            }

            var contraTransactions = list.ToArray();

            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: IssueDate,
                generalLedgerAccount: database.Single<BalanceSheetAccountsReceivableAccount>(),
                customer: customer,
                salesInvoice: salesInvoice,
                baseAmount: list.Sum(x => x.BaseAmount) * -1m,
                transactionAmount: list.Sum(x => x.TransactionAmount) * -1m,
                transactionCurrency: transactionCurrency,
                contraTransactions: contraTransactions,
                isBalancing: true,
                trackingCode: database.SingleOrDefault<Division>(customer?.Division)
            ));

            return list.ToArray();
        }

        int IComparable<CreditNote>.CompareTo(CreditNote other)
        {
            return (other.IssueDate, other.Reference).CompareTo((IssueDate, Reference));
        }
    }
}
