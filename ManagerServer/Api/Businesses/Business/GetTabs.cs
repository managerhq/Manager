using ManagerServer.Api.Businesses.Business.AmortizationEntries;
using ManagerServer.Api.Businesses.Business.Attachments;
using ManagerServer.Api.Businesses.Business.BankAndCashAccounts;
using ManagerServer.Api.Businesses.Business.BankReconciliations;
using ManagerServer.Api.Businesses.Business.BillableTime;
using ManagerServer.Api.Businesses.Business.CapitalAccounts;
using ManagerServer.Api.Businesses.Business.CreditNotes;
using ManagerServer.Api.Businesses.Business.Customers;
using ManagerServer.Api.Businesses.Business.DebitNotes;
using ManagerServer.Api.Businesses.Business.DeliveryNotes;
using ManagerServer.Api.Businesses.Business.DepreciationEntries;
using ManagerServer.Api.Businesses.Business.Employees;
using ManagerServer.Api.Businesses.Business.ExpenseClaims;
using ManagerServer.Api.Businesses.Business.FixedAssets;
using ManagerServer.Api.Businesses.Business.Folders;
using ManagerServer.Api.Businesses.Business.GoodsReceipts;
using ManagerServer.Api.Businesses.Business.IntangibleAssets;
using ManagerServer.Api.Businesses.Business.InterAccountTransfers;
using ManagerServer.Api.Businesses.Business.InventoryItems;
using ManagerServer.Api.Businesses.Business.InventoryTransfers;
using ManagerServer.Api.Businesses.Business.InventoryWriteOffs;
using ManagerServer.Api.Businesses.Business.Investments;
using ManagerServer.Api.Businesses.Business.JournalEntries;
using ManagerServer.Api.Businesses.Business.LatePaymentFees;
using ManagerServer.Api.Businesses.Business.Payments;
using ManagerServer.Api.Businesses.Business.Payslips;
using ManagerServer.Api.Businesses.Business.ProductionOrders;
using ManagerServer.Api.Businesses.Business.Projects;
using ManagerServer.Api.Businesses.Business.PurchaseInvoices;
using ManagerServer.Api.Businesses.Business.PurchaseOrders;
using ManagerServer.Api.Businesses.Business.PurchaseQuotes;
using ManagerServer.Api.Businesses.Business.Receipts;
using ManagerServer.Api.Businesses.Business.Reports;
using ManagerServer.Api.Businesses.Business.SalesInvoices;
using ManagerServer.Api.Businesses.Business.SalesOrders;
using ManagerServer.Api.Businesses.Business.SalesQuotes;
using ManagerServer.Api.Businesses.Business.Settings;
using ManagerServer.Api.Businesses.Business.SpecialAccounts;
using ManagerServer.Api.Businesses.Business.Suppliers;
using ManagerServer.Api.Businesses.Business.WithholdingTaxReceipts;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business
{
    internal sealed record TabsResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetTabs : AuthorizedEndpoint<TabsResource>
    {
        public override TabsResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["amortizationEntries"]   = new Link(new GetAmortizationEntryBatch   { Business = Business }.ToUrl());
            links["attachments"]           = new Link(new GetAttachmentBatch          { Business = Business }.ToUrl());
            links["bankOrCashAccounts"]    = new Link(new GetBankOrCashAccountBatch   { Business = Business }.ToUrl());
            links["bankReconciliations"]   = new Link(new GetBankReconciliationBatch  { Business = Business }.ToUrl());
            links["billableTime"]          = new Link(new GetBillableTimeBatch        { Business = Business }.ToUrl());
            links["capitalAccounts"]       = new Link(new GetCapitalAccountBatch      { Business = Business }.ToUrl());
            links["creditNotes"]           = new Link(new GetCreditNoteBatch          { Business = Business }.ToUrl());
            links["customers"]             = new Link(new GetCustomerBatch            { Business = Business }.ToUrl());
            links["debitNotes"]            = new Link(new GetDebitNoteBatch           { Business = Business }.ToUrl());
            links["deliveryNotes"]         = new Link(new GetDeliveryNoteBatch        { Business = Business }.ToUrl());
            links["depreciationEntries"]   = new Link(new GetDepreciationEntryBatch   { Business = Business }.ToUrl());
            links["employees"]             = new Link(new GetEmployeeBatch            { Business = Business }.ToUrl());
            links["expenseClaims"]         = new Link(new GetExpenseClaimBatch        { Business = Business }.ToUrl());
            links["fixedAssets"]           = new Link(new GetFixedAssetBatch          { Business = Business }.ToUrl());
            links["folders"]               = new Link(new GetFolderBatch              { Business = Business }.ToUrl());
            links["goodsReceipts"]         = new Link(new GetGoodsReceiptBatch        { Business = Business }.ToUrl());
            links["intangibleAssets"]      = new Link(new GetIntangibleAssetBatch     { Business = Business }.ToUrl());
            links["interAccountTransfers"] = new Link(new GetInterAccountTransferBatch{ Business = Business }.ToUrl());
            links["inventoryItems"]        = new Link(new GetInventoryItemBatch       { Business = Business }.ToUrl());
            links["inventoryTransfers"]    = new Link(new GetInventoryTransferBatch   { Business = Business }.ToUrl());
            links["inventoryWriteOffs"]    = new Link(new GetInventoryWriteOffBatch   { Business = Business }.ToUrl());
            links["investments"]           = new Link(new GetInvestmentBatch          { Business = Business }.ToUrl());
            links["journalEntries"]        = new Link(new GetJournalEntryBatch        { Business = Business }.ToUrl());
            links["latePaymentFees"]       = new Link(new GetLatePaymentFeeBatch      { Business = Business }.ToUrl());
            links["payments"]              = new Link(new GetPaymentBatch             { Business = Business }.ToUrl());
            links["payslips"]              = new Link(new GetPayslipBatch             { Business = Business }.ToUrl());
            links["productionOrders"]      = new Link(new GetProductionOrderBatch     { Business = Business }.ToUrl());
            links["projects"]              = new Link(new GetProjectBatch             { Business = Business }.ToUrl());
            links["purchaseInvoices"]      = new Link(new GetPurchaseInvoiceBatch     { Business = Business }.ToUrl());
            links["purchaseOrders"]        = new Link(new GetPurchaseOrderBatch       { Business = Business }.ToUrl());
            links["purchaseQuotes"]        = new Link(new GetPurchaseQuoteBatch       { Business = Business }.ToUrl());
            links["receipts"]              = new Link(new GetReceiptBatch             { Business = Business }.ToUrl());
            links["reports"]               = new Link(new GetReports                  { Business = Business }.ToUrl());
            links["salesInvoices"]         = new Link(new GetSalesInvoiceBatch        { Business = Business }.ToUrl());
            links["salesOrders"]           = new Link(new GetSalesOrderBatch          { Business = Business }.ToUrl());
            links["salesQuotes"]           = new Link(new GetSalesQuoteBatch          { Business = Business }.ToUrl());
            links["settings"]              = new Link(new GetSettings                 { Business = Business }.ToUrl());
            links["specialAccounts"]       = new Link(new GetSpecialAccountBatch      { Business = Business }.ToUrl());
            links["suppliers"]             = new Link(new GetSupplierBatch            { Business = Business }.ToUrl());
            links["withholdingTaxReceipts"] = new Link(new GetWithholdingTaxReceiptBatch { Business = Business }.ToUrl());

            return new TabsResource(links);
        }
    }
}
