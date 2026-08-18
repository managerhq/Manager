using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Helpers;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.CustomerPortal.Invoices
{
    [ProtoContract]
    class CustomerPortalInvoices : Table<CustomerPortalInvoices.Invoice>
    {
        protected override string GetTitle()
        {
            return Strings.Invoices;
        }

        protected override IEnumerable<Invoice> GetItems()
        {
            var database = ApplicationData.Businesses.Get(Business);
            var customerPortal = database.SingleOrDefault<ManagerServer.Model.CustomerPortal>(CustomerPortal);

            var list = new List<Invoice>();
            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchSalesInvoices(new Guid[] { customerPortal.Customer.Value }).Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && x.SalesInvoice != null && x.Customer != null && x.Customer.Key == customerPortal.Customer.Value).GroupBy(x => x.SalesInvoice);
            foreach (var e in generalLedger.OrderByDescending(x => x.Key.IssueDate))
            {
                var invoiceTotal = e.Where(x => x.SalesInvoiceAsTransaction != null).Sum(x => x.AccountAmount);
                var balanceDue = e.Sum(x => x.AccountAmount);
                var status = InvoiceStatus.Unpaid;
                if (balanceDue == 0m) status = InvoiceStatus.Paid;
                if (balanceDue < 0m) status = InvoiceStatus.Overpaid;

                list.Add(new Invoice()
                {
                    View = new CustomerPortalInvoice() { Business = Business, CustomerPortal = CustomerPortal, Key = e.Key.Key },
                    InvoiceTotal = invoiceTotal,
                    Description = e.Key.Description,
                    IssueDate = e.Key.IssueDate,
                    BalanceDue = balanceDue,
                    Reference = e.Key.Reference,
                    Status = status
                });
            }
            return list;
        }

        public sealed class Invoice : Item
        {
            public DateTime IssueDate;
            [Center] public string Reference;
            [Long] public string Description;
            public decimal InvoiceTotal;
            public decimal BalanceDue;
            public InvoiceStatus Status;
        }

        public enum InvoiceStatus
        {
            [Success] Paid,
            [Danger] Unpaid,
            Overpaid
        }
    }
}
