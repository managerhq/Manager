using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("1408c33b-6284-4f50-9e31-48cbea21f3cf")]
    public sealed class BankOrCashAccount : NamedObject, IBankOrCashAccount, IForeignCurrencyProvider, ICustomFields, IComparable<BankOrCashAccount>, ICode
    {
        [Guide("Enter the name of the bank or cash account as it should appear throughout the system.")]
        [Guide("For bank accounts, use descriptive names like 'Business Checking - ABC Bank' or 'Savings Account #1234'.")]
        [Guide("For cash accounts, use names like 'Petty Cash', 'Cash Register', or 'Cash on Hand'.")]
        [ProtoMember(1), NoWrap] public string Name { get; set; }

        [Guide("Enter a unique code to identify this account quickly in dropdown lists and reports.")]
        [Guide("Account codes are optional but useful for organizing multiple accounts. Examples: 'CHK001', 'SAV001', or 'CASH-01'.")]
        [Guide("The code appears before the account name in selection lists for easy identification.")]
        [ProtoMember(13), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }

        [Guide("Select a foreign currency if this account holds funds in a currency different from your base currency.")]
        [Guide("All transactions in this account will be recorded in the selected foreign currency and converted to base currency for reporting.")]
        [Guide("This field only appears if foreign currencies are enabled under `Settings` → `Currencies`.")]
        [ProtoMember(3), Autocomplete(typeof(ForeignCurrency))] public Guid? Currency { get; set; }

        [Guide("Assign this bank or cash account to a specific division for divisional reporting.")]
        [Guide("All transactions in this account will be allocated to the selected division for profit center analysis.")]
        [Guide("This field only appears if divisions are enabled under `Settings` → `Divisions`.")]
        [ProtoMember(16), Autocomplete(typeof(Division))] public Guid? Division { get; set; }

        [Guide("Select a custom control account to categorize this account differently on the balance sheet.")]
        [Guide("Custom control accounts help separate different types of bank accounts, such as operating accounts vs investment accounts, or restricted vs unrestricted funds.")]
        [Guide("This field only appears if custom control accounts for bank accounts have been created under `Settings` → `Control Accounts`.")]
        [ProtoMember(12), Autocomplete(typeof(ControlAccountForBankAccounts))] public Guid? ControlAccount { get; set; }

        [Guide("Enable this option to record the International Bank Account Number (IBAN) for this account.")]
        [Guide("IBANs are used for international wire transfers and are required in many countries. The IBAN will appear on remittance advices and payment instructions.")]
        [ProtoMember(20), Label(nameof(InternationalBankAccountNumber))] public bool HasInternationalBankAccountNumber { get; set; }
        [ProtoMember(21), NoLabel, IfTrue(nameof(HasInternationalBankAccountNumber))] public string InternationalBankAccountNumber { get; set; }

        [Guide("Enable pending transactions to track when payments and receipts clear your bank account.")]
        [Guide("When enabled, each transaction can have two dates: the transaction date and the clearance date. This helps with bank reconciliation and cash flow management.")]
        [Guide("Pending transactions appear separately in reports until they are marked as cleared.")]
        [ProtoMember(18)] public bool CanHavePendingTransactions { get; set; }

        [Guide("Enable this option to set a credit limit for overdraft facilities or credit card accounts.")]
        [Guide("Enter the maximum amount that can be overdrawn or charged. The system will warn when transactions would exceed this limit.")]
        [Guide("Useful for monitoring credit card balances and overdraft usage to avoid fees and manage cash flow.")]
        [ProtoMember(17), Label(nameof(Strings.CreditLimit))] public bool HasCreditLimit { get; set; }
        [ProtoMember(4), AppendCurrency, IfTrue(nameof(HasCreditLimit)), NoLabel] public decimal CreditLimit { get; set; }        

        [Guide("Mark this account as inactive to hide it from dropdown selection lists while preserving all transaction history.")]
        [Guide("Use this for closed bank accounts or discontinued cash accounts. Historical transactions remain in reports for audit purposes.")]
        [Guide("You can reactivate an account at any time by unchecking this box.")]
        [ProtoMember(10)] public bool Inactive { get; set; }
        [Guide("Custom fields allow you to track additional account information specific to your needs.")]
        [Guide("Common uses include account numbers, routing numbers, SWIFT codes, branch names, or bank contact information.")]
        [Guide("Create custom fields under `Settings` → `Custom Fields` to make them available here.")]
        [ProtoMember(9)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Enhanced custom fields that support different data types like dates, numbers, and dropdown lists. Configure these under `Settings` → `CustomFields`.")]
        [ProtoMember(19)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(39)] public byte[] Obsolete_BankFeedProviderConfiguration { get; set; }
        [ProtoMember(6)] public decimal Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(25)] public decimal Obsolete_ExchangeRate2 { get; set; }
        [ProtoMember(26)] public bool Obsolete_ExchangeRateIsInverse2 { get; set; }
        [ProtoMember(7)] public string Obsolete_AccountNumber { get; set; }
        [ProtoMember(8)] public string Obsolete_FinancialInsitution { get; set; }
        [ProtoMember(11)] public Enums.ControlAccountType Obsolete_ControlAccountType { get; set; }
        [ProtoMember(2)] public Enums.CashAccountType Obsolete_Type { get; set; }
        [ProtoMember(14)] public bool Obsolete_IsBankAccount { get; set; }
        [ProtoMember(5)] public bool Obsolete_HasStartingBalance { get; set; }
        [ProtoMember(15)] public decimal Obsolete_StartingBalance { get; set; }
        [ProtoMember(27)] public bool Obsolete_HasRelay { get; set; }
        [ProtoMember(28)] public string Obsolete_Relay { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        int IComparable<BankOrCashAccount>.CompareTo(BankOrCashAccount other) => (Inactive, Code, Name).CompareTo((other.Inactive, other.Code, other.Name));
        string ICode.Code => Code;

        public override bool IsInactive()
        {
            return Inactive;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            if (filter is ControlAccountForBankAccounts && ControlAccount != filter.Key) return false;
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

        public bool IsBankAccount => true;
        public bool IsCashAccount => false;

        Guid? IForeignCurrencyProvider.ForeignCurrency => Currency;

        public override string GetName()
        {
            return NameWithCode;
        }        
    }
}

