using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("b9c4cd62-7569-44f0-bc62-9df3007a6a5c")]
    public sealed class CapitalAccount : NamedObject, IExpenseClaimPayer, IComparable<CapitalAccount>, ICustomFields, ICode
    {
        [Guide("Enter the name of the capital account holder. This is typically the name of the business owner, partner, or shareholder.")]
        [ProtoMember(1), NoWrap] public string Name { get; set; }
        [Guide("Optionally, enter a code for this capital account. This can be useful for organizing multiple capital accounts or for reporting purposes.")]
        [ProtoMember(11), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Assign this capital account to a specific division if you use divisional accounting. This helps track owner equity by division.")]
        [ProtoMember(12), Autocomplete(typeof(ManagerServer.Model.Division))] public Guid? Division { get; set; }
        [Guide("Select a control account if you want this capital account to use a different equity account than the default. Useful for segregating different types of capital accounts.")]
        [ProtoMember(10), Autocomplete(typeof(ManagerServer.Model.ControlAccountForCapitalAccounts))] public Guid? ControlAccount { get; set; }
        [ProtoMember(9)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(13)] public CustomFields CustomFields2 { get; set; }
        [Guide("Mark this capital account as inactive to hide it from dropdown lists while preserving historical transactions. Useful for former partners or closed capital accounts.")]
        [ProtoMember(8)] public bool Inactive { get; set; }

        [ProtoMember(5)] public StartingBalanceType Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(3)] public decimal Obsolete_StartingBalanceAmount2 { get; set; }
        [ProtoMember(2)] public bool Obsolete_HasOpeningBalance { get; set; }
        [ProtoMember(4)] public DateTime Obsolete_OpeningBalanceDate { get; set; }
        [ProtoMember(6)] public bool Obsolete_HasStartingBalance { get; set; }
        [ProtoMember(7)] public decimal Obsolete_StartingBalance { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        string ICode.Code => Code;

        public override bool IsInactive() => Inactive;

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            if (filter is ControlAccountForCapitalAccounts && ControlAccount != filter.Key) return false;
            return true;
        }

        public override string GetCodeAndName()
        {
            if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + Name;
            else return Name;
        }

        public override string GetName()
        {
            return Name;
        }

        /*
        public override string GetDescriptionOrNull()
        {
            return null;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        protected override GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var startingBalance = StartingBalanceAmount;
            if (StartingBalance == Model.Enums.StartingBalanceType.AmountToPay) startingBalance *= -1;

            if (startingBalance == 0m) return null;

            var baseCurrency = database.Single<BaseCurrency>();

            var list = new List<GeneralLedgerTransaction>();

            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                generalLedgerAccount: database.Single<BalanceSheetCapitalAccountsAccount>(),
                transactionAmount: startingBalance,
                transactionCurrency: baseCurrency,
                capitalAccount: this,
                transaction: this,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));
            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>(),
                transactionAmount: startingBalance * -1m,
                transactionCurrency: baseCurrency,
                capitalAccount: this,
                transaction: this,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));

            return list.ToArray();
        }
        */

        int IComparable<CapitalAccount>.CompareTo(CapitalAccount other)
        {
            return (Inactive, Code, Name).CompareTo((other.Inactive, other.Code, other.Name));
        }
    }
}
