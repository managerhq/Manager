using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model
{
    public interface ICode : IObject
    {
        public string Code { get; }
    }
}
