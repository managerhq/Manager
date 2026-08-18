using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Enums
{
    public enum InventoryValuationMethod : int
    {
        FirstInFirstOut = 0,
        WeightedAverageCost = 1,
        Manual = 2
    }
}
