using System;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.StartingBalances))]
    [Guide("The *Starting Balances* feature, found under the **Settings** tab, allows you to set up starting balances for all your accounts and subsidiary ledgers.")]
    [SettingsItemScreenshot("fa-wand-magic-sparkles", nameof(Strings.StartingBalance))]
    [Guide("Many users prefer to establish their starting balances using a journal entry. However, this can result in overly long journal entries.")]
    [Guide("Starting balances involve more than just debits and credits. If you are using the **Inventory Items** tab, you might want to set up starting balances for *Qty on Hand*, *Qty to Deliver*, and *Qty to Receive*. These are starting balances for management purposes, not accounting purposes.")]
    [Namespace(typeof(StartingBalances))]
    internal sealed class StartingBalances : NakedNamespaces
    {
    }
}
