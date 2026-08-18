using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ObsoleteFeatures
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.ObsoleteFeatures))]
    [Guide("The *Obsolete Features* section, found under the **Settings** tab, allows you to enable features that are no longer recommended for use.")]
    [Guide("These features are maintained for compatibility with existing data but should not be used in new implementations.")]
    [SettingsItemScreenshot("fa-scroll-old", nameof(Strings.ObsoleteFeatures))]
    [Header("Classic Custom Fields")]
    [Guide("If you are currently using *Classic Custom Fields*, we recommend migrating to the new custom fields system.")]
    [Guide("The new system provides improved flexibility and better integration with other features.")]
    [LinkGuide("Learn more about classic custom fields:", typeof(ClassicCustomFields.ClassicCustomFields))]
    internal sealed class ObsoleteFeatures : NakedNamespaces
    {
    }
}