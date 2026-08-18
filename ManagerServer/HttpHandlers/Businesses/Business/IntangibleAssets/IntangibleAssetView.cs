using System;
using ManagerServer.Api.Businesses.Business.IntangibleAssets;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.IntangibleAssets
{
    [ProtoContract]
    [Title(nameof(Strings.IntangibleAsset))]
    [Guide("The *Intangible Asset* view displays comprehensive information about a specific intangible asset, including its current value, accumulated amortization, and other key details.")]
    [Guide("Intangible assets are non-physical assets such as patents, trademarks, copyrights, licenses, software, and goodwill that provide long-term value to your business.")]
    [Header("Available Actions")]
    [Guide("From this view, you can perform several actions:")]
    [Guide("• Click the **Edit** button to modify the asset's details, including its *purchase date*, *cost*, *useful life*, and *amortization method*")]
    [Guide("• View the complete transaction history showing all *amortization entries* and adjustments")]
    [Guide("• Attach supporting documents such as purchase invoices, valuation reports, or legal documents")]
    [Guide("• Generate reports to track the asset's *book value* and *amortization schedule*")]
    [LinkGuide("To learn about creating and editing intangible assets, see:", typeof(IntangibleAssetForm))]
    internal sealed class IntangibleAssetView : DefaultView<GetIntangibleAssetView>
    {
    }
}
