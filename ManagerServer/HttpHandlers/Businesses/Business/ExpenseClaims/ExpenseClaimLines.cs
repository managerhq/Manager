using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.ExpenseClaims
{
    [ProtoContract]
    [Guid("5B8B5D51-8284-4821-9448-D84AC1BA1349")]
    [Title(nameof(Strings.ExpenseClaim), nameof(Strings.Lines))]
    [Guide("The **Expense Claim - Lines** report displays all individual line items from expense claims in a consolidated view.")]
    [Guide("This report helps you analyze expenses across all expense claims, regardless of their status or the person who submitted them.")]
    [Guide("Each row represents a single line item from an expense claim, showing detailed information about that specific expense.")]
    [Guide("Use this report to review expense patterns, verify account allocations, or export data for further analysis.")]
    [Columns]
    internal sealed class ExpenseClaimLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.ExpenseClaim.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<ExpenseClaim>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null && !x.IsCostOfGoodsSold).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new ExpenseClaimForm() { Business = Business, Key = x.ExpenseClaim.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new ExpenseClaimView() { Business = Business, Key = x.ExpenseClaim.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, Center, MinWidth]
        [WhitespaceNoWrap]
        [Guid("E8A0F216-87CE-4D7E-AB56-DE4FEC3B14C0")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("48d9f695-78da-4056-9c06-2e0c1abb6fe6")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.ExpenseClaim.Reference).ToArray();
        }

        [Default]
        [Guid("1DC31EAC-AC28-4377-BEBB-9C6F66FFBD7A")]
        public string[] GetPayer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Employee>(x.ExpenseClaim.PaidBy)?.Name ?? database.SingleOrDefault<ManagerServer.Model.CapitalAccount>(x.ExpenseClaim.PaidBy)?.Name ?? database.SingleOrDefault<ManagerServer.Model.ExpenseClaimsPayer>(x.ExpenseClaim.PaidBy)?.Name).ToArray();
        }

        [Guid("B5C90C54-299C-4C73-8D25-D5F87328D45B")]
        public string[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.Name).ToArray();
        }

        [Guid("91CDC1D9-1DB0-45DE-9E50-AC1FAC74D2ED")]
        public string[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.Name).ToArray();
        }

        [Guid("24F5A0F6-75C0-4501-8B8D-75349CF189D4")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.ExpenseClaim.Description).ToArray();
        }

        [Guid("62A59E09-AD70-4F52-B588-16E1B84B74B3")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Default]
        [Guid("82F9BAA9-6A6E-409D-8AF8-F2D2E9C6AE17")]
        public string[] GetAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Account).ToArray();
        }

        [Guid("49E6F039-0257-46B9-A17D-2F410A539255")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Guid("429DCFEF-C2C4-4B17-A59C-238E4406AD59")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value*-1m : default(decimal?)).ToArray();
        }

        [Guid("BCC91FCA-F5FF-4F84-B851-1E697B6831BE")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetUnitPrice(x.Transaction).HasValue ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Guid("A4136D13-1BDE-47F8-86C4-D87B295A3653")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("7E0E3EE0-90C4-474E-B310-C834EEB2D335")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("7E227E57-7A54-4935-A8C4-4E7DAFF2E909")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Guid("11FD8F73-1C2F-4F97-A348-C2416BC5A038")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("BAC58DC3-4B8E-466C-841E-F0933604CF9A")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => new Tuple<decimal, Currency>(x.TransactionAmount, x.TransactionCurrency)).ToArray();
        }
    }
}
