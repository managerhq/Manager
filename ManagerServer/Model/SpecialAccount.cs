using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using System;
using System.Collections.Generic;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("e495f4e8-5fad-48ac-8a66-f35049ac4ef3")]
    public sealed class SpecialAccount : NamedObject, IComparable<SpecialAccount>, ICustomFields, ICode
    {
        [Guide("Enter the name of the special account. This should clearly identify what the account tracks, such as 'Loan from Bank ABC' or 'Prepaid Insurance'.")]
        [ProtoMember(1), NoWrap] public string Name { get; set; }
        [Guide("Optionally, enter a code for this special account. Codes help organize and quickly identify special accounts in lists and reports.")]
        [ProtoMember(11), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Assign a foreign currency if this special account tracks balances in a currency other than your base currency.")]
        [Guide("Note: This option appears only if foreign currencies are created in the system.")]
        [ProtoMember(5), Autocomplete(typeof(ForeignCurrency))] public Guid? Currency { get; set; }
        [Guide("Select a default tax code for transactions involving this special account. This can be overridden on individual transactions.")]
        [ProtoMember(6), Autocomplete(typeof(TaxCode))] public Guid? TaxCode { get; set; }
        [Guide("Assign this special account to a specific division if you use divisional accounting. This helps track special account balances by division.")]
        [ProtoMember(12), Autocomplete(typeof(ManagerServer.Model.Division))] public Guid? Division { get; set; }
        [Guide("Select a control account if you want this special account to post to a different balance sheet account than the default special accounts control.")]
        [ProtoMember(7), Autocomplete(typeof(ControlAccountForSpecialAccounts))] public Guid? ControlAccount { get; set; }
        [Guide("Mark this special account as inactive to hide it from dropdown lists while preserving historical transactions. Useful for closed or discontinued special accounts.")]
        [ProtoMember(10)] public bool Inactive { get; set; }
        [ProtoMember(13)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(14)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(3)] public decimal Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(4)] public DebitCredit Obsolete_StartingBalanceType2 { get; set; }
        [ProtoMember(15)] public decimal Obsolete_ExchangeRate2 { get; set; }
        [ProtoMember(16)] public bool Obsolete_ExchangeRateIsInverse2 { get; set; }
        [ProtoMember(8)] public BalanceSheetAccount Obsolete_BalanceSheetAccount { get; set; }
        [ProtoMember(2)] public bool Obsolete_HasStartingBalance { get; set; }
        [ProtoMember(9)] public decimal Obsolete_StartingBalance { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        int IComparable<SpecialAccount>.CompareTo(SpecialAccount other) => (Inactive, Code, Name).CompareTo((other.Inactive, other.Code, other.Name));
        string ICode.Code => Code;

        public override bool IsInactive() => Inactive;

        public override string GetCodeAndName()
        {
            if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + Name;
            else return Name;
        }

        public override string GetName()
        {
            return Name;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            if (filter is ControlAccountForSpecialAccounts && ControlAccount != filter.Key) return false;
            return true;
        }

        /*
        protected override GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var startingBalance = StartingBalance;
            if (StartingBalanceType == Model.Enums.DebitCredit.Credit) startingBalance *= -1;

            if (startingBalance == 0m) return null;

            var baseCurrency = database.Single<BaseCurrency>();
            var transactionCurrency = database.SingleOrDefault<ForeignCurrency>(Currency) as Currency ?? baseCurrency;
            decimal? baseAmount = null;
            if (transactionCurrency is ForeignCurrency)
            {
                var exchangeRate = ExchangeRate;
                if (exchangeRate == 0m) exchangeRate = 1m;

                if (ExchangeRateIsInverse) baseAmount = baseCurrency.Round(startingBalance / exchangeRate);
                if (!ExchangeRateIsInverse) baseAmount = baseCurrency.Round(startingBalance * exchangeRate);
            }

            var list = new List<GeneralLedgerTransaction>();
            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                generalLedgerAccount: database.Single<BalanceSheetSpecialAccountsAccount>(),
                transactionAmount: startingBalance,
                accountAmount: startingBalance,
                baseAmount: baseAmount,
                transactionCurrency: transactionCurrency,
                specialAccount: this,
                transaction: this,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));
            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>(),
                transactionAmount: startingBalance * -1m,
                accountAmount: baseAmount * -1m,
                baseAmount: baseAmount * -1m,
                transactionCurrency: transactionCurrency,
                specialAccount: this,
                transaction: this,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));
            return list.ToArray();
        }

        public override string GetDescriptionOrNull()
        {
            return null;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }
        */
    }
}
