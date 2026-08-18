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
    [Guid("2aa99eac-faca-4017-a157-edbbbcb160ac")]
    [Singleton]
    public sealed class ProfitAndLossStatementAccountRoundingExpense : NamedObject, IProfitAndLossAccount, ICode
    {
        [Guide("Enter the name for this expense account that captures small rounding differences in transactions.")]
        [Guide("The default name is `RoundingExpense` but you can customize it to match your business terminology.")]
        [Guide("This account accumulates minor discrepancies from currency rounding in sales and purchase transactions.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.RoundingExpense))] public string Name { get; set; }
        [Guide("Enter an optional account code to organize your chart of accounts systematically.")]
        [Guide("Account codes help with sorting accounts and can follow your existing numbering system.")]
        [Guide("Rounding expense is typically a miscellaneous expense with codes in the high expense range.")]
        [ProtoMember(11), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the profit and loss statement group where this expense account should appear.")]
        [Guide("Rounding expenses are typically shown under other expenses or administrative expenses.")]
        [Guide("Small immaterial amounts accumulate here to maintain exact transaction balancing.")]
        [ProtoMember(3), Autocomplete(typeof(ProfitAndLossStatementGroup)), Prepend(nameof(Strings.ProfitAndLossStatement))] public Guid? Group { get; set; }
        [ProtoMember(10)] public int Position { get; set; }

        public override string GetName()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            return Strings.RoundingExpense;
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
