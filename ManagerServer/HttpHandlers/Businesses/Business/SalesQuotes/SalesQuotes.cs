using System.Linq;
using System.Collections.Generic;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesQuotes
{
    [ProtoContract]    
    [NamespaceEntry]
    [Guid("49a7ab79-e9b0-4c9b-9374-c08d0e0bec46")]
    [Title(nameof(Strings.SalesQuotes))]
    [Guide("The `Sales Quotes` tab serves as a central area for creating, editing, and monitoring sales quotations provided to customers or prospects. This feature helps businesses efficiently generate professional-looking quotes, detailing prices, products, or services before finalizing a sale. With this tool, users can manage follow-ups on quotes and convert them into sales orders or sales invoices when needed.")]
    [TabScreenshot("fa-drafting-compass", nameof(Strings.SalesQuotes))]
    [Guide("To create a new sales quote, click the `New Sales Quote` button.")]
    [HeroButtonScreenshot(nameof(Strings.SalesQuotes), nameof(Strings.NewSalesQuote))]
    [Guide("The `Sales Quotes` tab displays several columns:")]
    [Columns]
    internal sealed class SalesQuotes : NakedObjectsWithAutomaticRows<SalesQuote>
    {
        [ProtoMember(1)] public Guid? Customer;

        protected override SalesQuote[] OnGetRows(SalesQuote[] rows)
        {
            if (Customer.HasValue) rows = rows.Where(x => x.Customer == Customer).ToArray();
            return rows;
        }

        [Default]
        [WarnIfFutureDate]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("323486cb-4954-4e93-b38f-6c67518c790d")]
        [Guide("Date when the sales quote was issued")]
        public DateTime[] GetIssueDate(SalesQuote[] rows)
        {
            return rows.Select(x => x.IssueDate).ToArray();
        }

        [Guid("637be51e-623f-4949-8dde-1f9eff848c7a")]
        [Guide("Date when the sales quote expires, if an expiry date has been set")]
        public DateTime?[] GetExpiryDate(SalesQuote[] rows)
        {
            return rows.Select(x => x.GetExpiryDate()).ToArray();
        }

        [Default]
        [PaddedSorting]
        [Guid("0ce48d62-0d8e-4839-a166-ff58bdfccbcc")]
        [Guide("Reference number of the sales quote")]
        public string[] GetReference(SalesQuote[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("f59d960b-62d8-4458-93bc-251b2ec53d2e")]
        [Guide("Customer who received the sales quote")]
        public string[] GetCustomer(SalesQuote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("86462ef9-1771-48bc-9ec5-cc2532619328")]
        [Guide("Description of the sales quote")]
        public string[] GetDescription(SalesQuote[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Sum, Right]
        [Guid("21420304-d0d6-493b-8ed3-b17926386ec9")]
        [Guide("Total amount of the sales quote")]
        public Tuple<decimal, Currency>[] GetAmount(SalesQuote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }

        [Default]
        [Center, MinWidth]
        [Guid("3dc13f33-17ef-4b95-bb6b-07da3a405574")]
        [Guide("The status of a sales quote can be `Active`, `Accepted`, `Cancelled`, or `Expired`. The status automatically changes to `Accepted` when the sales quote is linked to at least one `Sales Order` or `Sales Invoice`.")]
        public Status[] GetStatus(SalesQuote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var salesOrders = new HashSet<Guid>(database.OfType<SalesOrder>().Where(x => x.Customer.HasValue && x.SalesQuote.HasValue).Select(x => x.SalesQuote.Value));
            var salesInvoices = new HashSet<Guid>(database.OfType<SalesInvoice>().Where(x => x.Customer.HasValue && x.SalesQuote.HasValue).Select(x => x.SalesQuote.Value));
            var output = new List<Status>();
            foreach (var e in rows)
            {
                var status = Status.Active;
                if (e.Cancelled)
                {
                    status = Status.Cancelled;
                }
                else if (salesOrders.Contains(e.Key) || salesInvoices.Contains(e.Key))
                {
                    status = Status.Accepted;
                }
                else
                {
                    var expiryDate = e.GetExpiryDate();
                    if (expiryDate < DateTime.Today)
                    {
                        status = Status.Expired;
                    }
                }
                output.Add(status);
            }
            return output.ToArray();
        }

        public enum Status
        {
            [Success] Active,
            [Primary] Accepted,
            Cancelled,
            [Danger] Expired
        }

        protected override void OnFooterEndSection(Context context)
        {
            if (!Customer.HasValue)
            {
                using (A(href: new SalesQuoteLines() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.SalesQuotes + " - " + Strings.Lines);
            }
            base.OnFooterEndSection(context);
        }        
    }
}
