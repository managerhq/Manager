using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("a4dffac2-35d1-47e1-a4bd-b6de15975fdb")]
    [Singleton]
    public sealed class BalanceSheetWithholdingTaxPayableAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, ICode
    {
        [Guide("Enter the name for the withholding tax payable account. This account tracks taxes withheld from payments to suppliers, employees, or other parties that must be remitted to tax authorities.")]
        [Guide("The default name is `WithholdingTaxPayable`, but you can rename it to match your local tax terminology, such as 'TDS Payable' or 'Tax Deducted at Source'.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.WithholdingTaxPayable))] public string Name { get; set; }
        [Guide("Optionally, enter an account code to help organize your chart of accounts. Codes are useful for sorting accounts and can make it easier to find accounts in reports and transactions.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the `BalanceSheet` group where this account should appear. Withholding tax payable is typically shown under current liabilities as it represents taxes collected that must be paid to the government.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.WithholdingTaxPayable;
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
        string ICode.Code => Code;
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.OperatingActivities;
    }
}
