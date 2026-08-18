using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomButtons
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.CustomButtons))]
    [Guide("**Custom Buttons** launch extensions — custom web applications that run inside the Manager interface using an embedded iframe.")]
    [Guide("They allow developers to build tailored functionality without modifying the core Manager software.")]
    [SettingsItemScreenshot("fa-puzzle", nameof(Strings.CustomButtons))]
    [Header("What Extensions Can Do")]
    [Guide("Extensions can implement country-specific functionality such as *e-invoicing*, *tax reports*, and *bank feeds*.")]
    [Guide("They also enable general-purpose integrations with third-party applications or alternative data entry interfaces, such as a *point of sale system*.")]
    [Guide("Extensions provide a secure way to extend Manager's capabilities while keeping your core accounting data protected.")]
    internal sealed class CustomButtons : NakedObjectsWithAutomaticRows<ManagerServer.Model.CustomButton>
    {
        internal override bool IsEmpty(ManagerServer.Helpers.TabsExtensions.Item[] tabs)
        {
            return !ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.CustomButton>().Any();
        }

        [Default]
        public string[] GetName(CustomButton[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }
    }
}
