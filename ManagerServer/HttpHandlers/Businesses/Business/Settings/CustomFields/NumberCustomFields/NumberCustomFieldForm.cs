using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.NumberCustomFields
{
    [ProtoContract]
    [Title(nameof(Strings.NumberCustomFields), nameof(Strings.Edit))]
    [Guide("Create a custom field for numeric values.")]
    [Guide("Number fields are useful for tracking quantities, measurements, or other numeric data.")]
    [Fields(typeof(ManagerServer.Model.NumberCustomField))]
    internal sealed class NumberCustomFieldForm : NakedVueForm<ManagerServer.Model.NumberCustomField>
    {
    }
}
