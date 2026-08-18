using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.CheckboxCustomFields
{
    [ProtoContract]
    [Title(nameof(Strings.CheckboxCustomFields), nameof(Strings.Edit))]
    [Guide("Create a custom field with a checkbox for yes/no values.")]
    [Guide("Checkbox fields are useful for tracking boolean attributes like active/inactive status.")]
    [Fields(typeof(ManagerServer.Model.CheckboxCustomField))]
    internal sealed class CheckboxCustomFieldForm : NakedVueForm<ManagerServer.Model.CheckboxCustomField>
    {
    }
}
