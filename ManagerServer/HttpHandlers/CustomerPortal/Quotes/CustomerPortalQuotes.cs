using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.CustomerPortal.Quotes
{
    [ProtoContract]
    class CustomerPortalQuotes : Table<CustomerPortalQuotes.Quote>
    {
        protected override string GetTitle()
        {
            return Strings.Quotes;
        }

        protected override IEnumerable<Quote> GetItems()
        {
            var list = new List<Quote>();

            var database = ApplicationData.Businesses.Get(Business);

            var customerPortal = database.SingleOrDefault<ManagerServer.Model.CustomerPortal>(CustomerPortal);

            var salesOrders = new HashSet<Guid>(database.OfType<ManagerServer.Model.SalesOrder>().Where(x => x.Customer.HasValue && x.SalesQuote.HasValue).Select(x => x.SalesQuote.Value));
            var salesInvoices = new HashSet<Guid>(database.OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.Customer.HasValue && x.SalesQuote.HasValue).Select(x => x.SalesQuote.Value));

            var salesQuoteKey = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SalesQuote));

            foreach (var e in database.OfType<ManagerServer.Model.SalesQuote>().Where(x => x.Key != salesQuoteKey && x.Customer == customerPortal.Customer.Value).OrderByDescending(x => x.IssueDate))
            {
                var status = QuoteStatus.Active;

                if (e.Cancelled)
                {
                    status = QuoteStatus.Cancelled;
                }
                else if (salesOrders.Contains(e.Key) || salesInvoices.Contains(e.Key))
                {
                    status = QuoteStatus.Accepted;
                }
                else if (e.ExpiryDate.HasValue && e.IssueDate.AddDays(e.ExpiryDate.Value) < DateTime.Today)
                {
                    status = QuoteStatus.Expired;
                }

                list.Add(new Quote()
                {
                    View = new CustomerPortalQuote() { Business = Business, CustomerPortal = CustomerPortal, Key = e.Key },
                    Date = e.IssueDate,
                    Reference = e.Reference,
                    Description = e.Description,
                    Total = e.GetGeneralLedgerTransactions(database).Single(x => x.IsBalancing).AccountAmount,
                    Status = status
                });
            }

            return list;
        }

        public sealed class Quote : Item
        {
            public DateTime Date;
            public string Reference;
            [Long] public string Description;
            public decimal Total;
            public QuoteStatus Status;
        }

        public enum QuoteStatus
        {
            [Success] Active,
            [Danger] Expired,
            Cancelled,
            Accepted
        }
    }
}
