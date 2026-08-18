using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("ac4ab22b-0fbd-485f-8434-d25745b9be22")]
    [Singleton]
    public sealed class BalanceSheetInterdivisionalLoan : NamedObject, IBalanceSheetAccount, ICode
    {
        [Guide("Enter the name for the interdivisional loan account. This account tracks loans or transfers between different divisions or departments within your business.")]
        [Guide("The default name is `InterdivisionalLoan`, but you can rename it to better reflect your organizational structure, such as 'Inter-Department Loans' or 'Division Transfer Account'.")]
        [Guide("This account is automatically created and updated when transactions cross division boundaries.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.InterdivisionalLoan))] public string Name { get; set; }
        [Guide("Optionally, enter an account code to help organize your chart of accounts.")]
        [Guide("Codes are useful for sorting accounts and can make it easier to find accounts in reports and transactions.")]
        [Guide("Consider using a code that clearly identifies this as an internal account, like 'IDL' or '9999'.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the `BalanceSheet` group where this account should appear.")]
        [Guide("Interdivisional loans can be classified as either assets or liabilities depending on the net position.")]
        [Guide("If divisions typically owe the head office, place this under assets. If the head office typically owes divisions, place it under liabilities.")]
        [Guide("The account balance automatically reflects the net position between all divisions.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.InterdivisionalLoan;
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
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.FinancingActivities;
        string ICode.Code => Code;
    }
}
