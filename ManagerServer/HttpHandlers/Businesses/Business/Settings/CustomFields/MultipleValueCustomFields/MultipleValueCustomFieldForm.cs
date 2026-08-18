using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.MultipleValueCustomFields
{
    [ProtoContract]
    [Title(nameof(Strings.MultipleValueCustomFields), nameof(Strings.Edit))]
    [Guide("Create a custom field with predefined dropdown options.")]
    [Guide("Multiple value fields ensure data consistency by limiting input to specific choices.")]
    [Fields(typeof(ManagerServer.Model.MultipleValueCustomField))]
    internal sealed class MultipleValueCustomFieldForm : NakedVueForm<ManagerServer.Model.MultipleValueCustomField>
    {
    }
}
