using System;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("5170f738-cfba-42e3-bb8e-a7d5c5ab66f2")]
    public sealed class Project : NamedObject, IComparable<Project>, ICustomFields
    {
        [Guide("Enter a unique name to identify this project throughout your accounting system.")]
        [Guide("Project names appear in transaction forms, financial reports, and project-specific analyses.")]
        [Guide("Use clear, descriptive names that help staff quickly identify the correct project when entering transactions.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Check this box to mark the project as inactive when it's completed or no longer used.")]
        [Guide("Inactive projects are hidden from selection lists to reduce clutter and prevent new postings.")]
        [Guide("Historical transactions remain intact, allowing you to run reports on past project performance.")]
        [Guide("You can reactivate projects at any time by unchecking this box.")]
        [ProtoMember(3)] public bool Inactive { get; set; }
        [ProtoMember(5)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(4)] public CustomFields CustomFields2 { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public override bool IsInactive()
        {
            return Inactive;
        }

        public override string GetName()
        {
            return Name;
        }

        int IComparable<Project>.CompareTo(Project other)
        {
            return (Inactive, Name).CompareTo((other.Inactive, other.Name));
        }
    }
}
