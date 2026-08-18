using System;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("26b9e4a5-ce10-4f30-94c7-23a1ca4428f9")]
    public sealed class ProfitAndLossStatementAccount : NamedObject, IProfitAndLossAccount, ICustomGeneralLedgerAccount, IInventoryWriteOffAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, INonInventoryItemAccount, IPurchaseInvoiceAccount, ISalesInvoiceAccount, ICode
    {
        [Guide("The name of the account as it will appear in reports and transaction forms. Choose a descriptive name that clearly identifies the type of revenue or expense.")]
        [ProtoMember(1), NoWrap] public string Name { get; set; }
        [Guide("An optional account code for organizing and referencing accounts. Codes typically follow a numbering system (e.g., 4000-4999 for revenue, 5000-5999 for expenses).")]
        [ProtoMember(11), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Assign this account to a group for better organization in reports. Groups help categorize similar accounts together (e.g., 'Sales Revenue', 'Operating Expenses').")]
        [ProtoMember(3), Autocomplete(typeof(ProfitAndLossStatementGroup)), Prepend(nameof(Strings.ProfitAndLossStatement)), NoWrap] public Guid? Group { get; set; }
        [Guide("Specify how this account should be categorized in the Cash Flow Statement. Most revenue and expense accounts are classified as Operating Activities.")]
        [ProtoMember(14), NoWrap, EmptyLabel, Prepend(nameof(Strings.CashFlowStatement))] public CashFlowStatementCategory CashFlowStatement { get; set; }
        [Guide("Select the specific operating activity group for this account in the Cash Flow Statement.")]
        [ProtoMember(13), NoWrap, EmptyLabel, Autocomplete(typeof(CashFlowStatementOperatingActivityGroup)), IfEnum(nameof(CashFlowStatement), (int)CashFlowStatementCategory.OperatingActivities)] public Guid? CashFlowStatementOperatingActivityGroup { get; set; }
        [Guide("Select the specific financing activity group for this account in the Cash Flow Statement.")]
        [ProtoMember(15), NoWrap, EmptyLabel, Autocomplete(typeof(CashFlowStatementFinancingActivityGroup)), IfEnum(nameof(CashFlowStatement), (int)CashFlowStatementCategory.FinancingActivities)] public Guid? CashFlowStatementFinancingActivityGroup { get; set; }
        [Guide("Select the specific investing activity group for this account in the Cash Flow Statement.")]
        [ProtoMember(16), EmptyLabel, Autocomplete(typeof(CashFlowStatementInvestingActivityGroup)), IfEnum(nameof(CashFlowStatement), (int)CashFlowStatementCategory.InvestingActivities)] public Guid? CashFlowStatementInvestingActivityGroup { get; set; }
        [Guide("Enable this to automatically fill in a default description when this account is selected in transactions.")]
        [ProtoMember(18), Label(nameof(Strings.Autofill), nameof(Strings.LineDescription))] public bool HasDefaultLineDescription { get; set; }
        [Guide("The default description that will be automatically filled when this account is used in transactions. This saves time and ensures consistency.")]
        [ProtoMember(19), IfTrue(nameof(HasDefaultLineDescription)), NoLabel, Textarea] public string DefaultLineDescription { get; set; }
        [Guide("Enable this to automatically select a default tax code when this account is used in transactions.")]
        [ProtoMember(17), IfContains<TaxCode>, Label(nameof(Strings.Autofill), nameof(Strings.TaxCode))] public bool HasDefaultTaxCode { get; set; }
        [Guide("The default tax code that will be automatically selected when this account is used. This ensures the correct tax treatment is applied.")]
        [ProtoMember(8), IfTrue(nameof(HasDefaultTaxCode)), Autocomplete(typeof(TaxCode)), NoLabel, Short] public Guid? DefaultTaxCode { get; set; }
        [Guide("The display order of this account within its group. Lower numbers appear first. Use this to customize the sequence of accounts in reports.")]
        [ProtoMember(10)] public int Position { get; set; }
        [Guide("Check this box to make the account inactive. Inactive accounts won't appear in selection lists but historical transactions remain unchanged.")]
        [ProtoMember(12)] public bool Inactive { get; set; }

        [ProtoMember(2)] public int? Obsolete_Code { get; set; }
        [ProtoMember(9)] internal ManagerServer.Model.Obsolete.Obsolete18.GeneralLedgerAccount18 Obsolete_GeneralLedgerAccount;

        public string NameWithCode
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
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatement;
        string ICode.Code => Code;

        public override string GetCodeAndName()
        {
            return NameWithCode;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public override string GetName()
        {
            return Name;
        }

        public string GetCode()
        {
            return Code;
        }

        public Guid? GetCashFlowStatementGroup()
        {
            if (CashFlowStatement == CashFlowStatementCategory.OperatingActivities) return CashFlowStatementOperatingActivityGroup;
            if (CashFlowStatement == CashFlowStatementCategory.InvestingActivities) return CashFlowStatementInvestingActivityGroup;
            if (CashFlowStatement == CashFlowStatementCategory.FinancingActivities) return CashFlowStatementFinancingActivityGroup;
            return null;
        }
    }
}
