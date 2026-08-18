using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagerServer.Model.Enums
{
    public enum BillableTimeStatus : int
    {
        Uninvoiced = 0,
        Invoiced = 1,
        WrittenOff = 2
    }
}
