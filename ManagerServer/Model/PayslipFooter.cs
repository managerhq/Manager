using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("9c4197b6-212a-4c22-9338-6eaa659cfe49")]
    public sealed class PayslipFooter : NamedObject
    {
        [Guide("Enter a name to identify this footer. This name will appear in dropdown menus when selecting a footer for payslips.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Enter the content that will appear at the bottom of payslips. You can include HR contact details, company policies, or any other employee information.")]
        [ProtoMember(2), Textarea, Long] public string Content { get; set; }
        [Guide("Mark this footer as inactive to prevent it from appearing in footer selection dropdowns. Existing payslips using this footer will not be affected.")]
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
