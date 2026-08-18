using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using System.Text;
using System.IO;
using ManagerServer.Model;
using System.Reflection;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade1(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();

            var contacts = new Dictionary<string, Guid>();
            var reverseContacts = new Dictionary<Guid, string>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete01.Contact01>().Where(x => !string.IsNullOrWhiteSpace(x.Name)))
            {
                reverseContacts.Add(e.Key, e.Name);
                if (!contacts.ContainsKey(e.Name)) contacts.Add(e.Name, e.Key);
            }

            var taxCodes = new Dictionary<string, Guid>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete07.TaxCode07>().Where(x => !string.IsNullOrWhiteSpace(x.Code)))
            {
                if (!taxCodes.ContainsKey(e.Code)) taxCodes.Add(e.Code, e.Key);
            }

            var accounts = new Dictionary<string, Guid>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete01.GeneralLedgerAccount01>().Where(x => !string.IsNullOrWhiteSpace(x.Name)))
            {
                if (!accounts.ContainsKey(e.Name)) accounts.Add(e.Name, e.Key);
            }
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete01.SalesInvoice01>().Where(x => !string.IsNullOrWhiteSpace(x.To) && !string.IsNullOrWhiteSpace(x.Reference)))
            {
                var name = e.To + " " + e.Reference;
                if (!accounts.ContainsKey(name)) accounts.Add(name, e.Key);
            }
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete01.PurchaseInvoice01>().Where(x => !string.IsNullOrWhiteSpace(x.From) && !string.IsNullOrWhiteSpace(x.Reference)))
            {
                var name = e.From + " " + e.Reference;
                if (!accounts.ContainsKey(name)) accounts.Add(name, e.Key);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete02.SalesInvoice02>().Where(x => x.To.HasValue && reverseContacts.ContainsKey(x.To.Value) && !string.IsNullOrWhiteSpace(x.Reference)))
            {
                var name = reverseContacts[e.To.Value] + " " + e.Reference;
                if (!accounts.ContainsKey(name)) accounts.Add(name, e.Key);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete02.PurchaseInvoice02>().Where(x => x.From.HasValue && reverseContacts.ContainsKey(x.From.Value) && !string.IsNullOrWhiteSpace(x.Reference)))
            {
                var name = reverseContacts[e.From.Value] + " " + e.Reference;
                if (!accounts.ContainsKey(name)) accounts.Add(name, e.Key);
            }

            var australia_TaxCodes_GST_10_Key = new Guid("8cf9d117-3142-4d9c-82ee-b57a0e22c809");
            var unitedKingdom_TaxCodes_VAT_20_Key = new Guid("b926c2d8-09e4-496c-9a2c-818c8aaa36ed");
            var unitedKingdom_TaxCodes_VAT_05_Key = new Guid("56769971-405e-47bd-bd13-d64de0eae752");

            Func<string, Guid?> getTaxCode = x =>
            {
                if (string.IsNullOrWhiteSpace(x)) return null;

                if (x == "GST 10.00%") return australia_TaxCodes_GST_10_Key;
                if (x == "VAT 20.00%") return unitedKingdom_TaxCodes_VAT_20_Key;
                if (x == "VAT 5.00%") return unitedKingdom_TaxCodes_VAT_05_Key;

                if (taxCodes.ContainsKey(x)) return taxCodes[x];

                var rate = 0m;

                try
                {
                    var parts = x.Split(' ');
                    rate = decimal.Parse(parts.Last().Substring(0, parts.Last().Length - 1), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                }

                var taxCode = new Model.Obsolete.Obsolete07.TaxCode07() { Code = x, Rate = rate, Key = Guid.CreateVersion7() };
                list.Add(taxCode);
                taxCodes.Add(x, taxCode.Key);
                return taxCode.Key;
            };

            Func<string, Guid?> getAccount = x =>
            {
                if (string.IsNullOrWhiteSpace(x)) return null;
                if (accounts.ContainsKey(x)) return accounts[x];

                var account = new Model.Obsolete.Obsolete01.GeneralLedgerAccount01() { Name = x, Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Equity, Key = Guid.CreateVersion7() };
                list.Add(account);
                accounts.Add(x, account.Key);
                return account.Key;
            };

            Func<string, Guid?> getContact = x =>
            {
                if (string.IsNullOrWhiteSpace(x)) return null;
                if (contacts.ContainsKey(x)) return contacts[x];

                var contact = new Model.Obsolete.Obsolete01.Contact01() { Name = x, Key = Guid.CreateVersion7() };
                list.Add(contact);
                contacts.Add(x, contact.Key);
                return contact.Key;
            };

            var receipts = objects.OfType<Model.Obsolete.Obsolete01.Receipt01>().ToArray();
            var payments = objects.OfType<Model.Obsolete.Obsolete01.Payment01>().ToArray();
            var salesInvoices = objects.OfType<Model.Obsolete.Obsolete01.SalesInvoice01>().ToArray();
            var purchaseInvoices = objects.OfType<Model.Obsolete.Obsolete01.PurchaseInvoice01>().ToArray();
            var journalEntries = objects.OfType<Model.Obsolete.Obsolete01.JournalEntry01>().ToArray();

            foreach (var e in receipts)
            {
                var output = new Model.Obsolete.Obsolete02.Receipt02() { Key = e.Key };
                output.DebitAccount = getAccount(e.DebitAccount);
                output.Date = e.Date;
                output.Notes = e.Notes;
                output.Reference = e.Reference;
                output.From = e.From;
                output.Lines = (e.Lines ?? new Model.Obsolete.Obsolete01.Receipt01.ReceiptLine[0]).Select(x => new Model.Obsolete.Obsolete02.ReceiptLine() { Amount = x.Amount, CreditAccount = getAccount(x.CreditAccount), Tax = getTaxCode(x.Tax1) }).ToArray();
                list.Add(output);
            }

            foreach (var e in payments)
            {
                var output = new ManagerServer.Model.Obsolete.Obsolete02.Payment02() { Key = e.Key };
                output.CreditAccount = getAccount(e.CreditAccount);
                output.Date = e.Date;
                output.Notes = e.Notes;
                output.Reference = e.Reference;
                output.To = e.To;
                output.Lines = (e.Lines ?? new Model.Obsolete.Obsolete01.Payment01.PaymentLine[0]).Select(x => new ManagerServer.Model.Obsolete.Obsolete02.PaymentLine() { Amount = x.Amount, DebitAccount = getAccount(x.DebitAccount), Tax = getTaxCode(x.Tax1) }).ToArray();
                list.Add(output);
            }

            foreach (var e in salesInvoices)
            {
                var output = new ManagerServer.Model.Obsolete.Obsolete02.SalesInvoice02() { Key = e.Key };
                output.DueDate = e.DueDate;
                output.IssueDate = e.IssueDate;
                output.Reference = e.Reference;
                output.To = getContact(e.To);
                output.Notes = e.Notes;
                output.AmountsIncludeTax = e.AmountsIncludeTax;
                output.BillingAddress = e.BillingAddress;
                output.Lines = (e.Lines ?? new Model.Obsolete.Obsolete01.SalesInvoice01.SalesInvoiceLine[0]).Where(x => !string.IsNullOrWhiteSpace(x.Description) || ((x.Amount ?? 0m) != 0m)).Select(x => new ManagerServer.Model.Obsolete.Obsolete02.SalesInvoiceLine() { UnitPrice = x.Amount ?? 0, Qty = null, Item = getAccount(x.Item), Description = x.Description, Tax = getTaxCode(x.Tax1) }).ToArray();
                list.Add(output);
            }

            foreach (var e in purchaseInvoices)
            {
                var output = new ManagerServer.Model.Obsolete.Obsolete02.PurchaseInvoice02() { Key = e.Key };
                output.DueDate = e.DueDate;
                output.IssueDate = e.IssueDate;
                output.Reference = e.Reference;
                output.From = getContact(e.From);
                output.Notes = e.Notes;
                output.Lines = (e.Lines ?? new Model.Obsolete.Obsolete01.PurchaseInvoice01.PurchaseInvoiceLine[0]).Select(x => new ManagerServer.Model.Obsolete.Obsolete02.PurchaseInvoiceLine() { UnitPrice = x.Amount ?? 0, Qty = null, Item = getAccount(x.Item), Tax = getTaxCode(x.Tax1) }).ToArray();
                list.Add(output);
            }

            foreach (var e in journalEntries)
            {
                var output = new Model.Obsolete.Obsolete02.JournalEntry02() { Key = e.Key };
                output.Date = e.Date;
                output.IsReversing = e.IsReversing;
                output.Narration = e.Narration;
                output.Notes = e.Notes;
                output.Reference = e.Reference;
                output.Lines = (e.Lines ?? new Model.Obsolete.Obsolete01.JournalEntry01.JournalEntryLine[0]).Select(x => new Model.Obsolete.Obsolete02.JournalEntryLine() { Account = getAccount(x.Account), Credit = x.Credit, Debit = x.Debit, Tax = getTaxCode(x.Tax1) }).ToArray();
                list.Add(output);
            }

            return list.ToArray();
        }
    }
}
