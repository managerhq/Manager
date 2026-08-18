using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ReportTransformationReports
{
    [ProtoContract]
    [Title(nameof(Strings.Transactions))]
    internal sealed class ReportTransformationTransactions : TransactionViewer
    {
        [InheritedProtoMember(400)] public DateTime From;
        [InheritedProtoMember(401)] public DateTime To;
        [InheritedProtoMember(402)] public AccountingBasis AccountingBasis;
        [InheritedProtoMember(403)] public Guid? Employee;
        [InheritedProtoMember(404)] public Guid? Supplier;
        [InheritedProtoMember(405)] public Guid ReportingCategory;
        [InheritedProtoMember(406)] public bool? ReverseSigns;
        [InheritedProtoMember(407)] public bool? Sales;
        [InheritedProtoMember(408)] public bool? Purchases;

        protected override bool MultipleByOne()
        {
            return ReverseSigns == true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (AccountingBasis == AccountingBasis.CashBasis) generalLedger = generalLedger.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(From.AddDays(-1), To).AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(From.AddDays(-1), To);

            var transactions = generalLedger.Where(x => x.Date >= From && x.Date <= To).ToArray();
            if (Employee.HasValue) transactions = transactions.Where(x => x.Employee?.Key == Employee.Value).ToArray();
            if (Supplier.HasValue) transactions = transactions.Where(x => x.Supplier?.Key == Supplier.Value).ToArray();
            if (Sales.HasValue) transactions = transactions.Where(x => x.TaxCode == null || x.IsSale).ToArray();
            if (Purchases.HasValue) transactions = transactions.Where(x => x.TaxCode == null || !x.IsSale).ToArray();
            transactions = transactions.Where(x => x.ReportingCategory == ReportingCategory || x.ReportingCategoryReversed == ReportingCategory).ToArray();

            return transactions;
        }
    }
}
