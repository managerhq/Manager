using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomerPortals
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Customers))]
    [Title(nameof(Strings.CustomerPortals))]
    [Guide("Customer portals provide a secure online interface where your customers can access their account information without contacting you directly.")]
    [Guide("Each portal is assigned to a specific customer and provides them with self-service access to view their invoices, statements, and account balances.")]
    [Header("Features")]
    [Guide("Through their portal, customers can view their *outstanding invoices*, download PDF copies of documents, and check their current *account balance*.")]
    [Guide("This reduces the administrative burden on your business by allowing customers to access information they need at any time.")]
    [Header("Setting Up Portals")]
    [Guide("To create a customer portal, click the **New Customer Portal** button and select the customer who should have access.")]
    [Guide("Each customer can have only one portal, and you can enable or disable access at any time.")]
    [NewButton(nameof(Strings.NewCustomerPortal))]
    [Columns]
    internal sealed class CustomerPortals : PersistentObjectTable<ManagerServer.Model.CustomerPortal>
    {
        [Guid("2ef399be-b400-48e0-b690-71372e888e71")]
        [Guide("The customer who has been granted access to this portal. Each portal is uniquely assigned to one customer for secure access to their account information.")]
        public Customer GetCustomer(ManagerServer.Model.CustomerPortal row) => ApplicationData.Businesses.Get(Business).SingleOrDefault<Customer>(row.Customer);
    }
}