using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Currencies
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.Currencies))]
    [Guide("The **Currencies** screen, found under the **Settings** tab, allows you to manage and configure the currencies used in your business transactions.")]
    [Guide("This feature is essential for businesses involved in international activities, enabling you to establish a *base currency* and add multiple *foreign currencies* as needed.")]
    [Guide("The **Currencies** screen provides access to the following configuration options:")]
    [SettingsItemScreenshot(icon: "fa-coin", name: nameof(Strings.Currencies))]
    [Namespace(typeof(Currencies))]
    internal sealed class Currencies : NakedNamespaces
    {
    }
}