using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Enums
{
    public enum DeliveryNoteColumns : int
    {
        DeliveryDate = 0,
        Reference = 1,
        SalesInvoice = 2,
        InventoryLocation = 3,
        Customer = 4,
        Description = 5,
        CustomFields2 = 6,
        CustomFields = 7,
        OrderNumber = 8,
        InvoiceNumber = 9
    }
}