using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Enums
{
    public enum ControlAccountType : int
    {
        BankAccounts = 0,
        Customers = 1,
        Suppliers = 2,
        InventoryItems = 3,
        FixedAssets = 4,
        IntangibleAssets = 5,
        SpecialAccounts = 6,
        Employees = 7,
        CapitalAccounts = 9,
        FixedAssetsAccumulatedDepreciation = 10,
        IntangibleAssetsAccumulatedAmortization = 11,
        Investments = 12
    }
}
