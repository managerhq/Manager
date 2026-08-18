using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("6fbe0380-de89-4351-b5ac-eda06b5e7a80")]
    public sealed class PaymentFooter : NamedObject
    {
        [Guide("Enter a name to identify this footer. This name will appear in dropdown menus when selecting a footer for payments.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Enter the content that will appear at the bottom of payments. You can include bank information, authorization details, or any other relevant information.")]
        [ProtoMember(2), Textarea, Long] public string Content { get; set; }
        [Guide("Mark this footer as inactive to prevent it from appearing in footer selection dropdowns. Existing payments using this footer will not be affected.")]
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
