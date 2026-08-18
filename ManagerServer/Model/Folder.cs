using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("5d6fae1e-ff34-4870-80e9-8a755842d46e")]
    public sealed class Folder : Object, IComparable<Folder>, ICustomFields
    {
        [Guide("Enter a descriptive name for this folder to organize your attachments and documents. Examples include 'Tax Documents 2024', 'Customer Contracts', or 'Receipt Images'. Folders help you categorize and quickly locate related files within Manager.")]
        [ProtoMember(1)] public string Description { get; set; }
        [ProtoMember(2)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(3)] public CustomFields CustomFields2 { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        int IComparable<Folder>.CompareTo(Folder other)
        {
            return string.Compare(other.Description, Description);
        }
    }
}
