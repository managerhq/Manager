using System;
using ManagerServer.Api.Businesses.Business.Customers;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.Customers
{
    [ProtoContract]
    [Title(nameof(Strings.Customer), nameof(Strings.View))]
    [Guide("This screen displays complete details for an individual customer record, including contact information, billing address, and transaction history.")]
    [Header("Available Actions")]
    [Guide("Use the `Edit` button to modify customer information such as name, address, email, or phone number.")]
    [Guide("Click `View` to see all transactions associated with this customer, including invoices, receipts, and credit notes.")]
    [Guide("The `Attach` button allows you to upload and store documents related to this customer, such as contracts or correspondence.")]
    [Header("Quick Conversion")]
    [Guide("The `Copy to` button provides a convenient way to create a supplier record from this customer's information. This is useful when a customer also supplies goods or services to your business.")]
    [LinkGuide("To learn more about creating and managing customers, see:", typeof(CustomerForm))]
    internal sealed class CustomerView : DefaultView<GetCustomerView>
    {
        protected override Type[] GetCopyToOptions() => [typeof(ManagerServer.Model.Supplier)];
    }
}
