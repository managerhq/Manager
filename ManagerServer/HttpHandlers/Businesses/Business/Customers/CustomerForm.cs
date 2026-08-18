using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Customers
{
    [ProtoContract]
    [Title(nameof(Strings.Customer), nameof(Strings.Edit))]
    [Guide("Use this form to create new customers or edit existing customer information.")]
    [Guide("Customer records help you track sales, invoices, and outstanding balances for each client.")]
    [Header("Form Fields")]
    [Guide("Complete the following fields to set up your customer record:")]
    [Fields(typeof(Customer))]
    [Header("Custom Fields")]
    [Guide("Add custom fields to track additional customer information specific to your business needs.")]
    [Guide("Custom fields allow you to capture information not included in the standard customer form, such as customer type, preferred payment terms, or special requirements.")]
    [LinkGuide("Learn more about creating and managing custom fields:", typeof(Settings.CustomFields.CustomFields))]
    [Header("Customers Who Are Also Suppliers")]
    [Guide("If a business entity is both a customer and a supplier, create separate entries in both the `Customers` and `Suppliers` tabs.")]
    [Guide("This separation ensures accurate tracking of receivables and payables, even when dealing with the same entity.")]
    [Guide("To offset balances between customer and supplier accounts for the same entity:")]
    [Guide("• Option 1: Create a `Credit Note` to reduce the customer balance and a `Debit Note` to reduce the supplier balance")]
    [Guide("• Option 2: Use a `Journal Entry` to transfer amounts between the `Accounts Receivable` and `Accounts Payable` control accounts")]
    internal sealed class CustomerForm : NakedVueForm<Customer>
    {
        protected override bool CanHaveImage()
        {
            return true;
        }

        protected override void OnSource(Customer form, ManagerServer.Model.Object source)
        {
            if (source is Supplier supplier)
            {
                form.Name = supplier.Name;
                form.BillingAddress = supplier.Address;
                form.Code = supplier.Code;
                form.Currency = supplier.Currency;
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<Customer>(Business, supplier.CustomFields);
                form.Email = supplier.Email;
            }
        }
    }
}
