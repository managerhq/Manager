using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;
using System.Collections.Generic;
using System.Linq;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("bfb275ad-a639-4a6a-a112-b87e602df7e1")]
    [Title(nameof(Strings.StartingBalance))]
    public sealed class InventoryItemStartingBalance : ManagerServer.Model.Transaction
    {
        [Guide("Select inventory item that you have created under `InventoryItems` tab.")]
        [ProtoMember(1), Autocomplete(typeof(InventoryItem))] public Guid? InventoryItem { get; set; }
        [Guide("Check this option if you have any quantity physically on hand.")]
        [Fields(typeof(QtyOnHandLine))]
        [ProtoMember(2), Label(nameof(Strings.QtyOnHand))] public bool HasQtyToOnHand { get; set; }
        [ProtoMember(3), IfTrue(nameof(HasQtyToOnHand))] public QtyOnHandLine[] QtyOnHandLines { get; set; }
        [Guide("Check this option if you have any quantity that you have purchased from suppliers but haven't received yet.")]
        [Fields(typeof(QtyToReceiveLine))]
        [ProtoMember(4), Label(nameof(Strings.QtyToReceive))] public bool HasQtyToReceive { get; set; }
        [ProtoMember(5), IfTrue(nameof(HasQtyToReceive))] public QtyToReceiveLine[] QtyToReceiveLines { get; set; }
        [Guide("Check this option if you have any quantity that you have sold to customers but haven't delivered yet.")]
        [Fields(typeof(QtyToDeliverLine))]
        [ProtoMember(6), Label(nameof(Strings.QtyToDeliver))] public bool HasQtyToDeliver { get; set; }
        [ProtoMember(7), IfTrue(nameof(HasQtyToDeliver))] public QtyToDeliverLine[] QtyToDeliverLines { get; set; }
        [Fieldset(nameof(Strings.BookValue))]
        [Prepend(nameof(Strings.QtyOwned)), NoLabel, Expression(Zero, PlusArray, nameof(QtyOnHandLine.QtyOnHand), PlusArray, nameof(QtyToReceiveLine.QtyToReceive), MinusArray, nameof(QtyToDeliverLine.QtyToDeliver))] public object QtyOwned { get; set; }
        [ProtoMember(8), Prepend(nameof(Strings.AverageCost)), NoLabel, AppendBaseCurrency] public decimal AverageCost { get; set; }
        [Prepend(nameof(Strings.TotalCost)), NoLabel, AppendBaseCurrency, Expression(Zero, Plus, nameof(QtyOwned), Times, nameof(AverageCost), RoundToBaseCurrency)] public object TotalCost { get; set; }

        [ProtoContract]
        public sealed class QtyOnHandLine : ITransactionLine
        {
            [Guide("Select `InventoryLocation` where the inventory item is physically located.")]
            [ProtoMember(1), EmptyLabel, Autocomplete(typeof(CustomInventoryLocation)), Prepend(nameof(Strings.InventoryLocation))] public Guid? InventoryLocation { get; set; }
            [Guide("Enter quantity that is physically located at the inventory location.")]
            [ProtoMember(2), EmptyLabel, Sum] public decimal QtyOnHand { get; set; }
        }

        [ProtoContract]
        public sealed class QtyToReceiveLine : ITransactionLine
        {
            [Guide("Select `Supplier` who you purchased inventory item from but haven't delivered it yet.")]
            [ProtoMember(1), EmptyLabel, Autocomplete(typeof(Supplier)), Prepend(nameof(Strings.Supplier))] public Guid? Supplier { get; set; }
            [Guide("Enter quantity that has been purchased from supplier but hasn't been received yet.")]
            [ProtoMember(2), EmptyLabel, Sum] public decimal QtyToReceive { get; set; }
        }

        [ProtoContract]
        public sealed class QtyToDeliverLine : ITransactionLine
        {
            [Guide("Select `Customer` who purchased inventory item from you but hasn't received it yet.")]
            [ProtoMember(1), EmptyLabel, Autocomplete(typeof(Customer)), Prepend(nameof(Strings.Customer))] public Guid? Customer { get; set; }
            [Guide("Enter quantity that has been sold to customer but hasn't been delivered yet.")]
            [ProtoMember(2), EmptyLabel, Sum] public decimal QtyToDeliver { get; set; }
        }

        public override string GetReference()
        {
            return string.Empty;
        }

        public override string GetName()
        {
            return null;
        }

        public override string GetDescriptionOrNull()
        {
            return null;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        public override string TransactionTitle => Strings.StartingBalance;

        public override GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var transactions = new List<GeneralLedgerTransaction>();

            var baseCurrency = database.Single<BaseCurrency>();
            var inventoryItem = database.SingleOrDefault<InventoryItem>(InventoryItem);
            var division = database.SingleOrDefault<Division>(inventoryItem?.Division);

            if (inventoryItem != null)
            {
                if (QtyOnHandLines != null)
                {
                    foreach (var e in QtyOnHandLines)
                    {
                        transactions.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            date: DateTime.MinValue,
                            generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                            transactionAmount: 0m,
                            transactionCurrency: baseCurrency,
                            transaction: this,
                            qty: e.QtyOnHand,
                            transactionLine: e,
                            inventoryItem: inventoryItem,
                            trackingCode: division,
                            inventoryLocation: database.SingleOrDefault<CustomInventoryLocation>(e.InventoryLocation)
                        ));
                    }
                }

                if (QtyToReceiveLines != null)
                {
                    foreach (var e in QtyToReceiveLines)
                    {
                        transactions.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            date: DateTime.MinValue,
                            generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                            transactionAmount: 0m,
                            transactionCurrency: baseCurrency,
                            transaction: this,
                            qty: e.QtyToReceive,
                            transactionLine: e,
                            inventoryItem: inventoryItem,
                            trackingCode: division,
                            supplier: database.SingleOrDefault<Supplier>(e.Supplier)
                        ));
                    }
                }

                if (QtyToDeliverLines != null)
                {
                    foreach (var e in QtyToDeliverLines)
                    {
                        transactions.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            date: DateTime.MinValue,
                            generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                            transactionAmount: 0m,
                            transactionCurrency: baseCurrency,
                            transaction: this,
                            qty: e.QtyToDeliver*-1m,
                            transactionLine: e,
                            inventoryItem: inventoryItem,
                            trackingCode: division,
                            customer: database.SingleOrDefault<Customer>(e.Customer)
                        ));
                    }
                }
            }

            var qtyOwned = transactions.Sum(x => x.Qty.Value);
            if (qtyOwned > 0m)
            {
                if (AverageCost > 0m)
                {
                    var totalCost = baseCurrency.Round(qtyOwned * AverageCost);

                    transactions.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            date: DateTime.MinValue,
                            generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                            transactionAmount: totalCost,
                            transactionCurrency: baseCurrency,
                            transaction: this,
                            trackingCode: division,
                            inventoryItem: inventoryItem
                        ));

                    transactions.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            date: DateTime.MinValue,
                            generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>(),
                            transactionAmount: totalCost*-1m,
                            transactionCurrency: baseCurrency,
                            transaction: this,
                            trackingCode: division,
                            inventoryItem: inventoryItem
                        ));

                }
            }

            return transactions.ToArray();
        }
    }
}