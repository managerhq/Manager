using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("d64a6a73-731b-4fd6-9854-4744373e4531")]
    public sealed class CustomButton : NamedObject, IComparable<CustomButton>
    {
        [Guide("Enter a descriptive name to identify this extension. This name appears in the list of extensions and helps you manage multiple extensions.")]
        [Guide("Use clear names like 'Customer signature pad' or 'Inventory barcode scanner' to indicate the extension's purpose.")]
        [ProtoMember(1)] public string Name { get; set; }
        [Guide("Select how you want to add custom functionality to Manager:")]
        [Guide("`ExternalURL` - Embed content from another website or web application using an iframe")]
        [Guide("`CustomHTML` - Write your own HTML, CSS, and JavaScript code directly")]
        [ProtoMember(5)] public ExtensionSource Source { get; set; }
        [Guide("Enter the full URL of the external web page or application you want to embed. This creates an iframe that displays the external content.")]
        [Guide("The URL must use HTTPS for security. Common uses include embedding calculators, maps, or third-party tools.")]
        [Guide("The external page will have access to form data through URL parameters and can interact with Manager using JavaScript.")]
        [ProtoMember(2), Prepend("https://"), IfEnum(nameof(Source), 0)] public string Endpoint { get; set; }
        [Guide("Write custom HTML code that will be inserted directly into the Manager page. You can include HTML, CSS styles, and JavaScript.")]
        [Guide("Your code has access to form data and can interact with Manager's interface. Use this for custom buttons, calculations, or UI enhancements.")]
        [Guide("Common uses include adding custom validation, auto-calculations, or integration with external services via JavaScript.")]
        [ProtoMember(6), Code, IfEnum(nameof(Source), 1)] public string Html { get; set; }
        [Guide("Specify where this extension should appear within Manager. Enter the path segment that identifies the target page.")]
        [Guide("Common placements include:")]
        [Guide("- 'receipt-form' - Appears when creating or editing receipts")]
        [Guide("- 'payment-form' - Appears when creating or editing payments")]
        [Guide("- 'sales-invoice-form' - Appears when creating or editing sales invoices")]
        [Guide("- 'sales-invoice-view' - Appears when viewing sales invoices")]
        [Guide("- 'customer-form' - Appears when creating or editing customers")]
        [Guide("The extension will only load on pages matching this exact path.")]
        [ProtoMember(3), Prepend("/")] public string Placement { get; set; }
        [Guide("Check this box to temporarily disable this extension without deleting it. Inactive extensions won't load on any pages.")]
        [Guide("This is useful for troubleshooting or when you need to temporarily turn off custom functionality.")]
        [ProtoMember(4)] public bool Inactive { get; set; }

        public override string GetName()
        {
            return Name;
        }

        public bool IsMatch(string path)
        {
            if (Inactive) return false;
            else if (string.IsNullOrWhiteSpace(Placement)) return false;
            else if (path == '/' + Placement) return true;
            else return false;
        }

        public override bool IsInactive()
        {
            return Inactive;
        }

        int IComparable<CustomButton>.CompareTo(CustomButton other) => (Inactive, Name).CompareTo((other.Inactive, other.Name));
    }
}