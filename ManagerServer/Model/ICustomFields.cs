using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model
{
    public interface ICustomFields
    {
        public Dictionary<Guid, string> ClassicCustomFields { get; }
        public CustomFields CustomFields { get; }
    }
}
