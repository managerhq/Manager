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
        private static async Task<IEnumerable<Model.Object>> Upgrade122(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().ToArray())
            {
                e.BankClearStatus = BankClearStatus.Cleared;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().ToArray())
            {
                e.BankClearStatus = BankClearStatus.Cleared;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.InterAccountTransfer>().ToArray())
            {
                e.Obsolete_DebitClearStatus = BankClearStatus.Cleared;
                e.Obsolete_CreditClearStatus = BankClearStatus.Cleared;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete16.UnclearedPayment16>().ToArray())
            {
                list.Add(new ManagerServer.Model.Obsolete.Obsolete33.Payment33()
                {
                    Key = e.Key,
                    BankClearStatus = BankClearStatus.Pending,
                    Date = e.Date,
                    Reference = e.Reference,
                    Payee = e.Payee,
                    Lines = e.Lines,
                    Description = e.Description,
                    CreditAccount = e.BankAccount
                });
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete16.UnclearedReceipt16>().ToArray())
            {
                list.Add(new ManagerServer.Model.Obsolete.Obsolete33.Receipt33()
                {
                    Key = e.Key,
                    BankClearStatus = BankClearStatus.Pending,
                    Date = e.Date,
                    Reference = e.Reference,
                    Lines = e.Lines,
                    Description = e.Description,
                    DebitAccount = e.BankAccount
                });
            }

            {
                var unclearedDepositsAccount = new Guid("F0CFA351-9635-475D-B2AA-BF8541803EB0");
                var unclearedReceipts = objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().Where(x => x.BankClearStatus == BankClearStatus.Pending && x.Lines != null).ToDictionary(x => x.Key);
                var unclearedReceiptAmounts = unclearedReceipts.Values.ToDictionary(x => x.Key, x => x.Lines.Sum(y => y.Amount));
                foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().Where(x => x.Lines != null && x.BankClearStatus == BankClearStatus.Cleared).ToArray())
                {
                    var lines = e.Lines.ToList();
                    foreach (var e2 in lines.ToArray())
                    {
                        if (e2.Account == unclearedDepositsAccount && e2.Obsolete_BankDeposit.HasValue && unclearedReceipts.ContainsKey(e2.Obsolete_BankDeposit.Value) && e2.Amount == unclearedReceiptAmounts[e2.Obsolete_BankDeposit.Value] && e.DebitAccount == unclearedReceipts[e2.Obsolete_BankDeposit.Value].DebitAccount)
                        {
                            lines.Remove(e2);
                            unclearedReceipts[e2.Obsolete_BankDeposit.Value].BankClearStatus = BankClearStatus.Cleared;
                            unclearedReceipts[e2.Obsolete_BankDeposit.Value].BankClearDate = e.Date;
                            list.Add(unclearedReceipts[e2.Obsolete_BankDeposit.Value]);
                        }
                    }
                    if (lines.Count != e.Lines.Length)
                    {
                        e.Lines = lines.ToArray();
                        list.Add(e);
                    }
                }
            }

            {
                var unclearedPaymentsAccount = new Guid("86fc4532-3633-4238-b1a1-5092daac9a9f");
                var unclearedPayments = objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().Where(x => x.BankClearStatus == BankClearStatus.Pending && x.Lines != null).ToDictionary(x => x.Key);
                var unclearedPaymentAmounts = unclearedPayments.Values.ToDictionary(x => x.Key, x => x.Lines.Sum(y => y.Amount));
                foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().Where(x => x.Lines != null && x.BankClearStatus == BankClearStatus.Cleared).ToArray())
                {
                    var lines = e.Lines.ToList();
                    foreach (var e2 in lines.ToArray())
                    {
                        if (e2.Account == unclearedPaymentsAccount && e2.Obsolete_Cheque.HasValue && unclearedPayments.ContainsKey(e2.Obsolete_Cheque.Value) && e2.Amount == unclearedPaymentAmounts[e2.Obsolete_Cheque.Value] && e.CreditAccount == unclearedPayments[e2.Obsolete_Cheque.Value].CreditAccount)
                        {
                            lines.Remove(e2);
                            unclearedPayments[e2.Obsolete_Cheque.Value].BankClearStatus = BankClearStatus.Cleared;
                            unclearedPayments[e2.Obsolete_Cheque.Value].BankClearDate = e.Date;
                            list.Add(unclearedPayments[e2.Obsolete_Cheque.Value]);
                        }
                    }
                    if (lines.Count != e.Lines.Length)
                    {
                        e.Lines = lines.ToArray();
                        list.Add(e);
                    }
                }
            }
            return list;
        }
    }
}
