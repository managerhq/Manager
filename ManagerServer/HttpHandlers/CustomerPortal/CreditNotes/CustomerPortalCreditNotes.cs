using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.CustomerPortal.CreditNotes
{
    [ProtoContract]
    class CustomerPortalCreditNotes : Table<CustomerPortalCreditNotes.CreditNote>
    {
        protected override string GetTitle()
        {
            return Strings.CreditNotes;
        }

        protected override IEnumerable<CreditNote> GetItems()
        {
            var database = ApplicationData.Businesses.Get(Business);

            var customerPortal = database.SingleOrDefault<ManagerServer.Model.CustomerPortal>(CustomerPortal);

            var creditNoteKey = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.CreditNote));

            return database.OfType<ManagerServer.Model.CreditNote>().Where(x => x.Key != creditNoteKey && x.Customer == customerPortal.Customer.Value).OrderByDescending(x => x.IssueDate).Select(x => new CreditNote()
            {
                View = new CustomerPortalCreditNote() { Business = Business, CustomerPortal = CustomerPortal, Key = x.Key },
                Date = x.IssueDate,
                Reference = x.Reference,
                Description = x.Description,
                Total = x.GetGeneralLedgerTransactions(database).Single(x => x.IsBalancing).AccountAmount*-1m
            });
        }

        public sealed class CreditNote : Item
        {
            public DateTime Date;
            public string Reference;
            [Long] public string Description;
            public decimal Total;
        }
    }
}
