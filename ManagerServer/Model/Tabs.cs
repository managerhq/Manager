using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("ac789d1f-034f-4964-a8b5-ebfffc3511f2")]
    [Singleton]
    public sealed class Tabs : Object
    {
        [TabScreenshot("fa-coins", nameof(Strings.BankAndCashAccounts))]
        [Guide("The `BankAndCashAccounts` tab is designed for handling all transactions related to bank and cash, including keeping track of balances and movements within these accounts.")]        
        [ProtoMember(2), TabSwitch(true)] public bool BankAndCashAccounts { get; set; }

        [TabScreenshot("fa-plus-square", nameof(Strings.Receipts))]
        [Guide("The `Receipts` tab is designed for recording and tracking incoming money, helping you keep precise records of your income.")]
        [Guide("If you're utilizing this tab, you'll also need `BankAndCashAccounts` since every receipt must be connected to either a bank or cash account.")]
        [ProtoMember(44), TabSwitch(true), IfTrue(nameof(BankAndCashAccounts))] public bool Receipts { get; set; }

        [TabScreenshot("fa-minus-square", nameof(Strings.Payments))]
        [Guide("The `Payments` tab is used to record all outgoing payments, crucial for monitoring expenses and overseeing cash flow.")]
        [Guide("When using this tab, it's necessary to also use the `BankAndCashAccounts` feature since every payment must be linked to a bank or cash account.")]
        [ProtoMember(45), TabSwitch(true), IfTrue(nameof(BankAndCashAccounts))] public bool Payments { get; set; }

        [TabScreenshot("fa-arrow-alt-to-right", nameof(Strings.InterAccountTransfers))]
        [Guide("The `InterAccountTransfers` tab is used to record movements of funds between various bank or cash accounts owned by the business.")]
        [Guide("To use this tab effectively, you also require the `BankAndCashAccounts` function. This is because each transfer between accounts must be associated with either a bank or cash account.")]
        [ProtoMember(35), TabSwitch(false), IfTrue(nameof(BankAndCashAccounts))] public bool InterAccountTransfers { get; set; }

        [TabScreenshot("fa-clipboard-check", nameof(Strings.BankReconciliations))]
        [Guide("If you are utilizing this tab, it's necessary to also use `BankAndCashAccounts`. This is because each bank reconciliation must be associated with a bank or cash account.")]
        [ProtoMember(43), TabSwitch(true), IfTrue(nameof(BankAndCashAccounts))] public bool BankReconciliations { get; set; }

        [TabScreenshot("fa-wallet", nameof(Strings.ExpenseClaims))]
        [Guide("The `ExpenseClaims` tab is designed for handling the reimbursement process for costs that employees have incurred on behalf of the company.")]
        [ProtoMember(9), TabSwitch(false)] public bool ExpenseClaims { get; set; }

        [TabScreenshot("fa-users-class", nameof(Strings.Customers))]
        [Guide("The `Customers` tab is designed to keep a database of customer information, which is essential for managing relationships and sales effectively.")]
        [ProtoMember(14), TabSwitch(true)] public bool Customers { get; set; }

        [TabScreenshot("fa-drafting-compass", nameof(Strings.SalesQuotes))]
        [Guide("The `SalesQuotes` tab is designed for creating and managing price quotations that are offered to prospective customers.")]
        [Guide("To use this tab effectively, you must also have the `Customers` section set up, as each sales quote requires a customer to be issued to.")]
        [ProtoMember(4), TabSwitch(false), IfTrue(nameof(Customers))] public bool SalesQuotes { get; set; }

        [TabScreenshot("fa-shopping-cart", nameof(Strings.SalesOrders))]
        [Guide("The `SalesOrders` tab is designed for managing and monitoring customer orders until they are completed or billed.")]
        [Guide("If you're utilizing this tab, it's essential to also have `Customers` set up, as each sales order must be associated with a customer.")]
        [ProtoMember(17), TabSwitch(false), IfTrue(nameof(Customers))] public bool SalesOrders { get; set; }

        [TabScreenshot("fa-file-invoice", nameof(Strings.SalesInvoices))]
        [Guide("The `SalesInvoices` tab is used for creating and handling invoices that are sent to customers for goods or services they have purchased.")]
        [Guide("If you're utilizing this tab, you'll also require the `Customers` tab, since every sales invoice must be issued to a customer.")]
        [ProtoMember(3), TabSwitch(true), IfTrue(nameof(Customers))] public bool SalesInvoices { get; set; }

        [TabScreenshot("fa-cut", nameof(Strings.CreditNotes))]
        [Guide("The `CreditNotes` tab is designed for issuing credits to customers, commonly used for returns or to correct mistakes.")]
        [Guide("When using this tab, it's necessary to also have the `Customers` tab activated, as each credit note must be associated with a customer.")]
        [ProtoMember(5), TabSwitch(false), IfTrue(nameof(SalesInvoices))] public bool CreditNotes { get; set; }

        [TabScreenshot("fa-bell", nameof(Strings.LatePaymentFees))]
        [Guide("The `LatePaymentFees` tab is designed for the management and application of extra charges on payments from customers that are overdue.")]
        [Guide("To use this tab effectively, you also need the `Customers` tab, since each late payment fee must be associated with a customer.")]
        [ProtoMember(38), TabSwitch(false), IfTrue(nameof(SalesInvoices))] public bool LatePaymentFees { get; set; }

        [TabScreenshot("fa-stopwatch", nameof(Strings.BillableTime))]
        [Guide("The `BillableTime` tab is used to log hours worked on projects for customers which will be invoiced.")]
        [Guide("To utilize this tab effectively, you must also use the `Customers` and `SalesInvoices` tabs. This is because all billable time must be associated with a customer and ultimately billed using a sales invoice.")]
        [ProtoMember(13), TabSwitch(false), IfTrue(nameof(SalesInvoices))] public bool BillableTime { get; set; }

        [TabScreenshot("fa-file-certificate", nameof(Strings.WithholdingTaxReceipts))]
        [Guide("The `WithholdingTaxReceipts` tab is designed for organizing receipts that document the withholding tax taken out of payments or invoices.")]
        [Guide("To use this tab effectively, it's necessary to utilize the `Customers` and `SalesInvoices` tabs too. This is because the obligation for withholding tax is noted on the sales invoice, and each withholding tax receipt must be associated with a specific customer.")]
        [ProtoMember(46), TabSwitch(false), IfTrue(nameof(SalesInvoices))] public bool WithholdingTaxReceipts { get; set; }

        [TabScreenshot("fa-truck", nameof(Strings.DeliveryNotes))]
        [Guide("The `DeliveryNotes` tab is used to monitor the delivery of goods to customers, ensuring that orders are fulfilled.")]
        [ProtoMember(10), TabSwitch(false), IfTrue(nameof(Customers))] public bool DeliveryNotes { get; set; }

        [TabScreenshot("fa-city", nameof(Strings.Suppliers))]
        [Guide("The `Suppliers` tab is designated for managing supplier information, which is crucial for handling purchases and overseeing supply chain activities.")]
        [ProtoMember(15), TabSwitch(false)] public bool Suppliers { get; set; }

        [TabScreenshot("fa-drafting-compass", nameof(Strings.PurchaseQuotes))]
        [Guide("The `PurchaseQuotes` tab is designed for the creation and management of price quotations received from suppliers.")]
        [ProtoMember(39), TabSwitch(false), IfTrue(nameof(Suppliers))] public bool PurchaseQuotes { get; set; }

        [TabScreenshot("fa-shopping-cart", nameof(Strings.PurchaseOrders))]
        [Guide("The `PurchaseOrders` tab is used for creating and monitoring orders that have been made with suppliers for either goods or services.")]
        [ProtoMember(7), TabSwitch(false), IfTrue(nameof(Suppliers))] public bool PurchaseOrders { get; set; }

        [TabScreenshot("fa-file-invoice", nameof(Strings.PurchaseInvoices))]
        [Guide("The `PurchaseInvoices` tab is designed for keeping track of and managing invoices that have been received from suppliers.")]
        [ProtoMember(6), TabSwitch(false), IfTrue(nameof(Suppliers))] public bool PurchaseInvoices { get; set; }

        [TabScreenshot("fa-cut", nameof(Strings.DebitNotes))]
        [Guide("The `DebitNotes` tab is used to issue debit adjustments to suppliers, typically for returns or errors.")]
        [ProtoMember(20), TabSwitch(false), IfTrue(nameof(PurchaseInvoices))] public bool DebitNotes { get; set; }

        [TabScreenshot("fa-truck-loading", nameof(Strings.GoodsReceipts))]
        [Guide("The `GoodsReceipts` tab is used to document the arrival of goods from suppliers, facilitating inventory management.")]
        [ProtoMember(32), TabSwitch(false), IfTrue(nameof(Suppliers))] public bool GoodsReceipts { get; set; }

        [TabScreenshot("fa-chart-bar", nameof(Strings.Projects))]
        [Guide("The `Projects` tab allows for the management and tracking of different business projects, including their costs and revenues.")]
        [ProtoMember(47), TabSwitch(false)] public bool Projects { get; set; }

        [TabScreenshot("fa-inventory", nameof(Strings.InventoryItems))]
        [Guide("The `InventoryItems` tab is designed for managing stock items, including keeping track of their quantities and values.")]
        [ProtoMember(11), TabSwitch(false)] public bool InventoryItems { get; set; }

        [TabScreenshot("fa-person-dolly", nameof(Strings.InventoryTransfers))]
        [Guide("The `InventoryTransfers` tab is designed to document the transfer of inventory items between various locations or warehouses.")]
        [Guide("If you're utilizing this tab, you'll also require `InventoryItems` since each inventory transfer must be associated with one or more inventory items.")]
        [ProtoMember(33), TabSwitch(false), IfTrue(nameof(InventoryItems))] public bool InventoryTransfers { get; set; }

        [TabScreenshot("fa-eraser", nameof(Strings.InventoryWriteOffs))]
        [Guide("The `InventoryWriteOffs` tab is used to record inventory items that have been lost, stolen, or are unsellable, indicating their removal from inventory.")]
        [Guide("If you're utilizing this tab, you must also have `InventoryItems`, since each inventory write-off must be associated with one or more inventory items.")]
        [ProtoMember(21), TabSwitch(false), IfTrue(nameof(InventoryItems))] public bool InventoryWriteOffs { get; set; }

        [TabScreenshot("fa-conveyor-belt", nameof(Strings.ProductionOrders))]
        [Guide("The `ProductionOrders` tab is designed for overseeing the production process, starting with raw materials and culminating in finished goods.")]
        [Guide("When utilizing this tab, it's essential to also use `InventoryItems`. This is because each production order must be associated with one or more inventory items.")]
        [ProtoMember(22), TabSwitch(false), IfTrue(nameof(InventoryItems))] public bool ProductionOrders { get; set; }

        [TabScreenshot("fa-id-card", nameof(Strings.Employees))]
        [Guide("The `Employees` tab is designed for organizing information related to employees, such as their contact information and job roles.")]
        [ProtoMember(18), TabSwitch(true)] public bool Employees { get; set; }

        [TabScreenshot("fa-money-check-edit", nameof(Strings.Payslips))]
        [Guide("The `Payslips` tab is designed for creating and handling payslips for employees, detailing their salaries and deductions.")]
        [Guide("To use this tab effectively, it's necessary to also use the `Employees` tab since each payslip must be associated with an employee.")]
        [ProtoMember(19), TabSwitch(true), IfTrue(nameof(Employees))] public bool Payslips { get; set; }

        [TabScreenshot("fa-chart-pie", nameof(Strings.Investments))]
        [Guide("The `Investments` tab is designed for monitoring the performance and tracking of business investments.")]
        [ProtoMember(48), TabSwitch(false)] public bool Investments { get; set; }

        [TabScreenshot("fa-car-building", nameof(Strings.FixedAssets))]
        [Guide("The `FixedAssets` tab is designed for handling tangible, long-term assets that are utilized in operations, along with their depreciation.")]
        [ProtoMember(12), TabSwitch(false)] public bool FixedAssets { get; set; }

        [TabScreenshot("fa-sort-size-down", nameof(Strings.DepreciationEntries))]
        [Guide("The `DepreciationEntries` tab is used to record the depreciation expenses of fixed assets over a period.")]
        [Guide("If you're utilizing this tab, you'll also need `FixedAssets` since each depreciation entry must be connected to one or more fixed assets.")]
        [ProtoMember(41), TabSwitch(false), IfTrue(nameof(FixedAssets))] public bool DepreciationEntries { get; set; }

        [TabScreenshot("fa-wind", nameof(Strings.IntangibleAssets))]
        [Guide("The `IntangibleAssets` tab is designed to manage assets that do not have a physical form, such as patents or copyrights, including the process of their amortization.")]
        [ProtoMember(27), TabSwitch(false)] public bool IntangibleAssets { get; set; }

        [TabScreenshot("fa-sort-amount-down", nameof(Strings.AmortizationEntries))]
        [Guide("The `AmortizationEntries` tab is designed for documenting the gradual expense recognition of intangible assets.")]
        [Guide("If you utilize this tab, it's essential to use `IntangibleAssets` too, since every amortization entry must be connected to one or several intangible assets.")]
        [ProtoMember(42), TabSwitch(false), IfTrue(nameof(IntangibleAssets))] public bool AmortizationEntries { get; set; }

        [TabScreenshot("fa-user-chart", nameof(Strings.CapitalAccounts))]
        [Guide("The `CapitalAccounts` tab is designed to monitor the investments, withdrawals, and current balances of business owners or partners individually.")]
        [ProtoMember(26), TabSwitch(false)] public bool CapitalAccounts { get; set; }

        [TabScreenshot("fa-cubes", nameof(Strings.SpecialAccounts))]
        [Guide("The `SpecialAccounts` tab is designed to manage unique or specialized financial accounts that are not included under other tabs.")]
        [ProtoMember(30), TabSwitch(false)] public bool SpecialAccounts { get; set; }

        [TabScreenshot("fa-folders", nameof(Strings.Folders))]
        [Guide("The `Folders` tab allows you to categorize documents and transactions into specific groups, making them easy to access and manage.")]
        [ProtoMember(31), TabSwitch(false)] public bool Folders { get; set; }

        [ProtoMember(49)] public bool Obsolete_InvestmentRevaluations { get; set; }
        [ProtoMember(50)] public bool Obsolete_CurrencyRevaluations { get; set; }
        [ProtoMember(23)] public bool Obsolete_BillableExpenses { get; set; }
        [ProtoMember(16)] public bool Obsolete_Emails { get; set; }
        [ProtoMember(34)] public bool Obsolete_CashAccounts { get; set; }
        [ProtoMember(28)] public bool Obsolete_TaxDeductionsAtSource { get; set; }
        [ProtoMember(29)] public bool Obsolete_InventoryReturns { get; set; }
        [ProtoMember(1)] public bool Obsolete_BankAccounts { get; set; }
        [ProtoMember(37)] public bool Obsolete_CashTransactions { get; set; }
        [ProtoMember(40)] public bool Obsolete_Attachments { get; set; }
    }
}
