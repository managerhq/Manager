using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete60
{
    [ProtoContract]
    [Guid("a4ad8967-717f-4ad7-9909-d24afba2f7e4")]
    public sealed class UserPermissions : Object
    {
        [ProtoMember(56)]
        public Guid User;
        [ProtoMember(1)]
        public AccessType AccessType;
        [ProtoMember(155)]
        public string Business;

        [ProtoMember(57)]
        public Guid Obsolete_Business;

        // Tabs
        [ProtoMember(3)]
        public bool Summary;
        [ProtoMember(6)]
        public bool Customers;
        [ProtoMember(7)]
        public bool Suppliers;
        [ProtoMember(8)]
        public bool ChartOfAccounts;
        [ProtoMember(9)]
        public bool JournalEntries;
        [ProtoMember(10)]
        public bool EmailSettings;
        [ProtoMember(11)]
        public bool AgedReceivables;
        [ProtoMember(12)]
        public bool AgedPayables;
        [ProtoMember(13)]
        public bool ProfitAndLossStatement;
        [ProtoMember(14)]
        public bool BalanceSheet;
        [ProtoMember(15)]
        public bool TrialBalance;
        [ProtoMember(16)]
        public bool GeneralLedgerSummary;
        [ProtoMember(17)]
        public bool TaxAudit;
        [ProtoMember(18)]
        public bool TaxSummary;
        [ProtoMember(19)]
        public bool TaxCodes;
        [ProtoMember(20)]
        public bool StatementOfChangesInEquity;
        [ProtoMember(21)]
        public bool GeneralLedgerTransactions;
        [ProtoMember(22)]
        public bool Australia_GstCalculationWorksheet;
        [ProtoMember(23)]
        public bool UnitedKingdom_VatReturn;
        [ProtoMember(25)]
        public bool RemittanceAdvices;
        [ProtoMember(26)]
        public bool InventoryValueSummary;
        [ProtoMember(27)]
        public bool InventoryProfitMargin;
        [ProtoMember(28)]
        public bool FixedAssetSummary;
        [ProtoMember(29)]
        public bool CapitalAccountsSummary;
        [ProtoMember(30)]
        public bool BusinessDetails;
        [ProtoMember(33)]
        public bool SalesQuoteTemplate;
        [ProtoMember(34)]
        public bool SalesInvoiceTemplate;
        [ProtoMember(35)]
        public bool PurchaseOrderTemplate;
        [ProtoMember(36)]
        public bool CurrencyPrefixSuffix;
        [ProtoMember(37)]
        public bool WindowFacedEnvelope;
        [ProtoMember(38)]
        public bool BusinessLogo;
        [ProtoMember(39)]
        public bool CapitalSubaccounts;
        [ProtoMember(41)]
        public bool ExpenseClaims;
        [ProtoMember(42)]
        public bool SalesQuotes;
        [ProtoMember(43)]
        public bool SalesInvoices;
        [ProtoMember(44)]
        public bool CreditNotes;
        [ProtoMember(45)]
        public bool DeliveryNotes;
        [ProtoMember(46)]
        public bool PurchaseOrders;
        [ProtoMember(47)]
        public bool PurchaseInvoices;
        [ProtoMember(48)]
        public bool InventoryItems;
        [ProtoMember(49)]
        public bool BillableTime;
        [ProtoMember(50)]
        public bool FixedAssets;
        [ProtoMember(51)]
        public bool Reports;
        [ProtoMember(52)]
        public bool Settings;
        [ProtoMember(53)]
        public bool CustomFields;
        [ProtoMember(54)]
        public bool TrackingCodes;
        [ProtoMember(55)]
        public bool ClassifiedBalanceSheet;
        [ProtoMember(58)]
        public bool PaymentAdviceCutaway;
        [ProtoMember(59)]
        public bool CashSummary;
        [ProtoMember(60)]
        public bool Emails;
        [ProtoMember(61)]
        public bool CustomerStatementsUnpaidInvoices;
        [ProtoMember(62)]
        public bool ExchangeRates;
        [ProtoMember(63)]
        public bool SalesOrders;
        [ProtoMember(64)]
        public bool BaseCurrency;
        [ProtoMember(65)]
        public bool StartingBalances;
        [ProtoMember(66)]
        public bool Employees;
        [ProtoMember(67)]
        public bool Payslips;
        [ProtoMember(68)]
        public bool InventoryQuantitySummary;
        [ProtoMember(70)]
        public bool HtmlThemes;
        [ProtoMember(71)]
        public bool TaxTransactions;
        [ProtoMember(72)]
        public bool TaxReconciliation;
        [ProtoMember(73)]
        public bool ExpenseClaimPayers;
        [ProtoMember(74)]
        public bool DebitNotes;
        [ProtoMember(75)]
        public bool InventoryWriteOffs;
        [ProtoMember(76)]
        public bool WorkInProgressReport;
        [ProtoMember(77)]
        public bool ProductionOrders;
        [ProtoMember(78)]
        public bool MultiStepIncomeStatement;
        [ProtoMember(79)]
        public bool PayrollLiabilities;
        [ProtoMember(80)]
        public bool EmailTemplates;
        [ProtoMember(81)]
        public bool RecurringSalesInvoices;
        [ProtoMember(82)]
        public bool PayslipItems;
        [ProtoMember(83)]
        public bool LockDate;
        [ProtoMember(84)]
        public bool PayslipSummary;
        [ProtoMember(87)]
        public bool StartDate;
        [ProtoMember(88)]
        public int PermittedActions;
        [ProtoMember(90)]
        public bool ExpenseClaimsSummary;
        [ProtoMember(91)]
        public bool AutomaticCreditAllocations;
        [ProtoMember(92)]
        public bool BillableExpenses;
        [ProtoMember(93)]
        public bool InventoryKits;
        [ProtoMember(94)]
        public bool BankRules;
        [ProtoMember(97)]
        public bool CapitalAccounts;
        [ProtoMember(98)]
        public bool IntangibleAssets;
        [ProtoMember(99)]
        public bool Netherlands_VatReturn;
        [ProtoMember(100)]
        public bool SupplierStatementsUnpaidInvoices;
        [ProtoMember(101)]
        public bool TrackingExceptionReport;
        [ProtoMember(102)]
        public bool CustomReports;
        [ProtoMember(104)]
        public bool RecurringPayslips;        
        [ProtoMember(107)]
        public bool BankAccounts;
        [ProtoMember(108)]
        public Guid[] BankAccounts2;
        [ProtoMember(110)]
        public bool NonInventoryItems;
        [ProtoMember(111)]
        public bool SpecialAccounts;
        [ProtoMember(112)]
        public bool Folders;
        [ProtoMember(114)]
        public bool Themes;
        [ProtoMember(115)]
        public bool InventoryLocations;
        [ProtoMember(116)]
        public bool GoodsReceipts;
        [ProtoMember(117)]
        public bool InventoryTransfers;
        [ProtoMember(118)]
        public bool IntangibleAssetSummary;
        [ProtoMember(119)]
        public bool ProfitAndLossStatementActualVsBudget;
        [ProtoMember(120)]
        public bool InventoryQuantityByLocation;
        [ProtoMember(121)]
        public bool InventoryPriceList;
        [ProtoMember(122)]
        public bool CashAccounts;
        [ProtoMember(123)]
        public Guid[] CashAccounts2;
        [ProtoMember(124)]
        public bool InterAccountTransfers;
        [ProtoMember(125)]
        public bool ReceiptsAndPayments;
        [ProtoMember(126)]
        public bool CashTransactions;
        [ProtoMember(128)]
        public bool SalesInvoiceTotalsByCustomer;
        [ProtoMember(129)]
        public bool SalesInvoiceTotalsByItem;
        [ProtoMember(130)]
        public bool SalesInvoiceTotalsByCustomField;
        [ProtoMember(131)]
        public bool RecurringPurchaseInvoices;
        [ProtoMember(132)]
        public bool FormDefaults;
        [ProtoMember(133)]
        public bool RecurringJournalEntries;
        [ProtoMember(134)]
        public bool CustomerStatementsTransactions;
        [ProtoMember(135)]
        public bool SupplierStatementsTransactions;
        [ProtoMember(136)]
        public bool BankAccountSummary;
        [ProtoMember(137)]
        public bool CashAccountSummary;
        [ProtoMember(138)]
        public bool LatePaymentFees;
        [ProtoMember(139)]
        public bool ReportTransformations;
        [ProtoMember(140)]
        public bool EmployeeSummary;
        [ProtoMember(141)]
        public bool TaxablePurchasesPerSupplier;
        [ProtoMember(142)]
        public bool TaxableSalesPerCustomer;
        [ProtoMember(143)]
        public bool PayslipTotals;
        [ProtoMember(144)]
        public bool CustomerSummary;
        [ProtoMember(145)]
        public bool SupplierSummary;
        [ProtoMember(146)]
        public bool PurchaseQuotes;
        [ProtoMember(147)]
        public bool Attachments;
        [ProtoMember(148)]
        public bool DepreciationEntries;
        [ProtoMember(149)]
        public bool DepreciationCalculationWorksheet;
        [ProtoMember(150)]
        public bool AmortizationEntries;
        [ProtoMember(151)]
        public bool AmortizationCalculationWorksheet;
        [ProtoMember(152)]
        public bool BankReconciliations;
        [ProtoMember(153)]
        public bool ForeignCurrencies;
        [ProtoMember(154)]
        public bool ControlAccounts;
        [ProtoMember(156)]
        public bool BlankReports;
        [ProtoMember(157)]
        public bool RegionFormats;

        [ProtoMember(31)]
        public bool Obsolete_SalesInvoiceItems;
        [ProtoMember(32)]
        public bool Obsolete_PurchaseInvoiceItems;
        [ProtoMember(69)]
        public bool Obsolete_ProhibitDeleting;
        [ProtoMember(103)]
        public bool Obsolete_TaxDeductionsAtSource;
        [ProtoMember(105)]
        public bool Obsolete_InventoryReturns;
        [ProtoMember(4)]
        public bool Obsolete_BankAccounts;
        [ProtoMember(5)]
        public bool Obsolete_CashAccounts;
        [ProtoMember(109)]
        public bool Obsolete_ControlAccounts;
        [ProtoMember(89)]
        public bool Obsolete_ViewTemplates;
        [ProtoMember(106)]
        public bool Obsolete_BankReconciliationStatement;
    }

    public enum AccessType : int
    {
        NoAccess = 0,
        CustomAccess = 1,
        FullAccess = 2,
    }
}
