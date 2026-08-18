using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Master;

namespace ManagerServer.Query
{
    public static class GeneralLedgerAccounts
    {
        public static Account[] GetAccounts(string entityId)
        {
            var database = ApplicationData.Instance.Businesses.Get(entityId);

            var accounts = new Dictionary<Guid, Account>();

            var tabs = database.Single<ManagerServer.Model.Tabs>();

            // Suspense account
            accounts.Add(Model.Master.AccountKeys.Suspense, new Account() { Key = Model.Master.AccountKeys.Suspense, SystemName = Strings.Suspense, Group = ChartOfAccountGroups.Equity, IsSystemAccount = true });

            // General Ledger Accounts
            var balanceSheetAccounts = database.UnorderedOfType<ManagerServer.Model.BalanceSheetAccount>().ToDictionary(x => x.Key);
            foreach (var e in balanceSheetAccounts.Values)
            {
                accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, TaxCode = e.DefaultTaxCode, CashFlowStatementCategory = e.CashFlowStatement, Inactive = e.Inactive });
            }
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForBankAccounts>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, ControlAccountType = ControlAccountType.BankAccounts, Inactive = e.Inactive, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForCustomers>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, ControlAccountType = ControlAccountType.Customers, Inactive = e.Inactive, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForSuppliers>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, ControlAccountType = ControlAccountType.Suppliers, Inactive = e.Inactive, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForInventoryItems>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, ControlAccountType = ControlAccountType.InventoryItems, Inactive = e.Inactive, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForFixedAssets>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, ControlAccountType = ControlAccountType.FixedAssets, Inactive = e.Inactive, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForIntangibleAssets>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, ControlAccountType = ControlAccountType.IntangibleAssets, Inactive = e.Inactive, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForCapitalAccounts>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, ControlAccountType = ControlAccountType.CapitalAccounts, Inactive = e.Inactive, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForInvestments>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, ControlAccountType = ControlAccountType.Investments, Inactive = e.Inactive, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForSpecialAccounts>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, ControlAccountType = ControlAccountType.SpecialAccounts, Inactive = e.Inactive, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForEmployees>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, ControlAccountType = ControlAccountType.Employees, Inactive = e.Inactive, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForFixedAssetsAccumulatedDepreciation>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, Inactive = e.Inactive, ControlAccountType = ControlAccountType.FixedAssetsAccumulatedDepreciation, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            foreach (var e in database.UnorderedOfType<ManagerServer.Model.ControlAccountForIntangibleAssetsAccumulatedAmortization>()) accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, Inactive = e.Inactive, ControlAccountType = ControlAccountType.IntangibleAssetsAccumulatedAmortization, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory });
            var profitAndLossAccounts = database.UnorderedOfType<ManagerServer.Model.ProfitAndLossStatementAccount>().ToDictionary(x => x.Key);
            foreach (var e in profitAndLossAccounts.Values)
            {
                accounts.Add(e.Key, new Account() { Key = e.Key, CustomName = e.Name, Group = e.Group, Code = e.Code, Position = e.Position, IsProfitAndLossAccount = true, TaxCode = e.DefaultTaxCode, CashFlowStatementCategory = ((IGeneralLedgerAccount)e).CashFlowStatementCategory, Inactive = e.Inactive });
            }

            if (database.Any<ForeignCurrency>())
            {
                var currencyGainsLosses = database.Single<ProfitAndLossStatementAccountCurrencyGainsLosses>();
                accounts.Add(currencyGainsLosses.Key, new Account() { Key = currencyGainsLosses.Key, CustomName = currencyGainsLosses.Name, Position = currencyGainsLosses.Position, Code = currencyGainsLosses.Code, IsSystemAccount = true, SystemName = Strings.CurrencyGainsLosses, Group = currencyGainsLosses.Group ?? ChartOfAccountGroups.Expenses, IsProfitAndLossAccount = true });
            }

            var bankAccountControlAccounts = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForBankAccounts>().Select(x => x.Key));
            var bankAccounts = database.UnorderedOfType<ManagerServer.Model.BankOrCashAccount>().Any(x => !x.ControlAccount.HasValue || !bankAccountControlAccounts.Contains(x.ControlAccount.Value));
            if (bankAccounts)
            {
                var account = database.Single<BalanceSheetCashAtBankAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.CashAndCashEquivalents, Group = account.Group ?? ChartOfAccountGroups.Assets, ControlAccountType = ControlAccountType.BankAccounts });
            }

            if (database.UnorderedOfType<ManagerServer.Model.BankOrCashAccount>().Any())
            {
                var account = database.Single<BalanceSheetInterAccountTransfers>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.InterAccountTransfers, Group = account.Group ?? ChartOfAccountGroups.Equity, ControlAccountType = ControlAccountType.BankAccounts });
            }

            var customerControlAccounts = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForCustomers>().Select(x => x.Key));
            var customers = database.UnorderedOfType<ManagerServer.Model.Customer>().Any(x => !x.ControlAccount.HasValue || !customerControlAccounts.Contains(x.ControlAccount.Value));
            if (customers)
            {
                var account = database.Single<BalanceSheetAccountsReceivableAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.AccountsReceivable, Group = account.Group ?? ChartOfAccountGroups.Assets, ControlAccountType = ControlAccountType.Customers });
            }

            var supplierControlAccounts = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForSuppliers>().Select(x => x.Key));
            var suppliers = database.UnorderedOfType<ManagerServer.Model.Supplier>().Any(x => !x.ControlAccount.HasValue || !supplierControlAccounts.Contains(x.ControlAccount.Value));
            if (suppliers)
            {
                var account = database.Single<BalanceSheetAccountsPayableAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.AccountsPayable, Group = account.Group ?? ChartOfAccountGroups.Liabilities, ControlAccountType = ControlAccountType.Suppliers });
            }

            var inventoryItemControlAccounts = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForInventoryItems>().Select(x => x.Key));
            var inventoryItemsWithDefaultControlAccount = database.UnorderedOfType<ManagerServer.Model.InventoryItem>().Any(x => !x.ControlAccount.HasValue || !inventoryItemControlAccounts.Contains(x.ControlAccount.Value));
            if (inventoryItemsWithDefaultControlAccount)
            {
                var account = database.Single<BalanceSheetInventoryOnHandAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.InventoryOnHand, Group = account.Group ?? ChartOfAccountGroups.Assets, ControlAccountType = ControlAccountType.InventoryItems });
            }
            var inventoryItems = database.UnorderedOfType<ManagerServer.Model.InventoryItem>().Any();
            if (inventoryItems)
            {
                var account = database.Single<ProfitAndLossStatementAccountInventorySales>();
                accounts.Add(account.Key, new Account() { Key = account.Key, TaxCode = account.DefaultTaxCode, CustomName = account.Name, Code = account.Code, Position = account.Position, IsSystemAccount = true, SystemName = Strings.InventorySales, Group = account.Group ?? ChartOfAccountGroups.Income, IsProfitAndLossAccount = true });
            }
            if (inventoryItems)
            {
                var account = database.Single<ProfitAndLossStatementAccountInventoryPurchases>();
                accounts.Add(account.Key, new Account() { Key = account.Key, CustomName = account.Name, Position = account.Position, Code = account.Code, IsSystemAccount = true, SystemName = Strings.InventoryCost, Group = account.Group ?? ChartOfAccountGroups.Expenses, IsProfitAndLossAccount = true });
            }
            if (inventoryItems)
            {
                var account = database.Single<BalanceSheetNegativeInventoryClearing>();
                accounts.Add(account.Key, new Account() { Key = account.Key, CustomName = account.Name, Position = account.Position, Code = account.Code, IsSystemAccount = true, SystemName = Strings.NegativeInventoryClearing, Group = account.Group ?? ChartOfAccountGroups.Assets });
            }

            var specialAccountControlAccounts = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForSpecialAccounts>().Select(x => x.Key));
            var specialAccounts = database.UnorderedOfType<ManagerServer.Model.SpecialAccount>().Any(x => !x.ControlAccount.HasValue || !specialAccountControlAccounts.Contains(x.ControlAccount.Value));
            if (specialAccounts)
            {
                var account = database.Single<BalanceSheetSpecialAccountsAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.SpecialAccounts, Group = account.Group ?? ChartOfAccountGroups.Assets, ControlAccountType = ControlAccountType.SpecialAccounts });
            }

            var investmentControlAccounts = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForInvestments>().Select(x => x.Key));
            var investments = database.UnorderedOfType<ManagerServer.Model.Investment>().Any(x => !x.ControlAccount.HasValue || !investmentControlAccounts.Contains(x.ControlAccount.Value));
            if (investments)
            {
                var account = database.Single<BalanceSheetInvestmentsAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.Investments, Group = account.Group ?? ChartOfAccountGroups.Assets, ControlAccountType = ControlAccountType.Investments });
            }

            if (database.UnorderedOfType<ManagerServer.Model.Investment>().Any())
            {
                var plAccount = database.Single<ProfitAndLossStatementCapitalGainsOnInvestments>();
                accounts.Add(plAccount.Key, new Account() { Key = plAccount.Key, IsSystemAccount = true, Code = plAccount.Code, Position = plAccount.Position, CustomName = plAccount.Name, SystemName = Strings.InvestmentGainsLosses, Group = plAccount.Group ?? ChartOfAccountGroups.Income, IsProfitAndLossAccount = true });
            }

            if (database.UnorderedOfType<ManagerServer.Model.InventoryWriteOff>().Any())
            {
                var plAccount = database.Single<ProfitAndLossStatementAccountInventoryWriteOffs>();
                accounts.Add(plAccount.Key, new Account() { Key = plAccount.Key, IsSystemAccount = true, Code = plAccount.Code, Position = plAccount.Position, CustomName = plAccount.Name, SystemName = Strings.InventoryWriteOffs, Group = plAccount.Group ?? ChartOfAccountGroups.Expenses, IsProfitAndLossAccount = true });
            }

            var fixedAssetControlAccounts = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForFixedAssets>().Select(x => x.Key));
            var fixedAssetsWithDefaultControlAccount = database.UnorderedOfType<ManagerServer.Model.FixedAsset>().Any(x => !x.ControlAccountForFixedAssets.HasValue || !fixedAssetControlAccounts.Contains(x.ControlAccountForFixedAssets.Value));
            if (fixedAssetsWithDefaultControlAccount)
            {
                var account = database.Single<BalanceSheetFixedAssetsAtCostAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.Fixed_assets_at_cost, Group = account.Group ?? ChartOfAccountGroups.Assets, ControlAccountType = ControlAccountType.FixedAssets, CashFlowStatementCategory = CashFlowStatementCategory.InvestingActivities });
            }

            var fixedAssetControlAccounts2 = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForFixedAssetsAccumulatedDepreciation>().Select(x => x.Key));
            var fixedAssetsWithDefaultControlAccount2 = database.UnorderedOfType<ManagerServer.Model.FixedAsset>().Any(x => !x.ControlAccountForFixedAssetsAccumulatedDepreciation.HasValue || !fixedAssetControlAccounts2.Contains(x.ControlAccountForFixedAssetsAccumulatedDepreciation.Value));
            if (fixedAssetsWithDefaultControlAccount2)
            {
                var account = database.Single<BalanceSheetFixedAssetsAccumulatedDepreciationAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.FixedAssetsAccumulatedDepreciation, Group = account.Group ?? ChartOfAccountGroups.Assets, ControlAccountType = ControlAccountType.FixedAssetsAccumulatedDepreciation, CashFlowStatementCategory = CashFlowStatementCategory.InvestingActivities });
            }

            if (database.UnorderedOfType<ManagerServer.Model.FixedAsset>().Where(x => x.DisposalDate.HasValue).Any(x => !x.CustomExpenseAccountForDisposal.HasValue || !profitAndLossAccounts.ContainsKey(x.CustomExpenseAccountForDisposal.Value)))
            {
                var account = database.Single<ProfitAndLossStatementAccountFixedAssetLossOnDisposal>();
                accounts.Add(account.Key, new Account() { Key = account.Key, CustomName = account.Name, Position = account.Position, Code = account.Code, IsSystemAccount = true, SystemName = Strings.FixedAssetsLossOnDisposal, Group = account.Group ?? ChartOfAccountGroups.Expenses, IsProfitAndLossAccount = true });
            }
            if (database.UnorderedOfType<ManagerServer.Model.FixedAsset>().Any(x => !x.CustomDepreciationExpenseAccount || !x.CustomDepreciationExpenseAccountSelection.HasValue || !profitAndLossAccounts.ContainsKey(x.CustomDepreciationExpenseAccountSelection.Value)))
            {
                var account = database.Single<ProfitAndLossStatementAccountFixedAssetDepreciation>();
                accounts.Add(account.Key, new Account() { Key = account.Key, CustomName = account.Name, Position = account.Position, Code = account.Code, IsSystemAccount = true, SystemName = Strings.Fixed_assets_depreciation, Group = account.Group ?? ChartOfAccountGroups.Expenses, IsProfitAndLossAccount = true });
            }

            var intangibleAssetControlAccounts = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForIntangibleAssets>().Select(x => x.Key));
            var intangibleAssetsWithDefaultControlAccount = database.UnorderedOfType<ManagerServer.Model.IntangibleAsset>().Any(x => !x.ControlAccountForIntangibleAssets.HasValue || !intangibleAssetControlAccounts.Contains(x.ControlAccountForIntangibleAssets.Value));
            if (intangibleAssetsWithDefaultControlAccount)
            {
                var account = database.Single<BalanceSheetIntangibleAssetsAtCostAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.Intangible_assets_at_cost, Group = account.Group ?? ChartOfAccountGroups.Assets, ControlAccountType = ControlAccountType.IntangibleAssets, CashFlowStatementCategory = CashFlowStatementCategory.InvestingActivities });
            }

            var intangibleAssetControlAccounts2 = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForIntangibleAssetsAccumulatedAmortization>().Select(x => x.Key));
            var intangibleAssetsWithDefaultControlAccount2 = database.UnorderedOfType<ManagerServer.Model.IntangibleAsset>().Any(x => !x.ControlAccountForIntangibleAssetsAccumulatedAmortization.HasValue || !intangibleAssetControlAccounts2.Contains(x.ControlAccountForIntangibleAssetsAccumulatedAmortization.Value));
            if (intangibleAssetsWithDefaultControlAccount2)
            {
                var account = database.Single<BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.IntangibleAssetsAccumulatedAmortization, Group = account.Group ?? ChartOfAccountGroups.Assets, ControlAccountType = ControlAccountType.IntangibleAssetsAccumulatedAmortization, CashFlowStatementCategory = CashFlowStatementCategory.InvestingActivities });
            }

            if (database.UnorderedOfType<ManagerServer.Model.IntangibleAsset>().Any(x => !x.CustomAmortizationExpenseAccount || !x.CustomAmortizationExpenseAccountSelection.HasValue || !profitAndLossAccounts.ContainsKey(x.CustomAmortizationExpenseAccountSelection.Value)))
            {
                var account = database.Single<ProfitAndLossStatementAccountIntangibleAssetsAmortization>();
                accounts.Add(account.Key, new Account() { Key = account.Key, CustomName = account.Name, Position = account.Position, Code = account.Code, IsSystemAccount = true, SystemName = Strings.IntangibleAssetsAmortization, Group = account.Group ?? ChartOfAccountGroups.Expenses, IsProfitAndLossAccount = true });
            }
            if (database.UnorderedOfType<ManagerServer.Model.IntangibleAsset>().Where(x => x.DisposalDate.HasValue).Any(x => !x.CustomExpenseAccountForDisposal.HasValue || !profitAndLossAccounts.ContainsKey(x.CustomExpenseAccountForDisposal.Value)))
            {
                var account = database.Single<ProfitAndLossStatementAccountIntangibleAssetsGainsLossOnDisposal>();
                accounts.Add(account.Key, new Account() { Key = account.Key, CustomName = account.Name, Position = account.Position, Code = account.Code, IsSystemAccount = true, SystemName = Strings.IntangibleAssetsLossOnDisposal, Group = account.Group ?? ChartOfAccountGroups.Expenses, IsProfitAndLossAccount = true });
            }

            var capitalAccountControlAccounts = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForCapitalAccounts>().Select(x => x.Key));
            var capitalAccounts = database.UnorderedOfType<ManagerServer.Model.CapitalAccount>().Any(x => !x.ControlAccount.HasValue || !capitalAccountControlAccounts.Contains(x.ControlAccount.Value));
            if (capitalAccounts)
            {
                var account = database.Single<BalanceSheetCapitalAccountsAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, IsSystemAccount = true, Code = account.Code, Position = account.Position, CustomName = account.Name, SystemName = Strings.CapitalAccounts, Group = account.Group ?? ChartOfAccountGroups.Equity, ControlAccountType = ControlAccountType.CapitalAccounts, CashFlowStatementCategory = CashFlowStatementCategory.FinancingActivities });
            }

            var employeeControlAccounts = new HashSet<Guid>(database.UnorderedOfType<ManagerServer.Model.ControlAccountForEmployees>().Select(x => x.Key));
            var employees = database.UnorderedOfType<ManagerServer.Model.Employee>().Any(x => !x.ControlAccount.HasValue || !employeeControlAccounts.Contains(x.ControlAccount.Value));
            if (employees)
            {
                var account = database.Single<BalanceSheetEmployeeClearingAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, Code = account.Code, CustomName = account.Name, IsSystemAccount = true, SystemName = Strings.EmployeeClearingAccount, Group = account.Group ?? ChartOfAccountGroups.Liabilities, ControlAccountType = ControlAccountType.Employees });
            }

            var latePaymentFees = database.UnorderedOfType<ManagerServer.Model.LatePaymentFee>().Any();
            var billableTime = database.UnorderedOfType<ManagerServer.Model.BillableTime>().Any();
            var productionOrders = database.UnorderedOfType<ManagerServer.Model.ProductionOrder>().Any();
            var expenseClaims = database.UnorderedOfType<ManagerServer.Model.ExpenseClaimsPayer>().Any();

            var payslipEarningsItemsWithDefaultAccount = database.UnorderedOfType<ManagerServer.Model.PayslipEarningsItem>().Where(x => !x.ExpenseAccount.HasValue || !profitAndLossAccounts.ContainsKey(x.ExpenseAccount.Value)).Any();
            var payslipDeductionItemsWithDefaultAccount = database.UnorderedOfType<ManagerServer.Model.PayslipDeductionItem>().Where(x => !x.Account.HasValue || !balanceSheetAccounts.ContainsKey(x.Account.Value)).Any();
            var payslipContributionItemsWithDefaultLiabilityAccount = database.UnorderedOfType<ManagerServer.Model.PayslipContributionItem>().Where(x => !x.LiabilityAccount.HasValue || !balanceSheetAccounts.ContainsKey(x.LiabilityAccount.Value)).Any();
            var payslipContributionItemsWithDefaultExpenseAccount = database.UnorderedOfType<ManagerServer.Model.PayslipContributionItem>().Where(x => !x.ExpenseAccount.HasValue || !profitAndLossAccounts.ContainsKey(x.ExpenseAccount.Value)).Any();

            var billableExpenses = database.Single<BillableExpenses>();
            if (billableExpenses.Enabled)
            {
                var account = database.Single<BalanceSheetBillableExpensesAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, TaxCode = account.DefaultTaxCode, Code = account.Code, CustomName = account.Name, IsSystemAccount = true, SystemName = Strings.Billable_expenses, Group = account.Group ?? ChartOfAccountGroups.Assets });
            }
            if (billableExpenses.Enabled)
            {
                var account = database.Single<ProfitAndLossStatementAccountBillableExpensesInvoiced>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, CustomName = account.Name, Code = account.Code, IsSystemAccount = true, SystemName = Strings.Billable_expenses_invoiced, Group = account.Group ?? ChartOfAccountGroups.Income, IsProfitAndLossAccount = true });
            }
            if (billableExpenses.Enabled)
            {
                var account = database.Single<ProfitAndLossStatementAccountBillableExpensesCost>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, CustomName = account.Name, Code = account.Code, IsSystemAccount = true, SystemName = Strings.Billable_expenses_cost, Group = account.Group ?? ChartOfAccountGroups.Expenses, IsProfitAndLossAccount = true });
            }
            if (billableTime && customers)
            {
                var account = database.Single<ProfitAndLossStatementAccountBillableTimeInvoiced>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, CustomName = account.Name, Code = account.Code, IsSystemAccount = true, SystemName = Strings.Billable_time_invoiced, Group = account.Group ?? ChartOfAccountGroups.Income, IsProfitAndLossAccount = true });
            }
            if (billableTime && customers)
            {
                var account = database.Single<ProfitAndLossStatementAccountBillableTimeMovement>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, CustomName = account.Name, Code = account.Code, IsSystemAccount = true, SystemName = Strings.BillableTime_Movement, Group = account.Group ?? ChartOfAccountGroups.Income, IsProfitAndLossAccount = true });
            }
            if (billableTime && customers)
            {
                var account = database.Single<BalanceSheetBillableTimeAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, Code = account.Code, CustomName = account.Name, IsSystemAccount = true, SystemName = Strings.Billable_time, Group = account.Group ?? ChartOfAccountGroups.Assets });
            }

            /*
            if (productionOrders)
            {
                var account = objects.Single<BalanceSheetProductionInProgressAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, Code = account.Code, CustomName = account.Name, IsSystemAccount = true, SystemName = Strings.ProductionInProgress, Group = account.Group ?? Manager.Model.Enums.ChartOfAccountGroups.Assets });
            }
            */
            if (expenseClaims)
            {
                var account = database.Single<BalanceSheetExpenseClaimsAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, Code = account.Code, CustomName = account.Name, IsSystemAccount = true, SystemName = Strings.Expense_claims, Group = account.Group ?? ChartOfAccountGroups.Liabilities });
            }
            if (latePaymentFees)
            {
                var account = database.Single<ProfitAndLossStatementAccountLatePaymentFees>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, CustomName = account.Name, Code = account.Code, IsSystemAccount = true, SystemName = Strings.LatePaymentFees, Group = account.Group ?? ChartOfAccountGroups.Income, IsProfitAndLossAccount = true });
            }

            var withholdingTax = database.Single<WithholdingTax>();
            if (withholdingTax.WithholdingTaxReceivable)
            {
                var account = database.Single<BalanceSheetWithholdingTaxReceivableAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, Code = account.Code, CustomName = account.Name, IsSystemAccount = true, SystemName = Strings.WithholdingTaxReceivable, Group = account.Group ?? ChartOfAccountGroups.Assets });
            }
            if (withholdingTax.WithholdingTaxPayable)
            {
                var account = database.Single<BalanceSheetWithholdingTaxPayableAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, Code = account.Code, CustomName = account.Name, IsSystemAccount = true, SystemName = Strings.WithholdingTaxPayable, Group = account.Group ?? ChartOfAccountGroups.Liabilities });
            }

            if (database.UnorderedOfType<WithholdingTaxReceipt>().Any())
            {
                var account = database.Single<BalanceSheetWithholdingTaxAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, Code = account.Code, CustomName = account.Name, IsSystemAccount = true, SystemName = Strings.WithholdingTax, Group = account.Group ?? ChartOfAccountGroups.Assets });
            }            

            if (database.UnorderedOfType<ManagerServer.Model.SalesInvoice>().Any(x => x.Rounding))
            {
                var account = database.Single<ProfitAndLossStatementAccountRoundingExpense>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, CustomName = account.Name, Code = account.Code, IsSystemAccount = true, SystemName = Strings.RoundingExpense, Group = account.Group ?? ChartOfAccountGroups.Expenses, IsProfitAndLossAccount = true });
            }

            if (database.UnorderedOfType<TaxCode>().Any(x => x.HasDefaultControlAccount()))
            {
                var account = database.Single<BalanceSheetTaxPayableAccount>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, Code = account.Code, CustomName = account.Name, IsSystemAccount = true, SystemName = Strings.TaxPayable, Group = account.Group ?? ChartOfAccountGroups.Liabilities });
            }

            if (database.UnorderedOfType<Division>().Any())
            {
                var account = database.Single<BalanceSheetInterdivisionalLoan>();
                accounts.Add(account.Key, new Account() { Key = account.Key, Position = account.Position, Code = account.Code, CustomName = account.Name, IsSystemAccount = true, SystemName = Strings.InterdivisionalLoan, Group = account.Group ?? ChartOfAccountGroups.Liabilities });
            }

            var retainedEarningsAccount = database.Single<BalanceSheetRetainedEarningsAccount>();
            accounts.Add(retainedEarningsAccount.Key, new Account() { Key = retainedEarningsAccount.Key, Code = retainedEarningsAccount.Code, Position = retainedEarningsAccount.Position, CustomName = retainedEarningsAccount.Name, IsSystemAccount = true, SystemName = Strings.Retained_earnings, Group = retainedEarningsAccount.Group ?? ChartOfAccountGroups.Equity });

            return accounts.Values.ToArray();
        }        

        public sealed class Account
        {
            public Guid Key;
            public string Code;
            public int Position;
            public Guid? TaxCode;
            public string SystemName;
            public string CustomName;
            public bool IsProfitAndLossAccount;
            public bool IsSystemAccount;
            public bool Inactive;
            public Guid? Group;
            public CashFlowStatementCategory CashFlowStatementCategory;

            public ControlAccountType? ControlAccountType;

            public string Name
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(CustomName)) return CustomName;
                    return SystemName;
                }
            }

            public string NameWithCode
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + Name;
                    else return Name;
                }
            }
        }
    }
}