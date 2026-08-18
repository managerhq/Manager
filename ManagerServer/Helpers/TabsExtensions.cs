using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.HttpHandlers.Businesses.Business;

namespace ManagerServer.Helpers
{
    public static class TabsExtensions
    {
        internal static Collection GetTabs(this BusinessTemplate httpHandler, bool applyUserPermissions)
        {
            return GetTabs(httpHandler, applyUserPermissions, httpHandler.Business);
        }

        private static int GetCount<T>(Database database) where T : ManagerServer.Model.Object, new()
        {
            return database.GetCount<T>();
        }

        private static int GetPendingCount<T>(Database database) where T : ManagerServer.Model.Object, ManagerServer.Model.IRecurringTransaction, new()
        {
            if (database.Any<T>())
            {
                return database.UnorderedOfType<T>().Count(x => x.CanBeIssued());
            }
            return 0;
        }

        internal static Collection GetTabs(this HttpHandlers.HttpHandler httpHandler, bool applyUserPermissions, string fileId)
        {
            var database = ApplicationData.Instance.Businesses.Get(fileId);

            var collection = new Collection();

            var o = database.Single<ManagerServer.Model.Tabs>();
            if (o.Timestamp == 0) collection.IsDefault = true;

            collection.Summary = new Item() { DisplayName = Strings.Summary, Name = "Summary", HttpHandler = new HttpHandlers.Businesses.Business.Summary.SummaryView() { Business = fileId }, Visible = true };
            collection.BankAndCashAccounts = new Item() { DisplayName = Strings.BankAndCashAccounts, Count = GetCount<ManagerServer.Model.BankOrCashAccount>(database), Name = "BankAndCashAccounts", HttpHandler = new HttpHandlers.Businesses.Business.BankAndCashAccounts.BankAndCashAccounts() { Business = fileId }, Visible = o.BankAndCashAccounts };
            collection.Receipts = new Item() { DisplayName = Strings.Receipts, Count = GetCount<ManagerServer.Model.Receipt>(database), Name = "Receipts", HttpHandler = new HttpHandlers.Businesses.Business.Receipts.Receipts() { Business = fileId }, Visible = o.Receipts, PendingCount = GetPendingCount<ManagerServer.Model.RecurringReceipt>(database) };
            collection.Payments = new Item() { DisplayName = Strings.Payments, Count = GetCount<ManagerServer.Model.Payment>(database), Name = "Payments", HttpHandler = new HttpHandlers.Businesses.Business.Payments.Payments() { Business = fileId }, Visible = o.Payments, PendingCount = GetPendingCount<ManagerServer.Model.RecurringPayment>(database) };
            collection.InterAccountTransfers = new Item() { DisplayName = Strings.InterAccountTransfers, Count = GetCount < ManagerServer.Model.InterAccountTransfer>(database), Name = "InterAccountTransfers", HttpHandler = new HttpHandlers.Businesses.Business.InterAccountTransfers.InterAccountTransfers() { Business = fileId }, Visible = o.InterAccountTransfers, PendingCount = GetPendingCount<ManagerServer.Model.RecurringInterAccountTransfer>(database) };
            collection.BankReconciliations = new Item() { DisplayName = Strings.BankReconciliations, Count = GetCount < ManagerServer.Model.BankReconciliation>(database), Name = "BankReconciliations", HttpHandler = new HttpHandlers.Businesses.Business.BankReconciliations.BankReconciliations() { Business = fileId }, Visible = o.BankReconciliations };
            collection.ExpenseClaims = new Item() { DisplayName = Strings.ExpenseClaims, Count = GetCount < ManagerServer.Model.ExpenseClaim>(database), Name = "ExpenseClaims", HttpHandler = new HttpHandlers.Businesses.Business.ExpenseClaims.ExpenseClaims() { Business = fileId }, Visible = o.ExpenseClaims };
            collection.Customers = new Item() { DisplayName = Strings.Customers, Count = GetCount < ManagerServer.Model.Customer>(database), Name = "Customers", HttpHandler = new HttpHandlers.Businesses.Business.Customers.Customers() { Business = fileId }, Visible = o.Customers };
            collection.SalesQuotes = new Item() { DisplayName = Strings.SalesQuotes, Count = GetCount<ManagerServer.Model.SalesQuote>(database), Name = "SalesQuotes", HttpHandler = new HttpHandlers.Businesses.Business.SalesQuotes.SalesQuotes() { Business = fileId }, Visible = o.SalesQuotes, PendingCount = GetPendingCount<ManagerServer.Model.RecurringSalesQuote>(database) };
            collection.SalesOrders = new Item() { DisplayName = Strings.SalesOrders, Count = GetCount<ManagerServer.Model.SalesOrder>(database), Name = "SalesOrders", HttpHandler = new HttpHandlers.Businesses.Business.SalesOrders.SalesOrders() { Business = fileId }, Visible = o.SalesOrders, PendingCount = GetPendingCount<ManagerServer.Model.RecurringSalesOrder>(database) };
            collection.SalesInvoices = new Item() { DisplayName = Strings.SalesInvoices, Count = GetCount<ManagerServer.Model.SalesInvoice>(database), Name = "SalesInvoices", HttpHandler = new HttpHandlers.Businesses.Business.SalesInvoices.SalesInvoices() { Business = fileId }, Visible = o.SalesInvoices, PendingCount = GetPendingCount<ManagerServer.Model.RecurringSalesInvoice>(database) };
            collection.CreditNotes = new Item() { DisplayName = Strings.CreditNotes, Count = GetCount<ManagerServer.Model.CreditNote>(database), Name = "CreditNotes", HttpHandler = new HttpHandlers.Businesses.Business.CreditNotes.CreditNotes() { Business = fileId }, Visible = o.CreditNotes };
            collection.LatePaymentFees = new Item() { DisplayName = Strings.Late_Payment_Fees, Count = GetCount<ManagerServer.Model.LatePaymentFee>(database), Name = "LatePaymentFees", HttpHandler = new HttpHandlers.Businesses.Business.LatePaymentFees.LatePaymentFees() { Business = fileId }, Visible = o.LatePaymentFees };
            collection.DeliveryNotes = new Item() { DisplayName = Strings.DeliveryNotes, Count = GetCount < ManagerServer.Model.DeliveryNote>(database), Name = "DeliveryNotes", HttpHandler = new HttpHandlers.Businesses.Business.DeliveryNotes.DeliveryNotes() { Business = fileId }, Visible = o.DeliveryNotes };
            collection.WithholdingTaxReceipts = new Item() { DisplayName = Strings.WithholdingTaxReceipts, Count = GetCount<ManagerServer.Model.WithholdingTaxReceipt>(database), Name = "WithholdingTaxReceipts", HttpHandler = new HttpHandlers.Businesses.Business.WithholdingTaxReceipts.WithholdingTaxReceipts() { Business = fileId }, Visible = o.WithholdingTaxReceipts };
            collection.Suppliers = new Item() { DisplayName = Strings.Suppliers, Count = GetCount < ManagerServer.Model.Supplier>(database), Name = "Suppliers", HttpHandler = new HttpHandlers.Businesses.Business.Suppliers.Suppliers() { Business = fileId }, Visible = o.Suppliers };
            collection.PurchaseQuotes = new Item() { DisplayName = Strings.PurchaseQuotes, Count = GetCount<ManagerServer.Model.PurchaseQuote>(database), Name = "PurchaseQuotes", HttpHandler = new HttpHandlers.Businesses.Business.PurchaseQuotes.PurchaseQuotes() { Business = fileId }, Visible = o.PurchaseQuotes };
            collection.PurchaseOrders = new Item() { DisplayName = Strings.PurchaseOrders, Count = GetCount<ManagerServer.Model.PurchaseOrder>(database), Name = "PurchaseOrders", HttpHandler = new HttpHandlers.Businesses.Business.PurchaseOrders.PurchaseOrders() { Business = fileId }, Visible = o.PurchaseOrders, PendingCount = GetPendingCount<ManagerServer.Model.RecurringPurchaseOrder>(database) };
            collection.PurchaseInvoices = new Item() { DisplayName = Strings.PurchaseInvoices, Count = GetCount<ManagerServer.Model.PurchaseInvoice>(database), Name = "PurchaseInvoices", HttpHandler = new HttpHandlers.Businesses.Business.PurchaseInvoices.PurchaseInvoices() { Business = fileId }, Visible = o.PurchaseInvoices, PendingCount = GetPendingCount<ManagerServer.Model.RecurringPurchaseInvoice>(database) };
            collection.DebitNotes = new Item() { DisplayName = Strings.DebitNotes, Count = GetCount<ManagerServer.Model.DebitNote>(database), Name = "DebitNotes", HttpHandler = new HttpHandlers.Businesses.Business.DebitNotes.DebitNotes() { Business = fileId }, Visible = o.DebitNotes };
            collection.Employees = new Item() { DisplayName = Strings.Employees, Count = GetCount<ManagerServer.Model.Employee>(database), Name = "Employees", HttpHandler = new HttpHandlers.Businesses.Business.Employees.Employees() { Business = fileId }, Visible = o.Employees };
            collection.Payslips = new Item() { DisplayName = Strings.Payslips, Count = GetCount<ManagerServer.Model.Payslip>(database), Name = "Payslips", HttpHandler = new HttpHandlers.Businesses.Business.Payslips.Payslips() { Business = fileId }, Visible = o.Payslips, PendingCount = GetPendingCount<ManagerServer.Model.RecurringPayslip>(database) };
            collection.Projects = new Item() { DisplayName = Strings.Projects, Count = GetCount<ManagerServer.Model.Project>(database), Name = "Projects", HttpHandler = new HttpHandlers.Businesses.Business.Projects.Projects() { Business = fileId }, Visible = o.Projects };
            collection.InventoryItems = new Item() { DisplayName = Strings.InventoryItems, Count = GetCount<ManagerServer.Model.InventoryItem>(database), Name = "InventoryItems", HttpHandler = new HttpHandlers.Businesses.Business.InventoryItems.InventoryItems() { Business = fileId }, Visible = o.InventoryItems };
            collection.InventoryTransfers = new Item() { DisplayName = Strings.InventoryTransfers, Count = GetCount<ManagerServer.Model.InventoryTransfer>(database), Name = "InventoryTransfers", HttpHandler = new HttpHandlers.Businesses.Business.InventoryTransfers.InventoryTransfers() { Business = fileId }, Visible = o.InventoryTransfers };
            collection.InventoryWriteOffs = new Item() { DisplayName = Strings.InventoryWriteOffs, Count = GetCount<ManagerServer.Model.InventoryWriteOff>(database), Name = "InventoryWriteOffs", HttpHandler = new HttpHandlers.Businesses.Business.InventoryWriteOffs.InventoryWriteOffs() { Business = fileId }, Visible = o.InventoryWriteOffs };
            collection.GoodsReceipts = new Item() { DisplayName = Strings.GoodsReceipts, Count = GetCount<ManagerServer.Model.GoodsReceipt>(database), Name = "GoodsReceipts", HttpHandler = new HttpHandlers.Businesses.Business.GoodsReceipts.GoodsReceipts() { Business = fileId }, Visible = o.GoodsReceipts };
            collection.ProductionOrders = new Item() { DisplayName = Strings.ProductionOrders, Count = GetCount < ManagerServer.Model.ProductionOrder>(database), Name = "ProductionOrders", HttpHandler = new HttpHandlers.Businesses.Business.ProductionOrders.ProductionOrders() { Business = fileId }, Visible = o.ProductionOrders };
            collection.BillableTime = new Item() { DisplayName = Strings.BillableTime, Count = GetCount < ManagerServer.Model.BillableTime>(database), Name = "BillableTime", HttpHandler = new HttpHandlers.Businesses.Business.BillableTime.BillableTime() { Business = fileId }, Visible = o.BillableTime };
            collection.Investments = new Item() { DisplayName = Strings.Investments, Count = GetCount<ManagerServer.Model.Investment>(database), Name = "Investments", HttpHandler = new HttpHandlers.Businesses.Business.Investments.Investments() { Business = fileId }, Visible = o.Investments };
            collection.FixedAssets = new Item() { DisplayName = Strings.FixedAssets, Count = GetCount < ManagerServer.Model.FixedAsset>(database), Name = "FixedAssets", HttpHandler = new HttpHandlers.Businesses.Business.FixedAssets.FixedAssets() { Business = fileId }, Visible = o.FixedAssets };
            collection.DepreciationEntries = new Item() { DisplayName = Strings.DepreciationEntries, Count = GetCount < ManagerServer.Model.DepreciationEntry>(database), Name = "DepreciationEntries", HttpHandler = new HttpHandlers.Businesses.Business.DepreciationEntries.DepreciationEntries() { Business = fileId }, Visible = o.DepreciationEntries };
            collection.IntangibleAssets = new Item() { DisplayName = Strings.IntangibleAssets, Count = GetCount < ManagerServer.Model.IntangibleAsset>(database), Name = "IntangibleAssets", HttpHandler = new HttpHandlers.Businesses.Business.IntangibleAssets.IntangibleAssets() { Business = fileId }, Visible = o.IntangibleAssets };
            collection.AmortizationEntries = new Item() { DisplayName = Strings.AmortizationEntries, Count = GetCount < ManagerServer.Model.AmortizationEntry>(database), Name = "AmortizationEntries", HttpHandler = new HttpHandlers.Businesses.Business.AmortizationEntries.AmortizationEntries() { Business = fileId }, Visible = o.AmortizationEntries };
            collection.CapitalAccounts = new Item() { DisplayName = Strings.CapitalAccounts, Count = GetCount < ManagerServer.Model.CapitalAccount>(database), Name = "CapitalAccounts", HttpHandler = new HttpHandlers.Businesses.Business.CapitalAccounts.CapitalAccounts() { Business = fileId }, Visible = o.CapitalAccounts };
            collection.SpecialAccounts = new Item() { DisplayName = Strings.SpecialAccounts, Count = GetCount < ManagerServer.Model.SpecialAccount>(database), Name = "SpecialAccounts", HttpHandler = new HttpHandlers.Businesses.Business.SpecialAccounts.SpecialAccounts() { Business = fileId }, Visible = o.SpecialAccounts };
            collection.JournalEntries = new Item() { DisplayName = Strings.JournalEntries, Count = GetCount < ManagerServer.Model.JournalEntry>(database), Name = "JournalEntries", HttpHandler = new HttpHandlers.Businesses.Business.JournalEntries.JournalEntries() { Business = fileId }, Visible = true, PendingCount = GetPendingCount<ManagerServer.Model.RecurringJournalEntry>(database) };
            collection.Folders = new Item() { DisplayName = Strings.Folders, Count = GetCount<ManagerServer.Model.Folder>(database), Name = "Folders", HttpHandler = new HttpHandlers.Businesses.Business.Folders.Folders() { Business = fileId }, Visible = o.Folders };            
            collection.Reports = new Item() { DisplayName = Strings.Reports, Name = "Reports", HttpHandler = new HttpHandlers.Businesses.Business.Reports.Reports() { Business = fileId }, Visible = true };
            collection.Settings = new Item() { DisplayName = Strings.Settings, Name = "Settings", HttpHandler = new HttpHandlers.Businesses.Business.Settings.Settings() { Business = fileId }, Visible = true };

            foreach (var e in collection.GetAll().Where(x => !x.Visible && x.Count.HasValue && x.Count.Value > 0)) e.Visible = true;

            if (applyUserPermissions)
            {
                var userPermissions = httpHandler.GetCurrentUserPermissions(fileId);
                if (!userPermissions.FullAccess)
                {
                    foreach (var e in collection.GetAll())
                    {
                        if (!userPermissions.CanView(e.HttpHandler.GetType().Namespace)) e.Visible = false;
                    }                    
                }
            }

            return collection;
        }

        public sealed class Collection
        {
            public bool IsDefault;
            public Item Summary;
            public Item BankAndCashAccounts;
            public Item BankReconciliations;
            public Item Receipts;
            public Item Payments;
            public Item ExpenseClaims;
            public Item Customers;
            public Item SalesQuotes;
            public Item SalesInvoices;
            public Item CreditNotes;
            public Item DeliveryNotes;
            public Item Suppliers;
            public Item PurchaseQuotes;
            public Item PurchaseOrders;
            public Item PurchaseInvoices;
            public Item InventoryItems;
            public Item BillableTime;
            public Item FixedAssets;
            public Item JournalEntries;
            public Item Reports;
            public Item Settings;
            public Item SalesOrders;
            public Item Employees;
            public Item Payslips;
            public Item DebitNotes;
            public Item InventoryWriteOffs;
            public Item ProductionOrders;
            public Item CapitalAccounts;
            public Item IntangibleAssets;
            public Item SpecialAccounts;
            public Item Folders;
            public Item GoodsReceipts;
            public Item InventoryTransfers;
            public Item InterAccountTransfers;
            public Item LatePaymentFees;
            public Item DepreciationEntries;
            public Item AmortizationEntries;
            public Item WithholdingTaxReceipts;
            public Item Projects;
            public Item Investments;

            public Item[] GetAll()
            {
                return new Item[]
                {
                    Summary,
                    BankAndCashAccounts,                    
                    Receipts,
                    Payments,
                    InterAccountTransfers,
                    BankReconciliations,
                    ExpenseClaims,
                    Customers,
                    SalesQuotes,
                    SalesOrders,
                    SalesInvoices,
                    CreditNotes,
                    LatePaymentFees,
                    DeliveryNotes,
                    BillableTime,
                    WithholdingTaxReceipts,
                    Suppliers,
                    PurchaseQuotes,
                    PurchaseOrders,
                    PurchaseInvoices,
                    DebitNotes,
                    GoodsReceipts,                    
                    InventoryItems,
                    InventoryTransfers,
                    InventoryWriteOffs,
                    ProductionOrders,
                    Projects,
                    Employees,
                    Payslips,
                    Investments,
                    FixedAssets,
                    DepreciationEntries,
                    IntangibleAssets,
                    AmortizationEntries,
                    CapitalAccounts,
                    SpecialAccounts,
                    JournalEntries,
                    Folders,
                    Reports,
                    Settings
                };
            }
        }

        public sealed class Item
        {
            public string DisplayName;
            public int? Count;
            public int? PendingCount;
            public string Name;
            public ManagerServer.HttpHandlers.HttpHandler HttpHandler;
            public bool Visible;
        }
    }
}
