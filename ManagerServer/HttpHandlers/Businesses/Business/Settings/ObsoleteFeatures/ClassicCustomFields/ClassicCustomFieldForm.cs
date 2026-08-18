using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ObsoleteFeatures.ClassicCustomFields
{
    [ProtoContract]
    [Title(nameof(Strings.CustomField))]
    [Guide("Create classic custom fields for additional data tracking.")]
    [Guide("Note: This is a legacy feature. Consider using the new custom fields system instead.")]
    [Fields(typeof(ManagerServer.Model.CustomField))]
    internal sealed class ClassicCustomFieldForm : NakedVueForm<ManagerServer.Model.CustomField>
    {
    }
}
