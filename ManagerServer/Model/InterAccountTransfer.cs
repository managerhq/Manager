using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Currency(nameof(PaidFrom))]
    [Guid("dea4f923-c498-4504-b3ef-30be3c33175e")]
    public sealed class InterAccountTransfer : Transaction, IHasAutomaticReference, IRecurringTransactionDestination, IComparable<InterAccountTransfer>, ICustomFields, IForeignCurrencyTransaction, ICode, IHasCustomTheme
    {
        [Guide("Enter the date of the transfer. This is when the money moves between accounts.")]
        [Guide("The transfer date affects bank reconciliations and cash flow reports.")]
        [ProtoMember(1), NoWrap, TableColumn] public DateTime Date { get; set; }
        [Guide("Enter a reference number for this transfer. This could be a transaction number or description to identify the transfer.")]
        [Guide("References help match transfers with bank statements and provide an audit trail.")]
        [ProtoMember(6), TableColumn] public string Reference { get; set; }
        [Guide("Optionally, add a description or notes about this transfer, such as the purpose or reason for moving funds.")]
        [Guide("Common reasons include funding petty cash, moving funds for payroll, or consolidating accounts.")]
        [ProtoMember(5), Long, TableColumn] public string Description { get; set; }
        [Guide("Select the bank or cash account from which money is being transferred out.")]
        [Guide("This account's balance will be reduced by the transfer amount.")]
        [ProtoMember(2), NoWrap, Autocomplete(typeof(IBankOrCashAccount)), DoNotHide] public Guid? PaidFrom { get; set; }
        [Guide("Enter the amount being transferred out of the source account. This will be credited (reduced) from the 'Paid from' account.")]
        [Guide("For transfers between accounts with the same currency, this amount equals what is received.")]
        [ProtoMember(8), NoWrap, AppendCurrency(nameof(PaidFrom)), EmptyLabel, Prepend(nameof(Strings.Amount))] public decimal CreditAmount { get; set; }
        [Guide("For bank accounts, select whether this withdrawal has cleared the bank or is still pending.")]
        [Guide("Pending transactions appear in bank reconciliations as uncleared items.")]
        [ProtoMember(18), NoWrap, Prepend(nameof(Strings.Cleared)), EmptyLabel, IfNotNull(nameof(PaidFrom))] public BankAccountClearStatus CreditClearStatus { get; set; }
        [ProtoMember(11), Prepend(nameof(Strings.Date)), IfEnum(nameof(CreditClearStatus), (int)BankAccountClearStatus.OnALaterDate), EmptyLabel, Placeholder(nameof(Strings.Pending))] public DateTime? CreditClearDate { get; set; }
        [Guide("Select the bank or cash account into which money is being transferred.")]
        [Guide("This account's balance will be increased by the transfer amount.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(IBankOrCashAccount)), DoNotHide] public Guid? ReceivedIn { get; set; }
        [Guide("If transferring between accounts with different currencies, enter the amount received in the destination account's currency.")]
        [Guide("The difference between amounts sent and received represents the exchange gain or loss.")]
        [ProtoMember(9), NoWrap, AppendCurrency(nameof(ReceivedIn)), EmptyLabel, Prepend(nameof(Strings.Amount)), IfNotEqual(nameof(PaidFrom) +"."+nameof(Currency), nameof(ReceivedIn) + "." + nameof(Currency))] public decimal DebitAmount { get; set; }
        [Guide("For bank accounts, select whether this deposit has cleared the bank or is still pending.")]
        [Guide("Deposits may clear on a different date than withdrawals, especially for inter-bank transfers.")]
        [ProtoMember(19), NoWrap, Prepend(nameof(Strings.Cleared)), EmptyLabel, IfNotNull(nameof(ReceivedIn))] public BankAccountClearStatus DebitClearStatus { get; set; }
        [ProtoMember(12), Prepend(nameof(Strings.Date)), IfEnum(nameof(DebitClearStatus), (int)BankAccountClearStatus.OnALaterDate), EmptyLabel, Placeholder(nameof(Strings.Pending))] public DateTime? DebitClearDate { get; set; }
        [Guide("When transferring between accounts with different currencies, enter the exchange rate used for the conversion.")]
        [Guide("Use the actual rate from your bank or the rate at which you exchanged the currencies.")]
        [ProtoMember(25), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(PaidFrom), nameof(Model.BankOrCashAccount.Currency)), IfNotNull(nameof(ReceivedIn), nameof(Model.BankOrCashAccount.Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : getCurrencyCode()) }} = "), Append("{{ (ExchangeRateIsInverse ? getCurrencyCode() : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(26), IfNotNull(nameof(PaidFrom), nameof(Model.BankOrCashAccount.Currency)), IfNotNull(nameof(ReceivedIn), nameof(Model.BankOrCashAccount.Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }
        [ProtoMember(15), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(16), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(17), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(22), Label(nameof(Strings.Footers))] public bool HasInterAccountTransferFooters { get; set; }
        [ProtoMember(23), Autocomplete(typeof(InterAccountTransferFooter)), NoLabel, IfTrue(nameof(HasInterAccountTransferFooters))] public Guid[] InterAccountTransferFooters { get; set; }
        [ProtoMember(7)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(24)] public CustomFields CustomFields2 { get; set; }
        [ProtoMember(27), Hidden] public string FdxDebitTransactionId { get; set; }
        [ProtoMember(28), Hidden] public string FdxCreditTransactionId { get; set; }

        [ProtoMember(20)] public DateTime? Obsolete_DebitBankClearDate { get; set; }
        [ProtoMember(21)] public DateTime? Obsolete_CreditBankClearDate { get; set; }
        [ProtoMember(14)] public BankClearStatus Obsolete_CreditClearStatus { get; set; }
        [ProtoMember(13)] public BankClearStatus Obsolete_DebitClearStatus { get; set; }
        [ProtoMember(4)] public decimal Obsolete_Amount { get; set; }
        [ProtoMember(10)] public JournalEntry Obsolete_JournalEntry { get; set; }

        public override string GetReference() => Reference;

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        DateTime IRecurringTransactionDestination.Date { get => Date; set => Date = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        DateTime IForeignCurrencyTransaction.Date => Date;
        Guid? IForeignCurrencyTransaction.Currency => PaidFrom;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }
        string ICode.Code => Reference;

        public override string GetDescriptionOrNull()
        {
            if (!string.IsNullOrWhiteSpace(Description)) return Description;
            return null;
        }

        public override string GetName()
        {
            return Reference;
        }

        public DateTime? GetCreditClearDate()
        {
            if (CreditClearStatus == BankAccountClearStatus.OnTheSameDate) return Date;
            if (CreditClearDate.HasValue && CreditClearDate.Value < Date) return Date;
            return CreditClearDate;
        }

        public DateTime? GetDebitClearDate()
        {
            if (DebitClearStatus == BankAccountClearStatus.OnTheSameDate) return Date;
            if (DebitClearDate.HasValue && DebitClearDate.Value < Date) return Date;
            return DebitClearDate;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        public override ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var creditBankAccount = database.SingleOrDefault<BankOrCashAccount>(PaidFrom);
            var creditBankAccountCurrency = database.SingleOrDefault<ManagerServer.Model.ForeignCurrency>(creditBankAccount?.Currency) as Currency ?? database.Single<BaseCurrency>();

            var debitBankAccount = database.SingleOrDefault<BankOrCashAccount>(ReceivedIn);
            var debitBankAccountCurrency = database.SingleOrDefault<ManagerServer.Model.ForeignCurrency>(debitBankAccount?.Currency) as Currency ?? database.Single<BaseCurrency>();

            var transactionAmount = CreditAmount;
            var transactionCurrency = creditBankAccountCurrency;
            decimal? creditAccountAmount = null;
            decimal? debitAccountAmount = DebitAmount;

            if (creditBankAccountCurrency is ForeignCurrency && debitBankAccountCurrency is BaseCurrency)
            {
                transactionAmount = DebitAmount;
                transactionCurrency = debitBankAccountCurrency;
                creditAccountAmount = CreditAmount;
                debitAccountAmount = null;
            }

            transactionAmount = transactionCurrency.Round(transactionAmount);
            var baseCurrency = database.Single<BaseCurrency>();
            var baseAmount = baseCurrency.GetBaseAmount(transactionAmount, ExchangeRate, ExchangeRateIsInverse, transactionCurrency);

            var creditContraTransactions = new Query.GeneralLedger.GeneralLedgerTransaction[1];

            var creditTransaction = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: Date,
                transactionAmount: transactionAmount * -1m,
                accountAmount: creditAccountAmount * -1m,
                baseAmount: baseAmount * -1m,
                exchangeRate: transactionCurrency is not BaseCurrency ? ExchangeRate : null,
                isExchangeRateInverse: transactionCurrency is not BaseCurrency ? ExchangeRateIsInverse : default(bool),
                transaction: this,
                transactionCurrency: transactionCurrency,
                generalLedgerAccount: database.Single<BalanceSheetCashAtBankAccount>(),
                bankAccount: creditBankAccount,
                trackingCode: database.SingleOrDefault<Division>(creditBankAccount?.Division),
                contraTransactions: creditContraTransactions
            );

            var debitTransaction = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: Date,
                transactionAmount: transactionAmount,
                accountAmount: debitAccountAmount,
                baseAmount: baseAmount,
                exchangeRate: transactionCurrency is not BaseCurrency ? ExchangeRate : null,
                isExchangeRateInverse: transactionCurrency is not BaseCurrency ? ExchangeRateIsInverse : default(bool),
                transaction: this,
                transactionCurrency: transactionCurrency,
                generalLedgerAccount: database.Single<BalanceSheetCashAtBankAccount>(),
                bankAccount: debitBankAccount,
                trackingCode: database.SingleOrDefault<Division>(debitBankAccount?.Division),
                contraTransactions: new Query.GeneralLedger.GeneralLedgerTransaction[] { creditTransaction }
            );

            creditContraTransactions[0] = debitTransaction;

            return new Query.GeneralLedger.GeneralLedgerTransaction[]
            {
                creditTransaction,
                debitTransaction
            };
        }

        int IComparable<InterAccountTransfer>.CompareTo(InterAccountTransfer other)
        {
            return (!other.IsInactive(), other.Date, other.Reference).CompareTo((!IsInactive(), Date, Reference));
        }
    }
}
