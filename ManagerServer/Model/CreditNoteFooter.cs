using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("90f7ba80-5666-49a2-af1b-908cf9a651cd")]
    public sealed class CreditNoteFooter : NamedObject
    {
        [Guide("Enter a name to identify this footer. This name will appear in dropdown menus when selecting a footer for credit notes.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Enter the HTML content that will appear at the bottom of credit notes. You can include formatting, links, and other HTML elements as needed.")]
        [ProtoMember(2), Html] public string Content { get; set; }
        [Guide("Mark this footer as inactive to prevent it from appearing in footer selection dropdowns. Existing credit notes using this footer will not be affected.")]
        [ProtoMember(3)] public bool Inactive { get; set; }

        public override string GetName()
        {
            return Name;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public override bool IsInactive() => Inactive;
    }
}
