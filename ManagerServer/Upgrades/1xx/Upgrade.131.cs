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
using ManagerServer.Model.Master;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade131(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var groups = new HashSet<Guid>();

            if (objects.OfType<ManagerServer.Model.BalanceSheet>().Any(x => x.Obsolete_Type == ManagerServer.Model.Obsolete.Obsolete18.BalanceSheetType18.Classified))
            {
                foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete18.ClassifiedBalanceSheetAssetGroup18>().ToArray())
                {
                    list.Add(new ManagerServer.Model.BalanceSheetGroup() { Obsolete_Code = e.Position, Name = e.Name, Key = e.Key, Group = ChartOfAccountGroups.Assets });
                    groups.Add(e.Key);
                }
                foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete18.ClassifiedBalanceSheetLiabilityGroup18>().ToArray())
                {
                    list.Add(new ManagerServer.Model.BalanceSheetGroup() { Obsolete_Code = e.Position, Name = e.Name, Key = e.Key, Group = ChartOfAccountGroups.Liabilities });
                    groups.Add(e.Key);
                }
            }

            if (objects.OfType<ManagerServer.Model.ProfitAndLossStatement>().Any(x => x.Obsolete_Type == ManagerServer.Model.Obsolete.Obsolete18.IncomeStatementType18.MultiStep))
            {
                var index = 1;
                foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete18.MultiStepIncomeStatementTotal18>().OrderBy(x => x.Position ?? int.MaxValue).ThenBy(x => x.Name).ToArray())
                {
                    foreach (var e2 in objects.OfType<ManagerServer.Model.Obsolete.Obsolete18.MultiStepIncomeStatementGroup18>().Where(x => x.MultiStepIncomeStatementTotal == e.Key).OrderBy(x => x.Position ?? int.MaxValue).ThenBy(x => x.Name).ToArray())
                    {
                        list.Add(new ManagerServer.Model.ProfitAndLossStatementGroup() { Obsolete_Code = index, Name = e2.Name, Key = e2.Key, Type = (e2.IsExpense ? ProfitAndLossStatementGroupType.ExpenseGroup : ProfitAndLossStatementGroupType.IncomeGroup) });
                        groups.Add(e2.Key);
                        index++;
                    }

                    list.Add(new ManagerServer.Model.Subtotal() { Obsolete_Code = index, Key = e.Key, Name = e.Name });
                    groups.Add(e.Key);
                    index++;
                }
            }

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete18.GeneralLedgerAccount18>().ToArray())
            {
                if (e.Category == ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income) list.Add(new ManagerServer.Model.ProfitAndLossStatementAccount() { Obsolete_Code = e.Code, Key = e.Key, Name = e.Name, DefaultTaxCode = e.TaxCode, Group = (e.MultiStepIncomeStatementGroup.HasValue && groups.Contains(e.MultiStepIncomeStatementGroup.Value) ? e.MultiStepIncomeStatementGroup.Value : ChartOfAccountGroups.Income), Obsolete_GeneralLedgerAccount = e });
                else if (e.Category == ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses) list.Add(new ManagerServer.Model.ProfitAndLossStatementAccount() { Obsolete_Code = e.Code, Key = e.Key, Name = e.Name, DefaultTaxCode = e.TaxCode, Group = (e.MultiStepIncomeStatementGroup.HasValue && groups.Contains(e.MultiStepIncomeStatementGroup.Value) ? e.MultiStepIncomeStatementGroup.Value : ChartOfAccountGroups.Expenses), Obsolete_GeneralLedgerAccount = e });
                else if (e.Category == ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Assets) list.Add(new ManagerServer.Model.BalanceSheetAccount() { Obsolete_Code = e.Code, Key = e.Key, Name = e.Name, DefaultTaxCode = e.TaxCode, Obsolete_Currency = e.Currency, Obsolete_HasStartingBalance = e.HasStartingBalance, Obsolete_StartingBalance2 = e.StartingBalance, Obsolete_StartingBalanceType2 = e.StartingBalanceType, Group = (e.ClassifiedBalanceSheetAssetGroup.HasValue && groups.Contains(e.ClassifiedBalanceSheetAssetGroup.Value) ? e.ClassifiedBalanceSheetAssetGroup.Value : ChartOfAccountGroups.Assets), Obsolete_GeneralLedgerAccount = e });
                else if (e.Category == ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Liabilities) list.Add(new ManagerServer.Model.BalanceSheetAccount() { Obsolete_Code = e.Code, Key = e.Key, Name = e.Name, DefaultTaxCode = e.TaxCode, Obsolete_Currency = e.Currency, Obsolete_HasStartingBalance = e.HasStartingBalance, Obsolete_StartingBalance2 = e.StartingBalance, Obsolete_StartingBalanceType2 = e.StartingBalanceType, Group = (e.ClassifiedBalanceSheetLiabilityGroup.HasValue && groups.Contains(e.ClassifiedBalanceSheetLiabilityGroup.Value) ? e.ClassifiedBalanceSheetLiabilityGroup.Value : ChartOfAccountGroups.Liabilities), Obsolete_GeneralLedgerAccount = e });
                else if (e.Category == ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Equity) list.Add(new ManagerServer.Model.BalanceSheetAccount() { Obsolete_Code = e.Code, Key = e.Key, Name = e.Name, DefaultTaxCode = e.TaxCode, Obsolete_Currency = e.Currency, Obsolete_HasStartingBalance = e.HasStartingBalance, Obsolete_StartingBalance2 = e.StartingBalance, Obsolete_StartingBalanceType2 = e.StartingBalanceType, Group = ChartOfAccountGroups.Equity, Obsolete_GeneralLedgerAccount = e });
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete18.ControlAccount18>().ToArray())
            {
                if (e.Key == ManagerServer.Model.Master.AccountKeys.BillableExpensesInvoiced || e.Key == ManagerServer.Model.Master.AccountKeys.BillableTimeInvoiced || e.Key == ManagerServer.Model.Master.AccountKeys.BillableTimeMovement || e.Key == ManagerServer.Model.Master.AccountKeys.InventorySales || e.Key == ManagerServer.Model.Master.AccountKeys.LatePaymentFees)
                {
                    list.Add(new ManagerServer.Model.Obsolete.Obsolete62.ProfitAndLossStatementBuiltInAccount() { Obsolete_Code = e.Code, Key = e.Key, Name = e.Name, TaxCode = e.TaxCode, Group = (e.MultiStepIncomeStatementGroup.HasValue && groups.Contains(e.MultiStepIncomeStatementGroup.Value) ? e.MultiStepIncomeStatementGroup.Value : ChartOfAccountGroups.Income), Obsolete_ControlAccount = e });
                }
                else if (e.Key == ManagerServer.Model.Master.AccountKeys.CurrencyGainLoss || e.Key == ManagerServer.Model.Master.AccountKeys.FixedAssetDepreciation || e.Key == ManagerServer.Model.Master.AccountKeys.FixedAssetsLossOnDisposal || e.Key == ManagerServer.Model.Master.AccountKeys.IntangibleAssetsAmortization || e.Key == ManagerServer.Model.Master.AccountKeys.IntangibleAssetsGainLossOnDisposal || e.Key == ManagerServer.Model.Master.AccountKeys.InventoryPurchases || e.Key == ManagerServer.Model.Master.AccountKeys.RoundingExpense)
                {
                    list.Add(new ManagerServer.Model.Obsolete.Obsolete62.ProfitAndLossStatementBuiltInAccount() { Obsolete_Code = e.Code, Key = e.Key, Name = e.Name, TaxCode = e.TaxCode, Group = (e.MultiStepIncomeStatementGroup.HasValue && groups.Contains(e.MultiStepIncomeStatementGroup.Value) ? e.MultiStepIncomeStatementGroup.Value : ChartOfAccountGroups.Expenses), Obsolete_ControlAccount = e });
                }
                else if (e.Key == ManagerServer.Model.Master.AccountKeys.AccountsReceivable || e.Key == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount || e.Key == ManagerServer.Model.Master.AccountKeys.BillableTimeUnbilled || e.Key == ManagerServer.Model.Master.AccountKeys.Obsolete_CashAtBank || e.Key == ManagerServer.Model.Master.AccountKeys.Obsolete_CashOnHand || e.Key == ManagerServer.Model.Master.AccountKeys.Obsolete_SupplierCredits || e.Key == ManagerServer.Model.Master.AccountKeys.InventoryOnHand || e.Key == ManagerServer.Model.Master.AccountKeys.FixedAssets || e.Key == ManagerServer.Model.Master.AccountKeys.IntangibleAssets || e.Key == ManagerServer.Model.Master.AccountKeys.FixedAssetsAccumulatedDepreciation || e.Key == ManagerServer.Model.Master.AccountKeys.IntangibleAssetsAccumulatedAmortization || e.Key == ManagerServer.Model.Master.AccountKeys.WithholdingTax || e.Key == ManagerServer.Model.Master.AccountKeys.WithholdingTaxReceivable || (e.Key == ManagerServer.Model.Master.AccountKeys.CapitalAccounts && e.Category.HasValue && e.Category == Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Assets))
                {
                    list.Add(new ManagerServer.Model.Obsolete.Obsolete63.BalanceSheetBuiltInAccount() { Obsolete_Code = e.Code, Key = e.Key, Name = e.Name, TaxCode = e.TaxCode, Obsolete_HasStartingBalance = e.HasStartingBalance, Obsolete_StartingBalance2 = e.StartingBalance, Obsolete_StartingBalanceType = e.StartingBalanceType, Group = (e.ClassifiedBalanceSheetAssetGroup.HasValue && groups.Contains(e.ClassifiedBalanceSheetAssetGroup.Value) ? e.ClassifiedBalanceSheetAssetGroup.Value : ChartOfAccountGroups.Assets), Obsolete_ControlAccount = e });
                }
                else if (e.Key == ManagerServer.Model.Master.AccountKeys.Obsolete_CustomerCredits || e.Key == ManagerServer.Model.Master.AccountKeys.AccountsPayable || e.Key == new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71") || e.Key == ManagerServer.Model.Master.AccountKeys.ExpenseClaims || e.Key == ManagerServer.Model.Master.AccountKeys.EmployeeClearingAccount || (e.Key == ManagerServer.Model.Master.AccountKeys.CapitalAccounts && e.Category.HasValue && e.Category == Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Liabilities))
                {
                    list.Add(new ManagerServer.Model.Obsolete.Obsolete63.BalanceSheetBuiltInAccount() { Obsolete_Code = e.Code, Key = e.Key, Name = e.Name, TaxCode = e.TaxCode, Obsolete_HasStartingBalance = e.HasStartingBalance, Obsolete_StartingBalance2 = e.StartingBalance, Obsolete_StartingBalanceType = e.StartingBalanceType, Group = (e.ClassifiedBalanceSheetLiabilityGroup.HasValue && groups.Contains(e.ClassifiedBalanceSheetLiabilityGroup.Value) ? e.ClassifiedBalanceSheetLiabilityGroup.Value : ChartOfAccountGroups.Liabilities), Obsolete_ControlAccount = e });
                }
                else
                {
                    list.Add(new ManagerServer.Model.Obsolete.Obsolete63.BalanceSheetBuiltInAccount() { Obsolete_Code = e.Code, Key = e.Key, Name = e.Name, TaxCode = e.TaxCode, Obsolete_HasStartingBalance = e.HasStartingBalance, Obsolete_StartingBalance2 = e.StartingBalance, Obsolete_StartingBalanceType = e.StartingBalanceType, Group = ChartOfAccountGroups.Equity, Obsolete_ControlAccount = e });
                }
            }

            if (list.OfType<ManagerServer.Model.ProfitAndLossStatementAccount>().Any(x => x.Group == ChartOfAccountGroups.Income))
            {
                list.Add(new ManagerServer.Model.ProfitAndLossStatementGroup() { Key = ChartOfAccountGroups.Income, Name = ManagerServer.Globalization.Strings.Income, Obsolete_Code = 1 });
            }
            if (list.OfType<ManagerServer.Model.ProfitAndLossStatementAccount>().Any(x => x.Group == ChartOfAccountGroups.Expenses))
            {
                list.Add(new ManagerServer.Model.ProfitAndLossStatementGroup() { Key = ChartOfAccountGroups.Expenses, Name = ManagerServer.Globalization.Strings.Expenses, Obsolete_Code = 2, Type = ProfitAndLossStatementGroupType.ExpenseGroup });
            }
            return list;
        }
    }
}
