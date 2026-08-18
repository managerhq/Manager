using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete11
{
    [ProtoContract]
    [Guid("2518d618-03dd-474b-ac66-5192f9df2b66")]
    internal sealed class GeneralSettings11 : Object
    {
        [ProtoMember(1)]
        public bool BankAccounts;
        [ProtoMember(2)]
        public bool CashAccounts;
        [ProtoMember(3)]
        public bool SalesInvoices;
        [ProtoMember(4)]
        public bool SalesQuotes;
        [ProtoMember(5)]
        public bool CreditNotes;
        [ProtoMember(6)]
        public bool PurchaseInvoices;
        [ProtoMember(7)]
        public bool PurchaseOrders;
        [ProtoMember(8)]
        public bool SalesInvoiceItems;
        [ProtoMember(9)]
        public bool TaxCodes;
        [ProtoMember(10)]
        public bool GeneralLedgerSummary;
        [ProtoMember(13)]
        public bool CustomerStatements;
        [ProtoMember(14)]
        public bool AgedReceivables;
        [ProtoMember(15)]
        public bool AgedPayables;
        [ProtoMember(17)]
        public bool BusinessLogo;
        [ProtoMember(19)]
        public bool ExpenseClaims;
        [ProtoMember(20)]
        public bool TaxAudit;
        [ProtoMember(21)]
        public bool TaxSummary;
        [ProtoMember(22)]
        public bool DeliveryNotes;
        [ProtoMember(23)]
        public bool PurchaseInvoiceItems;
        [ProtoMember(24)]
        public bool CurrencyPrefixSuffix;
        [ProtoMember(25)]
        public bool WindowFacedEnvelope;
        [ProtoMember(26)]
        public bool GeneralLedgerTransactions;
        [ProtoMember(27)]
        public bool InventoryItems;
        [ProtoMember(28)]
        public bool FixedAssets;
        [ProtoMember(29)]
        public bool CapitalAccounts;
        [ProtoMember(30)]
        public bool Jobs;
    }
}
