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

namespace ManagerServer.HttpHandlers.Businesses.Business.FixedAssets
{
    [ProtoContract]
    [Title(nameof(Strings.FixedAsset), nameof(Strings.Edit))]
    [Guide("Fixed assets are long-term tangible assets that your business owns and uses in its operations to generate income. These assets typically have a useful life of more than one year.")]
    [Guide("Common examples of fixed assets include buildings, land, vehicles, machinery, office equipment, furniture, and computer hardware.")]
    [Header("Setting Up Your Fixed Asset")]
    [Guide("Use this form to record a new fixed asset or edit an existing one. You'll need to provide details about the asset, including its description, purchase date, cost, and depreciation information.")]
    [Guide("The system will automatically calculate depreciation based on the method and parameters you specify. This ensures accurate financial reporting and tax compliance.")]
    [Header("Important Considerations")]
    [Guide("Before creating a fixed asset, ensure you have the following information ready:")]
    [Guide("• Purchase invoice or receipt showing the asset's cost")]
    [Guide("• Expected useful life of the asset in your business")]
    [Guide("• Estimated residual value at the end of its useful life")]
    [Guide("• Preferred depreciation method for the asset")]
    [Header("Form Fields")]
    [Guide("Complete the fields below to set up your fixed asset. Required fields are marked with an asterisk.")]
    [Fields(typeof(ManagerServer.Model.FixedAsset))]
    internal sealed class FixedAssetForm : NakedVueForm<ManagerServer.Model.FixedAsset>
    {
        protected override bool CanHaveImage()
        {
            return true;
        }
    }
}