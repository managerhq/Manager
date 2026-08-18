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
    [Guid("ec37c11e-2b67-49c6-8a58-6eccb7dd75ee")]    
    public sealed class Customer : NamedObject, IForeignCurrencyProvider, ICustomFields, IComparable<Customer>, ICode
    {
        [Guide("Enter the customer's name as it should appear on all transactions and reports.")]
        [Guide("This name will be displayed in dropdown lists throughout the system and on printed documents like invoices and statements.")]
        [ProtoMember(1), NoWrap] public string Name { get; set; }

        [Guide("Enter a unique customer code to identify this customer quickly in the system.")]
        [Guide("Customer codes are optional but recommended for businesses with many customers. They allow you to search by code or name in dropdown menus throughout the system.")]
        [Guide("Common examples include account numbers, abbreviations, or alphanumeric codes like 'CUST001' or 'ACME-NY'.")]
        [ProtoMember(13), NoWrap, Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }

        [Guide("Set the maximum amount this customer can owe at any time. This helps control credit risk and manage cash flow.")]
        [Guide("To monitor credit usage, enable the `Available Credit` column in the `Customers` tab. This shows remaining credit before creating new sales invoices.")]
        [Guide("Leave blank for unlimited credit. The credit limit is checked when creating new sales invoices but not enforced for other transactions.")]
        [ProtoMember(16), AppendCurrency, Placeholder(nameof(Strings.Optional))] public decimal CreditLimit { get; set; }

        [Guide("Assign a foreign currency to customers operating in a different currency from your base currency. By default, all customer accounts are in your base currency. Selecting a foreign currency will issue all transactions (quotes, orders, invoices, credit notes) in that currency.")]
        [Guide("Note: This option appears only if foreign currencies are created in the system.")]
        [ProtoMember(14), Autocomplete(typeof(ForeignCurrency))] public Guid? Currency { get; set; }

        [Guide("Enter the customer's complete billing address as it should appear on invoices and other sales documents.")]
        [Guide("This address automatically populates when creating new `Sales Invoices`, `Sales Orders`, `Sales Quotes`, or `Credit Notes` for this customer.")]
        [Guide("Include street address, city, state/province, postal code, and country for complete documentation.")]
        [ProtoMember(2), Textarea] public string BillingAddress { get; set; }

        [Guide("Enter the customer's shipping or delivery address if different from the billing address.")]
        [Guide("This address automatically populates when creating new `Delivery Notes` for this customer.")]
        [Guide("Only visible if the `Delivery Notes` tab is enabled. Leave blank if delivery address is the same as billing address.")]
        [ProtoMember(20), Textarea] public string DeliveryAddress { get; set; }

        [Guide("Enter the customer's primary email address for sending invoices, statements, and other communications.")]
        [Guide("This email address automatically populates when using the email function within Manager to send documents to the customer.")]
        [Guide("Ensure the email address is valid and actively monitored by the customer for important business communications.")]
        [ProtoMember(3)] public string Email { get; set; }

        [Guide("Assign this customer to a specific division for divisional reporting and profit center tracking.")]
        [Guide("Divisions help analyze profitability by business segment, location, or product line. All transactions for this customer will be allocated to the selected division.")]
        [Guide("This field only appears if divisions are enabled under `Settings` → `Divisions`.")]
        [ProtoMember(25), Autocomplete(typeof(Division))] public Guid? Division { get; set; }

        [Guide("Select a custom control account if this customer should use a different accounts receivable account than the default.")]
        [Guide("Custom control accounts are useful for segregating different types of customers, such as retail vs wholesale, or domestic vs international.")]
        [Guide("This field only appears if custom control accounts for customers have been created under `Settings` → `Control Accounts`.")]
        [ProtoMember(19), Autocomplete(typeof(ControlAccountForCustomers))] public Guid? ControlAccount { get; set; }

        [Guide("Enable this option to set specific payment terms for this customer that differ from your standard terms.")]
        [Guide("When enabled, specify the number of days after the invoice date when payment is due. For example, enter 30 for net 30 payment terms.")]
        [Guide("These terms will automatically apply to all new `Sales Invoices` created for this customer.")]
        [Guide("Tip: If all customers have the same payment terms, configure default due dates under `Form Defaults` for sales invoices instead.")]
        [ProtoMember(26), Label(nameof(Strings.Autofill), nameof(SalesInvoice), nameof(Strings.DueDate))] public bool HasDefaultDueDateDays { get; set; }
        [ProtoMember(27), IfTrue(nameof(HasDefaultDueDateDays)), NoLabel, Prepend(nameof(Strings.Net)), Append(nameof(Strings.Days))] public int? DefaultDueDateDays { get; set; }

        [Guide("Enable this option to set a specific hourly billing rate for this customer.")]
        [Guide("When enabled, enter the hourly rate to charge this customer for billable time. This rate will automatically populate when recording `Billable Time` entries.")]
        [Guide("Useful for service businesses that charge different rates based on customer agreements, project types, or service levels.")]
        [Guide("Tip: If all customers are charged the same hourly rate, configure the default rate under `Form Defaults` for billable time instead.")]
        [ProtoMember(29), Label(nameof(Strings.Autofill), nameof(BillableTime), nameof(Strings.HourlyRate)), IfContains<BillableTime>] public bool HasDefaultHourlyRate { get; set; }
        [ProtoMember(30), IfTrue(nameof(HasDefaultHourlyRate)), NoLabel, AppendCurrency] public decimal DefaultHourlyRate { get; set; }

        [Guide("Mark this customer as inactive to hide them from dropdown selection lists while preserving all historical transactions.")]
        [Guide("Use this for customers you no longer do business with. Inactive customers can still be viewed and their transactions remain in reports.")]
        [Guide("You can reactivate a customer at any time by unchecking this box.")]
        [ProtoMember(15)] public bool Inactive { get; set; }

        [Guide("Custom fields allow you to track additional customer information specific to your business. Create custom fields under `Settings` tab to make them available here.")]
        [ProtoMember(12)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Enhanced custom fields that support different data types like dates, numbers, and dropdown lists. Configure these under `Settings` → `CustomFields`.")]
        [ProtoMember(28)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(11)] public decimal Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(31)] public decimal Obsolete_ExchangeRate2 { get; set; }
        [ProtoMember(32)] public bool Obsolete_ExchangeRateIsInverse2 { get; set; }
        [ProtoMember(10)] public string Obsolete_BusinessIdentifier { get; set; }
        [ProtoMember(8)] public string Obsolete_SouthAfrica_VAT_Number { get; set; }
        [ProtoMember(9)] public string Obsolete_Philippines_TIN_Number { get; set; }
        [ProtoMember(4)] public string Obsolete_Telephone { get; set; }
        [ProtoMember(5)] public string Obsolete_Fax { get; set; }
        [ProtoMember(6)] public string Obsolete_Mobile { get; set; }
        [ProtoMember(7)] public string Obsolete_Notes { get; set; }
        [ProtoMember(17)] public bool Obsolete_HasStartingBalance { get; set; }
        [ProtoMember(18)] public StartingBalanceType Obsolete_StartingBalanceType { get; set; }
        [ProtoMember(23)] public decimal Obsolete_StartingBalance { get; set; }
        [ProtoMember(24)] public bool Obsolete_CustomerPortal { get; set; }

        public bool HasDefaultBillingAddress => true;
        public string DefaultBillingAddress => BillingAddress;
        public bool HasDefaultDeliveryAddress => true;
        public string DefaultDeliveryAddress => DeliveryAddress;

        Guid? IForeignCurrencyProvider.ForeignCurrency => Currency;
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        int IComparable<Customer>.CompareTo(Customer other) => (Inactive, Code, Name).CompareTo((other.Inactive, other.Code, other.Name));
        string ICode.Code => Code;

        public override bool IsInactive() => Inactive;

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            if (filter is ControlAccountForCustomers && ControlAccount != filter.Key) return false;
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
    }
}
