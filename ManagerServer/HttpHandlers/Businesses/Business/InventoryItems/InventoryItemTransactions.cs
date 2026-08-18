using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.HttpHandlers.Businesses.Business.Summary;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryItem), nameof(Strings.Transactions))]
    [Guide("The **Inventory Item - Transactions** screen provides a comprehensive view of all movements for a specific inventory item.")]
    [Guide("This report helps you track and understand how inventory levels change over time by showing every transaction that affects the item's quantity or value.")]
    [Header("Transaction Types")]
    [Guide("The report includes all types of inventory movements:")]
    [Guide("• Purchases from suppliers")]
    [Guide("• Sales to customers")]
    [Guide("• Inventory transfers between locations")]
    [Guide("• Inventory write-offs")]
    [Guide("• Production orders that consume or produce the item")]
    [Header("Transaction Details")]
    [Guide("Transactions are displayed in chronological order with the following information:")]
    [Guide("• **Date** - When the transaction occurred")]
    [Guide("• **Description** - Details about the transaction")]
    [Guide("• **Quantity change** - How much the inventory level increased or decreased")]
    [Guide("• **Unit cost** - The cost per unit for the transaction")]
    [Guide("• **Running balance** - The inventory quantity after each transaction")]
    [Guide("This complete transaction history allows you to trace exactly how your inventory levels arrived at their current state and helps identify patterns in inventory movement.")]
    internal sealed class InventoryItemTransactions : BaseGeneralLedgerTransactionsInheritable
    {
    }
}