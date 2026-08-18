using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Model.Enums;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.Suppliers
{
    [ProtoContract]
    [Title(nameof(Strings.Supplier), nameof(Strings.Edit))]
    [Guide("Suppliers are businesses or individuals from whom you purchase goods, services, or other items for your business operations.")]
    [Guide("Use this form to create a new supplier or edit existing supplier details. Maintaining accurate supplier information helps you track purchases, manage payables, and organize your business relationships.")]
    [Guide("Each supplier can have their own currency, payment terms, and contact information. You can also assign a unique code to each supplier for easy identification in transactions.")]
    [Guide("The information you enter here will appear on purchase invoices, purchase orders, and other documents related to this supplier.")]
    [Fields(typeof(ManagerServer.Model.Supplier))]
    internal sealed class SupplierForm : NakedVueForm<Supplier>
    {
        protected override bool CanHaveImage()
        {
            return true;
        }

        protected override void OnSource(Supplier form, ManagerServer.Model.Object source)
        {
            if (source is Customer customer)
            {
                form.Name = customer.Name;
                form.Address = customer.BillingAddress;
                form.Code = customer.Code;
                form.Currency = customer.Currency;
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<Supplier>(Business, customer.CustomFields);
                form.Email = customer.Email;
            }
        }
    }
}