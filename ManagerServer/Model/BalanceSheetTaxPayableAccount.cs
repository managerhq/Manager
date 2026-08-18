using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("30c697fa-4196-438a-ab5a-1957478034b1")]
    [Singleton]
    public sealed class BalanceSheetTaxPayableAccount : NamedObject, IBalanceSheetAccount, ICode
    {
        [Guide("Name of account. The default name is `TaxPayable` but it can be renamed.")]
        [Guide("This account tracks the total tax amounts collected on sales that must be remitted to tax authorities.")]
        [Guide("Rename it to match your local tax terminology, such as 'VAT Payable', 'GST Payable', or 'Sales Tax Payable'.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.TaxPayable))] public string Name { get; set; }
        [Guide("Enter code of the account if desired.")]
        [Guide("Account codes help organize your chart of accounts and can follow your standard numbering system.")]
        [Guide("For example: '2100' for current liabilities or '2150' for tax-related liabilities.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select group on `BalanceSheet` under which this account should be presented.")]
        [Guide("Typically placed under 'Current Liabilities' as taxes are usually due within the fiscal year.")]
        [Guide("The placement affects how the account appears on your balance sheet report hierarchy.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.TaxPayable;
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
