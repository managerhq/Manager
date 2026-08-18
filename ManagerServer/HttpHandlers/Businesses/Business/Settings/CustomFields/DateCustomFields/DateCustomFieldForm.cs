using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.DateCustomFields
{
    [ProtoContract]
    [Title(nameof(Strings.DateCustomFields), nameof(Strings.Edit))]
    [Guide("Create a custom field for date values.")]
    [Guide("Date fields are useful for tracking expiry dates, renewal dates, or other important dates.")]
    [Fields(typeof(ManagerServer.Model.DateCustomField))]
    internal sealed class DateCustomFieldForm : NakedVueForm<ManagerServer.Model.DateCustomField>
    {
    }
}
