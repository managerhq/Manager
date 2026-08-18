using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.RecurringTransactions))]
    [Guide("Recurring transactions automate the creation of transactions that occur regularly in your business. This feature saves time by automatically generating transactions like monthly rent payments, regular customer invoices, or periodic journal entries.")]
    [Guide("To access recurring transactions, navigate to the **Settings** tab and click **Recurring Transactions**. Here you can set up templates for transactions that need to be created on a regular schedule.")]
    [Guide("You can create recurring templates for *sales invoices*, *purchase invoices*, *payslips*, *journal entries*, and other transaction types. Each template can be configured to repeat daily, weekly, monthly, or at custom intervals that suit your business needs.")]
    [SettingsItemScreenshot("fa-repeat", nameof(Strings.RecurringTransactions))]
    internal sealed class RecurringTransactions : NakedNamespaces
    {
    }
}
