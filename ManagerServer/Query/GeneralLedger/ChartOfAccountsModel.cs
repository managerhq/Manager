using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Master;

namespace ManagerServer.Query.GeneralLedger
{
    public sealed class ChartOfAccountsModel
    {
        public ChartOfAccountsModel(string fileId)
        {
            var database = ApplicationData.Instance.Businesses.Get(fileId);

            var balanceSheetGroups = database.UnorderedOfType<ManagerServer.Model.BalanceSheetGroup>().Select(x => new Group() { Key = x.Key, Name = x.Name, Position = x.Position }).ToDictionary(x => x.Key);
            balanceSheetGroups.Add(ChartOfAccountGroups.Assets, new Group() { Key = ChartOfAccountGroups.Assets, Name = Strings.Assets, Position = 0 });
            balanceSheetGroups.Add(ChartOfAccountGroups.Liabilities, new Group() { Key = ChartOfAccountGroups.Liabilities, Name = Strings.Liabilities, Position = 1 });
            balanceSheetGroups.Add(ChartOfAccountGroups.Equity, new Group() { Key = ChartOfAccountGroups.Equity, Name = Strings.Equity, Position = 2 });
            
            balanceSheetGroups.Add(Guid.Empty, new Group() { Key = Guid.Empty, Name = Strings.Uncategorized, Parent = balanceSheetGroups[ChartOfAccountGroups.Equity], Position = int.MaxValue });
            balanceSheetGroups[ChartOfAccountGroups.Equity].Items.Add(balanceSheetGroups[Guid.Empty]);

            var equity = database.Single<ManagerServer.Model.Equity>();
            if (!string.IsNullOrWhiteSpace(equity.Name))
            {
                balanceSheetGroups[ChartOfAccountGroups.Equity].Name = equity.Name;
            }

            foreach (var e in database.UnorderedOfType<ManagerServer.Model.BalanceSheetGroup>().ToArray())
            {
                var group = balanceSheetGroups[e.Key];
                if (e.Group.HasValue && e.Group.Value != e.Key && balanceSheetGroups.ContainsKey(e.Group.Value))
                {
                    group.Parent = balanceSheetGroups[e.Group.Value];
                }
                else
                {
                    group.Parent = balanceSheetGroups[Guid.Empty];
                }
                group.Parent.Items.Add(group);
            }

            var profitAndLossStatementGroups = database.UnorderedOfType<ManagerServer.Model.ProfitAndLossStatementGroup>().Select(x => new Group() { Key = x.Key, Name = x.Name, IsExpenseGroup = (x.Type == ManagerServer.Model.Enums.ProfitAndLossStatementGroupType.ExpenseGroup), Position = x.Position }).ToDictionary(x => x.Key);
            profitAndLossStatementGroups.Add(Guid.Empty, new Group() { Key = Guid.Empty, Name = Strings.Uncategorized, Position = int.MaxValue });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ProfitAndLossStatementGroup>().ToArray())
            {
                var group = profitAndLossStatementGroups[e.Key];
                if (e.Type == ManagerServer.Model.Enums.ProfitAndLossStatementGroupType.SubgroupOf && e.Group.HasValue && e.Group.Value != e.Key && profitAndLossStatementGroups.ContainsKey(e.Group.Value))
                {
                    group.Parent = profitAndLossStatementGroups[e.Group.Value];
                    group.Parent.Items.Add(group);
                }
            }
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.Subtotal>().ToArray())
            {
                profitAndLossStatementGroups.Add(e.Key, new Group() { Key = e.Key, Name = e.Name, IsSubtotal = true, Position = e.Position });
            }

            var profitAndLossStatementTotal = database.Single<ProfitAndLossStatementTotal>();
            profitAndLossStatementGroups.Add(profitAndLossStatementTotal.Key, new Group() { Key = profitAndLossStatementTotal.Key, Name = profitAndLossStatementTotal.GetName(), IsSubtotal = true, Position = int.MaxValue });

            var accounts = ManagerServer.Query.GeneralLedgerAccounts.GetAccounts(fileId);

            foreach (var e in accounts)
            {
                if (e.IsProfitAndLossAccount)
                {
                    if (e.Group.HasValue && profitAndLossStatementGroups.ContainsKey(e.Group.Value))
                    {
                        profitAndLossStatementGroups[e.Group.Value].Items.Add(new Account() { Key = e.Key, SystemName = e.SystemName, IsSystemAccount = e.IsSystemAccount, Name = e.Name, Code = e.Code, Position = e.Position, Parent = profitAndLossStatementGroups[e.Group.Value], TaxCode = e.TaxCode, Inactive = e.Inactive });
                    }
                    else
                    {
                        profitAndLossStatementGroups[Guid.Empty].Items.Add(new Account() { Key = e.Key, SystemName = e.SystemName, IsSystemAccount = e.IsSystemAccount, Name = e.Name, Code = e.Code, Position = e.Position, Parent = profitAndLossStatementGroups[Guid.Empty], TaxCode = e.TaxCode, Inactive = e.Inactive });
                    }
                }
                else
                {
                    if (e.Group.HasValue && balanceSheetGroups.ContainsKey(e.Group.Value))
                    {
                        balanceSheetGroups[e.Group.Value].Items.Add(new Account() { Key = e.Key, SystemName = e.SystemName, IsSystemAccount = e.IsSystemAccount, Name = e.Name, Code = e.Code, Position = e.Position, Parent = balanceSheetGroups[e.Group.Value], ControlAccountType = e.ControlAccountType, TaxCode = e.TaxCode, Inactive = e.Inactive });
                    }
                    else
                    {
                        balanceSheetGroups[Guid.Empty].Items.Add(new Account() { Key = e.Key, SystemName = e.SystemName, IsSystemAccount = e.IsSystemAccount, Name = e.Name, Code = e.Code, Position = e.Position, Parent = balanceSheetGroups[Guid.Empty], ControlAccountType = e.ControlAccountType, TaxCode = e.TaxCode, Inactive = e.Inactive });
                    }
                }
            }

            foreach (var e in balanceSheetGroups.Values.Where(x => !x.HasRoot()))
            {
                e.Parent.Items.Remove(e);
                e.Parent = balanceSheetGroups[Guid.Empty];
                balanceSheetGroups[Guid.Empty].Items.Add(e);
            }
            foreach (var e in profitAndLossStatementGroups.Values.Where(x => !x.HasRoot()))
            {
                e.Parent.Items.Remove(e);
                e.Parent = null;
            }

            if (profitAndLossStatementGroups[Guid.Empty].Items.Count == 0) profitAndLossStatementGroups.Remove(Guid.Empty);
            if (balanceSheetGroups[Guid.Empty].Items.Count == 0)
            {
                balanceSheetGroups[Guid.Empty].Parent.Items.Remove(balanceSheetGroups[Guid.Empty]);
                balanceSheetGroups.Remove(Guid.Empty);
            }

            BalanceSheet = balanceSheetGroups.Values.Where(x => x.Parent == null).ToArray();
            ProfitAndLossStatement = profitAndLossStatementGroups.Values.Where(x => x.Parent == null).OrderBy(x => x.Position).ThenBy(x => x.Name).ToArray();

            foreach (var e in GetAllGroups())
            {
                e.SortItems();
                foreach (var e2 in e.GetAllGroups())
                {
                    e2.SortItems();
                }
            }
        }

        public Group[] ProfitAndLossStatement;
        public Group[] BalanceSheet;

        public Group[] GetAllGroups()
        {
            var list = new List<Group>();
            list.AddRange(ProfitAndLossStatement);
            list.AddRange(BalanceSheet);
            return list.ToArray();
        }

        public abstract class Item
        {
            public Guid Key;
            public string Name;
            public string Code;
            public Group Parent;
            public int Position;
            public bool Inactive;

            public string NameWithCode
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(Code)) return Code + " " + Name;
                    return Name;
                }
            }
        }

        public sealed class Account : Item
        {
            public string SystemName;
            public bool IsSystemAccount;
            public Guid? TaxCode;
            public Guid? Currency;
            public ManagerServer.Model.Enums.ControlAccountType? ControlAccountType;
            public ManagerServer.Model.Enums.CashFlowStatementCategory CashFlowStatementCategory;
        }

        public sealed class Group : Item
        {
            public List<Item> Items = new List<Item>();
            public bool IsSubtotal;
            public bool IsExpenseGroup;

            public void SortItems()
            {
                Items = Items.OrderBy(x => x.Position).ThenBy(x => x.Name).ToList();
            }

            public bool HasRoot()
            {
                var parents = new List<Group>();
                parents.Add(this);
                var parent = Parent;
                while (parent != null)
                {
                    if (parents.Contains(parent)) return false;
                    parents.Add(parent);
                    parent = parent.Parent;
                }
                return true;
            }

            public Account[] GetAllAccounts()
            {
                var list = new List<Account>();
                foreach (var e in Items)
                {
                    if (e is Group)
                    {
                        list.AddRange(((Group)e).GetAllAccounts());
                    }
                    if (e is Account)
                    {
                        list.Add((Account)e);
                    }
                }
                return list.ToArray();
            }

            public Group[] GetAllGroups()
            {
                var list = new List<Group>();
                foreach (var e in Items)
                {
                    if (e is Group)
                    {
                        list.Add((Group)e);
                        list.AddRange(((Group)e).GetAllGroups());
                    }
                }
                return list.ToArray();
            }
        }
    }
}