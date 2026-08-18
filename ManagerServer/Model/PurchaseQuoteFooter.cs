using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("072dd522-a137-4e91-8966-93f56f987cda")]
    public sealed class PurchaseQuoteFooter : NamedObject
    {
        [Guide("Enter a name to identify this footer. This name will appear in dropdown menus when selecting a footer for purchase quotes.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Enter the content that will appear at the bottom of purchase quotes. You can include quotation validity periods, terms, or any other relevant information.")]
        [ProtoMember(2), Textarea, Long] public string Content { get; set; }
        [Guide("Mark this footer as inactive to prevent it from appearing in footer selection dropdowns. Existing purchase quotes using this footer will not be affected.")]
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
