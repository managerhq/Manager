using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomField
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByCustomField), nameof(Strings.Transactions))]
    [Guide("Shows sales invoice transactions grouped by custom field values.")]
    [Guide("Displays invoices and credit notes filtered by specific custom field criteria.")]
    internal sealed class SalesInvoiceTotalsByCustomFieldTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid CustomField;
        [ProtoMember(4)] public string Value;

        protected override bool ShowBaseAmount()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var salesInvoices = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => (x.Transaction is ManagerServer.Model.SalesInvoice || x.Transaction is ManagerServer.Model.CreditNote) && x.Date >= From && x.Date <= To).ToArray();

            var customField = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.CustomField>(CustomField);
            if (customField != null)
            {
                if (customField.Contains(typeof(ManagerServer.Model.SalesInvoice)))
                {
                    return salesInvoices.Where(x => x.IsBalancing && x.SalesInvoice?.CustomFields != null && x.SalesInvoice.CustomFields.ContainsKey(customField.Key) && x.SalesInvoice.CustomFields[customField.Key] == Value).ToArray();
                }
                if (customField.Contains(typeof(ManagerServer.Model.SalesInvoice.Line)))
                {
                    return salesInvoices.Where(x => x.TransactionLine?.GetCustomFields() != null && x.TransactionLine.GetCustomFields().ContainsKey(customField.Key) && x.TransactionLine.GetCustomFields()[customField.Key] == Value).ToArray();
                }
                if (customField.Contains(typeof(ManagerServer.Model.Customer)))
                {
                    return salesInvoices.Where(x => x.IsBalancing && x.Customer?.CustomFields != null && x.Customer.CustomFields.ContainsKey(customField.Key) && x.Customer.CustomFields[customField.Key] == Value).ToArray();
                }
                if (customField.Contains(typeof(ManagerServer.Model.InventoryItem)))
                {
                    return salesInvoices.Where(x => x.InventoryItem?.CustomFields != null && x.InventoryItem.CustomFields.ContainsKey(customField.Key) && x.InventoryItem.CustomFields[customField.Key] == Value).ToArray();
                }
                if (customField.Contains(typeof(ManagerServer.Model.NonInventoryItem)))
                {
                    return salesInvoices.Where(x => x.NonInventoryItem?.CustomFields != null && x.NonInventoryItem.CustomFields.ContainsKey(customField.Key) && x.NonInventoryItem.CustomFields[customField.Key] == Value).ToArray();
                }
            }
            return null;
        }
    }
}
