using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete45
{
    [ProtoContract]
    [Guid("6d9fd5d0-3ed9-4741-88fb-4d83f937705f")]
    internal sealed class CustomField45 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public Guid Type;
        [ProtoMember(3)]
        public CustomFieldStyle FieldType;
        [ProtoMember(4)]
        public CustomFieldSize FieldSize;
        [ProtoMember(5)]
        public int? Position;        
        [ProtoMember(7)]
        public bool DisplayOnList;
        [ProtoMember(8)]
        public bool DisplayOnView;
        [ProtoMember(9)]
        public string DropdownValues;
        [ProtoMember(10)]
        public bool Inactive;

        [ProtoMember(6)]
        public string Obsolete_DefaultValue;
    }
}
