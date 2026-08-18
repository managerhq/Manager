using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Query;
using ManagerServer.Model;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.IntangibleAssets
{
    [ProtoContract]
    [Title(nameof(Strings.IntangibleAsset), nameof(Strings.Edit))]
    [Guide("Use the `IntangibleAsset` form to create and manage intangible assets in your business. Intangible assets are non-physical assets that have value and can be owned or controlled by your business.")]
    [Guide("Common examples of intangible assets include patents, trademarks, copyrights, software licenses, customer databases, brand names, franchises, and goodwill. Unlike physical assets, intangible assets derive their value from intellectual property rights or competitive advantages they provide.")]
    [Header("Setting Up an Intangible Asset")]
    [Guide("When creating a new intangible asset, start by entering a descriptive name that clearly identifies the asset. You can optionally assign a code or reference number to help track the asset in your register.")]
    [Guide("Set the annual amortization rate to determine how the asset's value will be expensed over time. For example, if you expect to amortize the asset over 5 years using straight-line amortization, enter 20% as the rate.")]
    [Guide("Use the description field to record important details such as registration numbers, legal documentation references, acquisition dates, expiry dates, or any terms and conditions associated with the asset.")]
    [Header("Accounting Configuration")]
    [Guide("By default, intangible assets use the standard intangible assets accounts defined in your chart of accounts. However, you can override these defaults for specific categorization needs.")]
    [Guide("If you track different types of intangible assets separately, select custom control accounts for the asset cost and accumulated amortization. This allows you to report on different categories of intangible assets independently.")]
    [Guide("For amortization expenses, you can specify a custom expense account if you want to track amortization separately from the default account. This is useful when different intangible assets relate to different areas of your business operations.")]
    [Header("Asset Disposal")]
    [Guide("When an intangible asset is sold, expires, becomes obsolete, or is otherwise disposed of, mark it as disposed and enter the disposal date. The system will automatically stop calculating amortization from that date forward.")]
    [Guide("Any gain or loss on disposal will be calculated based on the asset's book value at the disposal date and posted to the designated disposal account.")]
    [Header("Form Fields")]
    [Fields(typeof(ManagerServer.Model.IntangibleAsset))]
    internal sealed class IntangibleAssetForm : NakedVueForm<ManagerServer.Model.IntangibleAsset>
    {
        protected override bool CanHaveImage()
        {
            return true;
        }
    }
}