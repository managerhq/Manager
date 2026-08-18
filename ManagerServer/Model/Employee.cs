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
    [Guid("dadb7f95-a5dd-45c0-945d-6ad4ee28776e")]
    public sealed class Employee : NamedObject, IExpenseClaimPayer, IForeignCurrencyProvider, IComparable<Employee>, ICustomFields, ICode
    {
        [Guide("Enter the employee's full name as it should appear on payslips and other employment documents.")]
        [Guide("This name will be displayed in dropdown lists throughout the system and on all employee-related transactions.")]
        [ProtoMember(1), NoWrap] public string Name { get; set; }
        [Guide("Enter a unique employee code or ID number to identify this employee in the system.")]
        [Guide("Employee codes are optional but recommended for larger organizations. Common examples include employee numbers, payroll IDs, or department codes.")]
        [Guide("This code helps with quick selection in dropdown menus and can be used for integration with payroll systems.")]
        [ProtoMember(14), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Enter the employee's complete residential or mailing address.")]
        [Guide("This address is used for official correspondence, tax documents, and payroll records.")]
        [Guide("Include street address, city, state/province, postal code, and country for complete employee records.")]
        [ProtoMember(2), Textarea] public string Address { get; set; }
        [Guide("Enter the employee's work or personal email address for business communications.")]
        [Guide("This email is used for sending payslips, expense claim notifications, and other employment-related documents.")]
        [Guide("Ensure the email address is current and actively monitored by the employee.")]
        [ProtoMember(3)] public string Email { get; set; }
        [Guide("Select a foreign currency if this employee is paid in a currency different from your base currency.")]
        [Guide("This setting affects all employee transactions including `Payslips`, `Expense Claims`, and reimbursements.")]
        [Guide("Useful for expatriate employees or remote workers in different countries. This field only appears if foreign currencies are enabled.")]
        [ProtoMember(7), Autocomplete(typeof(ForeignCurrency))] public Guid? Currency { get; set; }
        [Guide("Assign this employee to a specific division for cost allocation and divisional reporting.")]
        [Guide("All payroll costs and expense claims for this employee will be allocated to the selected division.")]
        [Guide("This field only appears if divisions are enabled under `Settings` → `Divisions`.")]
        [ProtoMember(16), Autocomplete(typeof(Division))] public Guid? Division { get; set; }
        [Guide("Select a custom control account if this employee should use a different employee clearing account than the default.")]
        [Guide("Custom control accounts help segregate different employee types, such as permanent staff vs contractors, or by department.")]
        [Guide("This field only appears if custom control accounts for employees have been created under `Settings` → `Control Accounts`.")]
        [ProtoMember(15), Autocomplete(typeof(ControlAccountForEmployees))] public Guid? ControlAccount { get; set; }
        [Guide("Custom fields allow you to track additional employee information specific to your organization.")]
        [Guide("Common uses include department, job title, hire date, employee ID, emergency contacts, or certification numbers.")]
        [Guide("Create custom fields under `Settings` → `Custom Fields` to make them available here.")]
        [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Enhanced custom fields that support different data types like dates, numbers, and dropdown lists. Configure these under `Settings` → `CustomFields`.")]
        [ProtoMember(17)] public CustomFields CustomFields2 { get; set; }
        [Guide("Mark this employee as inactive to hide them from dropdown selection lists while preserving all historical records.")]
        [Guide("Use this for employees who have left the organization. Their payroll history and transactions remain in the system for reporting.")]
        [Guide("You can reactivate an employee at any time by unchecking this box.")]
        [ProtoMember(9)] public bool Inactive { get; set; }

        [ProtoMember(12)] public StartingBalanceType Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(10)] public decimal Obsolete_StartingBalanceAmount2 { get; set; }
        [ProtoMember(18)] public decimal Obsolete_ExchangeRate2 { get; set; }
        [ProtoMember(19)] public bool Obsolete_ExchangeRateIsInverse2 { get; set; }
        [ProtoMember(4)] public string Obsolete_Telephone { get; set; }
        [ProtoMember(5)] public string Obsolete_Mobile { get; set; }
        [ProtoMember(6)] public string Obsolete_Notes { get; set; }
        [ProtoMember(11)] public bool Obsolete_HasStartingBalance { get; set; }
        [ProtoMember(13)] public decimal Obsolete_StartingBalance { get; set; }

        //public override string GetReference() => null;

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + Name;
                else return Name;
            }
        }

        Guid? IForeignCurrencyProvider.ForeignCurrency => Currency;
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        int IComparable<Employee>.CompareTo(Employee other) => (Inactive, Code, Name).CompareTo((other.Inactive, other.Code, other.Name));
        string ICode.Code => Code;

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            if (filter is ControlAccountForEmployees && ControlAccount != filter.Key) return false;
            return true;
        }

        public override bool IsInactive()
        {
            return Inactive;
        }

        public override string GetName()
        {
            return NameWithCode;
        }

        /*
        protected override GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var startingBalance = StartingBalanceAmount;
            if (StartingBalance == Model.Enums.StartingBalanceType.AmountToPay) startingBalance *= -1;

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
                generalLedgerAccount: database.Single<BalanceSheetEmployeeClearingAccount>(),
                transactionAmount: startingBalance,
                accountAmount: startingBalance,
                baseAmount: baseAmount,
                transactionCurrency: transactionCurrency,
                employee: this,
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
                employee: this,
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
