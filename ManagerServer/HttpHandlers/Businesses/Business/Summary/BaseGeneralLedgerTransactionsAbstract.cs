using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsAbstract : NakedObjectsWithSimpleSearch
    {
        protected BaseGeneralLedgerTransactionsInheritable GetRoot()
        {
            return (BaseGeneralLedgerTransactionsInheritable)this;
        }
    }
}