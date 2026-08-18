using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ObsoleteFeatures.ScriptExtensions
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.Extensions))]
    [NewButton(nameof(Strings.NewExtension))]
    [Guide("Script extensions allow you to customize Manager by adding custom JavaScript code that can modify the behavior and appearance of various screens.")]
    [Guide("Extensions can be used to add custom validations, calculations, or visual enhancements to forms and reports throughout the application.")]
    [Header("Important Notice")]
    [Guide("This is a legacy feature that has been superseded by the new extensions system. While existing script extensions will continue to work, we recommend using the new system for any new customizations.")]
    [Guide("The new extensions system provides better performance, more features, and improved security compared to script extensions.")]
    [Header("Managing Extensions")]
    [Guide("To create a new script extension, click the **New Extension** button. Each extension requires a name and the JavaScript code to execute.")]
    [Guide("Extensions can be temporarily disabled without deleting them by marking them as inactive. This is useful for troubleshooting or testing purposes.")]
    [Columns]
    internal sealed class ScriptExtensions : PersistentObjectTable<ManagerServer.Model.ScriptExtension>
    {
        [Guid("14112aeb-d807-4e45-9a54-79874db14ad7")]
        public string GetName(ManagerServer.Model.ScriptExtension o) => o.Name;
    }
}
