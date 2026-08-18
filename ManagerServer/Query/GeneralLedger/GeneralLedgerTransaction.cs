using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ProtoBuf;

namespace ManagerServer.Query.GeneralLedger
{
    [ProtoContract]
    public sealed class GeneralLedgerTransaction : ICustomFields
    {
        public readonly Transaction Transaction;
        public readonly Transaction AccrualBasisTransaction;
        public readonly DateTime Date;
        public readonly IGeneralLedgerAccount GeneralLedgerAccount;
        public readonly IBalanceSheetAccount BalanceSheetAccount;
        public readonly IProfitAndLossAccount ProfitAndLossAccount;
        public readonly BankOrCashAccount BankAccount;
        public readonly BankOrCashAccount InterAccountTransferAccount;
        public readonly Customer Customer;
        public readonly Supplier Supplier;
        public readonly Employee Employee;
        public readonly ExpenseClaimsPayer ExpenseClaimPayer;
        public readonly InventoryItem InventoryItem;
        public readonly NonInventoryItem NonInventoryItem;
        public readonly InventoryKit InventoryKit;
        public readonly FixedAsset FixedAsset;
        public readonly IntangibleAsset IntangibleAsset;
        public readonly SpecialAccount SpecialAccount;
        public readonly CapitalAccount CapitalAccount;
        public readonly PayslipEarningsItem PayslipEarningsItem;
        public readonly PayslipDeductionItem PayslipDeductionItem;
        public readonly PayslipContributionItem PayslipContributionItem;
        public readonly SalesInvoice SalesInvoice;
        public readonly PurchaseInvoice PurchaseInvoice;
        public readonly ITransactionLine TransactionLine;
        public readonly SubAccount CapitalSubaccount;
        public readonly Division Division;
        public readonly CustomInventoryLocation InventoryLocation;
        public readonly TaxCode TaxCode;
        public readonly GeneralLedgerTransaction[] ContraTransactions;
        public readonly SalesOrder SalesOrder;
        public readonly PurchaseOrder PurchaseOrder;
        [ProtoMember(1)] public readonly decimal? Qty;
        [ProtoMember(2)] public readonly decimal BaseAmount;
        [ProtoMember(3)] public readonly decimal TransactionAmount;
        [ProtoMember(4)] public readonly decimal Discount;
        public readonly Currency TransactionCurrency;
        [ProtoMember(5)] public readonly decimal AccountAmount;
        public readonly Currency AccountCurrency;
        [ProtoMember(6)] public readonly bool IsBalancing;
        [ProtoMember(7)] public readonly bool IsTaxTransaction;
        public readonly bool IsReversedTaxTransaction;
        public readonly bool IsCostOfGoodsSold;
        public readonly bool IsBillableExpense;
        public readonly bool IsPurchaseTaxTransaction;
        public readonly string TaxComponent;
        public readonly Guid? ReportingCategory;
        public readonly Guid? ReportingCategoryReversed;
        public readonly bool CashBasisAdjustment;
        public readonly DateTime? OriginalDate;
        public readonly Project Project;
        public readonly Investment Investment;
        public readonly bool IsLandingCost;
        public readonly bool IsPayslipEarningsLine;
        public readonly bool IsPayslipDeductionLine;
        public readonly bool IsPayslipContributionLine;
        public readonly int? LineNumber;
        public readonly decimal? ExchangeRate;
        public readonly bool IsExchangeRateInverse;
        public readonly bool IsFixedAssetDisposalTransaction;
        public readonly bool IsIntangibleAssetDisposalTransaction;
        public readonly decimal? PurchaseCost;
        public readonly InvestmentMarketPrice InvestmentMarketPrice;
        public readonly InventoryUnitCost InventoryUnitCost;

        public int GetHashCode2() => (
            (Date, GeneralLedgerAccount.Key, BankAccount?.Key, Customer?.Key, Supplier?.Key, Employee?.Key).GetHashCode(),
            (ExpenseClaimPayer?.Key, InventoryItem?.Key, NonInventoryItem?.Key, InventoryKit?.Key, FixedAsset?.Key, IntangibleAsset?.Key, SpecialAccount?.Key).GetHashCode(),
            (CapitalAccount?.Key, PayslipEarningsItem?.Key, PayslipDeductionItem?.Key, PayslipContributionItem?.Key, SalesInvoice?.Key, PurchaseInvoice?.Key, CapitalSubaccount?.Key).GetHashCode(),
            (InventoryUnitCost?.Key, Division?.Key, TaxCode?.Key, Qty, BaseAmount, TransactionAmount, TransactionCurrency?.Key).GetHashCode(),
            (AccountAmount, AccountCurrency?.Key, IsBalancing, IsTaxTransaction, IsReversedTaxTransaction, IsCostOfGoodsSold, IsBillableExpense).GetHashCode(),
            (TaxComponent, ReportingCategory, ReportingCategoryReversed, Project?.Key, Investment, IsLandingCost, IsPurchaseTaxTransaction, InterAccountTransferAccount).GetHashCode()
            ).GetHashCode();

        public static IEnumerable<GeneralLedgerTransaction> From(Database database, DateTime date, Transaction transaction, Currency transactionCurrency, ITransactionLine transactionLine, bool amountsIncludeTax, bool reverseSign = false, BankOrCashAccount bankAccount = null, Customer customer = null, Supplier supplier = null, Employee employee = null, SalesInvoice salesInvoice = null, PurchaseInvoice purchaseInvoice = null, SpecialAccount specialAccount = null, CapitalAccount capitalAccount = null, CustomInventoryLocation inventoryLocation = null, ExpenseClaimsPayer expenseClaimPayer = null, SalesOrder salesOrder = null, PurchaseOrder purchaseOrder = null, int? lineNumber = null, decimal? exchangeRate = null, bool exchangeRateIsInverse = false)
        {
            var lineTotalBeforeDiscount = transactionLine.GetLineTotal(transaction);
            lineTotalBeforeDiscount = transactionCurrency.Round(lineTotalBeforeDiscount);

            var lineTotalAfterDiscount = lineTotalBeforeDiscount;
            if (lineTotalBeforeDiscount != 0m && transactionLine.GetDiscountPercentage(transaction).HasValue && transactionLine.GetDiscountPercentage(transaction).Value != 0m)
            {
                lineTotalAfterDiscount = (lineTotalBeforeDiscount / 100m).SafeMultiply(100m - transactionLine.GetDiscountPercentage(transaction).Value);
            }

            if (transactionLine.GetDiscountAmount(transaction).HasValue)
            {
                lineTotalAfterDiscount = lineTotalBeforeDiscount - transactionLine.GetDiscountAmount(transaction).Value;
            }

            lineTotalAfterDiscount = transactionCurrency.Round(lineTotalAfterDiscount);

            var discount = lineTotalBeforeDiscount - lineTotalAfterDiscount;

            var lineQty = transactionLine.GetQty(transaction);

            if (reverseSign)
            {
                lineTotalAfterDiscount *= -1m;
                lineQty *= -1m;
            }

            if (lineQty.HasValue && lineTotalAfterDiscount != 0m && Math.Sign(lineQty.Value) != Math.Sign(lineTotalAfterDiscount)) lineQty *= -1m;

            var taxCode = database.SingleOrDefault<TaxCode>(transactionLine.GetTaxCode());
            var division = database.SingleOrDefault<Division>(transactionLine.GetDivision());
            var project = database.SingleOrDefault<Project>(transactionLine.GetProject(transaction));

            IGeneralLedgerAccount generalLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
            InventoryItem inventoryItem = null;
            InventoryKit inventoryKit = null;
            FixedAsset fixedAsset = null;
            IntangibleAsset intangibleAsset = null;
            Investment investment = null;
            SubAccount capitalSubaccount = null;
            NonInventoryItem nonInventoryItem = null;
            BankOrCashAccount interAccountTransferAccount = null;
            bool isPurchaseTaxTransaction = false;

            if (transactionLine.GetItem().HasValue)
            {
                var item = database.SingleOrDefault<NamedObject>(transactionLine.GetItem().Value);
                if (item is InventoryItem)
                {
                    inventoryItem = (InventoryItem)item;

                    var inventoryPurchasesAccount = database.SingleOrDefault<ProfitAndLossStatementAccount>(inventoryItem.GetExpenseAccount()) as IGeneralLedgerAccount ?? database.Single<ProfitAndLossStatementAccountInventoryPurchases>();

                    if (transaction is Receipt || transaction is SalesInvoice || transaction is CreditNote)
                    {
                        if (lineQty.HasValue)
                        {
                            var inventoryUnitCost = database.FindInventoryUnitCost(inventoryItem.Key, date);

                            if (inventoryUnitCost != null)
                            {
                                var baseCurrency = database.Single<BaseCurrency>();
                                var amount = 0m;
                                amount = baseCurrency.Round(lineQty.Value * inventoryUnitCost.UnitCost);

                                yield return new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                    database: database,
                                    date: date,
                                    generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                                    qty: lineQty,
                                    transactionAmount: amount,
                                    accountAmount: amount,
                                    baseAmount: amount,
                                    transactionCurrency: transactionCurrency,
                                    transaction: transaction,
                                    bankAccount: bankAccount,
                                    customer: customer,
                                    supplier: supplier,
                                    employee: employee,
                                    salesInvoice: salesInvoice,
                                    purchaseInvoice: purchaseInvoice,
                                    specialAccount: specialAccount,
                                    capitalAccount: capitalAccount,
                                    inventoryItem: inventoryItem,
                                    inventoryLocation: inventoryLocation,
                                    isCostOfGoodsSold: true,
                                    transactionLine: transactionLine,
                                    salesOrder: salesOrder,
                                    purchaseOrder: purchaseOrder,
                                    inventoryUnitCost: inventoryUnitCost
                                );

                                yield return new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                    database: database,
                                    date: date,
                                    generalLedgerAccount: inventoryPurchasesAccount,
                                    qty: lineQty,
                                    transactionAmount: amount * -1m,
                                    accountAmount: amount * -1m,
                                    baseAmount: amount * -1m,
                                    transactionCurrency: transactionCurrency,
                                    transaction: transaction,
                                    bankAccount: bankAccount,
                                    customer: customer,
                                    supplier: supplier,
                                    employee: employee,
                                    salesInvoice: salesInvoice,
                                    purchaseInvoice: purchaseInvoice,
                                    specialAccount: specialAccount,
                                    capitalAccount: capitalAccount,
                                    inventoryItem: inventoryItem,
                                    inventoryLocation: inventoryLocation,
                                    trackingCode: division,
                                    project: project,
                                    isCostOfGoodsSold: true,
                                    transactionLine: transactionLine,
                                    salesOrder: salesOrder,
                                    purchaseOrder: purchaseOrder,
                                    inventoryUnitCost: inventoryUnitCost
                                );
                            }
                            else
                            {
                                yield return new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                    database: database,
                                    date: date,
                                    generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                                    qty: lineQty,
                                    transactionAmount: 0m,
                                    accountAmount: 0m,
                                    baseAmount: 0m,
                                    transactionCurrency: transactionCurrency,
                                    transaction: transaction,
                                    bankAccount: bankAccount,
                                    customer: customer,
                                    supplier: supplier,
                                    employee: employee,
                                    salesInvoice: salesInvoice,
                                    purchaseInvoice: purchaseInvoice,
                                    specialAccount: specialAccount,
                                    capitalAccount: capitalAccount,
                                    inventoryItem: inventoryItem,
                                    inventoryLocation: inventoryLocation,
                                    isCostOfGoodsSold: true,
                                    transactionLine: transactionLine,
                                    salesOrder: salesOrder,
                                    inventoryUnitCost: database.Single<InventoryUnitCost>(),
                                    purchaseOrder: purchaseOrder
                                );
                            }
                        }

                        var inventorySalesAccount = database.SingleOrDefault<ProfitAndLossStatementAccount>(inventoryItem.GetIncomeAccount()) as IGeneralLedgerAccount ?? database.Single<ProfitAndLossStatementAccountInventorySales>();
                        generalLedgerAccount = inventorySalesAccount;
                    }
                    else
                    {
                        generalLedgerAccount = database.Single<BalanceSheetInventoryOnHandAccount>();
                        division = database.SingleOrDefault<Division>(inventoryItem.Division);
                        project = null;
                    }
                }
                else if (item is InventoryKit)
                {
                    inventoryKit = (InventoryKit)item;
                    generalLedgerAccount = database.SingleOrDefault<ProfitAndLossStatementAccount>(inventoryKit.IncomeAccount);
                    if (!inventoryKit.CustomIncomeAccount || generalLedgerAccount == null) generalLedgerAccount = database.Single<ProfitAndLossStatementAccountInventorySales>();
                    if (transactionLine.GetQty(transaction).HasValue)
                    {
                        if (inventoryKit.BillOfMaterials != null)
                        {
                            foreach (var e in inventoryKit.BillOfMaterials)
                            {
                                var inventoryItemWithinInventoryKit = database.SingleOrDefault<InventoryItem>(e.InventoryItem);
                                if (inventoryItemWithinInventoryKit != null)
                                {
                                    var inventoryPurchasesAccount = database.Single<ProfitAndLossStatementAccountInventoryPurchases>();

                                    var inventoryUnitCost = database.FindInventoryUnitCost(inventoryItemWithinInventoryKit.Key, date);

                                    if (inventoryUnitCost != null)
                                    {
                                        var baseCurrency = database.Single<BaseCurrency>();
                                        var amount = 0m;
                                        amount = baseCurrency.Round(lineQty.Value * e.Qty * inventoryUnitCost.UnitCost);

                                        yield return new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                                database: database,
                                                date: date,
                                                generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                                                qty: lineQty * e.Qty,
                                                transactionAmount: amount,
                                                accountAmount: amount,
                                                baseAmount: amount,
                                                transactionCurrency: transactionCurrency,
                                                transaction: transaction,
                                                bankAccount: bankAccount,
                                                customer: customer,
                                                supplier: supplier,
                                                employee: employee,
                                                salesInvoice: salesInvoice,
                                                purchaseInvoice: purchaseInvoice,
                                                specialAccount: specialAccount,
                                                capitalAccount: capitalAccount,
                                                inventoryKit: inventoryKit,
                                                inventoryItem: inventoryItemWithinInventoryKit,
                                                inventoryLocation: inventoryLocation,
                                                trackingCode: division,
                                                isCostOfGoodsSold: true,
                                                transactionLine: transactionLine,
                                                salesOrder: salesOrder,
                                                project: project,
                                                purchaseOrder: purchaseOrder,
                                                inventoryUnitCost: inventoryUnitCost
                                            );

                                        yield return new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                                database: database,
                                                date: date,
                                                generalLedgerAccount: inventoryPurchasesAccount,
                                                qty: lineQty * e.Qty,
                                                transactionAmount: amount * -1m,
                                                accountAmount: amount * -1m,
                                                baseAmount: amount * -1m,
                                                transactionCurrency: transactionCurrency,
                                                transaction: transaction,
                                                bankAccount: bankAccount,
                                                customer: customer,
                                                supplier: supplier,
                                                employee: employee,
                                                salesInvoice: salesInvoice,
                                                purchaseInvoice: purchaseInvoice,
                                                specialAccount: specialAccount,
                                                capitalAccount: capitalAccount,
                                                inventoryKit: inventoryKit,
                                                inventoryItem: inventoryItemWithinInventoryKit,
                                                inventoryLocation: inventoryLocation,
                                                trackingCode: division,
                                                project: project,
                                                isCostOfGoodsSold: true,
                                                transactionLine: transactionLine,
                                                salesOrder: salesOrder,
                                                purchaseOrder: purchaseOrder,
                                                inventoryUnitCost: inventoryUnitCost
                                            );
                                    }
                                    else
                                    {
                                        yield return new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                                database: database,
                                                date: date,
                                                generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                                                qty: lineQty * e.Qty,
                                                transactionAmount: 0m,
                                                accountAmount: 0m,
                                                baseAmount: 0m,
                                                transactionCurrency: transactionCurrency,
                                                transaction: transaction,
                                                bankAccount: bankAccount,
                                                customer: customer,
                                                supplier: supplier,
                                                employee: employee,
                                                salesInvoice: salesInvoice,
                                                purchaseInvoice: purchaseInvoice,
                                                specialAccount: specialAccount,
                                                capitalAccount: capitalAccount,
                                                inventoryKit: inventoryKit,
                                                inventoryItem: inventoryItemWithinInventoryKit,
                                                inventoryLocation: inventoryLocation,
                                                inventoryUnitCost: database.Single<InventoryUnitCost>(),
                                                isCostOfGoodsSold: true,
                                                transactionLine: transactionLine,
                                                salesOrder: salesOrder,
                                                project: project,
                                                purchaseOrder: purchaseOrder
                                            );
                                    }
                                }
                            }
                        }
                    }
                }
                else if (item is NonInventoryItem)
                {
                    nonInventoryItem = (NonInventoryItem)item;
                    if (transaction is Receipt || transaction is SalesInvoice || transaction is CreditNote)
                    {
                        generalLedgerAccount = database.SingleOrDefault<NamedObject>(nonInventoryItem.WhenSold) as IGeneralLedgerAccount ?? database.Single<BalanceSheetSuspenseAccount>();
                    }
                    else
                    {
                        generalLedgerAccount = database.SingleOrDefault<NamedObject>(nonInventoryItem.WhenPurchased) as IGeneralLedgerAccount ?? database.Single<BalanceSheetSuspenseAccount>();
                    }
                }
            }
            else if (transactionLine.GetAccount().HasValue)
            {
                var account = database.SingleOrDefault<NamedObject>(transactionLine.GetAccount().Value) as IGeneralLedgerAccount;
                if (account == null)
                {
                    account = database.Single(transactionLine.GetAccount().Value) as IGeneralLedgerAccount;
                }

                if (account != null)
                {
                    if (account is BalanceSheetAccountsReceivableAccount || account is ControlAccountForCustomers)
                    {
                        customer ??= database.SingleOrDefault<Customer>(transactionLine.GetAccountsReceivableCustomer());
                        if (customer != null)
                        {
                            generalLedgerAccount = account;
                            if (salesInvoice == null) salesInvoice = database.SingleOrDefault<SalesInvoice>(transactionLine.GetAccountsReceivableSalesInvoice());
                            taxCode = null;
                            project = null;
                            division = database.SingleOrDefault<Division>(customer.Division);
                        }
                    }
                    else if (account is BalanceSheetAccountsPayableAccount || account is ControlAccountForSuppliers)
                    {
                        supplier ??= database.SingleOrDefault<Supplier>(transactionLine.GetAccountsPayableSupplier());
                        if (supplier != null)
                        {
                            generalLedgerAccount = account;
                            if (purchaseInvoice == null) purchaseInvoice = database.SingleOrDefault<PurchaseInvoice>(transactionLine.GetAccountsPayablePurchaseInvoice());
                            taxCode = null;
                            project = null;
                            division = database.SingleOrDefault<Division>(supplier.Division);
                        }
                    }
                    else if (account is BalanceSheetEmployeeClearingAccount || account is ControlAccountForEmployees)
                    {
                        employee = database.SingleOrDefault<Employee>(transactionLine.GetEmployee());
                        if (employee != null)
                        {
                            generalLedgerAccount = account;
                            taxCode = null;
                            project = null;
                            division = database.SingleOrDefault<Division>(employee.Division);
                        }
                    }
                    else if (account is BalanceSheetFixedAssetsAtCostAccount || account is ControlAccountForFixedAssets)
                    {
                        fixedAsset = database.SingleOrDefault<FixedAsset>(transactionLine.GetFixedAsset());
                        if (fixedAsset != null)
                        {
                            generalLedgerAccount = account;
                            division = database.SingleOrDefault<Division>(fixedAsset.Division);
                        }
                    }
                    else if (account is BalanceSheetFixedAssetsAccumulatedDepreciationAccount || account is ControlAccountForFixedAssetsAccumulatedDepreciation)
                    {
                        fixedAsset = database.SingleOrDefault<FixedAsset>(transactionLine.GetFixedAsset());
                        if (fixedAsset != null)
                        {
                            generalLedgerAccount = account;
                            division = database.SingleOrDefault<Division>(fixedAsset.Division);
                        }
                    }
                    else if (account is BalanceSheetIntangibleAssetsAtCostAccount || account is ControlAccountForIntangibleAssets)
                    {
                        intangibleAsset = database.SingleOrDefault<IntangibleAsset>(transactionLine.GetIntangibleAsset());
                        if (intangibleAsset != null)
                        {
                            generalLedgerAccount = account;
                            division = database.SingleOrDefault<Division>(intangibleAsset.Division);
                        }
                    }
                    else if (account is BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount || account is ControlAccountForIntangibleAssetsAccumulatedAmortization)
                    {
                        intangibleAsset = database.SingleOrDefault<IntangibleAsset>(transactionLine.GetIntangibleAsset());
                        if (intangibleAsset != null)
                        {
                            generalLedgerAccount = account;
                            division = database.SingleOrDefault<Division>(intangibleAsset.Division);
                        }
                    }
                    else if (account is BalanceSheetSpecialAccountsAccount || account is ControlAccountForSpecialAccounts)
                    {
                        specialAccount = database.SingleOrDefault<SpecialAccount>(transactionLine.GetSpecialAccount());
                        if (specialAccount != null)
                        {
                            generalLedgerAccount = account;
                            division = database.SingleOrDefault<Division>(specialAccount.Division);
                        }
                    }
                    else if (account is BalanceSheetCashAtBankAccount || account is ControlAccountForBankAccounts)
                    {
                        bankAccount = database.SingleOrDefault<BankOrCashAccount>(transactionLine.GetBankOrCashAccount());
                        if (bankAccount != null)
                        {
                            generalLedgerAccount = account;
                            division = database.SingleOrDefault<Division>(bankAccount.Division);
                        }
                    }
                    else if (account is BalanceSheetAccount balanceSheetAccount)
                    {
                        generalLedgerAccount = balanceSheetAccount;
                    }
                    else if (account is BalanceSheetCapitalAccountsAccount || account is ControlAccountForCapitalAccounts)
                    {
                        capitalAccount = database.SingleOrDefault<CapitalAccount>(transactionLine.GetCapitalAccount());
                        if (capitalAccount != null)
                        {
                            generalLedgerAccount = account;
                            capitalSubaccount = database.SingleOrDefault<SubAccount>(transactionLine.GetSubAccount());
                            division = database.SingleOrDefault<Division>(capitalAccount.Division);
                        }
                    }
                    else if (account is BalanceSheetInterAccountTransfers)
                    {
                        if (bankAccount != null)
                        {
                            interAccountTransferAccount = database.SingleOrDefault<BankOrCashAccount>(transactionLine.GetInterAccountTransferAccount());
                            if (interAccountTransferAccount != null)
                            {
                                generalLedgerAccount = account;
                                taxCode = null;
                                // Division assignment missing?!
                            }
                        }
                    }
                    else if (account is BalanceSheetInvestmentsAccount || account is ControlAccountForInvestments)
                    {
                        investment = database.SingleOrDefault<Investment>(transactionLine.GetInvestment());
                        if (investment != null)
                        {
                            generalLedgerAccount = account;
                            // Division assignment missing?!                            
                        }
                    }
                    else if (account is BalanceSheetExpenseClaimsAccount)
                    {
                        expenseClaimPayer = database.SingleOrDefault<ExpenseClaimsPayer>(transactionLine.GetExpenseClaimPayer());
                        if (expenseClaimPayer != null)
                        {
                            generalLedgerAccount = account;
                            division = database.SingleOrDefault<Division>(expenseClaimPayer.Division);
                        }
                    }
                    else if (account is BalanceSheetBillableExpensesAccount)
                    {
                        customer = database.SingleOrDefault<Customer>(transactionLine.GetBillableExpenseCustomer());
                        if (customer != null)
                        {
                            generalLedgerAccount = account;
                            salesInvoice = database.SingleOrDefault<SalesInvoice>(transactionLine.GetBillableExpenseSalesInvoice());
                            if (salesInvoice != null && salesInvoice.Customer != customer.Key) salesInvoice = null;
                            if (division == null) division = database.SingleOrDefault<Division>(customer.Division);
                            isPurchaseTaxTransaction = true;
                        }
                    }
                    else if (account is ProfitAndLossStatementAccountBillableExpensesInvoiced)
                    {
                        generalLedgerAccount = account;
                        if (division == null && customer != null)
                        {
                            division = database.SingleOrDefault<Division>(customer.Division);
                        }
                    }
                    else if (account is BalanceSheetWithholdingTaxPayableAccount)
                    {
                        if (database.Single<WithholdingTax>().WithholdingTaxPayable)
                        {
                            supplier = database.SingleOrDefault<Supplier>(transactionLine.GetWithholdingTaxPayableSupplier());
                            if (supplier != null)
                            {
                                generalLedgerAccount = account;
                            }
                        }
                    }
                    else if (account is ProfitAndLossStatementAccountBillableTimeInvoiced)
                    {
                        generalLedgerAccount = account;
                    }
                    else if (account is BalanceSheetRetainedEarningsAccount)
                    {
                        generalLedgerAccount = account;
                    }
                    else if (account is ProfitAndLossStatementAccount)
                    {
                        generalLedgerAccount = account;
                    }
                    else if (account is ProfitAndLossStatementAccountFixedAssetLossOnDisposal)
                    {
                        generalLedgerAccount = account;
                    }
                    else if (account is ProfitAndLossStatementAccountIntangibleAssetsGainsLossOnDisposal)
                    {
                        generalLedgerAccount = account;
                    }
                    else if (account is ProfitAndLossStatementAccountInventorySales)
                    {
                        generalLedgerAccount = account;
                    }
                    else if (account is ProfitAndLossStatementAccountInventoryPurchases)
                    {
                        generalLedgerAccount = account;
                    }
                    else if (account is BalanceSheetWithholdingTaxAccount)
                    {
                        if (database.Any<WithholdingTaxReceipt>())
                        {
                            generalLedgerAccount = account;
                        }
                    }
                }
            }

            decimal? lineTotalInBaseCurrency = null;
            if (exchangeRate.HasValue)
            {
                lineTotalInBaseCurrency = transactionCurrency.GetBaseAmount(lineTotalAfterDiscount, exchangeRate.Value, exchangeRateIsInverse, database.Single<BaseCurrency>());
            }

            if (taxCode != null)
            {
                if (taxCode.TaxRate == Model.Enums.TaxRate.TotalRate)
                {
                    yield return new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                date: date,
                                generalLedgerAccount: database.SingleOrDefault<BalanceSheetAccount>(taxCode.Account) as IGeneralLedgerAccount ?? database.Single<BalanceSheetTaxPayableAccount>(),
                                transactionAmount: lineTotalAfterDiscount,
                                accountAmount: lineTotalInBaseCurrency,
                                baseAmount: lineTotalInBaseCurrency,
                                transactionCurrency: transactionCurrency,
                                transaction: transaction,
                                bankAccount: bankAccount,
                                customer: customer,
                                supplier: supplier,
                                employee: employee,
                                salesInvoice: salesInvoice,
                                purchaseInvoice: purchaseInvoice,
                                specialAccount: specialAccount,
                                capitalAccount: capitalAccount,
                                inventoryItem: inventoryItem,
                                inventoryKit: inventoryKit,
                                nonInventoryItem: nonInventoryItem,
                                fixedAsset: fixedAsset,
                                intangibleAsset: intangibleAsset,
                                expenseClaimPayer: expenseClaimPayer,
                                taxCode: taxCode,
                                taxComponent: taxCode.Name,
                                reportingCategory: taxCode.TaxAmountReportingCategory,
                                transactionLine: transactionLine,
                                isTaxTransaction: true,
                                trackingCode: division,
                                salesOrder: salesOrder,
                                purchaseOrder: purchaseOrder,
                                isPurchaseTaxTransaction: isPurchaseTaxTransaction,
                                exchangeRate: exchangeRate,
                                isExchangeRateInverse: exchangeRateIsInverse
                            );
                    lineTotalAfterDiscount = 0m;
                    if (lineTotalInBaseCurrency.HasValue) lineTotalInBaseCurrency = 0m;
                }
                else
                {
                    foreach (var e2 in taxCode.CalculateTaxAmounts(lineTotalAfterDiscount, transactionCurrency.GetDecimalPlaces(), amountsIncludeTax))
                    {
                        if (e2.Amount != 0m)
                        {
                            decimal? baseAmount2 = null;
                            if (exchangeRate.HasValue)
                            {
                                baseAmount2 = transactionCurrency.GetBaseAmount(e2.Amount, exchangeRate.Value, exchangeRateIsInverse, database.Single<BaseCurrency>());
                            }

                            var taxTransaction = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                date: date,
                                generalLedgerAccount: database.SingleOrDefault<BalanceSheetAccount>(e2.Account) as IGeneralLedgerAccount ?? database.Single<BalanceSheetTaxPayableAccount>(),
                                transactionAmount: e2.Amount,
                                accountAmount: baseAmount2,
                                baseAmount: baseAmount2,
                                transactionCurrency: transactionCurrency,
                                transaction: transaction,
                                bankAccount: bankAccount,
                                customer: customer,
                                supplier: supplier,
                                employee: employee,
                                salesInvoice: salesInvoice,
                                purchaseInvoice: purchaseInvoice,
                                specialAccount: specialAccount,
                                capitalAccount: capitalAccount,
                                inventoryItem: inventoryItem,
                                inventoryKit: inventoryKit,
                                nonInventoryItem: nonInventoryItem,
                                fixedAsset: fixedAsset,
                                intangibleAsset: intangibleAsset,
                                expenseClaimPayer: expenseClaimPayer,
                                taxCode: taxCode,
                                taxComponent: e2.Code,
                                reportingCategory: e2.TaxReportingCategory,
                                transactionLine: transactionLine,
                                isTaxTransaction: true,
                                salesOrder: salesOrder,
                                trackingCode: division,
                                purchaseOrder: purchaseOrder,
                                isPurchaseTaxTransaction: isPurchaseTaxTransaction,
                                exchangeRate: exchangeRate,
                                isExchangeRateInverse: exchangeRateIsInverse
                            );
                            yield return taxTransaction;
                            if (taxCode.ReverseCharged)
                            {
                                var taxTransaction2 = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                    database: database,
                                    date: date,
                                    generalLedgerAccount: database.SingleOrDefault<BalanceSheetAccount>(e2.Account) as IGeneralLedgerAccount ?? database.Single<BalanceSheetTaxPayableAccount>(),
                                    transactionAmount: e2.Amount * -1m,
                                    accountAmount: baseAmount2 * -1m,
                                    baseAmount: baseAmount2 * -1m,
                                    transactionCurrency: transactionCurrency,
                                    transaction: transaction,
                                    bankAccount: bankAccount,
                                    customer: customer,
                                    supplier: supplier,
                                    employee: employee,
                                    salesInvoice: salesInvoice,
                                    purchaseInvoice: purchaseInvoice,
                                    specialAccount: specialAccount,
                                    capitalAccount: capitalAccount,
                                    inventoryItem: inventoryItem,
                                    inventoryKit: inventoryKit,
                                    nonInventoryItem: nonInventoryItem,
                                    fixedAsset: fixedAsset,
                                    intangibleAsset: intangibleAsset,
                                    expenseClaimPayer: expenseClaimPayer,
                                    taxCode: taxCode,
                                    taxComponent: e2.Code,
                                    reportingCategory: e2.TaxReportingCategoryReversed,
                                    transactionLine: transactionLine,
                                    isTaxTransaction: true,
                                    isReversedTaxTransaction: true,
                                    salesOrder: salesOrder,
                                    trackingCode: division,
                                    purchaseOrder: purchaseOrder,
                                    isPurchaseTaxTransaction: isPurchaseTaxTransaction,
                                    exchangeRate: exchangeRate,
                                    isExchangeRateInverse: exchangeRateIsInverse
                                );
                                yield return taxTransaction2;
                            }
                            else
                            {
                                if (amountsIncludeTax || e2.NegativeRate)
                                {
                                    lineTotalAfterDiscount -= taxTransaction.TransactionAmount;
                                    if (lineTotalInBaseCurrency.HasValue) lineTotalInBaseCurrency -= taxTransaction.BaseAmount;
                                }
                            }
                        }
                    }
                }
            }

            var o = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: date,
                generalLedgerAccount: generalLedgerAccount,
                qty: lineQty,
                transactionAmount: lineTotalAfterDiscount,
                discount: discount,
                baseAmount: lineTotalInBaseCurrency,
                transactionCurrency: transactionCurrency,
                transaction: transaction,
                bankAccount: bankAccount,
                interAccountTransferAccount: interAccountTransferAccount,
                customer: customer,
                supplier: supplier,
                employee: employee,
                salesInvoice: salesInvoice,
                purchaseInvoice: purchaseInvoice,
                specialAccount: specialAccount,
                capitalAccount: capitalAccount,
                capitalSubaccount: capitalSubaccount,
                inventoryItem: inventoryItem,
                inventoryKit: inventoryKit,
                nonInventoryItem: nonInventoryItem,
                taxCode: taxCode,
                reportingCategory: taxCode?.ReportingCategory,
                reportingCategoryReversed: taxCode?.ReportingCategoryReversed,
                inventoryLocation: inventoryLocation,
                fixedAsset: fixedAsset,
                expenseClaimPayer: expenseClaimPayer,
                intangibleAsset: intangibleAsset,
                accountAmount: transactionLine.GetProposedAccountAmount(),
                transactionLine: transactionLine,
                trackingCode: division,
                salesOrder: salesOrder,
                project: project,
                purchaseOrder: purchaseOrder,
                investment: investment,
                lineNumber: lineNumber,
                exchangeRate: exchangeRate,
                isExchangeRateInverse: exchangeRateIsInverse
            );

            yield return o;

            if (generalLedgerAccount.Key == database.Single<BalanceSheetBillableExpensesAccount>().Key && salesInvoice != null)
            {
                yield return new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    date: salesInvoice.IssueDate,
                    generalLedgerAccount: generalLedgerAccount,
                    transactionAmount: lineTotalAfterDiscount * -1m,
                    baseAmount: lineTotalInBaseCurrency.HasValue ? decimal.Negate(lineTotalInBaseCurrency.Value) : null,
                    transactionCurrency: transactionCurrency,
                    transaction: transaction,
                    bankAccount: bankAccount,
                    customer: customer,
                    supplier: supplier,
                    employee: employee,
                    salesInvoice: salesInvoice,
                    purchaseInvoice: purchaseInvoice,
                    specialAccount: specialAccount,
                    expenseClaimPayer: expenseClaimPayer,
                    capitalAccount: capitalAccount,
                    transactionLine: transactionLine,
                    accountAmount: o.AccountAmount * -1m,
                    isBillableExpense: true,
                    salesOrder: salesOrder,
                    trackingCode: division,
                    purchaseOrder: purchaseOrder,
                    exchangeRate: exchangeRate,
                    isExchangeRateInverse: exchangeRateIsInverse
                );

                yield return new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    date: salesInvoice.IssueDate,
                    generalLedgerAccount: database.Single<ProfitAndLossStatementAccountBillableExpensesCost>(),
                    transactionAmount: lineTotalAfterDiscount,
                    baseAmount: lineTotalInBaseCurrency,
                    transactionCurrency: transactionCurrency,
                    transaction: transaction,
                    bankAccount: bankAccount,
                    customer: customer,
                    supplier: supplier,
                    employee: employee,
                    salesInvoice: salesInvoice,
                    purchaseInvoice: purchaseInvoice,
                    specialAccount: specialAccount,
                    expenseClaimPayer: expenseClaimPayer,
                    capitalAccount: capitalAccount,
                    transactionLine: transactionLine,
                    trackingCode: division,
                    isBillableExpense: true,
                    salesOrder: salesOrder,
                    purchaseOrder: purchaseOrder,
                    exchangeRate: exchangeRate,
                    isExchangeRateInverse: exchangeRateIsInverse
                );
            }
        }

        public GeneralLedgerTransaction(Database database, DateTime date, decimal transactionAmount, Currency transactionCurrency, IGeneralLedgerAccount generalLedgerAccount, Transaction transaction = null, BankOrCashAccount bankAccount = null, SpecialAccount specialAccount = null, CapitalAccount capitalAccount = null, TaxCode taxCode = null, Division trackingCode = null, IntangibleAsset intangibleAsset = null, FixedAsset fixedAsset = null, InventoryItem inventoryItem = null, CustomInventoryLocation inventoryLocation = null, Customer customer = null, Supplier supplier = null, Employee employee = null, SalesInvoice salesInvoice = null, PurchaseInvoice purchaseInvoice = null, ExpenseClaimsPayer expenseClaimPayer = null, PayslipEarningsItem payslipEarningsItem = null, PayslipDeductionItem payslipDeductionItem = null, PayslipContributionItem payslipContributionItem = null, InventoryKit inventoryKit = null, decimal? qty = null, bool isBalancing = false, decimal? accountAmount = null, decimal? baseAmount = null, ITransactionLine transactionLine = null, SubAccount capitalSubaccount = null, bool isTaxTransaction = false, bool isReversedTaxTransaction = false, bool isCostOfGoodsSold = false, decimal? costOfGoodsSoldQty = null, NonInventoryItem nonInventoryItem = null, GeneralLedgerTransaction[] contraTransactions = null, bool isBillableExpense = false, string taxComponent = null, Transaction accrualBasisTransaction = null, SalesOrder salesOrder = null, PurchaseOrder purchaseOrder = null, bool cashBasisAdjustment = false, DateTime? originalDate = null, Project project = null, Guid? reportingCategory = null, Guid? reportingCategoryReversed = null, Investment investment = null, bool isLandingCost = false, bool isPurchaseTaxTransaction = false, bool isPayslipEarningsLine = false, bool isPayslipDeductionLine = false, bool isPayslipContributionLine = false, int? lineNumber = null, decimal? exchangeRate = null, bool isExchangeRateInverse = false, decimal discount = 0m, BankOrCashAccount interAccountTransferAccount = null, decimal? purchaseCost = null, decimal? qtyReduction = null, bool isFixedAssetDisposalTransaction = false, bool isIntangibleAssetDisposalTransaction = false, InvestmentMarketPrice investmentMarketPrice = null, InventoryUnitCost inventoryUnitCost = null)
        {
            Date = date;
            OriginalDate = originalDate;

            TransactionAmount = transactionCurrency.Round(transactionAmount);
            TransactionCurrency = transactionCurrency;
            Qty = qty;

            GeneralLedgerAccount = generalLedgerAccount;
            BankAccount = bankAccount;
            InterAccountTransferAccount = interAccountTransferAccount;
            SpecialAccount = specialAccount;
            CapitalAccount = capitalAccount;
            CapitalSubaccount = capitalSubaccount;
            IntangibleAsset = intangibleAsset;
            FixedAsset = fixedAsset;
            Customer = customer;
            Supplier = supplier;
            Employee = employee;
            SalesInvoice = salesInvoice;
            PurchaseInvoice = purchaseInvoice;
            TaxCode = taxCode;
            InventoryLocation = inventoryLocation;
            InventoryItem = inventoryItem;
            InventoryKit = inventoryKit;
            PayslipEarningsItem = payslipEarningsItem;
            PayslipDeductionItem = payslipDeductionItem;
            PayslipContributionItem = payslipContributionItem;
            Division = trackingCode;
            TaxCode = taxCode;
            Transaction = transaction;
            IsBalancing = isBalancing;
            ExpenseClaimPayer = expenseClaimPayer;
            TransactionLine = transactionLine;
            IsTaxTransaction = isTaxTransaction;
            IsReversedTaxTransaction = isReversedTaxTransaction;
            IsBillableExpense = isBillableExpense;
            IsCostOfGoodsSold = isCostOfGoodsSold;
            NonInventoryItem = nonInventoryItem;
            ContraTransactions = contraTransactions;
            TaxComponent = taxComponent;
            ReportingCategory = reportingCategory;
            ReportingCategoryReversed = reportingCategoryReversed;
            Investment = investment;
            IsLandingCost = isLandingCost;
            IsPurchaseTaxTransaction = isPurchaseTaxTransaction;
            IsPayslipEarningsLine = isPayslipEarningsLine;
            IsPayslipDeductionLine = isPayslipDeductionLine;
            IsPayslipContributionLine = isPayslipContributionLine;
            LineNumber = lineNumber;
            AccrualBasisTransaction = accrualBasisTransaction;
            SalesOrder = salesOrder;
            CashBasisAdjustment = cashBasisAdjustment;
            Project = project;
            PurchaseOrder = purchaseOrder;
            ExchangeRate = exchangeRate;
            IsExchangeRateInverse = isExchangeRateInverse;
            Discount = discount;
            PurchaseCost = purchaseCost;
            IsFixedAssetDisposalTransaction = isFixedAssetDisposalTransaction;
            IsIntangibleAssetDisposalTransaction = isIntangibleAssetDisposalTransaction;
            InvestmentMarketPrice = investmentMarketPrice;
            InventoryUnitCost = inventoryUnitCost;

            if (GeneralLedgerAccount.IsAccountsReceivable)
            {
                if (SalesInvoice != null)
                {
                    if (Customer != null && Customer.Key == SalesInvoice.Customer)
                    {
                        // Sales Invoice belongs to customer - no action
                    }
                    else
                    {
                        Customer = database.SingleOrDefault<Customer>(SalesInvoice.Customer);
                    }
                }

                if (Customer == null)
                {
                    GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                    SalesInvoice = null;
                }
                else
                {
                    GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForCustomers>(Customer.ControlAccount) as IGeneralLedgerAccount ?? database.Single<BalanceSheetAccountsReceivableAccount>();
                    AccountCurrency = database.SingleOrDefault<ForeignCurrency>(this.Customer.Currency);
                }
            }
            else if (GeneralLedgerAccount.IsAccountsPayable)
            {
                if (PurchaseInvoice != null)
                {
                    if (Supplier != null && Supplier.Key == PurchaseInvoice.Supplier)
                    {
                        // Purchase Invoice belongs to supplier - no action
                    }
                    else
                    {
                        Supplier = database.SingleOrDefault<Supplier>(PurchaseInvoice.Supplier);
                    }
                }

                if (Supplier == null)
                {
                    GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                    PurchaseInvoice = null;
                }
                else
                {
                    GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForSuppliers>(Supplier.ControlAccount) as IGeneralLedgerAccount ?? database.Single<BalanceSheetAccountsPayableAccount>();
                    AccountCurrency = database.SingleOrDefault<ForeignCurrency>(this.Supplier.Currency);
                }
            }
            else if (GeneralLedgerAccount.IsCashAtBank)
            {
                if (BankAccount == null)
                {
                    GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                }
                else
                {
                    GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForBankAccounts>(BankAccount.ControlAccount) as IGeneralLedgerAccount ?? database.Single<BalanceSheetCashAtBankAccount>();
                    AccountCurrency = database.SingleOrDefault<ForeignCurrency>(BankAccount?.Currency);
                }
            }
            else if (GeneralLedgerAccount.IsInventoryOnHand)
            {
                if (InventoryItem == null)
                {
                    GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                }
                else
                {
                    GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForInventoryItems>(InventoryItem.ControlAccount) as IGeneralLedgerAccount ?? database.Single<BalanceSheetInventoryOnHandAccount>();
                }
            }
            else if (GeneralLedgerAccount.IsEmployeeClearingAccount)
            {
                if (Employee == null)
                {
                    GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                }
                else
                {
                    GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForEmployees>(Employee.ControlAccount) as IGeneralLedgerAccount ?? database.Single<BalanceSheetEmployeeClearingAccount>();
                    AccountCurrency = database.SingleOrDefault<ForeignCurrency>(Employee.Currency);
                }
            }
            else if (GeneralLedgerAccount.IsControlAccountForSpecialAccounts)
            {
                if (SpecialAccount == null)
                {
                    GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                }
                else
                {
                    GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForSpecialAccounts>(SpecialAccount.ControlAccount) as IGeneralLedgerAccount ?? database.Single<BalanceSheetSpecialAccountsAccount>();
                    AccountCurrency = database.SingleOrDefault<ForeignCurrency>(SpecialAccount.Currency);
                }
            }
            else if (GeneralLedgerAccount.IsControlAccountForCapitalAccounts)
            {
                if (CapitalAccount == null) GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                else GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForCapitalAccounts>(CapitalAccount.ControlAccount) as IGeneralLedgerAccount ?? database.Single<BalanceSheetCapitalAccountsAccount>();
            }
            else if (GeneralLedgerAccount.IsControlAccountForFixedAssets)
            {
                if (FixedAsset == null) GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                else GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForFixedAssets>(FixedAsset.ControlAccountForFixedAssets) as IGeneralLedgerAccount ?? database.Single<BalanceSheetFixedAssetsAtCostAccount>();
            }
            else if (GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation)
            {
                if (FixedAsset == null) GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                else GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForFixedAssetsAccumulatedDepreciation>(FixedAsset.ControlAccountForFixedAssetsAccumulatedDepreciation) as IGeneralLedgerAccount ?? database.Single<BalanceSheetFixedAssetsAccumulatedDepreciationAccount>();
            }
            else if (GeneralLedgerAccount.IsControlAccountForIntangibleAssets)
            {
                if (IntangibleAsset == null) GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                else GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForIntangibleAssets>(IntangibleAsset.ControlAccountForIntangibleAssets) as IGeneralLedgerAccount ?? database.Single<BalanceSheetIntangibleAssetsAtCostAccount>();
            }
            else if (GeneralLedgerAccount.IsControlAccountForIntangibleAssetsAccumulatedAmortization)
            {
                if (IntangibleAsset == null) GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                else GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForIntangibleAssetsAccumulatedAmortization>(IntangibleAsset.ControlAccountForIntangibleAssetsAccumulatedAmortization) as IGeneralLedgerAccount ?? database.Single<BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount>();
            }
            else if (GeneralLedgerAccount is BalanceSheetBillableExpensesAccount)
            {
                if (Customer == null) GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                else AccountCurrency = database.SingleOrDefault<ForeignCurrency>(Customer.Currency);
            }
            else if (GeneralLedgerAccount is BalanceSheetBillableTimeAccount)
            {
                if (Customer == null) GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                else AccountCurrency = database.SingleOrDefault<ForeignCurrency>(Customer.Currency);
            }
            else if (GeneralLedgerAccount is BalanceSheetWithholdingTaxPayableAccount)
            {
                if (Supplier == null) GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                else AccountCurrency = database.SingleOrDefault<ForeignCurrency>(Supplier.Currency);
            }
            else if (GeneralLedgerAccount is BalanceSheetWithholdingTaxReceivableAccount)
            {
                if (Customer == null) GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                else AccountCurrency = database.SingleOrDefault<ForeignCurrency>(Customer.Currency);
            }
            else if (GeneralLedgerAccount.IsControlAccountForInvestments)
            {
                if (Investment == null)
                {
                    GeneralLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                }
                else
                {
                    GeneralLedgerAccount = database.SingleOrDefault<ControlAccountForInvestments>(Investment.ControlAccount) as IGeneralLedgerAccount ?? database.Single<BalanceSheetInvestmentsAccount>();
                }
            }

            if (AccountCurrency == null)
            {
                if (TransactionCurrency is BaseCurrency)
                {
                    AccountCurrency = TransactionCurrency;
                }
                else
                {
                    AccountCurrency = database.Single<BaseCurrency>();
                }
            }

            if (baseAmount.HasValue)
            {
                if (GeneralLedgerAccount.CanHaveCurrencyAmount && AccountCurrency is BaseCurrency && TransactionCurrency is ForeignCurrency && transactionLine?.GetProposedAccountAmount() != null && transactionLine.GetProposedAccountAmount().Value != 0m && Transaction is not ManagerServer.Model.JournalEntry)
                {
                    var proposedAccountAmount = transactionLine.GetProposedAccountAmount().Value;
                    if (Math.Sign(proposedAccountAmount) != Math.Sign(baseAmount.Value)) proposedAccountAmount *= -1m;
                    BaseAmount = AccountCurrency.Round(proposedAccountAmount);
                    ExchangeRate = null;
                }
                else
                {
                    BaseAmount = baseAmount.Value;
                }

                if (AccountCurrency is BaseCurrency)
                {
                    AccountAmount = BaseAmount;
                }
                else if (AccountCurrency == TransactionCurrency)
                {
                    AccountAmount = TransactionAmount;
                }
                else if (accountAmount.HasValue && accountAmount.Value != 0m && GeneralLedgerAccount.CanHaveCurrencyAmount)
                {
                    if (Math.Sign(TransactionAmount) == Math.Sign(accountAmount.Value))
                    {
                        AccountAmount = AccountCurrency.Round(accountAmount.Value);
                    }
                    else
                    {
                        AccountAmount = AccountCurrency.Round(accountAmount.Value * -1m);
                    }
                }
                else
                {
                    if (TransactionCurrency is BaseCurrency)
                    {
                        var exchangeRate2 = database.FindExchangeRate(AccountCurrency.Key, date)?.GetBaseRate() ?? 1m;
                        AccountAmount = AccountCurrency.Round(TransactionAmount * exchangeRate2);
                    }
                    else
                    {
                        var exchangeRate1 = database.FindExchangeRate(TransactionCurrency.Key, date)?.GetBaseRate() ?? 1m;
                        var exchangeRate2 = database.FindExchangeRate(AccountCurrency.Key, date)?.GetBaseRate() ?? 1m;
                        AccountAmount = AccountCurrency.Round(TransactionAmount / exchangeRate1 * exchangeRate2);
                    }
                }
            }
            else
            {
                if (TransactionAmount != 0m)
                {
                    if (TransactionCurrency == AccountCurrency)
                    {
                        AccountAmount = TransactionAmount;
                    }
                    else if (accountAmount.HasValue && GeneralLedgerAccount.CanHaveCurrencyAmount)
                    {
                        if (Math.Sign(TransactionAmount) == Math.Sign(accountAmount.Value))
                        {
                            AccountAmount = AccountCurrency.Round(accountAmount.Value);
                        }
                        else
                        {
                            AccountAmount = AccountCurrency.Round(accountAmount.Value * -1m);
                        }
                    }
                    else
                    {
                        if (AccountCurrency is BaseCurrency)
                        {
                            var exchangeRate2 = database.FindExchangeRate(TransactionCurrency.Key, date)?.GetBaseRate() ?? 1m;
                            AccountAmount = AccountCurrency.Round(TransactionAmount / exchangeRate2);
                        }
                        else if (TransactionCurrency is BaseCurrency)
                        {
                            var exchangeRate2 = database.FindExchangeRate(AccountCurrency.Key, date)?.GetBaseRate() ?? 1m;
                            AccountAmount = AccountCurrency.Round(TransactionAmount * exchangeRate2);
                        }
                        else
                        {
                            var exchangeRate1 = database.FindExchangeRate(TransactionCurrency.Key, date)?.GetBaseRate() ?? 1m;
                            var exchangeRate2 = database.FindExchangeRate(AccountCurrency.Key, date)?.GetBaseRate() ?? 1m;
                            AccountAmount = AccountCurrency.Round(TransactionAmount / exchangeRate1 * exchangeRate2);
                        }
                    }

                    if (AccountCurrency is BaseCurrency)
                    {
                        BaseAmount = AccountAmount;
                    }
                    else if (TransactionCurrency is BaseCurrency)
                    {
                        BaseAmount = TransactionAmount;
                    }
                    else if (baseAmount.HasValue)
                    {
                        BaseAmount = baseAmount.Value;
                    }
                    else
                    {
                        var exchangeRate2 = database.FindExchangeRate(AccountCurrency.Key, date)?.GetBaseRate() ?? 1m;
                        var baseCurrency = database.Single<BaseCurrency>();
                        BaseAmount = baseCurrency.Round(AccountAmount / exchangeRate2);
                    }
                }
            }

            if (GeneralLedgerAccount is IBalanceSheetAccount balanceSheetAccount)
            {
                BalanceSheetAccount = balanceSheetAccount;
            }
            else
            {
                BalanceSheetAccount = database.Single<BalanceSheetRetainedEarningsAccount>();
                ProfitAndLossAccount = (IProfitAndLossAccount)GeneralLedgerAccount;
            }            
        }

        public IItem Item { get { return NonInventoryItem as IItem ?? InventoryKit as IItem ?? InventoryItem as IItem; } }
        public string Description
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(TransactionLine?.GetLineDescription(Transaction)))
                {
                    return TransactionLine?.GetLineDescription(Transaction);
                }
                else
                {
                    return Transaction?.GetDescriptionOrNull();
                }
            }
        }

        public Receipt Receipt { get { return Transaction as Receipt; } }
        public Payment Payment { get { return Transaction as Payment; } }
        public InterAccountTransfer InterAccountTransfer { get { return Transaction as InterAccountTransfer; } }
        public ExpenseClaim ExpenseClaim { get { return Transaction as ExpenseClaim; } }
        public CreditNote CreditNote { get { return Transaction as CreditNote; } }
        public DeliveryNote DeliveryNote { get { return Transaction as DeliveryNote; } }
        public GoodsReceipt GoodsReceipt { get { return Transaction as GoodsReceipt; } }
        public DebitNote DebitNote { get { return Transaction as DebitNote; } }
        public JournalEntry JournalEntry { get { return Transaction as JournalEntry; } }
        public BillableTime BillableTime { get { return Transaction as BillableTime; } }
        public InventoryWriteOff InventoryWriteOff { get { return Transaction as InventoryWriteOff; } }
        public Payslip Payslip { get { return Transaction as Payslip; } }
        public SalesQuote SalesQuote { get { return Transaction as SalesQuote; } }
        public SalesOrder SalesOrderAsTransaction { get { return Transaction as SalesOrder; } }
        public PurchaseQuote PurchaseQuote { get { return Transaction as PurchaseQuote; } }
        public PurchaseOrder PurchaseOrderAsTransaction { get { return Transaction as PurchaseOrder; } }
        public InventoryTransfer InventoryTransfer { get { return Transaction as InventoryTransfer; } }
        public DepreciationEntry DepreciationEntry { get { return Transaction as DepreciationEntry; } }
        public AmortizationEntry AmortizationEntry { get { return Transaction as AmortizationEntry; } }
        public ProductionOrder ProductionOrder { get { return Transaction as ProductionOrder; } }
        public SalesInvoice SalesInvoiceAsTransaction { get { return Transaction as SalesInvoice; } }
        public PurchaseInvoice PurchaseInvoiceAsTransaction { get { return Transaction as PurchaseInvoice; } }

        public decimal? Debit { get { return BaseAmount > 0m ? BaseAmount : default(decimal?); } }
        public decimal? Credit { get { return BaseAmount < 0m ? BaseAmount * -1m : default(decimal?); } }
        public decimal? TaxAmount { get { return IsTaxTransaction ? BaseAmount : default(decimal?); } }

        public decimal? QtyMultipliedByNegativeOne { get { return Qty.HasValue ? Qty.Value * -1m : default(decimal?); } }
        public decimal AmountMultipliedByNegativeOne { get { return BaseAmount * -1m; } }
        public decimal AccountAmountMultipliedByNegativeOne { get { return AccountAmount * -1m; } }
        public decimal? TaxAmountMultipliedByNegativeOne { get { return TaxAmount.HasValue ? TaxAmount.Value * -1m : default(decimal?); } }

        public string Account
        {
            get
            {
                if (GeneralLedgerAccount is BalanceSheetSuspenseAccount && AccountAmount == 0m && TransactionAmount == 0m && BaseAmount == 0m)
                {
                    return String.Empty;
                }

                var output = GeneralLedgerAccount.GetCodeAndName();
                if (GeneralLedgerAccount.IsAccountsPayable && Supplier != null)
                {
                    output += " — " + Supplier.GetCodeAndName();
                    if (PurchaseInvoice != null)
                    {
                        output += " — " + PurchaseInvoice.GetName();
                    }
                }
                else if (GeneralLedgerAccount.HasCustomers && Customer != null)
                {
                    output += " — " + Customer.GetCodeAndName();
                    if (SalesInvoice != null)
                    {
                        output += " — " + SalesInvoice.GetName();
                    }
                }
                else if (GeneralLedgerAccount.IsCashAtBank && BankAccount != null) output += " — " + BankAccount.GetCodeAndName();
                else if (GeneralLedgerAccount.IsControlAccountForCapitalAccounts && CapitalAccount != null)
                {
                    output += " — " + CapitalAccount.GetCodeAndName();
                    if (CapitalSubaccount != null) output += " — " + CapitalSubaccount.Name;
                }
                else if ((GeneralLedgerAccount.IsControlAccountForFixedAssets || GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation) && FixedAsset != null) output += " — " + FixedAsset.GetCodeAndName();
                else if ((GeneralLedgerAccount.IsControlAccountForIntangibleAssets || GeneralLedgerAccount.IsControlAccountForIntangibleAssetsAccumulatedAmortization) && IntangibleAsset != null) output += " — " + IntangibleAsset.GetCodeAndName();
                else if (GeneralLedgerAccount.IsControlAccountForSpecialAccounts && SpecialAccount != null) output += " — " + SpecialAccount.GetCodeAndName();
                else if (GeneralLedgerAccount.IsEmployeeClearingAccount && Employee != null) output += " — " + Employee.GetCodeAndName();
                else if (GeneralLedgerAccount.IsInventoryOnHand && InventoryItem != null) output += " — " + InventoryItem.GetCodeAndName();
                else if (GeneralLedgerAccount.IsControlAccountForInvestments && Investment != null) output += " — " + Investment.GetCodeAndName();
                else if (GeneralLedgerAccount.IsInterAccountTransfers && InterAccountTransferAccount != null) output += " — " + InterAccountTransferAccount.GetCodeAndName();
                return output;
            }
        }

        public NamedObject SubAccount
        {
            get
            {
                // Bank Accounts
                if (GeneralLedgerAccount is BalanceSheetCashAtBankAccount) return BankAccount;
                if (GeneralLedgerAccount is ControlAccountForBankAccounts) return BankAccount;

                // Customers
                if (GeneralLedgerAccount is BalanceSheetAccountsReceivableAccount) return Customer;
                if (GeneralLedgerAccount is ControlAccountForCustomers) return Customer;
                if (GeneralLedgerAccount is BalanceSheetBillableExpensesAccount) return Customer;
                if (GeneralLedgerAccount is BalanceSheetBillableTimeAccount) return Customer;
                if (GeneralLedgerAccount is BalanceSheetWithholdingTaxReceivableAccount) return Customer;

                // Suppliers
                if (GeneralLedgerAccount is BalanceSheetAccountsPayableAccount) return Supplier;
                if (GeneralLedgerAccount is ControlAccountForSuppliers) return Supplier;
                if (GeneralLedgerAccount is BalanceSheetWithholdingTaxPayableAccount) return Supplier;

                // Employees
                if (GeneralLedgerAccount is BalanceSheetEmployeeClearingAccount) return Employee;
                if (GeneralLedgerAccount is ControlAccountForEmployees) return Employee;

                // Special Accounts
                if (GeneralLedgerAccount is BalanceSheetSpecialAccountsAccount) return SpecialAccount;
                if (GeneralLedgerAccount is ControlAccountForSpecialAccounts) return SpecialAccount;

                // Inventory Items
                if (GeneralLedgerAccount is BalanceSheetInventoryOnHandAccount) return InventoryItem;
                if (GeneralLedgerAccount is ControlAccountForInventoryItems) return InventoryItem;

                // Investments
                if (GeneralLedgerAccount is BalanceSheetInvestmentsAccount) return Investment;
                if (GeneralLedgerAccount is ControlAccountForInvestments) return Investment;

                // Fixed Assets
                if (GeneralLedgerAccount is BalanceSheetFixedAssetsAtCostAccount) return FixedAsset;
                if (GeneralLedgerAccount is BalanceSheetFixedAssetsAccumulatedDepreciationAccount) return FixedAsset;
                if (GeneralLedgerAccount is ControlAccountForFixedAssets) return FixedAsset;
                if (GeneralLedgerAccount is ControlAccountForFixedAssetsAccumulatedDepreciation) return FixedAsset;

                // IntangibleAssets
                if (GeneralLedgerAccount is BalanceSheetIntangibleAssetsAtCostAccount) return IntangibleAsset;
                if (GeneralLedgerAccount is BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount) return IntangibleAsset;
                if (GeneralLedgerAccount is ControlAccountForIntangibleAssets) return IntangibleAsset;
                if (GeneralLedgerAccount is ControlAccountForIntangibleAssetsAccumulatedAmortization) return IntangibleAsset;

                // Capital Accounts
                if (GeneralLedgerAccount is BalanceSheetCapitalAccountsAccount) return CapitalAccount;
                if (GeneralLedgerAccount is ControlAccountForCapitalAccounts) return CapitalAccount;

                // Expense Claim Payers
                if (GeneralLedgerAccount is BalanceSheetExpenseClaimsAccount) return ExpenseClaimPayer;

                return null;
            }
        }

        public NamedObject ForeignCurrencyAccount
        {
            get
            {
                if (AccountCurrency is not ForeignCurrency) return null;

                // Bank Accounts
                if (GeneralLedgerAccount is BalanceSheetCashAtBankAccount) return BankAccount;
                if (GeneralLedgerAccount is ControlAccountForBankAccounts) return BankAccount;

                // Customers
                if (GeneralLedgerAccount is BalanceSheetAccountsReceivableAccount) return Customer;
                if (GeneralLedgerAccount is ControlAccountForCustomers) return Customer;
                if (GeneralLedgerAccount is BalanceSheetBillableExpensesAccount) return Customer;
                if (GeneralLedgerAccount is BalanceSheetBillableTimeAccount) return Customer;
                if (GeneralLedgerAccount is BalanceSheetWithholdingTaxReceivableAccount) return Customer;

                // Suppliers
                if (GeneralLedgerAccount is BalanceSheetAccountsPayableAccount) return Supplier;
                if (GeneralLedgerAccount is ControlAccountForSuppliers) return Supplier;
                if (GeneralLedgerAccount is BalanceSheetWithholdingTaxPayableAccount) return Supplier;

                // Employees
                if (GeneralLedgerAccount is BalanceSheetEmployeeClearingAccount) return Employee;
                if (GeneralLedgerAccount is ControlAccountForEmployees) return Employee;

                // Special Accounts
                if (GeneralLedgerAccount is BalanceSheetSpecialAccountsAccount) return SpecialAccount;
                if (GeneralLedgerAccount is ControlAccountForSpecialAccounts) return SpecialAccount;
                return null;
            }
        }

        public decimal? SalesAmount
        {
            get
            {
                if (IsSale) return BaseAmount * -1m;
                return null;
            }
        }

        public decimal? PurchaseAmount
        {
            get
            {
                if (!IsSale) return BaseAmount;
                return null;
            }
        }

        public decimal? TaxExclusiveSalesAmount
        {
            get
            {
                if (IsSale && !IsTaxTransaction) return BaseAmount * -1m;
                return null;
            }
        }

        public decimal? TaxExclusivePurchaseAmount
        {
            get
            {
                if (!IsSale && !IsTaxTransaction) return BaseAmount;
                return null;
            }
        }

        public decimal? SalesTaxAmount
        {
            get
            {
                if (IsSale && IsTaxTransaction) return BaseAmount * -1m;
                return null;
            }
        }

        public decimal? PurchaseTaxAmount
        {
            get
            {
                if (!IsSale && IsTaxTransaction) return BaseAmount;
                return null;
            }
        }

        public bool IsProjectCost
        {
            get
            {
                if (IsCostOfGoodsSold) return true;
                if (GeneralLedgerAccount.IsBillableExpense) return true;
                if (Customer == null && Receipt == null) return true;
                return false;
            }
        }

        public decimal QtyToDeliver
        {
            get
            {
                if (Transaction is DeliveryNote) return Qty ?? 0m;
                if (Transaction is SalesInvoice salesInvoice && !salesInvoice.AlsoActsAsDeliveryNote) return (Qty ?? 0m) * -1m;
                if (Transaction is CreditNote creditNote && !creditNote.AlsoActsAsDeliveryNote) return (Qty ?? 0m) * -1m;
                if (Transaction is InventoryItemStartingBalance && TransactionLine is InventoryItemStartingBalance.QtyToDeliverLine) return (Qty ?? 0m) * -1m;
                return 0m;
            }
        }

        public decimal QtyToReceive
        {
            get
            {
                if (Transaction is GoodsReceipt) return (Qty ?? 0m) * -1m;
                if (Transaction is PurchaseInvoice purchaseInvoice && !purchaseInvoice.AlsoActsAsGoodsReceipt) return Qty ?? 0m;
                if (Transaction is DebitNote debitNote && !debitNote.AlsoActsAsGoodsReceipt) return Qty ?? 0m;
                if (Transaction is InventoryItemStartingBalance && TransactionLine is InventoryItemStartingBalance.QtyToReceiveLine) return Qty ?? 0m;
                return 0m;
            }
        }

        public decimal QtyOwned
        {
            get
            {
                if (Transaction is GoodsReceipt) return 0m;
                if (Transaction is DeliveryNote) return 0m;
                return Qty ?? 0m;
            }
        }

        public decimal QtyOnHand
        {
            get
            {
                if (Transaction is SalesInvoice salesInvoice && !salesInvoice.AlsoActsAsDeliveryNote) return 0m;
                if (Transaction is PurchaseInvoice purchaseInvoice && !purchaseInvoice.AlsoActsAsGoodsReceipt) return 0m;
                if (Transaction is DebitNote debitNote && !debitNote.AlsoActsAsGoodsReceipt) return 0m;
                if (Transaction is CreditNote creditNote && !creditNote.AlsoActsAsDeliveryNote) return 0m;
                if (Transaction is InventoryItemStartingBalance && TransactionLine is not InventoryItemStartingBalance.QtyOnHandLine) return 0m;
                return Qty ?? 0m;
            }
        }

        public decimal QtyOnOrder
        {
            get
            {
                if (Transaction is PurchaseOrder purchaseOrder && !purchaseOrder.Cancelled) return Qty ?? 0m;
                if (PurchaseOrder != null && !PurchaseOrder.Cancelled)
                {
                    if (Transaction is PurchaseInvoice) return QtyOnHand * -1m;
                    if (Transaction is GoodsReceipt) return QtyOnHand * -1m;
                }
                return 0m;
            }
        }

        public decimal QtyReserved
        {
            get
            {
                if (Transaction is SalesOrder salesOrder && !salesOrder.Cancelled) return (Qty ?? 0m) * -1m;
                if (SalesOrder != null && !SalesOrder.Cancelled)
                {
                    if (Transaction is SalesInvoice) return QtyOnHand;
                    if (Transaction is DeliveryNote) return QtyOnHand;
                }
                return 0m;
            }
        }

        public decimal QtyOrdered
        {
            get
            {
                if (Transaction is SalesOrder) return (Qty ?? 0m) * -1m;
                if (Transaction is PurchaseOrder) return Qty ?? 0m;
                return 0m;
            }
        }

        public decimal QtyInvoiced
        {
            get
            {
                if (Transaction is SalesInvoice) return (Qty ?? 0m) * -1m;
                if (Transaction is PurchaseInvoice) return Qty ?? 0m;
                return 0m;
            }
        }

        public decimal QtyDelivered
        {
            get
            {
                if (Transaction is DeliveryNote) return (Qty ?? 0m) * -1m;
                if (Transaction is SalesInvoice salesInvoice && salesInvoice.AlsoActsAsDeliveryNote) return (Qty ?? 0m) * -1m;
                if (Transaction is PurchaseInvoice purchaseInvoice && purchaseInvoice.AlsoActsAsGoodsReceipt) return Qty ?? 0m;
                if (Transaction is GoodsReceipt) return Qty ?? 0m;
                return 0m;
            }
        }

        public string TransactionTitle
        {
            get
            {
                return Transaction?.TransactionTitle;
            }
        }

        public string TransactionName
        {
            get
            {
                return Transaction?.GetTransactionName();
            }
        }

        public bool IsSale
        {
            get
            {
                if (GeneralLedgerAccount.IsBillableExpense || IsPurchaseTaxTransaction)
                {
                    return false;
                }
                if (Supplier != null)
                {
                    if (!IsReversedTaxTransaction) return false;
                    else return true;
                }
                if (Customer != null)
                {
                    if (!IsReversedTaxTransaction) return true;
                    else return false;
                }
                if (JournalEntry != null && TaxCode != null)
                {
                    if (JournalEntry.ForTaxPurposesThisIs == Model.Enums.TaxTransactionType.SaleOrSaleAdjustment)
                    {
                        if (IsReversedTaxTransaction) return false;
                        return true;
                    }
                    if (JournalEntry.ForTaxPurposesThisIs == Model.Enums.TaxTransactionType.PurchaseOrPurchaseAdjustment)
                    {
                        if (IsReversedTaxTransaction) return true;
                        return false;
                    }
                }
                if (BaseAmount < 0m || (Qty.HasValue && Qty.Value < 0m))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsUncategorizedPaymentOrReceipt
        {
            get
            {
                if (Transaction is ManagerServer.Model.Receipt || Transaction is ManagerServer.Model.Payment)
                {
                    if (IsBalancing)
                    {
                        if (ContraTransactions != null && ContraTransactions.Length == 1 && ContraTransactions[0] != null && ContraTransactions[0].GeneralLedgerAccount is BalanceSheetSuspenseAccount)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public Tuple<BankOrCashAccount, BankOrCashAccount> InterAccountTransferPair
        {
            get
            {
                if (BankAccount == null) return null;
                if (InterAccountTransferAccount == null) return null;
                if (BankAccount.Key < InterAccountTransferAccount.Key)
                {
                    return new Tuple<BankOrCashAccount, BankOrCashAccount>(BankAccount, InterAccountTransferAccount);
                }
                else
                {
                    return new Tuple<BankOrCashAccount, BankOrCashAccount>(InterAccountTransferAccount, BankAccount);
                }
            }
        }

        public bool IsInvoiceTransaction
        {
            get
            {
                if (Transaction is SalesInvoice) return true;
                if (Transaction is PurchaseInvoice) return true;
                if (Transaction is CreditNote) return true;
                if (Transaction is DebitNote) return true;
                return false;
            }
        }

        public bool IsCashFlowStatementTransaction
        {
            get
            {
                if (Transaction is InterAccountTransfer) return true;
                if (Transaction is Receipt) return true;
                if (Transaction is Payment) return true;
                if (Transaction is ExpenseClaim) return true;
                //if (Transaction is SalesInvoice) return true;
                //if (Transaction is PurchaseInvoice) return true;
                //if (Transaction is CreditNote) return true;
                //if (Transaction is DebitNote) return true;
                if (Transaction is JournalEntry && JournalEntry.CashTransactionForCashFlowStatementPurposes) return true;
                return false;
            }
        }

        public string Contact
        {
            get
            {
                if (Transaction is SalesInvoice) return Customer?.Name;
                if (Transaction is CreditNote) return Customer?.Name;
                if (Transaction is PurchaseInvoice) return Supplier?.Name;
                if (Transaction is DebitNote) return Supplier?.Name;
                if (Transaction is Receipt) return Customer?.Name ?? Supplier?.Name ?? Employee?.Name ?? ExpenseClaimPayer?.Name ?? CapitalAccount?.Name ?? ((Receipt)Transaction).Contact;
                if (Transaction is Payment) return Customer?.Name ?? Supplier?.Name ?? Employee?.Name ?? ExpenseClaimPayer?.Name ?? CapitalAccount?.Name ?? ((Payment)Transaction).Contact;
                if (Transaction is ExpenseClaim) return ExpenseClaimPayer?.Name;
                if (Transaction is Payslip) return Employee?.Name;
                if (Transaction is BillableTime) return Customer?.Name;
                return null;
            }
        }

        public DateTime? ClearDate
        {
            get
            {
                if (BankAccount != null && BankAccount.CanHavePendingTransactions)
                {
                    if (Transaction is Receipt) return Receipt.GetClearDate();
                    if (Transaction is Payment) return Payment.GetClearDate();
                }
                if (Transaction is InterAccountTransfer && BankAccount != null)
                {
                    if (BaseAmount < 0m && InterAccountTransfer.PaidFrom == BankAccount.Key) return InterAccountTransfer.GetCreditClearDate();
                    if (BaseAmount > 0m && InterAccountTransfer.ReceivedIn == BankAccount.Key) return InterAccountTransfer.GetDebitClearDate();
                }
                return Date;
            }
        }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => TransactionLine?.GetCustomFields();
        CustomFields ICustomFields.CustomFields => TransactionLine?.GetCustomFields2();

        public Tuple<decimal, Currency> GetTransactionAmountWithCurrency() => new Tuple<decimal, Currency>(TransactionAmount, TransactionCurrency);
        public Tuple<decimal, Currency> GetReversedTransactionAmountWithCurrency() => new Tuple<decimal, Currency>(TransactionAmount * -1m, TransactionCurrency);
        public Tuple<decimal, Currency> GetDiscountAmountWithCurrency() => Discount != 0m ? new Tuple<decimal, Currency>(Discount, TransactionCurrency) : null;

        public IEnumerable<Guid> GetKeys()
        {
            yield return TransactionCurrency.Key;
            yield return AccountCurrency.Key;
            yield return ProfitAndLossAccount?.Key ?? Guid.Empty;
            yield return BalanceSheetAccount?.Key ?? Guid.Empty;
            yield return BankAccount?.Key ?? Guid.Empty;
            yield return InterAccountTransferAccount?.Key ?? Guid.Empty;
            yield return Customer?.Key ?? Guid.Empty;
            yield return Supplier?.Key ?? Guid.Empty;
            yield return Employee?.Key ?? Guid.Empty;
            yield return ExpenseClaimPayer?.Key ?? Guid.Empty;
            yield return InventoryItem?.Key ?? Guid.Empty;
            yield return NonInventoryItem?.Key ?? Guid.Empty;
            yield return InventoryKit?.Key ?? Guid.Empty;
            yield return FixedAsset?.Key ?? Guid.Empty;
            yield return IntangibleAsset?.Key ?? Guid.Empty;
            yield return SpecialAccount?.Key ?? Guid.Empty;
            yield return CapitalAccount?.Key ?? Guid.Empty;
            yield return PayslipEarningsItem?.Key ?? Guid.Empty;
            yield return PayslipDeductionItem?.Key ?? Guid.Empty;
            yield return PayslipContributionItem?.Key ?? Guid.Empty;
            yield return SalesInvoice?.Key ?? Guid.Empty;
            yield return SalesOrder?.Key ?? Guid.Empty;
            yield return PurchaseOrder?.Key ?? Guid.Empty;
            yield return PurchaseInvoice?.Key ?? Guid.Empty;
            yield return CapitalSubaccount?.Key ?? Guid.Empty;
            yield return Division?.Key ?? Guid.Empty;
            yield return InventoryLocation?.Key ?? Guid.Empty;
            yield return TaxCode?.Key ?? Guid.Empty;
            yield return ReportingCategory ?? Guid.Empty;
            yield return Project?.Key ?? Guid.Empty;
            yield return Investment?.Key ?? Guid.Empty;
            yield return InventoryUnitCost?.Key ?? Guid.Empty;
        }
    }
}
