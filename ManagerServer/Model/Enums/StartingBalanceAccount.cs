using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Enums
{
    public enum StartingBalanceAccount : int
    {
        BankOrCashAccount = 0,
        Customer = 1,
        Supplier = 2,
        Employee = 3,
        InventoryItem = 4,
        Investment = 5,
        FixedAsset = 6,
        IntangibleAsset = 7,
        CapitalAccount = 8,
        SpecialAccount = 9,
        Other = 10
    }
}
