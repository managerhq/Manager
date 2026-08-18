using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class InitialSizeAttribute : Attribute
    {
        public int Size;

        public InitialSizeAttribute(int size)
        {
            Size = size;
        }
    }
}
