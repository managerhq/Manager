using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Currencies.BaseCurrency
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.BaseCurrency))]
    [Guide("The `BaseCurrency` form is where you set the base currency for your business.")]
    [Guide("The base currency is the home currency of your business.")]
    [Guide("By default, every account is automatically configured to use the base currency, and all financial statements are presented in this currency.")]
    [Guide("To access `BaseCurrency` form, go to `Settings` tab, then `Currencies`.")]
    [SettingsItemScreenshot(icon: "fa-coin", name: nameof(Strings.Currencies))]
    [Guide("Then click `BaseCurrency`.")]
    [Guide("The form has the following fields:")]
    [Fields(typeof(ManagerServer.Model.BaseCurrency))]
    [Guide("Users can also change the base currency for an existing business.")]
    [Guide("This is rare requirement since typically businesses will have single base currency throughout its lifetime.")]
    [Guide("The process to change base currency involves several steps to ensure all financial data remains accurate and consistent.")]
    [Guide("Follow these steps to change the base currency:")]
    [Guide("**Update information on your `BaseCurrency` form to reflect new currency:**")]
    [Guide("- Go to `Settings` tab, then `Currencies`, then `BaseCurrency`")]
    [Guide("- Set new code, name, currency symbol and decimal places (if applicable)")]
    [Guide("**Create Previous Base Currency as a Foreign Currency:**")]
    [Guide("- Go to `Settings` tab, then `Currencies`, then `ForeignCurrencies`.")]
    [Guide("- Add the previous base currency as a new `ForeignCurrency`.")]
    [Guide("**Review and Update Currency for Balance Sheet Sub-Accounts:**")]
    [Guide("- Go to the `BankAndCashAccounts` tab and ensure bank and cash accounts that were previously using the base currency are now using the newly created foreign currency.")]
    [Guide("- Repeat this process under the `Customers`, `Suppliers`, `Employees`, and `SpecialAccounts` tabs.")]
    [Guide("**Review and Update Currency for Transactions:**")]
    [Guide("- Go to the `JournalEntries` tab and ensure all journal entries that were previously using the base currency are set to use the newly created foreign currency.")]
    [Guide("- Do the same under the `ExpenseClaims` tab if applicable.")]
    [Guide("**Update Exchange Rates:**")]
    [Guide("- Note that all previously entered exchange rates are now incorrect because they were based on the old base currency. These need to be updated.")]
    [Guide("- Update all exchange rates that reflect the new base currency.")]
    [Guide("**Batch Update Transactions:**")]
    [Guide("- After updating the exchange rates, batch update all transactions that used the old exchange rates to ensure they now use the new rates.")]
    [Guide("By following these steps, you can successfully change the base currency for your existing business, ensuring all financial data is accurately represented in the new base currency.")]
    internal sealed class BaseCurrencyForm : NakedVueForm<ManagerServer.Model.BaseCurrency>
    {
        internal override bool IsEmpty(ManagerServer.Helpers.TabsExtensions.Item[] tabs)
        {
            return !ApplicationData.Businesses.Get(Business).Exists<ManagerServer.Model.BaseCurrency>();
        }
    }
}
