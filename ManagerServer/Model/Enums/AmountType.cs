using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Enums
{
    public enum AmountType : int
    {
        AnyAmount = 0,
        Exactly = 1,
        MoreThan = 2,
        LessThan = 3
    }
}
