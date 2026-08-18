using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Enums
{
    public enum SalesInvoiceField : int
    {
        Reference = 0,
        IssueDate = 1,
        DueDate = 2,
        InvoiceTotal = 3,
        BalanceDue = 4,
        TaxAmount = 5,
        Customer = 6,
        CustomField = 7
    }
}
