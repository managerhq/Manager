using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("f728124f-c6b6-4dad-82c5-22fc0d8d0571")]
    [Singleton]
    public sealed class BalanceSheetExpenseClaimsAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, ICode
    {
        [Guide("Enter the name for the expense claims account. This account tracks expenses paid by employees or owners from their personal funds that need to be reimbursed by the business.")]
        [Guide("The default name is `Expense_claims`, but you can rename it to suit your needs, such as 'Employee Reimbursements' or 'Expense Reimbursements Payable'.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.Expense_claims))] public string Name { get; set; }
        [Guide("Optionally, enter an account code to help organize your chart of accounts. Codes are useful for sorting accounts and can make it easier to find accounts in reports and transactions.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the `BalanceSheet` group where this account should appear. Expense claims are typically shown under current liabilities since they represent amounts the business owes to employees or owners.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.Expense_claims;
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
