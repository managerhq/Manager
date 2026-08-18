using ManagerServer.Model;
using ManagerServer.HttpHandlers.Businesses.Business.Customers;
using ManagerServer.HttpHandlers.Businesses.Business.Employees;
using ManagerServer.HttpHandlers.Businesses.Business.Suppliers;
using Sentry.AspNetCore;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.JournalEntries
{
    [ProtoContract]
    [Title(nameof(Strings.JournalEntries))]
    [Guide("This utility resolves currency conversion issues that may occur in journal entries when working with control accounts and foreign currencies.")]
    [Guide("The utility automatically identifies journal entries where the currency settings conflict with the currency assigned to customers, suppliers, employees, or special accounts.")]
    [Header("What This Utility Does")]
    [Guide("When a journal entry is recorded in a foreign currency but references a control account (such as *Accounts receivable*, *Accounts payable*, *Employee clearing account*, or *Special accounts*) for an entity that uses the base currency, this can create incorrect currency conversions.")]
    [Guide("This utility scans all journal entries and corrects these mismatches by converting the foreign currency amounts back to base currency amounts where appropriate.")]
    [Header("When to Use This Utility")]
    [Guide("Use this utility if you notice incorrect balances in control accounts after recording journal entries that involve both foreign currencies and entities (customers, suppliers, or employees) that operate in your base currency.")]
    [Guide("This is particularly useful after importing data or when correcting historical entries that were recorded with incorrect currency settings.")]
    internal sealed class FixJournalEntries : BusinessTemplate
    {
        protected override void InnerGet2()
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var list = new List<ManagerServer.Model.JournalEntry>();
            foreach (var e in database.OfType<JournalEntry>())
            {

                if (!e.Currency.HasValue) continue;
                var foreignCurrency = database.SingleOrDefault<ForeignCurrency>(e.Currency.Value);
                if (foreignCurrency == null) continue;

                var balanceSheetAccountsReceivableAccount = database.Single<BalanceSheetAccountsReceivableAccount>();
                var balanceSheetAccountsPayableAccount = database.Single<BalanceSheetAccountsPayableAccount>();
                var balanceSheetEmployeeClearingAccount = database.Single<BalanceSheetEmployeeClearingAccount>();
                var balanceSheetSpecialAccountsAccount = database.Single<BalanceSheetSpecialAccountsAccount>();

                var convert = false;

                foreach (var e2 in e.Lines)
                {
                    if (e2.Debit == 0m && e2.Credit == 0m) continue;
                    if (e2.CurrencyAmount == 0m) continue;
                    if (!e2.Account.HasValue) continue;
                    var account = database.SingleOrDefault<NamedObject>(e2.Account.Value);

                    if (balanceSheetAccountsReceivableAccount.Key == e2.Account.Value || account is ControlAccountForCustomers)
                    {
                        var customer = database.SingleOrDefault<Customer>(e2.AccountsReceivableCustomer);
                        if (customer == null) continue;
                        if (customer.Currency.HasValue) continue;
                        convert = true;
                        break;
                    }
                    if (balanceSheetAccountsPayableAccount.Key == e2.Account.Value || account is ControlAccountForEmployees)
                    {
                        var supplier = database.SingleOrDefault<Supplier>(e2.AccountsPayableSupplier);
                        if (supplier == null) continue;
                        if (supplier.Currency.HasValue) continue;
                        convert = true;
                        break;
                    }
                    if (balanceSheetEmployeeClearingAccount.Key == e2.Account.Value || account is ControlAccountForEmployees)
                    {
                        var employee = database.SingleOrDefault<Employee>(e2.Employee);
                        if (employee == null) continue;
                        if (employee.Currency.HasValue) continue;
                        convert = true;
                        break;
                    }
                    if (balanceSheetSpecialAccountsAccount.Key == e2.Account.Value || account is ControlAccountForSpecialAccounts)
                    {
                        var specialAccount = database.SingleOrDefault<SpecialAccount>(e2.SpecialAccount);
                        if (specialAccount == null) continue;
                        if (specialAccount.Currency.HasValue) continue;
                        convert = true;
                        break;
                    }
                }

                if (convert)
                {
                    e.Currency = null;
                    foreach (var e2 in e.Lines)
                    {
                        if (e2.Debit == 0m && e2.Credit == 0m) continue;

                        var debit = e2.Debit;
                        var credit = e2.Credit;

                        e2.Debit = baseCurrency.GetBaseAmount(e2.Debit, e.ExchangeRate, e.ExchangeRateIsInverse, foreignCurrency);
                        e2.Credit = baseCurrency.GetBaseAmount(e2.Credit, e.ExchangeRate, e.ExchangeRateIsInverse, foreignCurrency);                        

                        if (e2.CurrencyAmount != 0m)
                        {
                            var account = database.SingleOrDefault<NamedObject>(e2.Account);

                            if (balanceSheetAccountsReceivableAccount.Key == e2.Account.Value || account is ControlAccountForCustomers)
                            {
                                var customer = database.SingleOrDefault<Customer>(e2.AccountsReceivableCustomer);
                                if (customer == null) continue;
                                if (customer.Currency.HasValue)
                                {
                                    if (customer.Currency == foreignCurrency.Key) e2.CurrencyAmount = debit + credit;
                                }
                                else
                                {
                                    if (debit > 0m) e2.Debit = e2.CurrencyAmount;
                                    else if (credit > 0m) e2.Credit = e2.CurrencyAmount;
                                    e2.CurrencyAmount = 0m;
                                }
                            }
                            else if (balanceSheetAccountsPayableAccount.Key == e2.Account.Value || account is ControlAccountForEmployees)
                            {
                                var supplier = database.SingleOrDefault<Supplier>(e2.AccountsPayableSupplier);
                                if (supplier == null) continue;
                                if (supplier.Currency.HasValue)
                                {
                                    if (supplier.Currency == foreignCurrency.Key) e2.CurrencyAmount = debit + credit;
                                }
                                else
                                {
                                    if (debit > 0m) e2.Debit = e2.CurrencyAmount;
                                    else if (credit > 0m) e2.Credit = e2.CurrencyAmount;
                                    e2.CurrencyAmount = 0m;
                                }
                            }
                            else if (balanceSheetEmployeeClearingAccount.Key == e2.Account.Value || account is ControlAccountForEmployees)
                            {
                                var employee = database.SingleOrDefault<Employee>(e2.Employee);
                                if (employee == null) continue;
                                if (employee.Currency.HasValue)
                                {
                                    if (employee.Currency == foreignCurrency.Key) e2.CurrencyAmount = debit + credit;
                                }
                                else
                                {
                                    if (debit > 0m) e2.Debit = e2.CurrencyAmount;
                                    else if (credit > 0m) e2.Credit = e2.CurrencyAmount;
                                    e2.CurrencyAmount = 0m;
                                }
                            }
                            else if (balanceSheetSpecialAccountsAccount.Key == e2.Account.Value || account is ControlAccountForSpecialAccounts)
                            {
                                var specialAccount = database.SingleOrDefault<SpecialAccount>(e2.SpecialAccount);
                                if (specialAccount == null) continue;
                                if (specialAccount.Currency.HasValue)
                                {
                                    if (specialAccount.Currency == foreignCurrency.Key) e2.CurrencyAmount = debit + credit;
                                }
                                else
                                {
                                    if (debit > 0m) e2.Debit = e2.CurrencyAmount;
                                    else if (credit > 0m) e2.Credit = e2.CurrencyAmount;
                                    e2.CurrencyAmount = 0m;
                                }
                            }
                            else
                            {
                                e2.CurrencyAmount = 0m;
                            }
                        }
                    }

                    var outOfBalance = e.Lines.Sum(x => x.Debit - x.Credit);
                    if (outOfBalance != 0m)
                    {
                        var line = e.Lines.First(x => x.CurrencyAmount != 0m);
                        if (line.Debit > 0m)
                        {
                            line.Debit -= outOfBalance;
                        }
                        else if (line.Credit > 0m)
                        {
                            line.Credit += outOfBalance;
                        }
                    }

                    list.Add(e);
                }
            }
            ApplicationData.Businesses.Process(Business, list.ToArray(), GetCurrentUser().Name);
            Write("OK");
        }
    }
}
