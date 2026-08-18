using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagerServer.Model.Enums
{
    public enum BankAccountClearStatus :int
    {
        OnTheSameDate = 0,
        OnALaterDate = 1
    }
}
