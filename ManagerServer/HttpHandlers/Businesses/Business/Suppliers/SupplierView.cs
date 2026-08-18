using System;
using ManagerServer.Api.Businesses.Business.Suppliers;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Suppliers
{
    [ProtoContract]
    [Title(nameof(Strings.Supplier))]
    [Guide("The `Supplier` view displays comprehensive information about a specific supplier in your system.")]
    [Guide("This view provides a centralized location to manage all aspects of your relationship with a supplier, including their contact information, transaction history, and related documents.")]
    [Header("Available Actions")]
    [Guide("From this view, you can perform several key actions:")]
    [Guide("- Click the `Edit` button to modify supplier details such as name, address, contact information, and payment terms")]
    [Guide("- View all transactions associated with this supplier, including purchase invoices, payments, and credit notes")]
    [Guide("- Attach and manage documents related to this supplier, such as contracts, certificates, or correspondence")]
    [Guide("- Use the `Copy to` function to quickly create a customer record based on this supplier's information")]
    [LinkGuide("For more information about creating and editing suppliers, see:", typeof(SupplierForm))]
    internal sealed class SupplierView : DefaultView<GetSupplierView>
    {
        protected override Type[] GetCopyToOptions() => [typeof(ManagerServer.Model.Customer)];
    }
}
