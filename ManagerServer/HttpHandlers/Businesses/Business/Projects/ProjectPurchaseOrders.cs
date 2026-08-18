using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using Newtonsoft.Json;

namespace ManagerServer.HttpHandlers.Businesses.Business.Projects
{
    [ProtoContract]
    [Guid("79e0b2fc-dfd6-4cbb-b1ec-06d0dc4dd32b")]
    [Title(nameof(Strings.Projects), nameof(Strings.PurchaseOrders))]
    [Guide("The **Project Purchase Orders** screen displays all purchase orders associated with a specific project.")]
    [Guide("This screen helps you monitor the financial status of purchases for your project by tracking three key amounts: what has been ordered, what has been invoiced, and what remains to be invoiced.")]
    [Header("Overview")]
    [Guide("Each row represents a purchase order that contains items or services allocated to this project.")]
    [Guide("The amounts shown reflect only the portion of each purchase order that relates to this specific project, not the total purchase order value.")]
    [Header("Column Information")]
    [Guide("The table displays essential information about each purchase order including the date, purchase order number, supplier details, and financial amounts.")]
    [Guide("The **Uninvoiced** column highlights amounts that have been ordered but not yet invoiced, helping you track outstanding commitments.")]
    [Columns]
    internal sealed class ProjectPurchaseOrders : NakedObjectsWithCustomFields<Tuple<ManagerServer.Model.PurchaseOrder, decimal, decimal>>
    {
        [ProtoMember(1), JsonProperty("project")] public Guid Project;

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);

            var output = new List<Tuple<PurchaseOrder, decimal, decimal>>();

            var purchaseInvoices = database.OfType<ManagerServer.Model.PurchaseInvoice>()
                .Where(x => x.PurchaseOrder.HasValue)
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.Project?.Key == Project)
                .GroupBy(x => x.PurchaseInvoiceAsTransaction.PurchaseOrder)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));

            foreach (var e in database.OfType<ManagerServer.Model.PurchaseOrder>())
            {
                if (e.Cancelled) continue;
                var amountOnPurchaseOrder = e.GetGeneralLedgerTransactions(database).Where(x => x.Project?.Key == Project).Sum(x => x.BaseAmount);
                if (amountOnPurchaseOrder == 0m) continue;

                //var amountOnSalesInvoices = 0m;
                purchaseInvoices.TryGetValue(e.Key, out var amountOnSalesInvoices);

                output.Add(new Tuple<PurchaseOrder, decimal, decimal>(e, amountOnPurchaseOrder, amountOnSalesInvoices));
            }            

            context.Set<Array>(output.ToArray());

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(Tuple<PurchaseOrder, decimal, decimal>[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PurchaseOrders.PurchaseOrderForm() { Business = Business, Key = x.Item1.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(Tuple<PurchaseOrder, decimal, decimal>[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PurchaseOrders.PurchaseOrderView() { Business = Business, Key = x.Item1.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guide("The date when the purchase order was created.")]
        public DateTime[] GetDate(Tuple<ManagerServer.Model.PurchaseOrder, decimal, decimal>[] rows)
        {
            return rows.Select(x => x.Item1.Date).ToArray();
        }

        [PaddedSorting]
        [Default]
        [Guide("The *purchase order number*, which may include a reference if one was entered.")]
        [Guide("Click the purchase order number to view or edit the full purchase order details.")]
        public string[] GetPurchaseOrder(Tuple<ManagerServer.Model.PurchaseOrder, decimal, decimal>[] rows)
        {
            return rows.Select(x => x.Item1.GetTransactionName()).ToArray();
        }

        [Default]
        [Guide("The *supplier* associated with this purchase order.")]
        [Guide("If a *supplier code* has been assigned, it will be displayed alongside the supplier name.")]
        public string[] GetSupplier(Tuple<ManagerServer.Model.PurchaseOrder, decimal, decimal>[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Supplier>(x.Item1.Supplier)?.NameWithCode).ToArray();
        }

        [Default]
        [Guide("The *project* to which these purchase order amounts are allocated.")]
        [Guide("This column shows the same project for all rows since you are viewing purchase orders for a specific project.")]
        public string[] GetProject(Tuple<ManagerServer.Model.PurchaseOrder, decimal, decimal>[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Project>(Project)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guide("The total value of items and services on this purchase order that are allocated to the project.")]
        [Guide("This represents the committed purchase amount for the project, regardless of whether invoices have been received.")]
        public decimal[] GetOrderAmount(Tuple<ManagerServer.Model.PurchaseOrder, decimal, decimal>[] rows)
        {
            return rows.Select(x => x.Item2).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guide("The amount that has already been invoiced against this purchase order for the project.")]
        [Guide("This represents *purchase invoices* that have been entered referencing this purchase order and allocating costs to the project.")]
        public Tuple<decimal, BusinessTemplate>[] GetInvoiceAmount(Tuple<ManagerServer.Model.PurchaseOrder, decimal, decimal>[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(x.Item3, this)).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guide("The remaining amount on the purchase order that has not yet been invoiced.")]
        [Guide("This is calculated as the *order amount* minus the *invoiced amount*, helping you track outstanding purchase commitments.")]
        [Guide("A zero value indicates the purchase order has been fully invoiced for this project.")]
        public decimal[] GetUninvoiced(Tuple<ManagerServer.Model.PurchaseOrder, decimal, decimal>[] rows)
        {
            return rows.Select(x => x.Item2 - x.Item3).Select(x => x < 0m ? 0m : x).ToArray();
        }
    }
}
