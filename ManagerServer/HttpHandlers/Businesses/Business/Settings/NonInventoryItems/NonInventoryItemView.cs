using ManagerServer.Api.Businesses.Business.Settings.NonInventoryItems;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using System;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.NonInventoryItems
{
    [ProtoContract]
    [Guide("View comprehensive details about a *non-inventory item*, including its description, pricing information, and associated accounts.")]
    [Guide("*Non-inventory items* are products or services that you buy or sell but do not track as inventory quantities. Examples include services, labor charges, or consumable items that are expensed immediately.")]
    [Guide("From this view, you can see the item's current pricing, tax settings, and account allocations. Use the **Edit** button to modify any of these details.")]
    [LinkGuide("For more information, see:", typeof(NonInventoryItemForm))]
    internal sealed class NonInventoryItemView : DefaultView<GetNonInventoryItemView>
    {
    }
}
