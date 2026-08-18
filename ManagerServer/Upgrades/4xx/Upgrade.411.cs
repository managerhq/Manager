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
        private static async Task<IEnumerable<Model.Object>> Upgrade411(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var startingBalanceJournalEntry = objects.OfType<JournalEntry>().Where(x => x.Date == new DateTime(2, 1, 1)).FirstOrDefault();

            if (startingBalanceJournalEntry == null) return null;

            var list = new List<ManagerServer.Model.Object>();
            var fixedAssetsAtCost = objects.Single<BalanceSheetFixedAssetsAtCostAccount>();
            var fixedAssetsDepreciation = objects.Single<BalanceSheetFixedAssetsAccumulatedDepreciationAccount>();
            var cashAndCashEquivalents = objects.Single<BalanceSheetCashAtBankAccount>();
            var retainedEarnings = objects.Single<BalanceSheetRetainedEarningsAccount>();
            var balanceSheetAccounts = new HashSet<Guid>(objects.OfType<ManagerServer.Model.BalanceSheetAccount>().Select(x => x.Key));

            var lines = startingBalanceJournalEntry.Lines.ToList();
            foreach (var e2 in lines.ToArray())
            {
                if (startingBalanceJournalEntry.ItemColumn && startingBalanceJournalEntry.QuantityColumn && e2.Item.HasValue && e2.Qty != 0m)
                {
                    var averageCost = (e2.Debit - e2.Credit) / e2.Qty;
                    list.Add(new InventoryItemStartingBalance()
                    {
                        Key = Guid.CreateVersion7(),
                        InventoryItem = e2.Item,
                        HasQtyToOnHand = true,
                        QtyOnHandLines = new InventoryItemStartingBalance.QtyOnHandLine[] {
                                new InventoryItemStartingBalance.QtyOnHandLine() {
                                    QtyOnHand = e2.Qty,
                                    InventoryLocation = e2.InventoryLocation
                                }
                            },
                        AverageCost = averageCost
                    });
                    lines.Remove(e2);
                }

                if (!e2.Item.HasValue && e2.Account.HasValue)
                {
                    if (e2.Account == fixedAssetsAtCost.Key)
                    {
                        list.Add(new FixedAssetStartingBalance()
                        {
                            Key = Guid.CreateVersion7(),
                            FixedAsset = e2.FixedAsset,
                            StartingBalance = e2.Debit - e2.Credit
                        });
                        lines.Remove(e2);
                    }

                    if (e2.Account == fixedAssetsDepreciation.Key)
                    {
                        list.Add(new FixedAssetStartingBalance()
                        {
                            Key = Guid.CreateVersion7(),
                            FixedAsset = e2.FixedAsset,
                            StartingBalanceAccumulatedDepreciation = e2.Credit - e2.Debit
                        });
                        lines.Remove(e2);
                    }

                    if (e2.Account == cashAndCashEquivalents.Key)
                    {
                        var startingBalance = e2.Debit - e2.Credit;
                        if (e2.CurrencyAmount != 0m)
                        {
                            if (startingBalance < 0m)
                            {
                                startingBalance = -e2.CurrencyAmount;
                            }
                            else
                            {
                                startingBalance = e2.CurrencyAmount;
                            }
                        }
                        list.Add(new BankOrCashAccountStartingBalance()
                        {
                            Key = Guid.CreateVersion7(),
                            BankOrCashAccount = e2.BankOrCashAccount,
                            StartingBalance = startingBalance
                        });
                        lines.Remove(e2);
                    }

                    if (e2.Account == retainedEarnings.Key || balanceSheetAccounts.Contains(e2.Account.Value))
                    {
                        if (e2.Debit != 0m)
                        {
                            list.Add(new BalanceSheetAccountStartingBalance()
                            {
                                Key = Guid.CreateVersion7(),
                                BalanceSheetAccount = e2.Account,
                                DebitCredit = DebitCredit.Debit,
                                StartingBalance = e2.Debit
                            });
                        }
                        if (e2.Credit != 0m)
                        {
                            list.Add(new BalanceSheetAccountStartingBalance()
                            {
                                Key = Guid.CreateVersion7(),
                                BalanceSheetAccount = e2.Account,
                                DebitCredit = DebitCredit.Credit,
                                StartingBalance = e2.Credit
                            });
                        }
                        lines.Remove(e2);
                    }
                }
            }

            if (lines.Count == 0)
            {
                startingBalanceJournalEntry.Lines = lines.ToArray();
                list.Add(startingBalanceJournalEntry);
                return list;
            }

            return null;
        }
    }
}
