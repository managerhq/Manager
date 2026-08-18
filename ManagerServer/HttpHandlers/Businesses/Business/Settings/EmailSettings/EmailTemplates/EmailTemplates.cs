using System;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.EmailTemplates))]
    [Guide("Email templates allow you to create predefined messages that automatically appear when sending transaction forms via email.")]
    [Guide("Instead of typing the same message repeatedly, you can set up templates that include your standard email content, saving time and ensuring consistency in your business communications.")]
    [Guide("Email templates can be used with various transaction forms such as *sales invoices*, *purchase orders*, *quotes*, and other documents that you regularly send to customers or suppliers.")]
    internal sealed class EmailTemplates : NakedNamespaces
    {
    }
}
