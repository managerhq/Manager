using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model
{
    public interface IForeignCurrencyProvider : IObject
    {
        public Guid? ForeignCurrency { get; }
    }
}
