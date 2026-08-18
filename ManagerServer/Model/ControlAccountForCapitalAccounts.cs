using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("95b9d17d-f5f8-4722-a89d-456fa0906e13")]
    public sealed class ControlAccountForCapitalAccounts : ControlAccount, IBalanceSheetAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, IPurchaseInvoiceAccount, ISalesInvoiceAccount, ICode
    {
        [Guide("Enter the name of the control account. This name will appear on financial reports instead of individual capital account names.")]
        [ProtoMember(1), NoWrap, TableColumn] public string Name { get; set; }
        
        [Guide("Optionally, enter an account code. Codes help organize accounts and can be used for searching and sorting.")]
        [ProtoMember(17), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        
        [Guide("Select the balance sheet group where this control account should appear. This determines its placement on the balance sheet.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet)), TableColumn] public Guid? Group { get; set; }
        
        [Guide("Set the position number to control the display order within the selected group. Lower numbers appear first.")]
        [ProtoMember(16)] public int Position { get; set; }
        
        [Guide("Mark this control account as inactive to prevent it from appearing in dropdown lists. Existing transactions will not be affected.")]
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
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.FinancingActivities;
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