using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ReportTransformationReports
{
    [ProtoContract]
    [Title(nameof(Strings.ReportTransformations))]
    internal sealed class ReportTransformationReportFigures : ObjectTable<ReportTransformationReportFigures.Record>
    {
        [InheritedProtoMember(300)] public DateTime From;
        [InheritedProtoMember(301)] public DateTime To;
        [InheritedProtoMember(302)] public Guid? Employee;
        [InheritedProtoMember(303)] public Guid? Supplier;
        [InheritedProtoMember(304)] public AccountingBasis AccountingMethod;
        [InheritedProtoMember(305)] public Guid[] ReportingCategories;
        [InheritedProtoMember(306)] public bool? ReverseSigns;
        [InheritedProtoMember(307)] public bool? Sales;
        [InheritedProtoMember(308)] public bool? Purchases;

        protected override bool IsInactive(Record row) => row.IsInactive;

        protected override Record[] GetObjects()
        {
            var database = ApplicationData.Businesses.Get(Business);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (AccountingMethod == ManagerServer.Model.Enums.AccountingBasis.CashBasis) transactions = transactions.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(From.AddDays(-1), To).AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(From.AddDays(-1), To);
            var periodTransactions = transactions.Where(x => x.Date >= From && x.Date <= To).ToArray();
            if (Employee.HasValue) periodTransactions = periodTransactions.Where(x => x.Employee?.Key == Employee.Value).ToArray();
            if (Supplier.HasValue) periodTransactions = periodTransactions.Where(x => x.Supplier?.Key == Supplier.Value).ToArray();
            if (Sales.HasValue) periodTransactions = periodTransactions.Where(x => x.IsSale).ToArray();
            if (Purchases.HasValue) periodTransactions = periodTransactions.Where(x => !x.IsSale).ToArray();
            var businessDetails = database.Single<ManagerServer.Model.BusinessDetails>();

            var rows = new List<Record>();
            foreach (var e in ReportingCategories)
            {
                var reportingCategory = ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country).SingleOrDefault(x => x.Key == e) as ManagerServer.Model.NamedObject ?? database.SingleOrDefault<ManagerServer.Model.NamedObject>(e);
                if (reportingCategory == null) continue;

                var reportingCategoryTransactions = periodTransactions.Where(x => x.ReportingCategory == e).ToArray();
                var amount = reportingCategoryTransactions.Sum(x => x.BaseAmount);

                var reportingCategoryTransactionsReversed = periodTransactions.Where(x => x.ReportingCategoryReversed == e).ToArray();
                amount += reportingCategoryTransactionsReversed.Sum(x => x.BaseAmount) * -1m;

                var httpHandler = new ReportTransformationTransactions();
                httpHandler.Business = Business;
                httpHandler.From = From;
                httpHandler.To = To;
                httpHandler.Employee = Employee;
                httpHandler.Supplier = Supplier;
                httpHandler.AccountingBasis = AccountingMethod;
                httpHandler.ReportingCategory = e;
                if (Sales.HasValue) httpHandler.Sales = true;
                if (Purchases.HasValue) httpHandler.Purchases = true;

                if (ReverseSigns == true)
                {
                    amount *= -1m;
                    httpHandler.ReverseSigns = true;
                }

                if (ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.TaxCodeReversedReportingCategory>(e) != null)
                {
                    httpHandler.ReverseSigns = !(httpHandler.ReverseSigns ?? false);
                }

                rows.Add(new Record()
                {
                    IsInactive = !reportingCategoryTransactions.Any(),
                    Name = reportingCategory.GetName(),
                    Amount = amount,
                    HttpHandler = httpHandler
                });
            }
            return rows.OrderBy(x => x.IsInactive).ToArray();
        }

        [Guid("7dd1f200-174d-430a-94c7-8b9a9529745c")]
        public string GetName(Record o) => o.Name;

        [Right, WhitespaceNoWrap, Sum, Bold]
        [Guid("74ae17fb-4bb1-4ed6-b81e-3dcb0ef7d82a")]
        public Tuple<decimal, string, string> GetAmount(Record o) => new Tuple<decimal, string, string>(o.Amount, o.Amount.ToNumberString(), o.HttpHandler.ToUrl());

        public record Record
        {
            public string Name;
            public decimal Amount;
            public bool IsInactive;
            public ReportTransformationTransactions HttpHandler;
        }
    }
}