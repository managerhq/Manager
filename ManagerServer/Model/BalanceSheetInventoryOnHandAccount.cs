using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("0fb45a62-fc42-43a8-a776-782e8b5ffc96")]
    [Singleton]
    public sealed class BalanceSheetInventoryOnHandAccount : NamedObject, IBalanceSheetAccount, IInventoryOnHandAccount, ICode
    {
        [Guide("Enter the name for this control account that tracks the value of inventory items in stock.")]
        [Guide("The default name is `InventoryOnHand` but you can customize it to match your business terminology.")]
        [Guide("This account aggregates the total cost value of all inventory items across all locations.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.InventoryOnHand))] public string Name { get; set; }
        [Guide("Enter an optional account code to organize your chart of accounts systematically.")]
        [Guide("Account codes help with sorting accounts and can follow your existing numbering system.")]
        [Guide("Common codes for inventory accounts range from 1300-1399 in many accounting systems.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the balance sheet group where this asset account should appear in financial reports.")]
        [Guide("Inventory on hand is typically classified as a current asset since it's expected to be sold within a year.")]
        [Guide("The account balance represents the cost value of unsold inventory using your chosen valuation method.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.InventoryOnHand;
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
