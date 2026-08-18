using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("841b2acb-8bb5-4742-864e-4226fa421f44")]
    [Singleton]
    public sealed class ProfitAndLossStatementAccountLatePaymentFees : NamedObject, IProfitAndLossAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, ICode
    {
        [Guide("Enter the name for this income account that tracks fees charged to customers for overdue payments.")]
        [Guide("The default name is `LatePaymentFees` but you can customize it to match your business terminology.")]
        [Guide("This account records revenue from finance charges, interest, or penalties on late customer payments.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.LatePaymentFees))] public string Name { get; set; }
        [Guide("Enter an optional account code to organize your chart of accounts systematically.")]
        [Guide("Account codes help with sorting accounts and can follow your existing numbering system.")]
        [Guide("Common codes for other income accounts range from 4900-4999 in many accounting systems.")]
        [ProtoMember(11), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the profit and loss statement group where this income account should appear.")]
        [Guide("Late payment fees are typically classified as other income or finance income.")]
        [Guide("The grouping affects how your income statement is organized and subtotaled.")]
        [ProtoMember(3), Autocomplete(typeof(ProfitAndLossStatementGroup)), Prepend(nameof(Strings.ProfitAndLossStatement))] public Guid? Group { get; set; }
        [ProtoMember(10)] public int Position { get; set; }

        public override string GetName()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            return Strings.LatePaymentFees;
        }

        Guid IGeneralLedgerAccount.Key => Key;
        string IGeneralLedgerAccount.Name => Name;
        string IGeneralLedgerAccount.Code => Code;
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.OperatingActivities;
        string ICode.Code => Code;

        public string GetCode()
        {
            return Code;
        }

        public override string GetCodeAndName()
        {
            return NameWithCode;
        }

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + GetName();
                else return GetName();
            }
        }
    }
}
