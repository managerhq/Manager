using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("eaaee0ba-9c58-4f7b-a800-2857f0a675af")]
    public sealed class SalesInvoiceFooter : NamedObject
    {
        [Guide("Enter a name to identify this footer. This name will appear in dropdown menus when selecting a footer for sales invoices.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Enter the HTML content that will appear at the bottom of sales invoices. You can include payment terms, bank details, or any other important information.")]
        [ProtoMember(2), Html] public string Content { get; set; }
        [Guide("Mark this footer as inactive to prevent it from appearing in footer selection dropdowns. Existing sales invoices using this footer will not be affected.")]
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
