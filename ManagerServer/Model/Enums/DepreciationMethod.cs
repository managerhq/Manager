using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagerServer.Model.Enums
{
    public enum DepreciationMethod : int
    {
        Manual = 0,
        StraightLine = 1,
        DecliningBalance = 2
    }
}
