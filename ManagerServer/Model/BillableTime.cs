using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Obsolete.Obsolete32;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Currency(nameof(Customer))]
    [Guid("6bfb652c-11cb-46fa-9e5a-c5950ccbae15")]
    public sealed class BillableTime : Transaction, IComparable<BillableTime>, ICustomFields, IForeignCurrencyTransaction
    {
        [Guide("Enter the date when the billable work was performed.")]
        [ProtoMember(1)] public DateTime Date { get; set; }
        [Guide("Select the customer for whom this time was worked. Their default hourly rate will populate automatically.")]
        [ProtoMember(9), Autocomplete(typeof(Customer)), OnChangeSetDefault(nameof(HourlyRate))] public Guid? Customer { get; set; }
        [Guide("Describe the work performed. This description can appear on customer invoices.")]
        [ProtoMember(3), Long, Typeahead] public string Description { get; set; }
        [Guide("Enter the hourly rate to charge for this work. This defaults from the customer's settings.")]
        [ProtoMember(8), AppendCurrency(nameof(Customer)), NoWrap] public decimal HourlyRate { get; set; }
        [Guide("If the customer uses a foreign currency, enter the exchange rate for conversion to base currency.")]
        [ProtoMember(19), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(Customer), nameof(Model.Customer.Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : getCurrencyCode()) }} = "), Append("{{ (ExchangeRateIsInverse ? getCurrencyCode() : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(20), IfNotNull(nameof(Customer), nameof(Model.Customer.Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }
        [Guide("Enter the number of hours worked. This will be multiplied by the hourly rate.")]
        [ProtoMember(6), Prepend(nameof(Strings.Hours)), NoWrap, Placeholder("0")] public int? TimeSpent { get; set; }
        [Guide("Enter any additional minutes worked. These will be converted to decimal hours.")]
        [ProtoMember(7), Prepend(nameof(Strings.Minutes)), EmptyLabel, Placeholder("0")] public int? TimeSpentMinutes { get; set; }
        [Guide("Optionally assign this time to a specific division for tracking divisional profitability.")]
        [ProtoMember(16), Autocomplete(typeof(Division))] public Guid? Division { get; set; }
        [Guide("The current status of this billable time entry - Uninvoiced, Invoiced, or Written off.")]
        [ProtoMember(14), NoWrap, DoNotCopy] public BillableTimeStatus Status { get; set; }
        [Guide("If invoiced, select the sales invoice where this time was billed.")]
        [ProtoMember(13), NoWrap, DoNotCopy, IfEnum(nameof(Status), (int)BillableTimeStatus.Invoiced), Autocomplete(typeof(SalesInvoice), Filter = nameof(Customer))] public Guid? SalesInvoice { get; set; }
        [Guide("If written off, enter the date when the time was written off as non-billable.")]
        [ProtoMember(15), DoNotCopy, IfEnum(nameof(Status), (int)BillableTimeStatus.WrittenOff), EmptyLabel, Prepend(nameof(Strings.Date))] public DateTime? WrittenOffDate { get; set; }
        [ProtoMember(17)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(18)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(2)] public decimal Obsolete_Units { get; set; }
        [ProtoMember(4)] public decimal Obsolete_Amount { get; set; }
        [ProtoMember(10)] public string Obsolete_Category { get; set; }
        [ProtoMember(11)] public string Obsolete_StaffMember { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        public override string GetReference() => null;

        DateTime IForeignCurrencyTransaction.Date => Date;
        Guid? IForeignCurrencyTransaction.Currency => Customer;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }        

        public decimal GetQty()
        {
            var qty = 0m;
            if (TimeSpent.HasValue && TimeSpent.Value != 0) qty += TimeSpent.Value;
            if (TimeSpentMinutes.HasValue && TimeSpentMinutes.Value != 0) qty += Math.Round(TimeSpentMinutes.Value / 60m, 2, MidpointRounding.AwayFromZero);
            return qty;
        }

        public decimal GetAmount(int numberDecimalDigits)
        {
            var units = 0m;
            if (TimeSpent.HasValue && TimeSpent.Value != 0) units += TimeSpent.Value;
            if (TimeSpentMinutes.HasValue && TimeSpentMinutes.Value != 0) units += (TimeSpentMinutes.Value / 60m);

            try
            {
                var total = units * HourlyRate;
                return Math.Round(total, numberDecimalDigits, MidpointRounding.AwayFromZero);
            }
            catch (OverflowException)
            {
                return decimal.MaxValue;
            }
        }

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

            var baseCurrency = database.Single<BaseCurrency>();
            var transactionCurrency = database.SingleOrDefault<ForeignCurrency>(customer?.Currency) as Currency ?? baseCurrency;

            var transactionAmount = GetAmount(transactionCurrency.GetDecimalPlaces());
            var baseAmount = baseCurrency.GetBaseAmount(transactionAmount, ExchangeRate, ExchangeRateIsInverse, transactionCurrency);

            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: Date,
                transactionAmount: transactionAmount,
                baseAmount: baseAmount,
                transactionCurrency: transactionCurrency,
                generalLedgerAccount: database.Single<BalanceSheetBillableTimeAccount>(),
                exchangeRate: ExchangeRate,
                isExchangeRateInverse: ExchangeRateIsInverse,
                customer: customer
            ));

            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: Date,
                transactionAmount: transactionAmount * -1m,
                baseAmount: baseAmount *-1m,
                transactionCurrency: transactionCurrency,
                generalLedgerAccount: database.Single<ProfitAndLossStatementAccountBillableTimeMovement>(),
                customer: customer,
                exchangeRate: ExchangeRate,
                isExchangeRateInverse: ExchangeRateIsInverse,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));

            if (Status == BillableTimeStatus.Invoiced && SalesInvoice.HasValue)
            {
                var salesInvoice = database.SingleOrDefault<SalesInvoice>(SalesInvoice);
                if (salesInvoice != null)
                {
                    var date = salesInvoice.IssueDate;
                    if (date < Date) date = Date;

                    list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        transaction: this,
                        date: date,
                        transactionAmount: transactionAmount * -1m,
                        baseAmount: baseAmount*-1m,
                        transactionCurrency: transactionCurrency,
                        generalLedgerAccount: database.Single<BalanceSheetBillableTimeAccount>(),
                        salesInvoice: salesInvoice,
                        exchangeRate: ExchangeRate,
                        isExchangeRateInverse: ExchangeRateIsInverse,
                        customer: customer
                    ));

                    list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        transaction: this,
                        date: date,
                        transactionAmount: transactionAmount,
                        baseAmount: baseAmount,
                        transactionCurrency: transactionCurrency,
                        generalLedgerAccount: database.Single<ProfitAndLossStatementAccountBillableTimeMovement>(),
                        customer: customer,
                        salesInvoice: salesInvoice,
                        exchangeRate: ExchangeRate,
                        isExchangeRateInverse: ExchangeRateIsInverse,
                        trackingCode: database.SingleOrDefault<Division>(Division)
                    ));
                }
            }

            if (Status == BillableTimeStatus.WrittenOff && WrittenOffDate.HasValue)
            {
                list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    date: WrittenOffDate.Value,
                    transactionAmount: transactionAmount * -1m,
                    baseAmount: baseAmount *-1m,
                    transactionCurrency: transactionCurrency,
                    generalLedgerAccount: database.Single<BalanceSheetBillableTimeAccount>(),
                    exchangeRate: ExchangeRate,
                    isExchangeRateInverse: ExchangeRateIsInverse,
                    customer: customer
                ));

                list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    date: WrittenOffDate.Value,
                    transactionAmount: transactionAmount,
                    baseAmount: baseAmount,
                    transactionCurrency: transactionCurrency,
                    generalLedgerAccount: database.Single<ProfitAndLossStatementAccountBillableTimeMovement>(),
                    customer: customer,
                    exchangeRate: ExchangeRate,
                    isExchangeRateInverse: ExchangeRateIsInverse,
                    trackingCode: database.SingleOrDefault<Division>(Division)
                ));
            }

            return list.ToArray();
        }

        int IComparable<BillableTime>.CompareTo(BillableTime other)
        {
            return (other.Date).CompareTo((Date));
        }
    }
}
