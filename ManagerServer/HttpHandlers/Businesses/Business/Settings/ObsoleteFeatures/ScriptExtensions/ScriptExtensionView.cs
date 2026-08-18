using ManagerServer.Api.Businesses.Business.Settings.ObsoleteFeatures.ScriptExtensions;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using System;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ObsoleteFeatures.ScriptExtensions
{
    [ProtoContract]
    [Guide("Script extensions allow you to customize and extend the functionality of the software through custom code.")]
    [Guide("This view displays the details of a specific script extension, including its name and configuration.")]
    [Guide("Script extensions are an obsolete feature and may be replaced with more modern alternatives in future versions.")]
    [LinkGuide("To edit this script extension, see:", typeof(ScriptExtensionForm))]
    internal sealed class ScriptExtensionView : DefaultView<GetScriptExtensionView>
    {
    }
}
