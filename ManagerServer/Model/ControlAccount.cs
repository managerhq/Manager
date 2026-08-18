using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model
{
    public abstract class ControlAccount : NamedObject
    {
        public abstract string NameWithCode { get; }
    }
}
