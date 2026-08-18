using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using System.Collections.Generic;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("a8f95068-fc73-43f7-aabb-fd868e506b51")]
    public sealed class Investment : NamedObject, ICustomFields, IComparable<Investment>, ICode
    {
        [Guide("Enter a code or ticker symbol for this investment. This provides a short identifier for reports and transactions.")]
        [ProtoMember(1), Short, NoWrap] public string Code { get; set; }
        [Guide("Enter the full name or description of this investment, such as company name or fund description.")]
        [ProtoMember(2)] public string Name { get; set; }
        [Guide("Select a control account if you want to group this investment with others for reporting purposes.")]
        [ProtoMember(7), Autocomplete(typeof(ManagerServer.Model.ControlAccountForInvestments))] public Guid? ControlAccount { get; set; }
        [Guide("Check this box to deactivate the investment. Inactive investments won't appear in selection lists but retain their history.")]
        [ProtoMember(3)] public bool Inactive { get; set; }
        [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(9)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(10)] public Guid? Obsolete_Currency { get; set; }
        [ProtoMember(4)] public decimal Obsolete_MarketPrice { get; set; }
        [ProtoMember(5)] public decimal Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(6)] public decimal Obsolete_StartingBalanceTotalCost2 { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        int IComparable<Investment>.CompareTo(Investment other) => (Inactive, Code, Name).CompareTo((other.Inactive, other.Code, other.Name));
        string ICode.Code => Code;        

        public override string GetCodeAndName()
        {
            if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + Name;
            else return Name;
        }

        public override string GetName()
        {
            return Name;
        }

        public override bool IsInactive()
        {
            return Inactive;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            if (filter is ControlAccountForInvestments && ControlAccount != filter.Key) return false;
            return true;
        }

        /*
        public override string GetDescriptionOrNull()
        {
            return Strings.StartingBalance;
        }

        protected override GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            if (StartingBalance == 0m && StartingBalanceTotalCost == 0m) return null;

            var baseCurrency = database.Single<BaseCurrency>();

            var list = new List<Manager.Query.GeneralLedger.GeneralLedgerTransaction>();

            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                transaction: this,
                generalLedgerAccount: database.Single<BalanceSheetInvestmentsAccount>(),
                investment: this,
                transactionAmount: StartingBalanceTotalCost,
                qty: StartingBalance,
                transactionCurrency: baseCurrency
             ));
            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                transaction: this,
                generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>(),
                investment: this,
                transactionAmount: StartingBalanceTotalCost * -1,
                transactionCurrency: baseCurrency
            ));

            return list.ToArray();
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }
        */
    }
}