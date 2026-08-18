using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("c03d1921-7a45-4eda-8742-a2d9082dcf4f")]
    public sealed class BalanceSheetGroup : BalanceSheetAbstractGroup
    {
        [Guide("Enter the name of this balance sheet group. This will appear as a heading on the balance sheet.")]
        [ProtoMember(1)] public string Name { get; set; }
        [Guide("Select a parent group if you want to nest this group under another group on the balance sheet.")]
        [ProtoMember(3), Autocomplete(typeof(ManagerServer.Model.BalanceSheetAbstractGroup))] public Guid? Group { get; set; }
        [Guide("Set the display order for this group. Lower numbers appear first on the balance sheet.")]
        [ProtoMember(4)] public int Position { get; set; }

        [ProtoMember(2)] public int? Obsolete_Code { get; set; }

        public override string GetName()
        {
            return Name;
        }
    }
}
