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
        private static async Task<IEnumerable<Model.Object>> Upgrade169(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var bankAccounts = objects.OfType<ManagerServer.Model.BankOrCashAccount>().ToDictionary(x => x.Key);
            var cashAccounts = objects.OfType<ManagerServer.Model.Obsolete.Obsolete78.CashAccount>().ToDictionary(x => x.Key);
            var customFields = objects.OfType<ManagerServer.Model.CustomField>().Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToDictionary(x => x.Key);

            var bankTransactionCustomFields = new Dictionary<string, ManagerServer.Model.CustomField>();
            var cashTransactionCustomFields = new Dictionary<string, ManagerServer.Model.CustomField>();

            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().ToArray())
            {
                var newCustomFields = new Dictionary<Guid, string>();

                if (!e.CreditAccount.HasValue) continue;
                else if (bankAccounts.ContainsKey(e.CreditAccount.Value))
                {
                    if (e.CustomFields != null && e.CustomFields.Count > 0)
                    {
                        foreach (var e2 in e.CustomFields)
                        {
                            if (!string.IsNullOrWhiteSpace(e2.Value) && customFields.ContainsKey(e2.Key))
                            {
                                var customField = customFields[e2.Key];
                                if (!bankTransactionCustomFields.ContainsKey(customField.Name))
                                {
                                    bankTransactionCustomFields.Add(customField.Name, new Model.CustomField() { Key = Guid.CreateVersion7(), Obsolete_DefaultValue = customField.Obsolete_DefaultValue, Obsolete_DisplayOnList = customField.Obsolete_DisplayOnList, DisplayOnView = customField.DisplayOnView, OptionsForDropdownList = customField.OptionsForDropdownList, Size = customField.Size, Type = customField.Type, Position = customField.Position, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment)), Name = customField.Name });
                                    newCustomFields.Add(bankTransactionCustomFields[customField.Name].Key, e2.Value);
                                }
                            }
                        }
                    }

                    if (newCustomFields.Count == 0) newCustomFields = null;
                    list.Add(new ManagerServer.Model.Obsolete.Obsolete42.BankPayment42() { Key = e.Key, BankAccount = e.CreditAccount.Value, BankClearDate = e.BankClearDate, BankClearStatus = e.BankClearStatus, Payee = e.Payee, CustomFields = newCustomFields, Date = e.Date, Description = e.Description, InventoryLocation = e.InventoryLocation, Lines = e.Lines, Obsolete_Notes = e.Notes, Reference = e.Reference, Obsolete_Payment = e });
                }
                else if (cashAccounts.ContainsKey(e.CreditAccount.Value))
                {
                    if (e.CustomFields != null && e.CustomFields.Count > 0)
                    {
                        foreach (var e2 in e.CustomFields)
                        {
                            if (!string.IsNullOrWhiteSpace(e2.Value) && customFields.ContainsKey(e2.Key))
                            {
                                var customField = customFields[e2.Key];
                                if (!cashTransactionCustomFields.ContainsKey(customField.Name))
                                {
                                    cashTransactionCustomFields.Add(customField.Name, new Model.CustomField() { Key = Guid.CreateVersion7(), Obsolete_DefaultValue = customField.Obsolete_DefaultValue, Obsolete_DisplayOnList = customField.Obsolete_DisplayOnList, DisplayOnView = customField.DisplayOnView, OptionsForDropdownList = customField.OptionsForDropdownList, Size = customField.Size, Type = customField.Type, Position = customField.Position, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete44.CashTransaction44)), Name = customField.Name });
                                    newCustomFields.Add(cashTransactionCustomFields[customField.Name].Key, e2.Value);
                                }
                            }
                        }
                    }

                    if (newCustomFields.Count == 0) newCustomFields = null;
                    list.Add(new ManagerServer.Model.Obsolete.Obsolete43.CashPayment43() { Key = e.Key, CashAccount = e.CreditAccount.Value, Payee = e.Payee, CustomFields = newCustomFields, Date = e.Date, Description = e.Description, InventoryLocation = e.InventoryLocation, Lines = e.Lines, Obsolete_Notes = e.Notes, Reference = e.Reference, Obsolete_Payment = e });
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().ToArray())
            {
                var newCustomFields = new Dictionary<Guid, string>();

                if (!e.DebitAccount.HasValue) continue;
                else if (bankAccounts.ContainsKey(e.DebitAccount.Value))
                {
                    if (e.CustomFields != null && e.CustomFields.Count > 0)
                    {
                        foreach (var e2 in e.CustomFields)
                        {
                            if (!string.IsNullOrWhiteSpace(e2.Value) && customFields.ContainsKey(e2.Key))
                            {
                                var customField = customFields[e2.Key];
                                if (!bankTransactionCustomFields.ContainsKey(customField.Name))
                                {
                                    bankTransactionCustomFields.Add(customField.Name, new Model.CustomField() { Key = Guid.CreateVersion7(), Obsolete_DefaultValue = customField.Obsolete_DefaultValue, Obsolete_DisplayOnList = customField.Obsolete_DisplayOnList, DisplayOnView = customField.DisplayOnView, OptionsForDropdownList = customField.OptionsForDropdownList, Size = customField.Size, Type = customField.Type, Position = customField.Position, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment)), Name = customField.Name });
                                    newCustomFields.Add(bankTransactionCustomFields[customField.Name].Key, e2.Value);
                                }
                            }
                        }
                    }

                    if (newCustomFields.Count == 0) newCustomFields = null;
                    list.Add(new ManagerServer.Model.Obsolete.Obsolete42.BankReceipt42() { Key = e.Key, BankAccount = e.DebitAccount.Value, BankClearDate = e.BankClearDate, BankClearStatus = e.BankClearStatus, Payer = e.Payer, CustomFields = newCustomFields, Date = e.Date, Description = e.Description, InventoryLocation = e.InventoryLocation, Lines = e.Lines, Obsolete_Notes = e.Notes, Reference = e.Reference, Obsolete_Receipt = e });
                }
                else if (cashAccounts.ContainsKey(e.DebitAccount.Value))
                {
                    if (e.CustomFields != null && e.CustomFields.Count > 0)
                    {
                        foreach (var e2 in e.CustomFields)
                        {
                            if (!string.IsNullOrWhiteSpace(e2.Value) && customFields.ContainsKey(e2.Key))
                            {
                                var customField = customFields[e2.Key];
                                if (!cashTransactionCustomFields.ContainsKey(customField.Name))
                                {
                                    cashTransactionCustomFields.Add(customField.Name, new Model.CustomField() { Key = Guid.CreateVersion7(), Obsolete_DefaultValue = customField.Obsolete_DefaultValue, Obsolete_DisplayOnList = customField.Obsolete_DisplayOnList, DisplayOnView = customField.DisplayOnView, OptionsForDropdownList = customField.OptionsForDropdownList, Size = customField.Size, Type = customField.Type, Position = customField.Position, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete44.CashTransaction44)), Name = customField.Name });
                                    newCustomFields.Add(cashTransactionCustomFields[customField.Name].Key, e2.Value);
                                }
                            }
                        }
                    }

                    if (newCustomFields.Count == 0) newCustomFields = null;
                    list.Add(new ManagerServer.Model.Obsolete.Obsolete43.CashReceipt43() { Key = e.Key, CashAccount = e.DebitAccount.Value, Payer = e.Payer, CustomFields = newCustomFields, Date = e.Date, Description = e.Description, InventoryLocation = e.InventoryLocation, Lines = e.Lines, Obsolete_Notes = e.Notes, Reference = e.Reference, Obsolete_Receipt = e });
                }
            }

            foreach (var e in bankTransactionCustomFields) list.Add(e.Value);
            foreach (var e in cashTransactionCustomFields) list.Add(e.Value);

            return list.ToArray();
        }
    }
}
