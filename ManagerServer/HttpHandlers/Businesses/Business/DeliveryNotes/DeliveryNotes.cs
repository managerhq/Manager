using System;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.DeliveryNotes
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("7c1b0a44-de00-4ef7-88b2-97bd73688f93")]
    [Title(nameof(Strings.DeliveryNotes))]
    [Guide("The **Delivery Notes** tab helps you track items delivered to customers. You can create, edit, and manage all your delivery notes in one place, ensuring accurate records of items sent for each order.")]
    [TabScreenshot("fa-truck", nameof(Strings.DeliveryNotes))]
    [Guide("To create a new delivery note, click the **New Delivery Note** button.")]
    [HeroButtonScreenshot(nameof(Strings.DeliveryNotes), nameof(Strings.NewDeliveryNote))]
    [Guide("The **Delivery Notes** tab contains the following columns:")]
    [Columns]
    internal sealed class DeliveryNotes : NakedObjectsWithAutomaticRows<DeliveryNote>
    {
        [ProtoMember(1)] public Guid? Customer;

        protected override DeliveryNote[] OnGetRows(DeliveryNote[] rows)
        {
            if (Customer.HasValue) rows = rows.Where(x => x.Customer == Customer).ToArray();
            return rows;
        }

        [Default]
        [WarnIfFutureDate, Center, MinWidth]
        [WhitespaceNoWrap]
        [Guid("b7b42f7b-04db-4338-928f-580fedb808c1")]
        [Guide("The **Date** column shows when items were delivered to the customer.")]
        [Guide("This date records the actual delivery, not when the delivery note was created.")]
        [Guide("Use accurate delivery dates for *inventory tracking* and customer service records.")]
        public DateTime[] GetDeliveryDate(DeliveryNote[] rows)
        {
            return rows.Select(x => x.DeliveryDate).ToArray();
        }

        [PaddedSorting]
        [Guid("9a31f16c-db75-427a-9c4d-dcec6dda1830")]
        [Guide("The **Reference** column displays the unique identifier for each delivery note.")]
        [Guide("Reference numbers help track shipments and match delivery notes to customer inquiries.")]
        [Guide("You can use automatic numbering or enter custom references like tracking numbers.")]
        public string[] GetReference(DeliveryNote[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Guid("ede596d9-c769-4583-8f78-ded198ce2678")]
        [Guide("The **Order Number** column shows which *sales order* this delivery fulfills.")]
        [Guide("Link delivery notes to sales orders to track partial shipments and order completion.")]
        [Guide("This connection ensures accurate order fulfillment and *inventory allocation*.")]
        public string[] GetOrderNumber(DeliveryNote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<SalesOrder>(x.SalesOrder)?.Reference).ToArray();
        }

        [Guid("1f931c36-c861-440c-9cc5-fa2495991c12")]
        [Guide("The **Invoice Number** column displays the *sales invoice* associated with this delivery.")]
        [Guide("Linking deliveries to invoices helps verify that customers are billed for shipped items.")]
        [Guide("This ensures proper revenue recognition and prevents billing errors.")]
        public string[] GetInvoiceNumber(DeliveryNote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<SalesInvoice>(x.SalesInvoice)?.Reference).ToArray();
        }

        [Default]
        [Guid("d86b2df9-3357-42b5-967b-1deae3b966ec")]
        [Guide("The **Customer** column identifies who received the delivered items.")]
        [Guide("Customer information includes their code and name for easy identification.")]
        [Guide("This helps track delivery history and resolve shipping inquiries.")]
        public string[] GetCustomer(DeliveryNote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.GetCodeAndName()).ToArray();
        }

        [Guid("3d4dea32-16c4-44aa-890d-e24f46e73036")]
        [Guide("The **Inventory Location** column shows which warehouse or location shipped the items.")]
        [Guide("Multiple locations help track inventory movement between warehouses and stores.")]
        [Guide("This information is crucial for *inventory control* and logistics management.")]
        public string[] GetInventoryLocation(DeliveryNote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<CustomInventoryLocation>(x.InventoryLocation)?.Name).ToArray();
        }

        [Default]
        [Guid("1b623b49-a699-4a92-a53e-b3002394ae68")]
        [Guide("The **Description** column provides additional details about the delivery.")]
        [Guide("Include shipping instructions, special handling notes, or delivery conditions.")]
        [Guide("Descriptions help staff and customers understand the context of each delivery.")]
        public string[] GetDescription(DeliveryNote[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Right]
        [Guid("c877ea77-4559-471b-8eb3-f65e0732b852")]
        [Guide("The **Qty Delivered** column shows the total quantity of items delivered.")]
        [Guide("This represents the sum of all *line items* on the delivery note.")]
        [Guide("Use this to quickly assess delivery volumes and verify shipment completeness.")]
        public decimal[] GetQtyDelivered(DeliveryNote[] rows)
        {
            return rows.Select(x => x.Lines?.Sum(x => x.Qty ?? 0m) ?? 0m).ToArray();
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: new DeliveryNoteLines() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.DeliveryNotes + " - " + Strings.Lines);
            base.OnFooterEndSection(context);
        }
    }
}
