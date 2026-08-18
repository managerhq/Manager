using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.TaxCodes
{
    [ProtoContract]
    [Title(nameof(Strings.TaxCode), nameof(Strings.Transactions))]
    [Guide("This screen displays all transactions that have been recorded using a specific *tax code*.")]
    [Guide("Use this report to review where and how a particular *tax code* has been applied throughout your accounting records.")]
    [Guide("The list includes transactions from various sources such as *sales invoices*, *purchase invoices*, *receipts*, *payments*, and *journal entries*.")]
    [Guide("This comprehensive view helps ensure your *tax code* usage is consistent and assists with tax compliance and reporting.")]
    [LinkGuide("For more information, see:", typeof(TaxCodeForm))]
    internal sealed class TaxCodeTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid TaxCode;

        protected override bool HideAmounts()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.TaxCode?.Key == TaxCode).GroupBy(x => x.Transaction).Select(x => x.First());
        }
    }
}
