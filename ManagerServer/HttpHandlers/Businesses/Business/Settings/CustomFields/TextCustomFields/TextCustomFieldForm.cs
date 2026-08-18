using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.TextCustomFields
{
    [ProtoContract]
    [Title(nameof(Strings.TextCustomFields), nameof(Strings.Edit))]
    [Guide("The Text Custom Field form is used to create custom text fields for various objects.")]
    [Guide("Text custom fields allow you to capture additional text information on forms.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.TextCustomField))]
    internal sealed class TextCustomFieldForm : NakedVueForm<ManagerServer.Model.TextCustomField>
    {
    }
}
