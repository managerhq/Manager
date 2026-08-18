using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Obsolete.Obsolete84
{
    [ProtoContract]
    [Guid("C92218AB-45D8-4D76-B6B1-084398107B8E")]
    public sealed class InvestmentRevaluationWorksheet : Object
    {
        [ProtoMember(1)] public DateTime Date;
    }
}
