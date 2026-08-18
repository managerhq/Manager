using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("650a36fe-801f-4031-8d5b-ab422d061fca")]
    [Singleton]
    public sealed class BalanceSheetEmployeeClearingAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, IPurchaseInvoiceAccount, ICode
    {
        [Guide("Enter the name for the employee clearing account. This account tracks amounts owed to or from employees, such as expense reimbursements, salary advances, or other employee-related transactions.")]
        [Guide("The default name is `EmployeeClearingAccount`, but you can rename it to better suit your business needs, such as 'Employee Advances' or 'Staff Reimbursements'.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.EmployeeClearingAccount))] public string Name { get; set; }
        [Guide("Optionally, enter an account code to help organize your chart of accounts. Codes are useful for sorting accounts and can make it easier to find accounts in reports and transactions.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the `BalanceSheet` group where this account should appear. Employee clearing accounts are typically shown under current assets (if employees owe money) or current liabilities (if the business owes employees).")]
        [Guide("Choose the appropriate group based on whether your business typically has net amounts receivable from or payable to employees.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.EmployeeClearingAccount;
            return Name;
        }

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + GetName();
                else return GetName();
            }
        }

        public override string GetCodeAndName()
        {
            return NameWithCode;
        }        

        public string GetCode()
        {
            return Code;
        }

        Guid IGeneralLedgerAccount.Key => Key;
        string IGeneralLedgerAccount.Name => Name;
        string IGeneralLedgerAccount.Code => Code;
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.OperatingActivities;
        string ICode.Code => Code;
    }
}
