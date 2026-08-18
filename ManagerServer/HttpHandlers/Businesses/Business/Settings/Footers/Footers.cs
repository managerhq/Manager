using System;
using System.Linq;
using System.Reflection;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.Footers))]
    [Guide("Footers allow you to add static text to the bottom of printed documents such as quotes, orders, invoices, and similar items.")]
    [Guide("You can access the `Footers` feature in the `Settings` tab.")]
    [Header("Creating Footers")]
    [Guide("You can create footers using either plain text or HTML format.")]
    [Guide("Footers support both static text and dynamic content. When creating or editing a footer, you'll see a list of available merge tags that can be used to insert dynamic information.")]
    [Guide("To add an image to a footer, convert the image to Base64 format using a tool like <a href=\"https://www.base64-image.de\">www.base64-image.de</a>. After conversion, paste the IMG tag into the footer.")]
    [SettingsItemScreenshot("fa-file-dashed-line", nameof(Strings.Footers))]
    [Header("Using Footers")]
    [Guide("After creating a footer for a specific document type (such as a sales invoice), you can apply it by selecting the `Footers` field when editing that document.")]
    [Guide("To automatically apply one or more footers to new transactions, use the `Form Defaults` feature.")]
    [LinkGuide("For more information, see:", typeof(NakedObjectsWithCreateNewAndFormDefaultsButtons<>))]
    internal sealed class Footers : NakedNamespaces
    {
    }
}
