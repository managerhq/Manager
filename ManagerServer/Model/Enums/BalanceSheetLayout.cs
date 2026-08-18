using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Enums
{
    public enum BalanceSheetLayout : int
    {
        AssetsLiabilitiesEqualsEquity = 0,
        AssetsEqualsLiabilitiesEquity = 1,
        AssetsEqualsEquityLiabilities = 2
    }
}
