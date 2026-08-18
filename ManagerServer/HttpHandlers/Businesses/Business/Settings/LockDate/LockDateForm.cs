using System;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.LockDate
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.LockDate))]
    [Guide("The **Lock Date** feature, found under the **Settings** tab, enables you to specify a date beyond which transactions occurring on or before it cannot be edited.")]
    [SettingsItemScreenshot("fa-lock-alt", nameof(Strings.LockDate))]
    [Guide("Once the date is set, you can continue to make small adjustments to transactions, provided they do not alter the numbers in your financial statements.")]
    [Fields(typeof(ManagerServer.Model.LockDate))]
    internal sealed class LockDateForm : NakedVueForm<ManagerServer.Model.LockDate>
    {
        internal override bool IsEmpty(ManagerServer.Helpers.TabsExtensions.Item[] tabs)
        {
            return !ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.LockDate>().LockAccountingPeriods;
        }
    }
}
