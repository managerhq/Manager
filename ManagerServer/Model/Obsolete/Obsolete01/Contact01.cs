using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete01
{
    [ProtoContract]
    [Guid("ee6fbe3b-630e-43d6-9601-3935d3dd6da7")]
    internal sealed class Contact01 : Object
    {
        [ProtoMember(2)]
        public string Name;
        /*
        [ProtoMember(3)]
        public string FirstName;
        [ProtoMember(4)]
        public string LastName;
         */
        [ProtoMember(5)]
        public string Email;
        [ProtoMember(6)]
        public string Address;
        [ProtoMember(8)]
        public string Telephone;
        [ProtoMember(9)]
        public string Fax;
        [ProtoMember(10)]
        public string Mobile;
        [ProtoMember(11)]
        public string Notes;
        [ProtoMember(12)]
        public string Group;
    }
}
