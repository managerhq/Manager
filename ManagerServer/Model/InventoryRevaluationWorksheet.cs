using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("bc9eee1b-7f52-4896-9619-ae2964fb3928")]
    public sealed class InventoryRevaluationWorksheet : Object
    {
        [Guide("Select the date as of which you want to calculate inventory revaluation. The worksheet will compare current inventory values with the new values you specify.")]
        [ProtoMember(1)] public DateTime Date { get; set; }
    }
}
