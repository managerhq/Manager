using System;
using ManagerServer.Api.Businesses.Business.FixedAssets;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.FixedAssets
{
    [ProtoContract]
    [Title(nameof(Strings.FixedAsset))]
    [Guide("The `Fixed Asset` view displays comprehensive information about a specific fixed asset, including its description, purchase details, depreciation settings, and current book value.")]
    [Header("Managing Asset Information")]
    [Guide("Use the `Edit` button to modify asset details such as purchase date, cost, depreciation method, or disposal information.")]
    [Guide("Any changes made will automatically recalculate depreciation schedules and update the asset's book value.")]
    [Header("Viewing Related Transactions")]
    [Guide("The `Transactions` tab shows all journal entries related to this asset, including the initial purchase, depreciation entries, and any disposal transactions.")]
    [Guide("You can attach supporting documents like purchase invoices, receipts, or asset photos using the `Attachments` feature at the bottom of the view.")]
    [LinkGuide("For more information about creating and editing fixed assets, see:", typeof(FixedAssetForm))]
    internal sealed class FixedAssetView : DefaultView<GetFixedAssetView>
    {
    }
}
