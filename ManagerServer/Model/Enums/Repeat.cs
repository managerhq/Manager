using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Enums
{
    public enum Repeat : int
    {
        Never = 0,
        EveryDay = 1,
        EveryWeek = 2,
        EveryTwoWeeks = 3,
        EveryMonth = 4,
        EveryTwoMonths = 5,
        EveryThreeMonths = 6,
        EverySixMonths = 7,
        EveryYear = 8
    }
}
