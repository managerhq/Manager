using System;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings
{
    [ProtoContract]
    [Title(nameof(Strings.Settings))]
    [Guide("The `Settings` tab is your control center for configuring Manager to match your business needs.")]
    [Guide("Here you can customize how Manager works, from basic preferences to advanced features.")]
    [Guide("Settings affect your entire business file and determine what features and options are available throughout the program.")]
    [Header("Understanding the Layout")]
    [TabScreenshot("fa-cog", nameof(Strings.Settings))]
    [Guide("The `Settings` screen uses an intuitive two-part layout to help you manage features:")]
    [Guide("The upper section shows settings and features you're currently using, making them easy to access and modify.")]
    [Guide("The lower section displays available features you haven't activated yet.")]
    [Guide("To start using any new feature, simply click on it in the lower section. No complex setup is required.")]
    [Guide("As you activate features, they automatically move to the upper section for easy management.")]
    [Header("Feature Categories")]
    [Guide("The `Settings` tab organizes features into logical categories:")]
    [Namespace(typeof(Settings))]
    [Header("Default Settings")]
    [Guide("New businesses start with three essential settings already active:")]
    [Guide("• `Business Details` - Your company name, address, and contact information that appears on documents")]
    [Guide("• `Chart of Accounts` - The financial structure that organizes your income, expenses, assets, and liabilities")]
    [Guide("• `Date & Number Format` - How dates and numbers display throughout Manager based on your location")]
    [Guide("These core settings provide the foundation for your accounting system.")]
    internal sealed class Settings : NakedNamespaces
    {
    }
}
