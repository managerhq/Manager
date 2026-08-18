using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Currency(nameof(Customer))]
    [Guid("4dada073-022f-464e-bdb3-ff38c83e577f")]
    public sealed class LatePaymentFee : Transaction, IComparable<LatePaymentFee>, IForeignCurrencyTransaction
    {
        [Guide("Enter the date when the late payment fee is charged. This is typically when the fee is calculated or applied.")]
        [ProtoMember(1)] public DateTime Date { get; set; }
        [Guide("Select the customer to charge the late payment fee. This determines the currency and links to their account.")]
        [ProtoMember(2), Autocomplete(typeof(Customer)), OnChangeSetNull(nameof(SalesInvoice))] public Guid? Customer { get; set; }
        [Guide("Select the specific overdue invoice this fee relates to. The fee will be linked to this invoice.")]
        [ProtoMember(3), IfNotNull(nameof(Customer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(Customer))] public Guid? SalesInvoice { get; set; }
        [Guide("Enter the late payment fee amount. This could be a fixed fee or calculated interest on the overdue amount.")]
        [ProtoMember(4), NoWrap, IfNotNull(nameof(Customer)), IfNotNull(nameof(SalesInvoice)), AppendCurrency(nameof(Customer))] public decimal Amount { get; set; }
        [Guide("If the customer uses a foreign currency, enter the exchange rate to convert to your base currency.")]
        [ProtoMember(6), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(Customer), nameof(Model.Customer.Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : getCurrencyCode()) }} = "), Append("{{ (ExchangeRateIsInverse ? getCurrencyCode() : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(7), IfNotNull(nameof(Customer), nameof(Model.Customer.Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }

        [ProtoMember(5)] public Guid? Obsolete_Division { get; set; }

        DateTime IForeignCurrencyTransaction.Date => Date;
        Guid? IForeignCurrencyTransaction.Currency => Customer;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }

        public override string GetDescriptionOrNull()
        {
            return null;
        }

        public override string GetReference() => null;

        public override string GetName()
        {
            return null;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        public override Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            if (!SalesInvoice.HasValue) return [];
            var salesInvoice = database.SingleOrDefault<SalesInvoice>(SalesInvoice.Value);
            if (salesInvoice == null) return [];
            if (!salesInvoice.Customer.HasValue) return [];
            var customer = database.SingleOrDefault<Customer>(salesInvoice.Customer.Value);
            if (customer == null) return [];

            var baseCurrency = database.Single<BaseCurrency>();
            var transactionCurrency = database.SingleOrDefault<ForeignCurrency>(customer.Currency) as Currency ?? baseCurrency;

            var transactionAmount = transactionCurrency.Round(Amount);
            var baseAmount = baseCurrency.GetBaseAmount(transactionAmount, ExchangeRate, ExchangeRateIsInverse, transactionCurrency);

            var list = new List<Query.GeneralLedger.GeneralLedgerTransaction>();
            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: Date,
                generalLedgerAccount: database.Single<BalanceSheetAccountsReceivableAccount>(),
                customer: customer,
                salesInvoice: salesInvoice,
                transactionAmount: transactionAmount,
                baseAmount: baseAmount,
                exchangeRate: ExchangeRate,
                isExchangeRateInverse: ExchangeRateIsInverse,
                transactionCurrency: transactionCurrency
            ));

            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: Date,
                generalLedgerAccount: database.Single<ProfitAndLossStatementAccountLatePaymentFees>(),
                customer: customer,
                salesInvoice: salesInvoice,
                transactionAmount: transactionAmount * -1m,
                baseAmount: baseAmount *-1m,
                exchangeRate: ExchangeRate,
                isExchangeRateInverse: ExchangeRateIsInverse,
                transactionCurrency: transactionCurrency,
                isBalancing: true,
                trackingCode: database.SingleOrDefault<Division>(customer.Division)
            ));

            return list.ToArray();
        }

        int IComparable<LatePaymentFee>.CompareTo(LatePaymentFee other)
        {
            return (other.Date).CompareTo((Date));
        }
    }
}
