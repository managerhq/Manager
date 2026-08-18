using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Currency(nameof(Customer))]
    [Guid("8f7510d9-a92d-4b4c-9421-fd745e198b3c")]
    public sealed class WithholdingTaxReceipt : Transaction, IComparable<WithholdingTaxReceipt>, ICustomFields, IForeignCurrencyTransaction
    {
        [Guide("Enter the date of the withholding tax receipt. This is typically when the customer withheld the tax.")]
        [ProtoMember(2)] public DateTime Date { get; set; }
        [Guide("Select the customer who withheld tax from their payment. This links the receipt to the customer's account.")]
        [ProtoMember(1), Autocomplete(typeof(Customer))] public Guid? Customer { get; set; }
        [Guide("Enter the amount of tax withheld by the customer. This creates a receivable that can offset your tax liabilities.")]
        [ProtoMember(3), AppendCurrency(nameof(Customer)), NoWrap] public decimal Amount { get; set; }
        [Guide("If the customer uses a foreign currency, enter the exchange rate to convert to your base currency.")]
        [ProtoMember(7), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(Customer), nameof(Model.Customer.Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : getCurrencyCode()) }} = "), Append("{{ (ExchangeRateIsInverse ? getCurrencyCode() : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(8), IfNotNull(nameof(Customer), nameof(Model.Customer.Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }
        [Guide("Optionally, add a description for this withholding tax receipt, such as invoice references or tax certificate numbers.")]
        [ProtoMember(4), Long] public string Description { get; set; }
        [ProtoMember(5)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(6)] public CustomFields CustomFields2 { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        DateTime IForeignCurrencyTransaction.Date => Date;
        Guid? IForeignCurrencyTransaction.Currency => Customer;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }

        public override string GetReference() => null;

        public override string GetDescriptionOrNull()
        {
            if (!string.IsNullOrWhiteSpace(Description)) return Description;
            return null;
        }

        public override string GetName()
        {
            return null;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        public override ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var customer = database.SingleOrDefault<Customer>(Customer);
            if (customer == null) return [];

            var baseCurrency = database.Single<BaseCurrency>();

            var transactionCurrency = database.SingleOrDefault<ForeignCurrency>(customer.Currency) as Currency ?? baseCurrency;

            var transactionAmount = transactionCurrency.Round(Amount);
            var baseAmount = baseCurrency.GetBaseAmount(transactionAmount, ExchangeRate, ExchangeRateIsInverse, transactionCurrency);

            return new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[2]
            {
                new Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    date: Date,
                    transactionAmount: transactionAmount*-1m,
                    transactionCurrency: transactionCurrency,
                    baseAmount: baseAmount *-1m,
                    generalLedgerAccount: database.Single<BalanceSheetWithholdingTaxReceivableAccount>(),
                    exchangeRate: ExchangeRate,
                    isExchangeRateInverse: ExchangeRateIsInverse,
                    customer: customer
                ),
                new Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    isBalancing: true,
                    date: Date,
                    transactionAmount: transactionAmount,
                    transactionCurrency: transactionCurrency,
                    baseAmount: baseAmount,
                    generalLedgerAccount: database.Single<BalanceSheetWithholdingTaxAccount>(),
                    exchangeRate: ExchangeRate,
                    isExchangeRateInverse: ExchangeRateIsInverse,
                    customer: customer
                )
            };
        }

        int IComparable<WithholdingTaxReceipt>.CompareTo(WithholdingTaxReceipt other)
        {
            return (other.Date).CompareTo((Date));
        }
    }
}