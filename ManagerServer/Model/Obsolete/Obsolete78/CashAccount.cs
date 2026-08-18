using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete78
{
    [ProtoContract]
    [Guid("25064eec-02ab-46f3-b71f-f8e78ad4ca45")]
    public sealed class CashAccount : Object
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(8)] public string Code;
        [ProtoMember(2)] public Guid? Currency;
        [ProtoMember(10)] public Guid? Division;
        [ProtoMember(7)] public Guid? ControlAccount;
        [ProtoMember(4)] public decimal StartingBalance;        
        [ProtoMember(6)] public bool Inactive;
        [ProtoMember(5)] public Dictionary<Guid, string> CustomFields;

        [ProtoMember(3)] public bool Obsolete_HasStartingBalance;
        [ProtoMember(9)] public decimal Obsolete_StartingBalance;
    }
}