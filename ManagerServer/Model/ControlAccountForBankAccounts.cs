using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("c97264e3-eed6-4afd-8d2c-e1c1a00b4dc1")]
    public sealed class ControlAccountForBankAccounts : ControlAccount, IBalanceSheetAccount, IJournalEntryAccount, ICode
    {
        [Guide("Enter a descriptive name for this control account that groups similar bank accounts together.")]
        [Guide("Control accounts consolidate multiple bank accounts into a single line on financial reports.")]
        [Guide("Examples: 'Operating Bank Accounts', 'Investment Accounts', 'Restricted Funds', or 'Foreign Currency Accounts'.")]
        [ProtoMember(1), NoWrap, TableColumn] public string Name { get; set; }
        
        [Guide("Enter an optional account code to identify this control account in reports and listings.")]
        [Guide("Account codes facilitate integration with other systems and make account selection faster.")]
        [Guide("Consider using a systematic numbering scheme like '1100' for operating accounts, '1200' for savings accounts.")]
        [ProtoMember(17), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        
        [Guide("Select the balance sheet group that determines where this control account appears on financial statements.")]
        [Guide("Most bank accounts belong under 'Current Assets', but you may have specific groups for different account types.")]
        [Guide("The group selection affects how accounts are subtotaled and presented on the balance sheet.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet)), TableColumn] public Guid? Group { get; set; }
        
        [Guide("Enter a position number to control the display order of this account within its balance sheet group.")]
        [Guide("Lower numbers appear first. Use gaps (like 10, 20, 30) to allow inserting accounts later.")]
        [Guide("This helps organize your balance sheet in a logical order, such as listing operating accounts before investment accounts.")]
        [ProtoMember(16)] public int Position { get; set; }
        
        [Guide("Check this box to deactivate the control account without deleting it.")]
        [Guide("Inactive control accounts are hidden from selection lists but retain all historical data.")]
        [Guide("Use this when reorganizing your chart of accounts or phasing out old account groupings.")]
        [ProtoMember(18)] public bool Inactive { get; set; }

        public override string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + Name;
                else return Name;
            }
        }

        Guid IGeneralLedgerAccount.Key => Key;
        string IGeneralLedgerAccount.Name => Name;
        string IGeneralLedgerAccount.Code => Code;
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.CashAndCashEquivalents;
        string ICode.Code => Code;

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public override string GetCodeAndName()
        {
            return NameWithCode;
        }

        public override string GetName()
        {
            return Name;
        }

        public string GetCode()
        {
            return Code;
        }

        public override bool IsInactive()
        {
            return Inactive;
        }
    }
}
