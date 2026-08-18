using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Enums
{
    public enum InventoryItemStartingBalanceType : int
    {
        QtyOnHand = 0,
        QtyToDeliver = 1,
        QtyToReceive = 2
    }
}
