using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("6d2dc48d-2053-4e45-8330-285ebd431242")]
    public sealed class Supplier : NamedObject, IForeignCurrencyProvider, ICustomFields, IComparable<Supplier>, ICode
    {
        [Guide("Enter the supplier's name as it should appear on all transactions and reports.")]
        [Guide("This name will be displayed in dropdown lists throughout the system and on documents like purchase invoices and purchase orders.")]
        [ProtoMember(1), NoWrap] public string Name { get; set; }
        [Guide("Enter a unique supplier code to identify this supplier quickly in the system.")]
        [Guide("Supplier codes are optional but recommended for businesses with many suppliers. They allow you to search by code or name in dropdown menus.")]
        [Guide("Common examples include vendor numbers, abbreviations, or alphanumeric codes like 'SUPP001' or 'ACME-SUP'.")]
        [ProtoMember(10), Short, NoWrap, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Set the maximum amount you can owe this supplier at any time. This helps manage cash flow and purchasing limits.")]
        [Guide("The system will warn you when creating purchase transactions that would exceed this limit.")]
        [Guide("Leave blank for unlimited credit. Monitor your credit usage through the `Suppliers` tab summary.")]
        [ProtoMember(13), AppendCurrency, Placeholder(nameof(Strings.Optional))] public decimal CreditLimit { get; set; }
        [Guide("Assign a foreign currency to suppliers operating in a different currency from your base currency. By default, all supplier accounts are in your base currency. Selecting a foreign currency will issue all transactions (purchase orders, invoices, debit notes) in that currency.")]
        [Guide("Note: This option appears only if foreign currencies are created in the system.")]
        [ProtoMember(11), Autocomplete(typeof(ForeignCurrency))] public Guid? Currency { get; set; }
        [Guide("Enter the supplier's complete business address for correspondence and documentation.")]
        [Guide("This address automatically populates when creating new `Purchase Invoices`, `Purchase Orders`, or `Debit Notes` for this supplier.")]
        [Guide("Include street address, city, state/province, postal code, and country for complete records.")]
        [ProtoMember(7), Textarea] public string Address { get; set; }
        [Guide("Enter the supplier's primary email address for business communications.")]
        [Guide("Use this email for sending purchase orders, payment remittances, and other supplier correspondence.")]
        [Guide("Ensure this is the correct email address for accounts receivable or order processing at the supplier.")]
        [ProtoMember(2)] public string Email { get; set; }
        [Guide("Assign this supplier to a specific division for divisional reporting and cost center tracking.")]
        [Guide("All purchases from this supplier will be allocated to the selected division for profitability analysis.")]
        [Guide("This field only appears if divisions are enabled under `Settings` → `Divisions`.")]
        [ProtoMember(18), Autocomplete(typeof(Division))] public Guid? Division { get; set; }
        [Guide("Select a custom control account if this supplier should use a different accounts payable account than the default.")]
        [Guide("Custom control accounts help segregate different supplier types, such as trade creditors vs other creditors, or domestic vs international suppliers.")]
        [Guide("This field only appears if custom control accounts for suppliers have been created under `Settings` → `Control Accounts`.")]
        [ProtoMember(16), Autocomplete(typeof(ControlAccountForSuppliers))] public Guid? ControlAccount { get; set; }
        [Guide("Custom fields allow you to track additional supplier information specific to your business. Create custom fields under `Settings` tab to make them available here.")]
        [ProtoMember(9)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Enhanced custom fields that support different data types like dates, numbers, and dropdown lists. Configure these under `Settings` → `CustomFields`.")]
        [ProtoMember(19)] public CustomFields CustomFields2 { get; set; }
        [Guide("Mark this supplier as inactive to hide them from dropdown selection lists while preserving all historical transactions.")]
        [Guide("Use this for suppliers you no longer purchase from. Inactive suppliers can still be viewed and their transactions remain in reports.")]
        [Guide("You can reactivate a supplier at any time by unchecking this box.")]
        [ProtoMember(12)] public bool Inactive { get; set; }

        [ProtoMember(8)] public decimal Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(20)] public decimal Obsolete_ExchangeRate2 { get; set; }
        [ProtoMember(21)] public bool Obsolete_ExchangeRateIsInverse2 { get; set; }
        [ProtoMember(3)] public string Obsolete_Telephone { get; set; }
        [ProtoMember(4)] public string Obsolete_Fax { get; set; }
        [ProtoMember(5)] public string Obsolete_Mobile { get; set; }
        [ProtoMember(6)] public string Obsolete_Notes { get; set; }
        [ProtoMember(14)] public bool Obsolete_HasStartingBalance { get; set; }
        [ProtoMember(15)] public StartingBalanceType Obsolete_StartingBalanceType { get; set; }
        [ProtoMember(17)] public decimal Obsolete_StartingBalance { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        Guid? IForeignCurrencyProvider.ForeignCurrency => Currency;
        int IComparable<Supplier>.CompareTo(Supplier other) => (Inactive, Code, Name).CompareTo((other.Inactive, other.Code, other.Name));
        string ICode.Code => Code;

        public override bool IsInactive() => Inactive;

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            if (filter is ControlAccountForSuppliers && ControlAccount != filter.Key) return false;
            return true;
        }

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + Name;
                else return Name;
            }
        }

        public override string GetName()
        {
            return NameWithCode;
        }

        /*
        protected override GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            if (StartingBalance <= 0m) return null;

            var baseCurrency = database.Single<BaseCurrency>();
            var transactionCurrency = database.SingleOrDefault<ForeignCurrency>(Currency) as Currency ?? baseCurrency;
            decimal? baseAmount = null;
            if (transactionCurrency is ForeignCurrency)
            {
                var exchangeRate = ExchangeRate;
                if (exchangeRate == 0m) exchangeRate = 1m;

                if (ExchangeRateIsInverse) baseAmount = baseCurrency.Round(StartingBalance / exchangeRate);
                if (!ExchangeRateIsInverse) baseAmount = baseCurrency.Round(StartingBalance * exchangeRate);
            }

            var list = new List<GeneralLedgerTransaction>();

            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                generalLedgerAccount: database.Single<BalanceSheetAccountsPayableAccount>(),
                transactionAmount: StartingBalance,
                accountAmount: StartingBalance,
                baseAmount: baseAmount,
                transactionCurrency: transactionCurrency,
                supplier: this,
                transaction: this,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));
            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>(),
                transactionAmount: StartingBalance * -1m,
                accountAmount: baseAmount * -1m,
                baseAmount: baseAmount * -1m,
                transactionCurrency: transactionCurrency,
                supplier: this,
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
