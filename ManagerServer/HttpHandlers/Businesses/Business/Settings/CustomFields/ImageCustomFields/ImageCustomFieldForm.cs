using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.ImageCustomFields
{
    [ProtoContract]
    [Title(nameof(Strings.ImageCustomFields), nameof(Strings.Edit))]
    [Guide("Create a custom field for uploading images.")]
    [Guide("Image fields allow attaching photos, logos, or other visual content to records.")]
    [Fields(typeof(ManagerServer.Model.ImageCustomField))]
    internal sealed class ImageCustomFieldForm : NakedVueForm<ManagerServer.Model.ImageCustomField>
    {
    }
}
